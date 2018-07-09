unit UntCertificados;

interface

uses
  SysUtils, ACBrCAPICOM_TLB, ActiveX;

type
  TCertificados = class
  public
    class function SelecionarCertificado: string;
  private
    class function ObterNumeroSerial: string;
  end;

implementation

{ TCertificados }

class function TCertificados.SelecionarCertificado: string;
begin
  CoInitialize(nil);
  try
    result := TCertificados.ObterNumeroSerial;
  finally
    CoUninitialize;
  end;
end;

class function TCertificados.ObterNumeroSerial: string;
var
  Store: IStore3;
  certificados: ICertificates2;
  certificado: ICertificate2;
  index: Integer;
  subject: string;
  selected: string;
  selectedIndex: integer;
const
  CAPICOM_STORE_NAME = 'My';
begin
  result := '';
  try

    Writeln('Lista de certificados instalados:');
    Writeln('');

    Store := CoStore.Create;
    Store.Open(CAPICOM_CURRENT_USER_STORE, CAPICOM_STORE_NAME, CAPICOM_STORE_OPEN_MAXIMUM_ALLOWED);
    try
      certificados := Store.Certificates as ICertificates2;

      for index := 1 to certificados.Count do
      begin
        try
          certificado := IInterface(certificados.Item[index]) as ICertificate2;

          subject := certificado.SubjectName;
          if Pos(',', subject) > 0 then
            subject := Copy(subject, 1, Pos(',', subject) - 1);

          Writeln(Copy(IntToStr(10000000 + index), 5, 4) + ' - ' + subject);
        except
        end;
      end;

      Writeln('');
      Write(Format('Informe um numero de %d ate %d do certificado que voce deseja utilizar: ', [1, certificados.Count]));

      repeat
        Readln(selected);
        selectedIndex := StrToIntDef(selected, 0);
        if (selectedIndex <= 0) or (selectedIndex > certificados.Count) then
          Write(Format('Numero invalido! Informe um numero entre %d e %d de acordo com o certificado desejado na lista acima: ', [1, certificados.Count]));
      until (selectedIndex > 0) and (selectedIndex < certificados.Count);

      certificado := IInterface(certificados.Item[selectedIndex]) as ICertificate2;
      Writeln('');
      Writeln('Certificado selecionado, Numero Serial: ' + certificado.SerialNumber);
      result := Copy(certificado.SerialNumber, 1, Length(certificado.SerialNumber));
    finally
      Store.Close;
    end;
  except
    on e: Exception do
      raise Exception.Create('Não foi possível carregar a lista de certificados!' + #13 + #13 + 'Erro: ' + e.Message);
  end;
end;

end.

