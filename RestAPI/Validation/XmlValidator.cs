using Commons.Xml.Relaxng;
using System.Xml;
using System.Xml.Schema;

namespace RestAPI.Validation
{
    public static class XmlValidator
    {
        public static string ValidateWithXsd(Stream xmlStream, string xsdPath)
        { 
            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add("", xsdPath);

            XmlReaderSettings settings = new XmlReaderSettings
            {
                ValidationType = ValidationType.Schema,
                Schemas = schemas
            };

            string validationErrors = "";
            settings.ValidationEventHandler += (sender, e) =>
            {
                validationErrors += $"{e.Message}\n";
            };

            try
            {
                using (XmlReader reader = XmlReader.Create(xmlStream, settings))
                {
                    while (reader.Read()) {}
                    return string.IsNullOrEmpty(validationErrors) ? "XSD validation passed." : validationErrors;
                }
            }
            catch (XmlException ex)
            {
                return ex.ToString();
            }
        }


        public static string ValidateWithRng(Stream xmlStream, string rngPath)
        {
            try
            {
                using var rngReader = new XmlTextReader(rngPath);
                var pattern = RelaxngPattern.Read(rngReader);

                using var xmlReader = XmlReader.Create(xmlStream);
                var validatingReader = new RelaxngValidatingReader(xmlReader, pattern);

                while (validatingReader.Read()){}

                return "RNG validation passed.";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }
}
