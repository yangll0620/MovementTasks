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
    }
}
