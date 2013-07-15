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
using Excel = Microsoft.Office.Interop.Excel;

namespace TobiiReportingTool
{
    public partial class Form1 : Form
    {
        public Study study { get; set; }
        public Form1()
        {
            InitializeComponent();
            deleteBtn_tooltip.SetToolTip(this.clearTextBox_btn1, "Clear Folder Path");
            study = new Study();
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
            if (study.ValidateDeckFolder(path_textBox.Text))
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
            study.Threshold = thresholdInt;
        }

        public void ParseData()
        {
            this.study.Threshold = Convert.ToInt32(threshold_upDown.Value);
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
            study.LoadData();
            Console.WriteLine("Threshold set to: "+study.Threshold.ToString());

            Reporter AprilONeil = new Reporter(study);

            AprilONeil.GenerateReports();

            AprilONeil.Dispose();

        }

        private void saveThreshold_btn_Click(object sender, EventArgs e)
        {

        }

    }
}
