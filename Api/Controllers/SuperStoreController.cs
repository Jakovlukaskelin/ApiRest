using Commons.Xml.Relaxng;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuperStoreController : ControllerBase
    {


        public ( bool IsValid , string ErrorMessage) ProcessXmlFileWithXSD(IFormFile file)
        {
            //spremanje dadoteke na privremeno mjesto

            var filepath = Path.GetTempFileName();

            using (var stream = System.IO.File.Create(filepath))
            {
                file.CopyTo(stream);
            }

            var solutionDirectoryPath = FindSolutionPath();
            string xmlFilePath = Path.Combine((string)(solutionDirectoryPath ?? AppDomain.CurrentDomain.BaseDirectory), "SuperStoreXML.xml");
            string xsdFilePath = Path.Combine((string)(solutionDirectoryPath ?? AppDomain.CurrentDomain.BaseDirectory), "SuperStoreXSD.xsd");

            //ucitaj xsd
            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add(null, XmlReader.Create(xsdFilePath));

            //citaj xml
            XmlDocument doc = new XmlDocument();
            doc.Load(file.OpenReadStream());

            //validacija
            string msg = "";
            doc.Schemas = schemas;
            doc.Validate((sender, args) =>
            {
                msg += args.Message + Environment.NewLine;
            });

            if (string.IsNullOrEmpty(msg))
            {
                doc.Save(xmlFilePath);
                return (true, string.Empty);
            }
            else
            {
                return (false, msg);
            }
        }
        public (bool IsValid, string ErrorMessage) ProcessXmlFileWithRNG(IFormFile file)
        {
           
            var filePath = Path.GetTempFileName();
            using (var stream = System.IO.File.Create(filePath))
            {
                file.CopyTo(stream);
            }

            var solutionDirectoryPath = FindSolutionPath();
            string xmlFilePath = Path.Combine((string)solutionDirectoryPath, "SuperStoreXML.xml");
            string rngFilePath = Path.Combine((string)solutionDirectoryPath, "SuperStore.RNG.rng");

            // lodaj rng
            RelaxngPattern rng;
            using (var rngReader = XmlReader.Create(rngFilePath))
            {
                rng = RelaxngPattern.Read(rngReader);
            }

            // lodaj xml
            XmlDocument doc = new XmlDocument();
            doc.Load(file.OpenReadStream());

            // Validation
            string msg = "";
            using (var xmlReader = new RelaxngValidatingReader(new XmlTextReader(xmlFilePath), rng))
            {
                try
                {
                    while (xmlReader.Read()) { } // Exception ako je invalid
                }
                catch (Exception ex)
                {
                    msg += ex.Message + Environment.NewLine;
                }
            }

            if (string.IsNullOrEmpty(msg))
            {
                doc.Save(xmlFilePath);
                return (true, string.Empty);
            }
            else
            {
                return (false, msg);
            }
        }

        [HttpPost(Name = "SaveWithXSD")]

        public IActionResult SaveWithXSD(IFormFile file)
        {
            try
            {
                var (isValid, errorMessage) = ProcessXmlFileWithXSD(file);
                if (isValid)
                {
                    return Ok("XML file is valid according to the provided XSD schema.");
                }
                else
                {
                    //TOCNA pogreska koja se dogodila 
                    return BadRequest($"XML file is not valid according to the provided XSD schema. Error: {errorMessage}");
                }
            }
            catch (XmlSchemaException ex)
            {
                return BadRequest("Error: " + ex.Message);
            }
        }


        [HttpPost(Name = "SaveWithRNG")]
        public IActionResult SaveWithRNG(IFormFile file)
        {
            try
            {
                var (isValid, errorMessage) = ProcessXmlFileWithRNG(file);
                if (isValid)
                {
                    return Ok("XML file is valid according to the provided RNG schema.");
                }
                else
                {
                    
                    return BadRequest($"XML file is not valid according to the provided RNG schema. Error: {errorMessage}");
                }
            }
            catch (XmlSchemaException ex)
            {
                return BadRequest("Error: " + ex.Message);
            }
        }

        [HttpGet("Temperature/{cityName}")]
        public async Task<Dictionary<string, string>> GetCurrentTemperatures(string cityName)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string xmlContent = await client.GetStringAsync("https://vrijeme.hr/hrvatska_n.xml");
                    XDocument xmlDoc = XDocument.Parse(xmlContent);

                    var cityElements = xmlDoc.Descendants("Grad")
                        .Where(g => g.Element("GradIme")?.Value.Trim().Contains(cityName.Trim(), StringComparison.OrdinalIgnoreCase) == true);

                    Dictionary<string, string> cityTemperatures = new Dictionary<string, string>();

                    foreach (var cityElement in cityElements)
                    {
                        var city = cityElement.Element("GradIme")?.Value.Trim();
                        var temperatureElement = cityElement.Element("Podatci")?.Element("Temp");
                        if (city != null && temperatureElement != null)
                        {
                            cityTemperatures.Add(city, temperatureElement.Value);
                        }
                    }

                    if (cityTemperatures.Count > 0)
                    {
                        return cityTemperatures;
                    }
                    else
                    {
                        throw new Exception($"Temperature data for cities containing {cityName} not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while fetching the temperatures: {ex.Message}");
            }
        }



        private object FindSolutionPath()
        {
            string solutionDirectoryPath = AppDomain.CurrentDomain.BaseDirectory; // Gets the bin/debug directory path
            for (int i = 0; i < 5; i++)
            {
                var directoryInfo = Directory.GetParent(solutionDirectoryPath);
                if (directoryInfo != null)
                {
                    solutionDirectoryPath = directoryInfo.FullName;
                }
                else
                {
                    throw new Exception($"Could not find parent directory at level {i}");
                }
            }
            return solutionDirectoryPath;
        }
    }
}
