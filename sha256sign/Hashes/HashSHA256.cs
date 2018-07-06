using System;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using sha256sign.Extensions;
using sha256sign.Log;
using sha256sign.Repository;
using System.Deployment.Internal.CodeSigning;

namespace sha256sign.Hashes
{
    public class HashSHA256
    {
        private readonly Logger _log;

        public HashSHA256(Logger log)
        {
            _log = log;
        }

        public string Sign(string xml, string nodeToSign, string certificateSerialNumber, string certificatePassword = null)
        {
            try
            {
                _log.Debug("");
                _log.Debug("NOVA ASSINATURA");
                _log.Debug(new string('-', 150));
                _log.Debug("");
                _log.Debug($"O conteúdo do XML é NULO ou Vazio? {string.IsNullOrEmpty(xml)}");
                _log.Debug("");
                _log.Debug($"nodeToSign: {nodeToSign}");
                _log.Debug($"certificateSerialNumber: {certificateSerialNumber}");
                _log.Debug($"certificatePassword: {certificatePassword}");
                _log.Debug("");
                _log.Debug($"XML Recebido:{Environment.NewLine}{Environment.NewLine}{xml}");
                _log.Debug("");

                if (string.IsNullOrEmpty(xml))
                    throw new Exception("Conteúdo de XML inválido");

                xml = xml.NormalizeXml();

                if (string.IsNullOrEmpty(xml))
                    throw new Exception("O conteúdo do XML não foi informado");

                var doc = new XmlDocument();
                try
                {
                    doc.LoadXml(xml);
                }
                catch (Exception e)
                {
                    _log.Error(e, "Erro ao carregar o Documento XML");
                    throw;
                }

                _log.Debug("Documento XML criado");

                var nodes = doc.GetElementsByTagName(nodeToSign);
                if (nodes.Count == 0)
                    throw new Exception("Conteúdo de XML inválido");

                _log.Debug($"Tag {nodeToSign} encontrada");

                var certificate = new CertificateRepository().GetBySerialNumber(certificateSerialNumber);
                if (certificate == null)
                    throw new Exception("Não foi possível encontrar o certificado");

                _log.Debug($"Certificado obtido: {certificate.Subject}");

                foreach (XmlElement node in nodes)
                {
                    _log.Debug("Adicionar tipo de criptografia a engine");

                    CryptoConfig.AddAlgorithm(typeof(RSAPKCS1SHA256SignatureDescription), "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256");

                    _log.Debug("RSAPKCS1SHA256SignatureDescription adicionada");

                    var keyInfo = new KeyInfo();
                    keyInfo.AddClause(new KeyInfoX509Data(certificate));

                    _log.Debug("KeyInfo criado e cláusula adicionada");

                    var Key = (RSACryptoServiceProvider)certificate.PrivateKey;

                    _log.Debug("key obtida");

                    var signedXml = new SignedXml(node)
                    {
                        SigningKey = Key,
                        KeyInfo = keyInfo
                    };

                    _log.Debug("SignedXML criado");

                    using (var rsa = ReadCard(Key, certificatePassword))
                    {
                        signedXml.SigningKey = rsa;
                        signedXml.SignedInfo.CanonicalizationMethod = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315";
                        signedXml.SignedInfo.SignatureMethod = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";

                        _log.Debug("SignedXML preparado");

                        // O atributo id no eSocial é impresso com o "I" maiúsculo.
                        // Já no Reinf é com "i" minúsculo.
                        // no eSocial não deve ter valor no atributo URI da tag reference.
                        // Já no Reinf deve conter o valor do "id"

                        //var id = node.Attributes.GetNamedItem("Id")?.InnerText;
                        var id = node.Attributes.GetNamedItem("id")?.InnerText; // assim não encontra o id quando for do eSocial, mas encontrará do Reinf

                        _log.Debug($"ID #{id}");

                        var reference = new Reference($"#{id}");
                        reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
                        reference.AddTransform(new XmlDsigC14NTransform(false));
                        reference.DigestMethod = "http://www.w3.org/2001/04/xmlenc#sha256";

                        if (string.IsNullOrEmpty(id?.Trim()))
                        {
                            reference.Uri = "";
                        }

                        _log.Debug("Referências criadas");

                        signedXml.AddReference(reference);

                        _log.Debug("Referências adicionadas");

                        _log.Debug("A criar assinatura");

                        signedXml.ComputeSignature();

                        _log.Debug("Assinatura criada");

                        var signature = signedXml.GetXml();

                        _log.Debug("A adicionar a assinatura no documento");

                        var parentNode = node.ParentNode;
                        if (parentNode == null)
                            throw new Exception("Não foi possível encontrar o Nó do eSocial");

                        parentNode.AppendChild(signature);
                    }

                    _log.Debug("Assinatura adicionada");
                }

                _log.Debug("Atualizando XML de saída");

                var sb = new StringBuilder();
                using (var writer = XmlWriter.Create(sb, new XmlWriterSettings { Indent = false }))
                    doc.WriteTo(writer);

                _log.Debug($"XML Assinado:{Environment.NewLine}{Environment.NewLine}{doc.OuterXml}{Environment.NewLine}");

                var signatureCount = doc.GetElementsByTagName("Signature").Count;
                _log.Debug($"Quantidade de assinaturas geradas: {signatureCount}");

                return doc.OuterXml;
            }
            catch (Exception e)
            {
                _log.Debug("Erro");
                _log.Error(e, "");
                throw;
            }
        }

        private RSACryptoServiceProvider ReadCard(RSACryptoServiceProvider key, string cardPin = null)
        {
            var csp = new CspParameters(key.CspKeyContainerInfo.ProviderType, key.CspKeyContainerInfo.ProviderName)
            {
                ProviderType = key.CspKeyContainerInfo.ProviderType,
                ProviderName = key.CspKeyContainerInfo.ProviderName,
                KeyNumber = key.CspKeyContainerInfo.KeyNumber == KeyNumber.Exchange ? 1 : 2,
                KeyContainerName = key.CspKeyContainerInfo.KeyContainerName
            };

            _log.Debug(
                "PROVIDER",
                "-----------------------------------------------",
                $"ProviderType: {csp.ProviderType}",
                $"ProviderName: {csp.ProviderName}",
                $"KeyNumber: {csp.KeyNumber}",
                $"KeyContainerName: {csp.KeyContainerName}");

            if (cardPin != null && !string.IsNullOrEmpty(cardPin.Trim()))
            {
                var ss = new SecureString();

                foreach (var a in cardPin ?? "")
                    ss.AppendChar(a);

                csp.KeyPassword = ss;
                csp.Flags = CspProviderFlags.NoPrompt | CspProviderFlags.UseDefaultKeyContainer;
            }
            else
            {
                csp.Flags = CspProviderFlags.UseDefaultKeyContainer;
            }

            var rsa = new RSACryptoServiceProvider(csp);
            return rsa;
        }
    }
}