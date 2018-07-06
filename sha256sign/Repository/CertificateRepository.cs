using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace sha256sign.Repository
{
    internal class CertificateRepository
    {
        public IEnumerable<X509Certificate2> GetAll()
        {
            var stores = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            try
            {
                stores.Open(OpenFlags.ReadOnly);

                var certificates = new List<X509Certificate2>();

                foreach (var certificate in stores.Certificates)
                    if (certificate.NotAfter > DateTime.Now)
                        certificates.Add(certificate);

                return certificates;
            }
            finally
            {
                stores.Close();
            }
        }

        public X509Certificate2 GetBySerialNumber(string serialNumber)
        {
            try
            {
                if (string.IsNullOrEmpty(serialNumber))
                    return null;

                var certificates = GetAll();

                // Remove all non allowed characters that entered the value while copy/paste
                var rgx = new Regex("[^a-fA-F0-9]");
                serialNumber = rgx.Replace(serialNumber, string.Empty).ToUpper();

                var certificate = certificates.SingleOrDefault(cert => cert.SerialNumber == serialNumber);
                return certificate;
            }
            catch (Exception e)
            {
                throw new Exception("Erro ao obter o certificado!", e);
            }
        }
    }
}
