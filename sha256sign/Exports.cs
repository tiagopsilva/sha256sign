using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;
using RGiesecke.DllExport;
using sha256sign.Hashes;
using sha256sign.Log;

namespace sha256sign
{
    public static class Exports
    {
        [DllExport("SignSHA256Ansi", CallingConvention = CallingConvention.StdCall)]
        public static void SignSHA256Ansi(
            [MarshalAs(UnmanagedType.LPStr)] ref string xml,
            [MarshalAs(UnmanagedType.LPStr)] string nodeToSign,
            [MarshalAs(UnmanagedType.LPStr)] string certificateSerialNumber,
            [MarshalAs(UnmanagedType.LPStr)] string certificatePassword)
        {
            try
            {
                Console.WriteLine("Parâmetros:");
                Console.WriteLine($"Xml: {xml}");
                Console.WriteLine($"nodeToSign: {nodeToSign}");
                Console.WriteLine($"certificateSerialNumber: {certificateSerialNumber}");
                Console.WriteLine($"certificatePassword: {certificatePassword}");

                var log = new Logger($"log{DateTime.Now:yyyyMMdd}.txt");
                xml = new HashSHA256(log).Sign(xml, nodeToSign, certificateSerialNumber, certificatePassword) + null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [DllExport("SignSHA256Unicode", CallingConvention = CallingConvention.StdCall)]
        public static void SignSHA256Unicode(
            [MarshalAs(UnmanagedType.LPWStr)] ref string xml,
            [MarshalAs(UnmanagedType.LPWStr)] string nodeToSign,
            [MarshalAs(UnmanagedType.LPWStr)] string certificateSerialNumber,
            [MarshalAs(UnmanagedType.LPWStr)] string certificatePassword)
        {
            try
            {
                Console.WriteLine("Parâmetros:");
                Console.WriteLine($"Xml: {xml}");
                Console.WriteLine($"nodeToSign: {nodeToSign}");
                Console.WriteLine($"certificateSerialNumber: {certificateSerialNumber}");
                Console.WriteLine($"certificatePassword: {certificatePassword}");

                var log = new Logger($"log{DateTime.Now:yyyyMMdd}.txt");
                xml = new HashSHA256(log).Sign(xml, nodeToSign, certificateSerialNumber, certificatePassword) + null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [DllExport("SignFileWithSHA256Ansi", CallingConvention = CallingConvention.StdCall)]
        public static void SignFileWithSHA256Ansi(
            [MarshalAs(UnmanagedType.LPStr)] string fileName,
            [MarshalAs(UnmanagedType.LPStr)] string nodeToSign,
            [MarshalAs(UnmanagedType.LPStr)] string certificateSerialNumber,
            [MarshalAs(UnmanagedType.LPStr)] string certificatePassword)
        {
            try
            {
                Console.WriteLine("Parâmetros:");
                Console.WriteLine($"fileName: {fileName}");
                Console.WriteLine($"nodeToSign: {nodeToSign}");
                Console.WriteLine($"certificateSerialNumber: {certificateSerialNumber}");
                Console.WriteLine($"certificatePassword: {certificatePassword}");

                var log = new Logger($"log{DateTime.Now:yyyyMMdd}.txt");
                var doc = new XmlDocument();
                doc.Load(fileName);
                var xml = new HashSHA256(log).Sign(doc.OuterXml, nodeToSign, certificateSerialNumber, certificatePassword);
                log.Info($"XML de Saída: {xml}");
                File.WriteAllText(fileName, xml);
                Console.WriteLine("Saved!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [DllExport("SignFileWithSHA256Unicode", CallingConvention = CallingConvention.StdCall)]
        public static void SignFileWithSHA256Unicode(
            [MarshalAs(UnmanagedType.LPWStr)] string fileName,
            [MarshalAs(UnmanagedType.LPWStr)] string nodeToSign,
            [MarshalAs(UnmanagedType.LPWStr)] string certificateSerialNumber,
            [MarshalAs(UnmanagedType.LPWStr)] string certificatePassword)
        {
            try
            {
                Console.WriteLine("Parâmetros:");
                Console.WriteLine($"fileName: {fileName}");
                Console.WriteLine($"nodeToSign: {nodeToSign}");
                Console.WriteLine($"certificateSerialNumber: {certificateSerialNumber}");
                Console.WriteLine($"certificatePassword: {certificatePassword}");

                var log = new Logger($"log{DateTime.Now:yyyyMMdd}.txt");
                var doc = new XmlDocument();
                doc.Load(fileName);
                var xml = new HashSHA256(log).Sign(doc.OuterXml, nodeToSign, certificateSerialNumber, certificatePassword);
                File.WriteAllText(fileName, xml);
                Console.WriteLine("Salvo!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
