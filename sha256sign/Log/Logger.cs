using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace sha256sign.Log
{
    public class Logger
    {
        private static readonly object _locker = new object();
        private readonly string _fileName;

        public Logger(string fileName)
        {
            Console.WriteLine($"Directory of LOG: {AppDomain.CurrentDomain.BaseDirectory}");
            Console.WriteLine($"File of LOG: {fileName}");
            _fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            ValidatePath();
        }

        public void Info(params string[] messages)
        {
            Log(LevelType.Info, null, $"{DateTemplate()}|Info|", messages);
        }

        public void Debug(params string[] messages)
        {
#if DEBUG
            Log(LevelType.Debug, null, $"{DateTemplate()}|Debug|", messages);
#endif
        }

        public void Warning(params string[] messages)
        {
            Log(LevelType.Warning, null, $"{DateTemplate()}|Warn|", messages);
        }

        public void Error(Exception exception, params string[] messages)
        {
            Log(LevelType.Error, exception, $"{DateTemplate()}|Error|", messages);
        }

        public void Fatal(Exception exception, params string[] messages)
        {
            Log(LevelType.Fatal, exception, $"{DateTemplate()}|Fatal|", messages);
        }

        private void Log(LevelType levelType, Exception exception, string template, params string[] messages)
        {
            var lines = new List<string>(messages.Select(msg => $"{template}{msg}"));

            if (levelType == LevelType.Error)
            {
                lines.Insert(0, "");
                lines.Add("");

                if (exception != null)
                {
                    var moreLines = exception.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                    lines.AddRange(moreLines.Select(exmsg => $"{template}{exmsg}"));
                }
            }

            Write(lines);
        }

        private void ValidatePath()
        {
            var path = Path.GetDirectoryName(_fileName);

            if (string.IsNullOrEmpty(path?.Trim()))
                throw new Exception("O diretório para o arquivo de LOG é inválido");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        private static string DateTemplate()
        {
            return DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }

        private void Write(IEnumerable<string> messages)
        {
            Task.Factory.StartNew(() =>
            {
                lock (_locker)
                {
                    using (var file = new StreamWriter(_fileName, true))
                    {
                        foreach (var message in messages)
                        {
                            Console.WriteLine(message);
                            file.WriteLine(message);
                        }
                    }
                }
            }).Wait();
        }
    }
}