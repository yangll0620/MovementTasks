using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksShared
{
    public class TouchTaskConfig : BaseMoveTaskConfig
    {

        [JsonProperty(PropertyName = "Times")]
        private TouchTimeConfig touchTimeConfig = new TouchTimeConfig();

        [JsonProperty(PropertyName = "Target")]
        private TouchTargetNumPosConfig touchTargetNumPosConfig = new TouchTargetNumPosConfig();


        [JsonProperty(PropertyName = "Colors")]
        private TouchColorConfig touchColorConfig = new TouchColorConfig();


        public void LoadJsonFile2TouchConfig(string configFile)
        {

            using (StreamReader r = new StreamReader(configFile))
            {
                string jsonStr = r.ReadToEnd();
                dynamic config = JsonConvert.DeserializeObject(jsonStr);


                JsonString2TouchMainConfig(config);

                // Times Setup
                var configTime = config["Times"];
                touchTimeConfig.JsonObject2TouchTimeConfig(configTime);

                // Target Setup
                var configTarget = config["Target"];
                touchTargetNumPosConfig.JsonObject2TouchGoTargetConfig(configTarget);


                // Color
                var configColor = config["Colors"];
                touchColorConfig.JsonString2TouchColorConfig(configColor);
            }
        }

        public void JsonString2TouchMainConfig(JObject config)
        {
            JsonString2BaseMainConfig(config);
        }

        public void SaveTouchJsonString2TxtFile(string txtSavedfile)
        {
            SaveBaseJsonString2TxtFile(txtSavedfile);
        }
    }

    public class TouchTimeConfig : BaseTimeConfig
    {
        [JsonProperty(PropertyName = "Ready Show Time Range")]
        public float[] tRange_ReadyTimeS;


        [JsonProperty(PropertyName = "Max Reaction Time")]
        public float t_MaxReactionTimeS;
        

        public void JsonObject2TouchTimeConfig(JObject configTime)
        {
            JsonString2BaseTimeConfig(configTime);

            t_MaxReactionTimeS = float.Parse((string)configTime["Max Reaction Time"]);
            tRange_ReadyTimeS = new float[] { float.Parse((string)configTime["Ready Show Time Range"][0]), float.Parse((string)configTime["Ready Show Time Range"][1]) };
        }
        public void SaveTouchJsonTimeString2TxtFile(string txtSavedfile)
        {
            SaveBaseJsonTimeString2TxtFile(txtSavedfile);
            using (StreamWriter file = File.AppendText(txtSavedfile))
            {
                // Save Time Settings
                file.WriteLine(String.Format("{0, -40}:  {1}", "Max Reaction Time (s)", t_MaxReactionTimeS.ToString()));
                file.WriteLine(String.Format("{0, -40}:  [{1} {2}]", "Ready Interface Show Time Range (s)", tRange_ReadyTimeS[0].ToString(), tRange_ReadyTimeS[1].ToString()));
            }
        }

  
    }

    public class TouchTargetNumPosConfig
    {
        [JsonProperty(PropertyName = "Target Diameter (Inch)")]
        public float targetDiaInch;

        [JsonProperty(PropertyName = "Target No of Positions")]
        public int targetNoOfPositions;

        [JsonProperty(PropertyName = "Optional Positions")]
        public List<int[]> optPostions_OCenter_List = new List<int[]>();

        public void JsonObject2TouchGoTargetConfig(JObject configTarget)
        {

            targetDiaInch = float.Parse((string)configTarget["Target Diameter (Inch)"]);
            targetNoOfPositions = int.Parse((string)configTarget["Target No of Positions"]);
            dynamic tmp = configTarget["Optional Positions"];
            foreach (var xyPos in tmp)
            {
                int a = int.Parse((string)xyPos[0]);
                int b = int.Parse((string)xyPos[1]);
                optPostions_OCenter_List.Add(new int[] { a, b });
            }
        }

        public void SaveTouchJsonTouchGoTargetNumPosString2TxtFile(string txtSavedfile)
        {
            using (StreamWriter file = File.AppendText(txtSavedfile))
            {
                file.WriteLine(String.Format("{0, -40}:  {1}", "Target Diameter (Inch)", targetDiaInch.ToString()));
                file.WriteLine(String.Format("{0, -40}:  {1}", "Number of Target Positions", targetNoOfPositions.ToString()));
                file.WriteLine("Center Coordinates of Each Target (Pixal, (0,0) in Screen Center, Right and Down Direction is Positive):");
                for (int i = 0; i < optPostions_OCenter_List.Count; i++)
                {
                    int[] position = optPostions_OCenter_List[i];
                    file.WriteLine(String.Format("{0, -40}:{1}, {2}", "Postion " + i.ToString(), position[0], position[1]));
                }
            }
        }
    }


    public class TouchColorConfig : BaseColorConfig
    {
        [JsonProperty(PropertyName = "Go Fill Color")]
        public string goFillColorStr;


        [JsonProperty(PropertyName = "Correct Fill")]
        public string CorrFillColorStr;


        [JsonProperty(PropertyName = "Correct Outline")]
        public string CorrOutlineColorStr;

        [JsonProperty(PropertyName = "Error Fill")]
        public string ErrorFillColorStr;

        [JsonProperty(PropertyName = "Error Outline")]
        public string ErrorOutlineColorStr;


        public void JsonString2TouchColorConfig(JObject configColors)
        {
            JsonString2BaseColorConfig(configColors);

            goFillColorStr = (string)configColors["Go Fill Color"];
            CorrFillColorStr = (string)configColors["Correct Fill"];
            CorrOutlineColorStr = (string)configColors["Correct Outline"];
            ErrorFillColorStr = (string)configColors["Error Fill"];
            ErrorOutlineColorStr = (string)configColors["Error Outline"];
        }


        public void SaveTouchJsonColorString2TxtFile(string txtSavedfile)
        {
            SaveBaseJsonString2TxtFile(txtSavedfile); 
            using (StreamWriter file = File.AppendText(txtSavedfile))
            {
                file.WriteLine(String.Format("{0, -40}:  {1}", "Go Target Fill Color", goFillColorStr));
                file.WriteLine(String.Format("{0, -40}:  {1}", "Fill Color for Correct Feedback", CorrFillColorStr));
                file.WriteLine(String.Format("{0, -40}:  {1}", "Outline Color for Correct Feedback", CorrOutlineColorStr));
                file.WriteLine(String.Format("{0, -40}:  {1}", "Fill Color for Error Feedback", ErrorFillColorStr));
                file.WriteLine(String.Format("{0, -40}:  {1}", "Outline Color for Error Feedback", ErrorOutlineColorStr));
            }
        }
    }
}
