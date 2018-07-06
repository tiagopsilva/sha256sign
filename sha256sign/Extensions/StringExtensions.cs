namespace sha256sign.Extensions
{
    internal static class StringExtensions
    {
        public static string NormalizeXml(this string xml)
        {
            while (xml.Contains("\r\n"))
                xml = xml.Replace("\r\n", "");

            while (xml.Contains("\t"))
                xml = xml.Replace("\t", "");

            return xml;
        }
    }
}