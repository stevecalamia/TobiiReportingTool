using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using Excel = Microsoft.Office.Interop.Excel;

namespace TobiiReportingTool
{
    public partial class Form1 : Form
    {
        public Dictionary<string,dynamic> Settings = new Dictionary<string,dynamic>();
        public Study Study { get; set; }
        private dynamic filename;
        public Form1()
        {
            InitializeComponent();
            deleteBtn_tooltip.SetToolTip(this.clearTextBox_btn1, "Clear Folder Path");
            Study = new Study();
            UpdateThreshold((int)threshold_upDown.Value);
        }

        private void selectFolder_Click(object sender, EventArgs e)
        {
            chooseFolder();
        }
        private void chooseFolder()
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                path_textBox.Text = folderBrowserDialog1.SelectedPath;
                enableGenerateReportButton();
            }
        }

        private void path_textBox_Enter(object sender, EventArgs e)
        {
            pasteOrGetPath();
        }
        private void path_textBox_Enter(object sender, MouseEventArgs e)
        {
            pasteOrGetPath();
        }
        private void pasteOrGetPath()
        {
            if (path_textBox.Text.ToString() == "")
            {
                if (Clipboard.ContainsText() && Directory.Exists(Clipboard.GetText()))
                {
                    path_textBox.Text = Clipboard.GetText();
                }
                else { chooseFolder(); }
            }

            enableGenerateReportButton();
        }
        private void path_textBox_Leave(object sender, EventArgs e)
        {
            enableGenerateReportButton();
        }
        private void enableGenerateReportButton()
        {
            if (Study.ValidateDeckFolder(path_textBox.Text))
            {
                genReport_btn.Enabled = true;
            }
        }

        private void threshold_upDown_ValueChanged(object sender, EventArgs e)
        {
            UpdateThreshold((int)threshold_upDown.Value);
        }
        private void threshold_trackBar_Scroll(object sender, EventArgs e)
        {
            UpdateThreshold(threshold_trackBar.Value);
        }

        public void UpdateThreshold(int thresholdInt)
        {
            threshold_upDown.Value = (decimal)thresholdInt;
            threshold_trackBar.Value = thresholdInt;
            Study.Threshold = thresholdInt;
        }

        public void ParseData()
        {
            this.Study.Threshold = Convert.ToInt32(threshold_upDown.Value);
            String basePath = path_textBox.Text;
            String exportFile = basePath + "\\DataExport.tsv";

            string[] exportData = System.IO.File.ReadAllLines(exportFile);
            IEnumerable<String[]> header =
                from line in exportData
                let fields = line.Split(new char[] {'\t'})
                where fields.Contains("AOI")
                select fields;

        }

        private void clearTextBox_btn1_Click(object sender, EventArgs e)
        {
            path_textBox.Focus();
            path_textBox.Text = "";
        }

        private void genReport_btn_Click(object sender, EventArgs e)
        {

            // Load up Study with data
            Study.LoadData();
            Console.WriteLine("Threshold set to: "+Study.Threshold.ToString());

            Reporter AprilONeil = new Reporter(Study);

            AprilONeil.GenerateReports();

            AprilONeil.Dispose();

        }

        private void saveThreshold_btn_Click(object sender, EventArgs e)
        {
            SaveSettings("Threshold", threshold_trackBar.Value);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadSettings();
        }

        private void SaveSettings(string setting, dynamic value)
        {
            if (Settings.ContainsKey(setting))
            {
                Settings[setting] = value;
            }
            else
            {
                Settings.Add(setting, value);
            }
            try {
                FileStream fsSource = new FileStream(filename, FileMode.Open, FileAccess.Read);
                XmlSerializer serializer = new XmlSerializer(typeof(SettingsItem[]), new XmlRootAttribute() { ElementName = "Settings"});
                serializer.Serialize(fsSource, Settings.Select(kv => new SettingsItem() { name = kv.Key, value = kv.Value }).ToArray());
            }
            catch (FileNotFoundException ioEx)
            {
                MessageBox.Show(@"
We're sorry, but the settings file was not found. This may be a program error.
Please contact support@tobii.com, and send them this information:
" + ioEx.Message, "Settings File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            catch (InvalidOperationException ioEx)
            {
                Console.WriteLine(ioEx.Message);
                Console.WriteLine(ioEx.StackTrace);
                Console.WriteLine(ioEx.InnerException);
            }

        }
        private void LoadSettings()
        {
            filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TRTsettings.xml");
            if (File.Exists(filename))
            {
                try
                {
                    FileStream fsSource = new FileStream(filename, FileMode.Open, FileAccess.Read);
                    XmlSerializer serializer = new XmlSerializer(typeof(SettingsItem[]), new XmlRootAttribute() { ElementName = "Settings" });
                    Settings = ((SettingsItem[])serializer.Deserialize(fsSource)).ToDictionary(i => i.name, i => i.value);

                    if (Settings.ContainsKey("ResourceFolder"))
                    {
                        path_textBox.Text = Settings["ResourceFolder"];
                    }

                    if (Settings.ContainsKey("Threshold"))
                    {
                        UpdateThreshold(Convert.ToInt32(Settings["Threshold"]));
                    }

                }
                catch (FileNotFoundException ioEx)
                {
                    string msg = @"
We're sorry, but the settings file was not found.
If this is the first time you're seeing this message, try saving your Threshold.
Otherwise, this may be a program error.  
If so, please contact support@tobii.com, and send them this information:

";
                    MessageBox.Show(msg + ioEx.Message + "\n" + ioEx.StackTrace, "Settings File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }
            }
            else
            {
                File.Create(filename).Dispose();
            }

        }

    }
}
