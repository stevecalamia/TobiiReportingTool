using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TobiiReportingTool
{
    public class Stimulus
    {
        private List<string> aois;
        private string name;
        private Study study;
        private string mediaName;
        private TsvParser gazeData;
        private string[] rawGazeData;
        private TsvParser statsData;
        private string[] rawStatsData;
        private List<string> recordingDates;

        public string Name { get { return name; } }
        public string FileName { get; set; }
        public string HeatmapFileName
        {
            get
            {
                return study.ImageFolderPath + "/Heatmaps/" + name + ".jpg";
            }
        }
        public string AOIsFileName 
        {
            get
            {
                return study.ImageFolderPath + "/AOIs/" + name + ".jpg";
            }
        }
        public string BeeswarmsFileName 
        {
            get
            {
                return study.ImageFolderPath + "/Beeswarms/" + name + ".jpg";
            }
        }
        public string AOIwLabelsFileName { get; set; }
        public string RecordingDates { 
            get {
                string _rtn = "";
                bool first = true;
                foreach (string recDate in recordingDates){
                    if (recDate == "RecordingDate")
                    {
                        continue;
                    }

                    if (first)
                    {
                        _rtn = recDate;
                        first = false;
                    }
                    else
                    {
                        _rtn += ", "+ recDate;
                    }
                }
                return _rtn; 
            } 
            private set { 
                if (!recordingDates.Contains(value))
                {
                    recordingDates.Add(value);
                } 
            } 
        }
        public List<String> RecordingDatesList { get { return recordingDates; } }
        public List<String> AOIs { get { return aois; } }
        public Dictionary<string, int> AOIColOrder { get; set; }
        public List<Participant> Participants { get; set; }
        public List<String> InvalidParticipants { get; set; }
        public List<Participant> ValidParticipants
        {
            get
            {
                List<Participant> _validParticipants = new List<Participant>();
                foreach (Participant p in Participants)
                {
                    if (p.isValid)
                    {
                        _validParticipants.Add(p);
                    }
                }
                return _validParticipants;
            }
        }
        public int TotalValidParticipants { get { return Participants.Count - InvalidParticipants.Count; } }
        public Dictionary<string, Dictionary<string, dynamic>> TimeToFirstFixation;
        public Dictionary<string, Dictionary<string, dynamic>> TrueTimeToFirstFixation;
        public Image NormalImage { get; set; }
        public Image AOIOverlayImage { get; set; }
        public Image AOIOverlayNoLabelImage { get; set; }

        public Stimulus(string _name, Study _study)
        {
            name = _name;
            study = _study;
            parseTobiiStudioData();
            recordingDates = new List<string>();
            getRecordingDates();
            InvalidParticipants = new List<string>();
        }

        private void parseTobiiStudioData()
        {
            Console.WriteLine("Data Folder: " + study.DataFolderPath);
            string datapath = study.DataFolderPath + "\\" + name + ".tsv";

            // TODO: Add try catch statement to catch for misnamed files. ...it can happen.
            rawGazeData = File.ReadAllLines(datapath);
            gazeData = new TsvParser(rawGazeData);

            datapath = study.DataFolderPath + "\\" + name + "_stats.txt";
            rawStatsData = File.ReadAllLines(datapath);
            statsData = new TsvParser(rawStatsData);

            IEnumerable<string> mediaNameQ =
                from dynamic data in gazeData
                select (string) data.MediaName;

            mediaName = mediaNameQ.Distinct().FirstOrDefault();
            Console.WriteLine("MediaName = " + mediaName);

        }
        private void getRecordingDates()
        {
            IEnumerable<string> recordingQuery =
                from dynamic gazeEvent in gazeData
                select (string)gazeEvent.RecordingDate;

            foreach (string recDate in recordingQuery.Distinct()) {
                recordingDates.Add(recDate);
            }
        }

        public float GetValidityData(string participant)
        {
            Participant _participant = Participants.Find(
                delegate(Participant p)
                {
                    return p.Name == participant;
                }
            );
            if (_participant == null)
            {
                _participant = new Participant();
                _participant.Name = participant;
                Participants.Add(_participant);
            }

            if(_participant.ValidityPercentage == 0.0) 
            {
                IEnumerable<dynamic> validityQuery =
                    from dynamic gazeEvent in gazeData
                    where gazeEvent.ParticipantName == participant && gazeEvent.StudioEvent == ""
                    select gazeEvent;

                int numInvalid = 0;
                int gazeEventCount = 0;
                foreach (dynamic gz in validityQuery)
                {
                    gazeEventCount++;
                    int validity = Convert.ToInt32(gz.ValidityLeft) + Convert.ToInt32(gz.ValidityRight);
                    if (validity > 4)
                    {
                        numInvalid++;
                    }
                }

                float numValid = (float)gazeEventCount - (float)numInvalid;
                _participant.ValidityPercentage = (numValid / (float)gazeEventCount) * (float)100.0;
            }

            _participant.isValid = _participant.ValidityPercentage >= study.Threshold ? true : false;
            if (!_participant.isValid && !InvalidParticipants.Contains(_participant.Name)) 
                InvalidParticipants.Add(_participant.Name);

            return _participant.ValidityPercentage;
        }
        public List<Participant> GetParticipants()
        {
            if (Participants == null)
            {
                Participants = new List<Participant>();
            }
            IEnumerable<string> participantQuery =
                from dynamic gazeEvent in gazeData
                select (string)gazeEvent.ParticipantName;

            string lastParticipantName = "";
            foreach (string participantName in participantQuery.Distinct())
            {
                if (lastParticipantName == participantName || participantName == "ParticipantName")
                {
                    continue;
                }
                else
                {
                    Participant _p = Participants.Find(
                        delegate(Participant p)
                        {
                            return p.Name == participantName;
                        }
                    );
                    if (_p == null)
                    {
                        _p = new Participant();
                        _p.Name = participantName;
                        Participants.Add(_p);
                    }
                }
                lastParticipantName = participantName;
            }
            return Participants;
        }
        public List<String> GetAOIs()
        {
            if (aois == null)
            {
                aois = new List<string>();
            }
            IEnumerable<string> aoiQuery =
                from string headerName in rawGazeData[0].Split(new char[] { '\t' })
                where headerName.StartsWith("AOI[")
                select headerName;
            foreach (string aoi in aoiQuery)
            {
                int aoiNameStartCharIndex = aoi.IndexOf('[') + 1;
                int aoiNameEndCharIndex = aoi.IndexOf(']');
                int aoiNameLength = aoiNameEndCharIndex - aoiNameStartCharIndex;
                aois.Add(aoi.Substring(aoiNameStartCharIndex, aoiNameLength));
            }
            return AOIs;
        }
        public Dictionary<string, Dictionary<string, dynamic>> GetAOIsGazeOrder()
        {                                                                                                                  
            Dictionary<string, Dictionary<string, dynamic>> ret = new Dictionary<string, Dictionary<string, dynamic>>();
            TimeToFirstFixation = new Dictionary<string, Dictionary<string, dynamic>>();
            TrueTimeToFirstFixation = new Dictionary<string, Dictionary<string, dynamic>>();
            // Returs a dictionary of dictionarys like this:
            //      (Participant.Name, (aoi, gazeOrder))

            string ttff_str = "Time to First Fixation";
            char[] delimiter = new char[] {'_'};

            if (aois == null || aois.Count < 1) { GetAOIs(); }
            if (Participants == null || Participants.Count < 1) { GetParticipants(); }

            foreach (Participant p in ValidParticipants)
            {
                var ttffQuery =
                    from stats in rawStatsData
                    let line = stats.Split(new char[] { '\t' })
                    where line[0] == p.Name
                    select line;

                foreach (string[] line in ttffQuery)
                {
                    Dictionary<string, dynamic> aoiGazeOrder = new Dictionary<string, dynamic>();
                    Dictionary<string, dynamic> ttff = new Dictionary<string, dynamic>();
                    Dictionary<string, dynamic> true_ttff = new Dictionary<string, dynamic>();

                    foreach (string aoi in AOIs)
                    {
                        string d = new string(delimiter[0], 1);
                        string fieldName = ttff_str + d + mediaName + d + aoi + d + "Sum";
                        int keyIndex = statsData.GetKeyIndex(fieldName);
                        float val = 0;
                        if (line[keyIndex] == "-")
                        {
                            val = (float)7; //(float)AOIs.Count();
                        }
                        else
                        {
                            val = (float)Convert.ToDouble(line[keyIndex]);
                        }
                        ttff.Add(aoi, val);
                        true_ttff.Add(aoi, val == (float)7.0 ? 0 : val);
                    }
                    TimeToFirstFixation.Add(p.Name, ttff);
                    TrueTimeToFirstFixation.Add(p.Name, true_ttff);

                    var gazeOrderQuery =
                        from val in ttff
                        let aoi = val.Key
                        let time = val.Value
                        orderby time
                        ascending
                        select val;
                    int gazeOrderIndex = 1;
                    foreach (var ttffData in gazeOrderQuery)
                    {
                        int gazeOrder = gazeOrderIndex;
                        if (ttffData.Value == AOIs.Count())
                        {
                            gazeOrder = (dynamic)ttffData.Value;
                        }
                        aoiGazeOrder.Add(ttffData.Key, gazeOrder);
                        gazeOrderIndex++;
                    }

                    ret.Add(p.Name, aoiGazeOrder);
                }
            }


            if (AOIColOrder == null) AOIColOrder = new Dictionary<string, int>();

            // Add up all the gaze order values per AOI...
            Dictionary<string, int> gazeOrderSums = new Dictionary<string, int>();
            bool first = true;
            foreach (KeyValuePair<string, Dictionary<string, dynamic>> participantGazeOrder in ret)
            {
                if (first)
                {
                    foreach (string aoi in AOIs)
                    {
                        gazeOrderSums.Add(aoi, participantGazeOrder.Value[aoi]);
                    }
                    first = false;
                }
                else
                {
                    foreach (string aoi in AOIs)
                    {
                        gazeOrderSums[aoi] += participantGazeOrder.Value[aoi];
                    }
                }
            }

            // Order the AOIs by the lowest total ordinal gaze index to highest
            var gazeOrderSumQuery =
                from gazeOrderSum in gazeOrderSums
                let sum = gazeOrderSum.Value
                orderby sum
                ascending
                select gazeOrderSum;

            int colOrderValue = 1;
            foreach (KeyValuePair<string, int> aoiSum in gazeOrderSumQuery)
            {
                // Log what column which AOI is in. It will remain this way from here on out.
                AOIColOrder.Add(aoiSum.Key, colOrderValue);
                colOrderValue++;
            }

            return ret;
        }
        public void AdjustAOIColumnOrder(int offset)
        {
            Dictionary<string, int> newOrder = new Dictionary<string, int>();
            foreach (KeyValuePair<string, int> colOrderValue in AOIColOrder)
            {
                newOrder.Add(colOrderValue.Key, AOIColOrder[colOrderValue.Key] + offset);
            }
            AOIColOrder = newOrder;
        }
        public float GetPercentagePanelistsNotingAOIbyTimeInSeconds(string aoi, float time, float maxtime = 7)
        {
            float retVal = maxtime;
            int numOfParticipantsNoting = 0;
            foreach (Participant p in ValidParticipants)
            {
                if (TimeToFirstFixation[p.Name][aoi] < time)
                {
                    numOfParticipantsNoting++;
                }
            }
            retVal = (float)numOfParticipantsNoting / (float)ValidParticipants.Count * 100;
            return retVal;
        }
        public Dictionary<string, Dictionary<string, dynamic>> GetTotalFixationDurationByAOI()
        {
            Dictionary<string, Dictionary<string, dynamic>> retVal = new Dictionary<string, Dictionary<string, dynamic>>();
            string tfd_str = "Total Fixation Duration";
            char[] delimiter = new char[] { '_' };

            foreach (Participant p in ValidParticipants)
            {
                var tfdQuery =
                    from stats in rawStatsData
                    let line = stats.Split(new char[] { '\t' })
                    where line[0] == p.Name
                    select line;

                foreach (string[] line in tfdQuery)
                {
                    Dictionary<string, dynamic> tfd = new Dictionary<string, dynamic>();
                    foreach (string aoi in AOIs)
                    {
                        string d = new string(delimiter[0], 1);
                        string fieldname = tfd_str + d + mediaName + d + aoi + d + "Sum";
                        int keyIndex = statsData.GetKeyIndex(fieldname);
                        float val = line[keyIndex] == "-" ? (float)0 : (float)Convert.ToDouble(line[keyIndex]);
                        tfd.Add(aoi, val);
                    }
                    retVal.Add(p.Name, tfd);
                }
            }
            return retVal;
        }

        public Dictionary<string, Dictionary<string, float>> GetFixationCountsPerAOI()
        {
            Dictionary<string, Dictionary<string, float>> retVal = new Dictionary<string, Dictionary<string, float>>();
            string fc_str = "Fixation Count (Include Zeros)";
            char[] delimiter = new char[] { '_' };

            foreach (Participant p in ValidParticipants)
            {
                var fcQuery =
                    from stats in rawStatsData
                    let line = stats.Split(new char[] { '\t' })
                    where line[0] == p.Name
                    select line;

                foreach (string[] line in fcQuery)
                {
                    Dictionary<string, float> fc = new Dictionary<string, float>();
                    foreach (string aoi in AOIs)
                    {
                        string d = new string(delimiter[0], 1);
                        string fieldname = fc_str + d + mediaName + d + aoi + d + "Sum";
                        int keyIndex = statsData.GetKeyIndex(fieldname);
                        float val = line[keyIndex] == "-" ? (float)0 : (float)Convert.ToDouble(line[keyIndex]);
                        fc.Add(aoi, val);
                    }
                    retVal.Add(p.Name, fc);
                }
            }
            return retVal;
        }
    }
}
