unit sha256sign;

interface

uses
  Windows, SysUtils;

const
  DLLNAME = 'sha256sign.dll';

type
  TSha256Sign = class
    class procedure Sign(AXmlFileName: string; ANodeToSign: string; ASerialNumber: string; APassword: string);
  end;

implementation

{ TSha256Sign }

class procedure TSha256Sign.Sign(AXmlFileName, ANodeToSign, ASerialNumber, APassword: string);
type
  TProc = procedure(AXmlFileName: PAnsiChar; ANodeToSign: PAnsiChar; ASerialNumber: PAnsiChar; APassword: PAnsiChar); stdcall;
var
  dllHandle: THandle;
  proc: TProc;
begin
  dllHandle := LoadLibrary(DLLNAME);
  if dllHandle < HINSTANCE_ERROR then
  begin
    raise Exception.Create('Não foi possível encontrar a DLL ' + DLLNAME + '.' + #13 + SysErrorMessage(GetLastError));
  end;
  try
    @proc := GetProcAddress(dllHandle, 'SignFileWithSHA256Ansi');
    if Assigned(@proc) then
    begin
      try
      {$WARNINGS OFF}
        proc(
          PAnsiChar(AnsiString(AXmlFileName)),
          PAnsiChar(AnsiString(ANodeToSign)),
          PAnsiChar(AnsiString(ASerialNumber)),
          PAnsiChar(AnsiString(APassword)));
      {$WARNINGS ON}
      except
        on e: Exception do
          raise Exception.Create('Erro ao executar sha256sign.dll!' + #13 + 'Technical Message: ' + e.Message);
      end;
    end
    else
    begin
      raise Exception.Create('Não foi possível encontrar o método SignFileWithSHA256Ansi na DLL.' + #13 +
        'Verifique se o build da dll foi feito com a opção "Platform Target" como "x86".');
    end;
  finally
    FreeLibrary(dllHandle);
  end;
end;

end.
