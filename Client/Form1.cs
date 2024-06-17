
using Newtonsoft.Json;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Xml;
using Formatting = Newtonsoft.Json.Formatting;

namespace Client
{
    public partial class Form1 : Form
    {
        private int buttonSize = 150;
        private readonly HttpClient _client;
        public Form1()
        {
            InitializeComponent();
            this.Text = "IIS-JakovLukaSkelin";
            this.Width = 900;
            this.Height = 485;
            _client = new HttpClient();


            for (int i = 0; i <= 6; i++)
            {
                int buttonSpacing = 74;
                Button button = new Button();
                button.Name = "ButtonI" + i;
                button.Width = buttonSize;
                button.Height = 75;
                button.Left = 0;
                button.Top = buttonSpacing * (i - 1);
                button.Font = new System.Drawing.Font("Arial", 10);

                switch (i)
                {
                    case 1:
                        button.Text = "I1 - Create SuperStore XSD";
                        button.Click += ButtonI1_Click;
                        break;
                    case 2:
                        button.Text = "I2 - Create SuperStore RNG";
                        button.Click += ButtonI2_Click;
                        break;
                    case 3:
                        button.Text = "I3 - Search superStore";
                        button.Click += ButtonI3_Click;
                        break;
                    case 4:
                        button.Text = "I4 - Validation with JAXB and XSD";
                        button.Click += ButtonI4_Click;
                        break;
                    case 5:
                        button.Text = "I5 - Get current temperature by city";
                        button.Click += ButtonI5_Click;
                        break;
                    case 6:
                        button.Text = "I6 - Get Country";
                        button.Click += ButtonI6_Click;
                        break;


                }
                this.Controls.Add(button); // dodaj  button u form's controls
            }
        }

        private void ButtonI6_Click(object? sender, EventArgs e)
        {
            ClearContent();
            GetI6DataAsync();
        }

        private async void GetI6DataAsync()
        {
           
            
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://weatherapi-com.p.rapidapi.com/current.json?q=53.1%2C-0.13"),
                Headers =
    {
        { "x-rapidapi-key", "20683d3772mshfe92c2f4ea6db96p185b7ejsnccd1454aea37" },
        { "x-rapidapi-host", "weatherapi-com.p.rapidapi.com" },
    },
            };

            using (var response = await _client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                string formattedJson = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(body), Formatting.Indented);

