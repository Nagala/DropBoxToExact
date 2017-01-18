using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Nemiro.OAuth;
using Nemiro.OAuth.LoginForms;
using System.IO;
using ExactOnline.Client.Models;
using ExactOnline.Client.Sdk;
using ExactOnline.Client.Sdk.Controllers;
using DotNetOpenAuth.OAuth2;
namespace DropBox
{
    public partial class Form1 : Form
    {
        
      
        private string CurrentPath = "/";
        public Form1()
        {
            InitializeComponent();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(Properties.Settings.Default.AccessToken1))
            {
                this.GetAccessToken();
            }
        }
        
       
       

        private void GetAccessToken()
        {
            var login = new DropboxLogin("k10o7ajlmmtp8qj","3g69wza3v700xw0");
            login.Owner = this;
            login.ShowDialog();
            if (login.IsSuccessfully)
            {
                Properties.Settings.Default.AccessToken1 = login.AccessToken.Value;
                Properties.Settings.Default.Save();
            }
            else
            {
            MessageBox.Show("error..");
            }
        }
        private void GetFiles()
        {
            OAuthUtility.GetAsync
                (
                "https://api.dropbox.com/1/metadata/auto/",
                new HttpParameterCollection
                 {
                     {"path",Path.Combine(this.CurrentPath, Path.GetFileName(ofd.FileName)).Replace("\\", "/")},
                     {"access_token",Properties.Settings.Default.AccessToken1 }
                 },
                callback: GetFiles_Result
                );
        }
        private void GetFiles_Result(RequestResult result)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<RequestResult>(GetFiles_Result), result);
                return;
            }
            if (result.StatusCode == 200)
            {
               

            }
            else 
            {
                MessageBox.Show("Error..");
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
          
            OAuthUtility.PutAsync
                (
                "https://content.dropboxapi.com/1/files_put/auto/",
                new HttpParameterCollection
                {
            {"access_token",Properties.Settings.Default.AccessToken1},
            {"path",Path.Combine(this.CurrentPath,Path.GetFileName(ofd.FileName)).Replace("\\","/")},
            {"overwrite", "true"},
            {"autorename","true"},
            {ofd.OpenFile()}
                },
                callback: Upload_Result
                );
            
        }

        private static Guid GetCategoryId(ExactOnlineClient client)
        {
            var categories = client.For<DocumentCategory>().Select("ID").Where("Description+eq+'General'").Get();
            var category = categories.First().ID;
            return category;
        }


        private void Upload_Result(RequestResult result)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<RequestResult>(Upload_Result),result);
                return;
            }

            if (result.StatusCode == 200)
            {
                string line;
                string filePath = ofd.FileName;
                string body = "";
                if (File.Exists(filePath))
                {
                    
                    // Read the file and display it line by line.
                    StreamReader file = new StreamReader(filePath);
                    while ((line = file.ReadLine()) != null)
                    {
                         body = line;
                    }
                    file.Close();
                }

                this.GetFiles();
             
                const string clientId = "42942aa2-aa89-4d72-9a21-21bad8f8a29c";
                const string clientSecret = "z0UR3MWZ43wp";

               
                // This can be any url as long as it is identical to the callback url you specified for your app in the App Center.
                var callbackUrl = new Uri("https://YOUR_AUTH0_DOMAIN/login/callback");

                var connector = new Connector(clientId, clientSecret, callbackUrl);
                var client = new ExactOnlineClient(connector.EndPoint, connector.GetAccessToken);
                string subject = Path.Combine(this.CurrentPath, Path.GetFileName(ofd.FileName)).Replace("/", "");
                Document document = new Document
                {

                    Subject = subject,
                    //StreamReader reader = new StreamReader(stream, Encoding.UTF8)
                    Body = body,
                    Category = GetCategoryId(client),/*Guid.Parse("3b6d3833-b31b-423d-bc3c-39c62b8f2b12")*/
                    Type = 55, //Miscellaneous
                    DocumentDate = DateTime.Now.Date,
                };
                bool created = client.For<Document>().Insert(ref document);
               
                Application.Exit();
                
        }
            else 
            {
                if (result["error"].HasValue)
                {
                    MessageBox.Show(result["error"].ToString());
                }
                else
                {
                    MessageBox.Show(result.ToString());
                }
            }
        }
        OpenFileDialog ofd = new OpenFileDialog();
        private void button2_Click(object sender, EventArgs e)
        {

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = ofd.FileName;
            } 
        }
    }
}
