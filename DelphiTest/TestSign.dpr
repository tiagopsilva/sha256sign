program TestSign;

{$APPTYPE CONSOLE}

uses
  {$if CompilerVersion > 20 }
  System.SysUtils,
  System.Classes,
  {$else}
  SysUtils,
  Classes,
  {$ifend }
  Windows,
  ActiveX,
  UntCertificados in 'UntCertificados.pas',
  ACBrCAPICOM_TLB in 'ACBrCAPICOM_TLB.pas',
  sha256sign in '..\sha256sign.pas';

var
  directory: string;
  fileName: string;
  xml: TStringList;
  CertTokenA3SerialNumber: string;
  CertTokenA3Pin: string;

begin
  xml := TStringList.Create;
  try
    try
      // Obtem os valores de SerialNumber e PIN do certificado pelas vari�veis de ambiente previamente configuradas.
      // Para realizar seu teste voc� pode cadastrar essas vari�veis com os valores do seu certificado ou mudar essas
      // linhas colocando os valores diretamente.
      CertTokenA3SerialNumber := GetEnvironmentVariable('CERT_TOKEN_A3_SERIAL_NUMBER');
      CertTokenA3Pin := GetEnvironmentVariable('CERT_TOKEN_A3_PIN');

      // se o SerialNumber n�o for encontrado nas vari�veis de ambiente, ser� ent�o exibida uma lista dos certificados
      // instalados e dispon�veis para o teste
      if Trim(CertTokenA3SerialNumber) = '' then
      begin
        CertTokenA3SerialNumber := TCertificados.SelecionarCertificado;
      end;

      directory := ExtractFilePath(ParamStr(0));
      fileName := directory + 'teste_' + FormatDateTime('yyyyMMdd-hhmmss', now) + '.xml';
      CopyFile(PChar(directory + 'envio-sem-assinatura.xml'), PChar(fileName), false);

      TSha256Sign.Sign(fileName, 'evtInfoEmpregador', CertTokenA3SerialNumber, CertTokenA3Pin);

      Writeln('');
      Writeln('');
      Writeln('Processo da DLL finalizado!');
      Writeln('');
      Writeln('');
      Writeln('Arquivo Gerado!');

      xml.LoadFromFile(fileName);
      Writeln(xml.Text);

      Writeln('');
    except
      on E: Exception do
        Writeln(E.ClassName, ': ', E.Message);
    end;
  finally
    xml.Free;
    Writeln('');
    Write('Processo finalizado! Pressione ENTER para sair... ');
    Readln;
  end;
end.

