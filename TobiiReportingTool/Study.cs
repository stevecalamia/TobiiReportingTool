using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Excel = Microsoft.Office.Interop.Excel;

namespace TobiiReportingTool
{
    public class Study
    {
        private string deckFolderPath;
        private DirectoryInfo deckFolder { get; set; }
        private DirectoryInfo dataFolder { get; set; }
        private DirectoryInfo imageFolder { get; set; }
        private DirectoryInfo stimFolder { get; set; }

        public string ProjectName { get; set; }
        public string DeckName { get; set; }
        public string DeckFolderPath
        {
            get { return deckFolderPath; }
            set { ValidateDeckFolder(value); } 
        }
        public string DataFolderPath { get; private set; }
        public string ImageFolderPath { get; private set; }
        public List<Participant> Participants { get; private set; }
        public List<Stimulus> Stimuli { get; private set; }
        public Int32 Threshold { get; set; }
        public Dictionary<string, int> AOIColOrder = new Dictionary<string, int>();

        public Excel.Workbook Workbook { get; set; }

        public Study() {
        } // If there's no directory to look at, then we have nothing to set up.

        public Study(string baseFolder)
        {
            ValidateDeckFolder(baseFolder);
        }

        public void LoadData()
        {
            getStimuli();
            getParticipants();
            foreach (Stimulus stim in Stimuli)
            {
                stim.GetAOIs();
            }
        }

        
        public Boolean ValidateDeckFolder(string folderPath, Boolean setFolderProps = true)
        {
            bool _retVal = false;
            string _dataFolderPath;
            string _imageFolderPath;

            if (folderPath != DeckFolderPath && Directory.Exists(folderPath))
            {
                _dataFolderPath = folderPath + "\\Data";
                _imageFolderPath = folderPath + "\\Images";

                if (setFolderProps)
                {
                    deckFolderPath = folderPath;
                    deckFolder = new DirectoryInfo(deckFolderPath);
                    ProjectName = deckFolder.Name;
                }
                
                if (Directory.Exists(_imageFolderPath))
                {
                    if (setFolderProps)
                    {
                        ImageFolderPath = _imageFolderPath;
                        imageFolder = new DirectoryInfo(ImageFolderPath);
                    }

                    if (Directory.Exists(_dataFolderPath))
                    {
                        if (setFolderProps)
                        {
                            DataFolderPath = _dataFolderPath;
                            dataFolder = new DirectoryInfo(DataFolderPath);
                        }

                        _retVal = true;
                    }
                    else { _retVal = false; }

                }
                else { _retVal = false; }
            }
            else { _retVal = false; }

            return _retVal;
        }

        private void getStimuli()
        {
            DirectoryInfo imgdir = new DirectoryInfo(ImageFolderPath + "\\Stimuli");
            IEnumerable<FileInfo> fileList = imgdir.GetFiles("*.*", SearchOption.AllDirectories);
            string[] imageTypes = { ".png",".jpg",".jpeg",".gif" };

            var queryMatchingFiles =
                from file in fileList
                where imageTypes.Contains(file.Extension.ToLower())
                select file;

            Stimuli = new List<Stimulus>();
            foreach (FileInfo fileName in queryMatchingFiles) 
            {
                Stimulus stim = new Stimulus(fileName.Name.Remove(fileName.Name.IndexOf(".")), this);
                stim.FileName = fileName.Name;
                Stimuli.Add(stim);
            }
        }
        private void getParticipants()
        {
            Participants = new List<Participant>();
        }
    }
}