                Form dataForm = new Form();
                dataForm.Text = "Weather Data"; 

               
                TextBox textBox = new TextBox
                {
                    Multiline = true,
                    ReadOnly = true,
                    ScrollBars = ScrollBars.Both,
                    Text = formattedJson,
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = SystemColors.Control,
                    ForeColor = SystemColors.WindowText,
                    Font = new Font("Courier New", 10),
                    Dock = DockStyle.Fill 
                };

                
                dataForm.Controls.Add(textBox);

               
                dataForm.ClientSize = new Size(600, 400); 

              
                dataForm.ShowDialog();



            }
        }

        private void ButtonI5_Click(object? sender, EventArgs e)
        {   ClearContent();
            GetI5DataAsync();
            
        }

        private void GetI5DataAsync()
        {
            ClearContent();
            TextBox tbCityName = new TextBox();
            tbCityName.Left = 360;
            tbCityName.Top = 50;
            tbCityName.Width = 200;
            tbCityName.Height = 30; 
            tbCityName.Font = new System.Drawing.Font("Arial", 15);
            tbCityName.Text = "City name";
            tbCityName.ForeColor = System.Drawing.Color.LightGray;
            tbCityName.Click += (sender, e) =>
            {
                tbCityName.ForeColor = System.Drawing.Color.Black;
                if (tbCityName.Text == "City name")
                {
                    tbCityName.Text = String.Empty;
                }
            };
            Button btnSubmit = new Button();
            btnSubmit.Name = "SubmitButton";
            btnSubmit.Text = "Submit";
            btnSubmit.Left = 600;
            btnSubmit.Top = 50;
            btnSubmit.Height = 30;
            btnSubmit.Width = 100;
            btnSubmit.Font = new System.Drawing.Font("Arial", 15);
            this.Controls.Add(tbCityName);
            this.Controls.Add(btnSubmit);

          
            TextBox tbTemperatures = new TextBox();
            tbTemperatures.Multiline = true;
            tbTemperatures.ScrollBars = ScrollBars.Vertical;
            tbTemperatures.Width = 600;
            tbTemperatures.Height = 200;
            tbTemperatures.Top = 100;
            tbTemperatures.Left = 280;
            tbTemperatures.Font = new System.Drawing.Font("Arial", 15);
            tbTemperatures.ReadOnly = true; 
            this.Controls.Add(tbTemperatures);

            btnSubmit.Click += async (sender, e) =>
            {
                string cityName = tbCityName.Text;
                await BtnSubmit_Click(sender, e, cityName, tbTemperatures);
            };
        }
        private async Task BtnSubmit_Click(object sender, EventArgs e, string cityName, TextBox tbTemperatures)
        {
            ClearLabels();

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync($"http://localhost:5000/api/SuperStore/Temperature/{cityName}");

                if (response.IsSuccessStatusCode)
                {
                    string temperature = await response.Content.ReadAsStringAsync();
                    tbTemperatures.AppendText($"The current temperature in {cityName} is {temperature}° Celsius\n");
                }
                else
                {
                    MessageBox.Show($"The city {cityName} does not exist. Please enter a valid city!", "City Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        
        private void ButtonI4_Click(object? sender, EventArgs e)
        {
            ClearContent();
            GetI4DataAsync();
        }

        private async void GetI4DataAsync()
        {
            ClearLabels();
            //Pronalazak XSD i XML datoteke
            string path = AppDomain.CurrentDomain.BaseDirectory; // Gets the bin/debug directory path
            string solutionDirectoryPath = Directory.GetParent(path).Parent.Parent.Parent.FullName;
            string xsdPath = Path.Combine(solutionDirectoryPath, "SuperStoreXSD.xsd");
            string storeSearchList = Path.Combine(solutionDirectoryPath, "storeSearch.xml");

            if (xsdPath != null && storeSearchList != null)
            {
                // JAXB validacija => validiraj storeSearchList.xml sa XSD
                await RunJavaValidation(storeSearchList, xsdPath);
            }
            else
            {
                System.Windows.Forms.Label label = new System.Windows.Forms.Label();
                label.Width = 600;
                label.Top = 200;
                label.Left = 280;
                label.Font = new System.Drawing.Font("Arial", 15);
                label.Text = $"The list with all the stores or the XSD file does not exist, please run I3 first!";
                label.ForeColor = System.Drawing.Color.Red;
                label.Name = "storeLabel";
                this.Controls.Add(label);
            }

        }

        private async Task RunJavaValidation(string xmlPath, string xsdPath)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory; 
            string solutionDirectoryPath = Directory.GetParent(path).Parent.Parent.Parent.FullName; 
            string jarPath = Path.Combine(solutionDirectoryPath, "XmlValidator.jar");

            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "java"; // java mora bit u path
            psi.Arguments = $"-jar \"{jarPath}\" \"{xmlPath}\" \"{xsdPath}\"";
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.UseShellExecute = false;

            Process p = Process.Start(psi);
            string stdout = await p.StandardOutput.ReadToEndAsync();
            string stderr = await p.StandardError.ReadToEndAsync();
            p.WaitForExit();

            System.Windows.Forms.Label label = new System.Windows.Forms.Label();
            label.Width = 600;
            label.Height = 300;
            label.Top = 200;
            label.Left = 280;
            label.Font = new System.Drawing.Font("Arial", 15);
            if (stderr == "")
            {
                label.Text = $"Output: {stdout}";
            }
            else
            {
                label.Text = $"Error: {stderr}";
                label.ForeColor = System.Drawing.Color.Red;
            }
            label.Name = "storeLabel";
            this.Controls.Add(label);
        }

        private void ButtonI3_Click(object? sender, EventArgs e)
        {
            ClearContent();
            GetI3DataAsync();
        }

        private void GetI3DataAsync()
        {
            ClearContent();
            TextBox tbStoreName = new TextBox();
            tbStoreName.Left = 360;
            tbStoreName.Top = 50;
            tbStoreName.Width = 200;
            tbStoreName.Height = 100;
            tbStoreName.Font = new System.Drawing.Font("Arial", 15);
            tbStoreName.Text = "Store ID";
            tbStoreName.ForeColor = System.Drawing.Color.LightGray;
            tbStoreName.Click += (sender, e) =>
            {
                tbStoreName.Text = String.Empty;
                tbStoreName.ForeColor = System.Drawing.Color.Black;
            };
            Button btnSubmit = new Button();
            btnSubmit.Name = "SubmitButton";
            btnSubmit.Text = "Search";
            btnSubmit.Left = 600;
            btnSubmit.Top = 50;
            btnSubmit.Height = 30;
            btnSubmit.Width = 100;
            btnSubmit.Font = new System.Drawing.Font("Arial", 15);
            this.Controls.Add(tbStoreName);
            this.Controls.Add(btnSubmit);
            btnSubmit.Click += (sender, e) =>
            {
                string store = tbStoreName.Text;
                BtnSearch_Click(sender, e, store);
            };
        }

        private async void BtnSearch_Click(object sender, EventArgs e, string superStore)
        {
            ClearLabels();
            using (HttpClient client = new HttpClient())
            {
                string url = "http://localhost:44328/SuperStore.asmx/GetSuperStoreInfo";
                string requestData = $"<OrderID>{HttpUtility.UrlEncode(superStore)}";

                try
                {
                    var content = new StringContent(requestData, Encoding.UTF8, "application/x-www-form-urlencoded");
                    HttpResponseMessage response = await client.PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();

                        // dohvati superstore xml
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(responseContent);
                        string superStoreData = xmlDoc.InnerText;

                        System.Windows.Forms.Label label = new System.Windows.Forms.Label();
                        label.Width = 600;
                        label.Height = 300;
                        label.Top = 200;
                        label.Left = 400;
                        label.Font = new System.Drawing.Font("Arial", 15);
                        label.Text = $"-Store information- \n{superStoreData}";
                        label.Name = "storeLabel";
                        this.Controls.Add(label);
                    }
                    else
                    {
                        System.Windows.Forms.Label label = new System.Windows.Forms.Label();
                        label.Width = 600;
                        label.Top = 200;
                        label.Left = 280;
                        label.Font = new System.Drawing.Font("Arial", 15);
                        label.Text = $"The store {superStore} does not exist. Please enter a valid store!";
                        label.ForeColor = System.Drawing.Color.Red;
                        label.Name = "storeLabel";
                        this.Controls.Add(label);
                    }
                }
                catch (HttpRequestException ex)
                {
                    
                    Console.WriteLine("An error occurred while sending the request: " + ex.Message);
                }
            }
        }

        private void ClearLabels()
        {
            foreach (Control control in this.Controls)
            {
                if (control is System.Windows.Forms.Label)
                {
                    control.Dispose();
                    continue;
                }
            }
        }

        private void ButtonI2_Click(object? sender, EventArgs e)
        {
            ClearContent();
            GetI2DataAsync();
        }

        private async void GetI2DataAsync()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory; 
            string solutionDirectoryPath = Directory.GetParent(path).Parent.Parent.Parent.FullName; 
            string fixedFilePath = Path.Combine(solutionDirectoryPath, "SuperStoreXML.xml"); 
            string newFilePath = Path.GetTempFileName(); 

            
            File.Copy(fixedFilePath, newFilePath, true);

           
            Process.Start(@"C:\Program Files\Notepad++\notepad++.exe", newFilePath)?.WaitForExit();

           
            string fileContent = File.ReadAllText(newFilePath);

         
            File.WriteAllText(newFilePath, fileContent);

            using (var client = new HttpClient())
            {
                var formContent = new MultipartFormDataContent();

                var fileContentTemp = new ByteArrayContent(File.ReadAllBytes(newFilePath));
                fileContentTemp.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

               
                formContent.Add(fileContentTemp, "file", Path.GetFileName(newFilePath));

                // Send the POST request to the server
                var response = await client.PostAsync("http://localhost:5000/api/SuperStore/SaveWithRNG", formContent);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("File successfully validated and uploaded.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    MessageBox.Show($"There was an error validating and uploading your file:{errorContent}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
            }
        }

        private void ButtonI1_Click(object? sender, EventArgs e)
        {
            ClearContent();
            GetI1DataAsync();
        }

        private async void GetI1DataAsync()
        {
          
            string path = AppDomain.CurrentDomain.BaseDirectory;
            string solutionDirectoryPath = Directory.GetParent(path).Parent.Parent.Parent.FullName; 
            string fixedFilePath = Path.Combine(solutionDirectoryPath, "SuperStoreXML.xml"); 
            string newFilePath = Path.GetTempFileName(); // genrira i povremeni file

            //compira pravi u temp
            File.Copy(fixedFilePath, newFilePath, true);

            // otvori temp file
            Process.Start(@"C:\Program Files\Notepad++\notepad++.exe", newFilePath)?.WaitForExit();

            // citaj fixed file
            string fileContent = File.ReadAllText(newFilePath);

            // Save  new file
            File.WriteAllText(newFilePath, fileContent);

            using (var client = new HttpClient())
            {
                var formContent = new MultipartFormDataContent();

                var fileContentTemp = new ByteArrayContent(File.ReadAllBytes(newFilePath));
                fileContentTemp.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

                //novi  file u POST request
                formContent.Add(fileContentTemp, "file", Path.GetFileName(newFilePath));

                // posalji POST request do server
                var response = await client.PostAsync("http://localhost:5000/api/SuperStore/SaveWithXSD", formContent);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("File successfully validated and uploaded.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
                else
                { var errorContent = await response.Content.ReadAsStringAsync();

                    MessageBox.Show($"There was an error validating and uploading your file:{errorContent}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
            }
        }

        private void ClearContent()
        {
            Button button1 = this.Controls.OfType<Button>().FirstOrDefault(btn => btn.Name == "SubmitButton");
            System.Windows.Forms.Label label1 = this.Controls.OfType<System.Windows.Forms.Label>().FirstOrDefault(lbl => lbl.Name == "temperatureLabel");

            //Clear the Label
            foreach (Control control in this.Controls)
            {
                if (control is System.Windows.Forms.Label)
                {
                    control.Dispose();
                    continue;
                }
            }

            if (button1 != null)
            {
                this.Controls.Remove(button1);
                button1.Dispose();
            }

            if (label1 != null)
            {
                this.Controls.Remove(label1);
                label1.Dispose();
            }

            foreach (Control control in this.Controls)
            {
                if (control is Button)
                {
                    continue;
                }
                control.Dispose();
            }
        }
    }
}
