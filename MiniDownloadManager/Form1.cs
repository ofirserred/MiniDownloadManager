using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace MiniDownloadManager
{
    public partial class Form1 : Form
    {
        private DownloadFile selectedFile;

        public Form1()
        {
            InitializeComponent();
        }

        // fetch files and show the one with the highest score
        private async void Form1_Load_1(object sender, EventArgs e)
        {
            var files = await GetFilesAsync();
            selectedFile = files.OrderByDescending(f => f.Score).First();
            lblTitle.Text = selectedFile.Title;

            using (var client = new HttpClient())
            {
                var stream = await client.GetStreamAsync(selectedFile.ImageURL);
                picImage.Image = Image.FromStream(stream);
            }
        }

        // fetch the list from remote json
        private async Task<List<DownloadFile>> GetFilesAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                string url = "https://4qgz7zu7l5um367pzultcpbhmm0thhhg.lambda-url.us-west-2.on.aws/";
                string json = await client.GetStringAsync(url);
                return JsonConvert.DeserializeObject<List<DownloadFile>>(json);
            }
        }

        // download and open the file
        private async void btnDownload_Click_1(object sender, EventArgs e)
        {
            if (selectedFile == null)
            {
                MessageBox.Show("No file selected. Please restart the application.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string tempPath = System.IO.Path.GetTempPath();
            string fileName = System.IO.Path.GetFileName(selectedFile.FileURL);
            string fullPath = System.IO.Path.Combine(tempPath, fileName);

            if (System.IO.File.Exists(fullPath))
            {
                var result = MessageBox.Show(
                    "The file already exists in the temp folder. Do you want to open it?",
                    "File Already Exists",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information
                );
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        System.Diagnostics.Process.Start(fullPath);
                    }
                    catch { }

                    System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{fullPath}\"");
                }
                return;
            }

            using (HttpClient client = new HttpClient())
            {
                var bytes = await client.GetByteArrayAsync(selectedFile.FileURL);
                System.IO.File.WriteAllBytes(fullPath, bytes);
            }

            MessageBox.Show("The file was downloaded successfully! The folder will open automatically.", "Download Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);

            try
            {
                System.Diagnostics.Process.Start(fullPath);
            }
            catch { }

            // show the folder
            System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{fullPath}\"");
        }
    }
}
