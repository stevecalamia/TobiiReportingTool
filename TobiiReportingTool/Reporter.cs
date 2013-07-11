using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace TobiiReportingTool
{
    public class Reporter
    {
        private Excel.Application ExcelApp;
        private Excel.Workbook ReportWorkbook;
        private Excel.Sheets ReportWorksheets;
        private Excel.Worksheet ValidityWorksheet;

        public Study Study { get; set; }
        public string ReportGenerationDate { get; private set; }

        public Reporter()
        {
        }

        public Reporter(Study aStudy)
        {
            Study = aStudy;

            ExcelApp = new Excel.Application();
            ExcelApp.Visible = true;
            ExcelApp.DisplayAlerts = false;
            ExcelApp.SheetsInNewWorkbook = 1;
            ReportWorkbook = ExcelApp.Workbooks.Add();
            ReportWorksheets = ReportWorkbook.Worksheets;

            ReportGenerationDate = DateTime.Now.ToString("M/d/yyyy HH:mm");
        }

        public void GenerateReports()
        {
            generateValidityReport();
            foreach (Stimulus stimulus in Study.Stimuli)
            {
                // Init Stimulus Report Worksheet
                Excel.Worksheet _stimulusWorksheet = ReportWorksheets.Add();
                _stimulusWorksheet.Name = stimulus.FileName;
                insertHeaderInfo(_stimulusWorksheet);
                _stimulusWorksheet.Range["B3"].Value2 = stimulus.RecordingDates;

                // Run Stimulus Reports
                Excel.Range activeCell;
                activeCell = generateAvgGazeOrderPerAOIReport(stimulus, _stimulusWorksheet);
                activeCell = _stimulusWorksheet.Cells[activeCell.Row + 3, activeCell.Column];

                activeCell = generateAvgFirstNotingTimeReport(stimulus, _stimulusWorksheet, activeCell.Row);
                activeCell = _stimulusWorksheet.Cells[activeCell.Row + 3, activeCell.Column];

                activeCell = generatePercentPanelistsNotingAOIReport(stimulus, _stimulusWorksheet, activeCell.Row);
                activeCell = _stimulusWorksheet.Cells[activeCell.Row + 3, activeCell.Column];

                activeCell = generateAvgGazeTimePerAOIReport(stimulus, _stimulusWorksheet, activeCell.Row);
                activeCell = _stimulusWorksheet.Cells[activeCell.Row + 3, activeCell.Column];

                activeCell = generateAvgFixationCountPerAOIReport(stimulus, _stimulusWorksheet, activeCell.Row);


                // Finalize Stimuls Report Worksheet
                _stimulusWorksheet.Range["A1:B1"].Columns.AutoFit();
            }
            SaveWorkbook();
        }

        public void SaveWorkbook()
        {
            // TODO: REally need to get a trycatch in here in case the file is already open.
            ReportWorkbook.SaveAs(
                Study.DeckFolderPath + "\\Report.xls", 
                Excel.XlFileFormat.xlWorkbookNormal, 
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, 
                Excel.XlSaveAsAccessMode.xlNoChange, 
                Excel.XlSaveConflictResolution.xlLocalSessionChanges,
                false, Type.Missing, Type.Missing, Type.Missing);
        }

        private void generateValidityReport()
        {
            ValidityWorksheet = ReportWorksheets["Sheet1"];
            ValidityWorksheet.Name = "Validity Report";

            Excel.Range recordingDatesCell = ValidityWorksheet.Range["B3"];

            // Fill in static labels
            insertHeaderInfo(ValidityWorksheet);
            ValidityWorksheet.Range["B4"].Value2 = "Validity Report";

            ValidityWorksheet.Range["D4"].Value2 = "Threshold=" + Study.Threshold + "%";

            // Fill out Validity Chart
            int headerRow = 5;
            int panelistColumn = 2;
            string recDates = "";
            List<string> recordingDatesList = new List<string>();
            Excel.Range currentColumn = ValidityWorksheet.Cells[headerRow, panelistColumn];
            currentColumn.Value2 = "Panelist";
            foreach (Stimulus stim in Study.Stimuli)
            {
                currentColumn = currentColumn.Next;
                currentColumn.Value2 = stim.FileName;

                foreach (string recDate in stim.RecordingDatesList)
                {
                    if (!recordingDatesList.Contains(recDate))
                    {
                        recordingDatesList.Add(recDate);
                    }
                }

                foreach (Participant p in stim.GetParticipants())
                {
                    Excel.Range participantCell = findOrCreateCellInRange(ValidityWorksheet, p.Name, panelistColumn, stim.Participants.Count(), (headerRow + 1));
                    ValidityWorksheet.Cells[participantCell.Row, currentColumn.Column].Value2 = (int)stim.GetValidityData(p.Name) + "%";
                }

                // Calculate Validity for each participant per stimulus.
                //ValidityWorksheet.Cells.Item(currentColumn.Row + 1, currentColumn.Column).Value2;
                
            }

            // Output recording dates properly now that they are compiled and deduped.
            int i = 1;
            foreach (string date in recordingDatesList)
            {
                if (i == 1) { }
                else if (i == 2)
                    recDates = date;
                else
                    recDates = recDates + ", " + date;
                i++;
            }
            recordingDatesCell.Value2 = recDates;


            // LAST STEP: Formatting
            ValidityWorksheet.Range["A1:B1"].Columns.AutoFit();
        }

        private void runReport(string reportName, Stimulus stim, Excel.Worksheet ws, int topRow, int leftCol = 2)
        {
            insertReportHeader(ws, topRow, leftCol, stim, reportName); 

            switch (reportName)
            {
                case "Individual and Average Gaze Order per AOI (Area Of Interest)":
                    break;
                case "FirstNoting":
                    break;
                case "PercentNoting":
                    break;
                case "AvgGazeTime":
                    break;
                case "FixationCount":
                    break;
            }


        }


        // REPORTS
        private Excel.Range generateAvgGazeOrderPerAOIReport(Stimulus stim, Excel.Worksheet ws, int topRow = 4, int leftCol = 2) 
        {
            // Set up basic info
            insertReportHeader(ws, topRow, leftCol, stim, "Individual and Average Gaze Order per AOI (Area Of Interest)");

            // Calculate Gaze Order for each participant
            Dictionary<string, Dictionary<string, dynamic>> _AOIGazeOrderByParticipant = stim.GetAOIsGazeOrder();
            stim.AdjustAOIColumnOrder(leftCol);

            Excel.Range activeCell = insertReportDataTable(ws, stim,(Dictionary<string, Dictionary<string,dynamic>>) _AOIGazeOrderByParticipant, topRow);

            // Calculate Percent First Noting (per AOI)
            activeCell = ws.Cells[activeCell.Row + 2, leftCol];
            activeCell.Value2 = "% First Noting";

            foreach (string aoi in stim.AOIs)
            {
                activeCell = activeCell.Next;
                string address = ws.Cells[topRow + 1, activeCell.Column].Address + ":" + ws.Cells[topRow + stim.TotalValidParticipants + 1, activeCell.Column].Address;
                string formula = "=COUNTIF("+address+",1)/COUNT("+address+") *100";
                activeCell.Value2 = formula;
            }

            // Calculate Percent Revisited (per AOI)
            activeCell = ws.Cells[activeCell.Row + 1, leftCol];
            activeCell.Value2 = "% Revisited";

            activeCell = insertMinMaxMeanMedianStdDevFormulas(stim, ws, activeCell, leftCol, topRow+2);
            activeCell = ws.Cells[activeCell.Row, leftCol];

            // List excluded panelists, if any
            activeCell = addInvalidParticipants(stim, ws, activeCell);

            // Add in questions:
            //  What is the typical path? Is there one dominant path or several paths?
            //  Are key elements keeping their eyes in one zone? Are missed items outside the path? 
            activeCell = ws.Cells[activeCell.Row + 2, leftCol];
            activeCell.Value2 = "What is the typical path? Is there one dominant path or several paths?";
            activeCell = ws.Cells[activeCell.Row + 1, leftCol];
            activeCell.Value2 = "Are key elements keeping their eyes in one zone? Are missed items outside the path?";

            return activeCell;

        }
        private Excel.Range generateAvgFirstNotingTimeReport(Stimulus stim, Excel.Worksheet ws, int topRow = 25, int leftCol = 2) 
        {
            // Individual and Average First Noting Time (seconds) per AOI (Area of Interest)

            // Set up basic info
            insertReportHeader(ws, topRow, leftCol, stim, "Individual and Average First Noting Time (seconds) per AOI (Area of Interest)");

            // Fill in the data table header
            Excel.Range activeCell = insertReportDataTable(ws, stim, stim.TrueTimeToFirstFixation, topRow);
            activeCell = ws.Cells[activeCell.Row + 1, leftCol];

            // min
            // max
            // mean
            // median
            // std dev
            activeCell = insertMinMaxMeanMedianStdDevFormulas(stim, ws, activeCell, leftCol, topRow+2);
            activeCell = ws.Cells[activeCell.Row + 2, leftCol];
            // List excluded panelists, if any
            activeCell = addInvalidParticipants(stim, ws, activeCell);

            return activeCell;
        }
        private Excel.Range generatePercentPanelistsNotingAOIReport(Stimulus stim, Excel.Worksheet ws, int topRow, int leftCol = 2) 
        { 
            // Percent Panelists Noting an AOI (Area of Interest) as View Time Increases
            insertReportHeader(ws, topRow, leftCol, stim, "Percent Panelists Noting an AOI (Area of Interest) as View Time Increases");

            // This report is not done by participant, but rather by seconds counting up to total number of seconds
            //   for PnG, that is always 7 seconds
            //   This field is called: % Noting
            
            // Fill in the data table header
            Excel.Range activeCell = insertDataTableHeader(ws, topRow + 2, leftCol, stim, "% Noting");

            for (int numSeconds = 1; numSeconds <= 7; numSeconds++)
            {
                activeCell = ws.Cells[activeCell.Row + 1, leftCol];
                activeCell.Value2 = numSeconds == 1 ? "1 second" : numSeconds + " seconds";
                foreach (string aoi in stim.AOIs)
                {
                    activeCell = ws.Cells[activeCell.Row, stim.AOIColOrder[aoi]];
                    activeCell.Value2 = stim.GetPercentagePanelistsNotingAOIbyTimeInSeconds(aoi, (float)numSeconds);
                }
            }

            activeCell = ws.Cells[activeCell.Row + 1, leftCol];

            // min
            // max
            // mean
            // median
            // std dev
            activeCell = insertMinMaxMeanMedianStdDevFormulas(stim, ws, activeCell, leftCol, topRow+2, 7);
            activeCell = ws.Cells[activeCell.Row + 2, leftCol];

            // List excluded panelists, if any
            activeCell = addInvalidParticipants(stim, ws, activeCell);
            return activeCell;
        }
        private Excel.Range generateAvgGazeTimePerAOIReport(Stimulus stim, Excel.Worksheet ws, int topRow, int leftCol = 2) 
        {
            // Individual and Average Gaze Time (seconds) per AOI (Area Of Interest)
            insertReportHeader(ws, topRow, leftCol, stim, "Individual and Average Gaze Time (seconds) per AOI (Area Of Interest)");
            
            // Fill in the data table header
            Excel.Range activeCell = insertReportDataTable(ws, stim, stim.GetTotalFixationDurationByAOI(), topRow);
                
            activeCell = ws.Cells[activeCell.Row + 1, leftCol];

            // Calculate % Total View Time
            activeCell = ws.Cells[activeCell.Row + 2, leftCol];
            activeCell.Value2 = "% Total View Time";

            foreach (string aoi in stim.AOIs)
            {
                activeCell = ws.Cells[activeCell.Row,stim.AOIColOrder[aoi]];
                int topDatarow = topRow + 3;
                int bottomDatarow = topDatarow + stim.ValidParticipants.Count;
                int dataCol = stim.AOIColOrder[aoi];
                int rightDataCol = leftCol + stim.AOIs.Count;
                string topLeftAddress = ws.Cells[topDatarow,leftCol+1].Address;
                string bottomRightAddress = ws.Cells[bottomDatarow,rightDataCol].Address;
                string topDataColumnAddress = ws.Cells[topDatarow, dataCol].Address;
                string bottomDataColumnAddress = ws.Cells[bottomDatarow,dataCol].Address;

                string formula = "=SUM("+topDataColumnAddress+":"+bottomDataColumnAddress+")/SUM("+topLeftAddress+":"+bottomRightAddress+")"; // "=COUNTIF("+address+",1)/COUNT("+address+") *100";
                activeCell.Value2 = formula;
            }
            // min
            // max
            // mean
            // median
            // std dev
            activeCell = insertMinMaxMeanMedianStdDevFormulas(stim, ws, activeCell, leftCol, topRow+2);
            activeCell = ws.Cells[activeCell.Row + 2, leftCol];

            // List excluded panelists, if any
            activeCell = addInvalidParticipants(stim, ws, activeCell);
            return activeCell;
        }
        private Excel.Range generateAvgFixationCountPerAOIReport(Stimulus stim, Excel.Worksheet ws, int topRow, int leftCol = 2)
        {
            insertReportHeader(ws, topRow, leftCol, stim, "Individual and Average Fixation Count per AOI (Area Of Interest)");
 
            // Fill in the data table header
            Excel.Range activeCell = insertDataTableHeader(ws, topRow + 2, leftCol, stim);
            
            // Fill in the data
            Dictionary<string, Dictionary<string, float>> _fixationCountByAOI = stim.GetFixationCountsPerAOI();
            foreach (Participant p in stim.ValidParticipants)
            {
                activeCell = ws.Cells[activeCell.Row + 1, leftCol];
                activeCell.Value2 = p.Name;
                foreach (string aoi in stim.AOIs)
                {
                    activeCell = ws.Cells[activeCell.Row, stim.AOIColOrder[aoi]];
                    activeCell.Value2 = _fixationCountByAOI[p.Name][aoi];
                }
            }

            activeCell = ws.Cells[activeCell.Row + 1, leftCol];

            activeCell = insertMinMaxMeanMedianStdDevFormulas(stim, ws, activeCell, leftCol, topRow+2);
            activeCell = ws.Cells[activeCell.Row + 2, leftCol];

            // List excluded panelists, if any
            activeCell = addInvalidParticipants(stim, ws, activeCell);
            return activeCell;
        }

        // Helper Functions 
        private void insertHeaderInfo(Excel.Worksheet ws)
        {
            ws.Range["A1"].Value2 = "Report Generation Date:";
            ws.Range["B1"].Value2 = ReportGenerationDate;
            ws.Range["A2"].Value2 = "Project Name:";
            ws.Range["B2"].Value2 = Study.ProjectName;
            ws.Range["A3"].Value2 = "Recording Dates:";

            // Make Project Name Bold Blue
            ws.Range["A2:B2"].Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Blue);
            ws.Range["A2:B2"].Font.Bold = true;
        }
        private void insertReportHeader(Excel.Worksheet ws, int row, int col, Stimulus stim, string reportName)
        {
            Excel.Range filenamecell = ws.Cells[row, col < 2 ? 1 : col - 1];
            filenamecell.Value2 = stim.FileName;
            filenamecell.Columns.AutoFit();

            ws.Cells[row, col < 2 ? 2 : col].Value2 = reportName;
        }
        private Excel.Range insertDataTableHeader(Excel.Worksheet ws, int row, int col, Stimulus stim, string rowNameHeader = "Panelist")
        {
            // Fill in the data table header
            Excel.Range activeCell = ws.Cells[row, col];
            activeCell.Value2 = rowNameHeader;
            foreach (KeyValuePair<string, int> aoi in stim.AOIColOrder)
            {
                activeCell = ws.Cells[activeCell.Row, aoi.Value];
                activeCell.Value2 = aoi.Key;
            }
            return activeCell;
        }
        private Excel.Range insertReportDataTable(Excel.Worksheet ws, Stimulus stim, Dictionary<string, Dictionary<string, dynamic>> participantGazeData, int topRow, int leftCol = 2)
        {
            // Fill in the data table header
            Excel.Range activeCell = insertDataTableHeader(ws, topRow + 2, leftCol, stim);

            foreach (KeyValuePair<string, Dictionary<string, dynamic>> p in participantGazeData)
            {
                // Fill in Participant Name
                activeCell = ws.Cells[activeCell.Row + 1, leftCol];
                activeCell.Value2 = p.Key;

                // Fill in the value for each AOI
                foreach (KeyValuePair<string, dynamic> aoiDataPair in p.Value)
                {
                    activeCell = ws.Cells[activeCell.Row, stim.AOIColOrder[aoiDataPair.Key]];
                    activeCell.Value2 = aoiDataPair.Value;
                }
            }
            return activeCell;
        }
        private Excel.Range insertMinMaxMeanMedianStdDevFormulas(Stimulus stim, Excel.Worksheet ws, Excel.Range activeCell, int leftCol, int dataHeaderRow, int rows = 0)
        {
            rows = rows == 0 ? stim.TotalValidParticipants + 1 : rows;
            // Calculate Min (Float) 0.00
            activeCell = ws.Cells[activeCell.Row + 1, leftCol];
            activeCell.Value2 = "Min";

            foreach (string aoi in stim.AOIs)
            {
                activeCell = activeCell.Next;
                string formula =  "=MIN(" + ws.Cells[dataHeaderRow + 1, activeCell.Column].Address + ":" + ws.Cells[dataHeaderRow + rows, activeCell.Column].Address + ")";
                activeCell.Value2 = formula;
            }

            // Calculate Max (Float) 0.00
            activeCell = ws.Cells[activeCell.Row + 1, leftCol];
            activeCell.Value2 = "Max";

            foreach (string aoi in stim.AOIs)
            {
                activeCell = activeCell.Next;
                string formula =  "=MAX(" + ws.Cells[dataHeaderRow + 1, activeCell.Column].Address + ":" + ws.Cells[dataHeaderRow + rows, activeCell.Column].Address + ")";
                activeCell.Value2 = formula;
            }

            // Calculate Mean (Float) 0.00
            activeCell = ws.Cells[activeCell.Row + 1, leftCol];
            activeCell.Value2 = "Mean";

            foreach (string aoi in stim.AOIs)
            {
                activeCell = activeCell.Next;
                string formula =  "=AVERAGE(" + ws.Cells[dataHeaderRow + 1, activeCell.Column].Address + ":" + ws.Cells[dataHeaderRow + rows, activeCell.Column].Address + ")";
                activeCell.Value2 = formula;
            }

            // Calculate Median (Float) 0.00
            activeCell = ws.Cells[activeCell.Row + 1, leftCol];
            activeCell.Value2 = "Median";

            foreach (string aoi in stim.AOIs)
            {
                activeCell = activeCell.Next;
                string formula =  "=MEDIAN(" + ws.Cells[dataHeaderRow + 1, activeCell.Column].Address + ":" + ws.Cells[dataHeaderRow + rows, activeCell.Column].Address + ")";
                activeCell.Value2 = formula;
            }

            // Calculate Std Dev (Float) 0.00
            activeCell = ws.Cells[activeCell.Row + 1, leftCol];
            activeCell.Value2 = "Std Dev";

            foreach (string aoi in stim.AOIs)
            {
                activeCell = activeCell.Next;
                string formula =  "=STDEV(" + ws.Cells[dataHeaderRow + 1, activeCell.Column].Address + ":" + ws.Cells[dataHeaderRow + rows, activeCell.Column].Address + ")";
                activeCell.Value2 = formula;
            }
            return activeCell;
        }
        private Excel.Range insertStimulusImages(Stimulus stim, Excel.Worksheet ws, Excel.Range activeCell)
        {
            
            return activeCell;
        }
        private Excel.Range addInvalidParticipants(Stimulus stim, Excel.Worksheet ws, Excel.Range startCell)
        {
            Excel.Range activeCell = startCell;
            if (stim.InvalidParticipants.Count > 0)
            {
                activeCell.Value2 = "Panelist(s) were excluded because their %Valid were below the threshold!";
                foreach (string pName in stim.InvalidParticipants)
                {
                    activeCell = ws.Cells[activeCell.Row + 1, activeCell.Column];
                    activeCell.Value2 = pName;
                }
            }
            return activeCell;
        }
        private Excel.Range findOrCreateCellInRange(Excel.Worksheet ws, string search, int column, int rangeLength, int startRow = 1)
        {
            Excel.Range returnCell = null;
            Excel.Range topCell = ws.Cells[startRow, column];
            Excel.Range bottomCell = ws.Cells[startRow + rangeLength - 1, column];
            Excel.Range cells = ws.Range[topCell.Address+":"+bottomCell.Address];

            foreach (Excel.Range cell in cells)
            {
                if (cell.Value2 == search || cell.Value2 == null)
                {
                    cell.Value2 = search;
                    returnCell = cell;
                    break;
                }
            }

            return returnCell;
        }
        
    }
}
