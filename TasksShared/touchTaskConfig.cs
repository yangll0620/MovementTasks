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
        public TargetNumPosConfig targetNumPosConfig = new TargetNumPosConfig();


        public void LoadJsonFile2TouchConfig(string configFile)
        {
            LoadJsonFile2BaseConfig(configFile);

            using (StreamReader r = new StreamReader(configFile))
            {
                string jsonStr = r.ReadToEnd();
                dynamic config = JsonConvert.DeserializeObject(jsonStr);


                // Times Setup
                var configTime = config["Times"];
                touchTimeConfig.JsonObject2TouchTimeConfig(configTime);

                // Target Setup
                var configTarget = config["Target"];
                targetNumPosConfig.JsonObject2TouchGoTargetConfig(configTarget);
            }
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

    public class TargetNumPosConfig
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
}
