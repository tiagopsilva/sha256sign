# sha256sign

DLL para assinatura de XML´s usando o algoritmo SHA256 e com Certificado A3.

## Detalhes

Essa é uma pequena alteração da **[eSocialSignature](https://github.com/tiagopsilva/eSocialSignature)** para torná-la possível de ser usada em mais de um tipo de software/processo (como o Reinf), obedecendo somente a um requisito para o eSocial que diz que o atributo **URI** da tag **Reference** deve estar vazio, para as outras não.

- DLL escrita em C# para ser usada como uma DLL comum, ou seja, como escrita em C;
- ~Escrita em .NET 4.0 para compatibilidade com Windows XP;~
- Baseada na **[CertFly](https://github.com/leivio/CertFly)**;

## Como usar

- Caso deseje usar com Delphi você pode copiar o arquivo **[sha256sign.pas](https://github.com/tiagopsilva/sha256sign/blob/master/sha256sign.pas)**;

- Caso contrário você pode fazer referência aos métodos diretamente, ou carregar dinamicamente (recomendado):

### Exemplo com a sha256sign.pas e plataforma com strings em ANSI:

```
var
  fileName: string;
begin
  fileName := 'c:\temp\xml-sem-assinatura.xml'; // path do arquivo xml, sem assinatura, no disco
  TSha256Sign.Sign(fileName, 'evtInfoEmpregador', 'eaee2da6eabd4e0aa211e2a18e7c749c', '1234');
  xmlDoc.LoadXml(fileName);
end;
```

---
## MIT License

Copyright (c) 2018 Tiago Silva

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
