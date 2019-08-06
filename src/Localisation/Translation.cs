using ColossalFramework.Globalization;
using System.Xml;
using System.IO;

namespace CSM.Localisation
{
    public class Translation
    {

        public static string XMLTranslationContents;

        public static string TranslationName;

        public static void GetXMLTranslation()
        {
            TranslationName = LocaleManager.cultureInfo.Name.Substring(0, 2);
            XMLTranslationContents = PullXMLFile(TranslationName+".xml");
        }

        public static string PullTranslation(string getlanguageidname)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(XMLTranslationContents);

            string result = "ERROR TRANSLATION";
            XmlNode xn = xml.SelectSingleNode($"/language/translation[@id='{getlanguageidname}']");

            if (xn != null)
            {
                result = xn.InnerText;
            }

            return result;
        }

        public static string PullXMLFile(string filename)
        {
            string result = string.Empty;
            Translation get = new Translation();

            using (Stream stream = get.GetType().Assembly.
                       GetManifestResourceStream("CSM.Localisation.Languages." + filename))
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    result = sr.ReadToEnd();
                }
            }
            return result;
        }
    }
}