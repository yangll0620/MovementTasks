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
    public class GoNogoTaskConfig : TouchTaskConfig
    {
        [JsonProperty(PropertyName = "Total Trial Num Per Position Per Session")]
        public int totalTrialNumPerPosSess;

        [JsonProperty(PropertyName = "noGo Trial Num Per Position Per Session")]
        public int nogoTrialNumPerPosSess;


        [JsonProperty(PropertyName = "Times")]
        private GoNogoTimeConfig goNogoTimeConfig = new GoNogoTimeConfig();



        public void LoadJsonFile2GoNogoConfig(string configFile)
        {
            LoadJsonFile2TouchConfig(configFile);

            using (StreamReader r = new StreamReader(configFile))
            {
                string jsonStr = r.ReadToEnd();
                dynamic config = JsonConvert.DeserializeObject(jsonStr);


                totalTrialNumPerPosSess = config["Total Trial Num Per Position Per Session"];
                nogoTrialNumPerPosSess = config["noGo Trial Num Per Position Per Session"];

                // Times Setup
                var configTime = config["Times"];
                goNogoTimeConfig.JsonObject2GoNogoTimeConfig(configTime);
            }
        }

        public GoNogoTimeConfig Get_GoNogoTimeConfig()
        {
            return goNogoTimeConfig;
        }
    }


    public class GoNogoTimeConfig : TouchTimeConfig
    {
        [JsonProperty(PropertyName = "Cue Show Time Range")]
        public float[] tRange_CueTimeS;

        [JsonProperty(PropertyName = "Nogo Show Time Range")]
        public float[] tRange_NogoShowTimeS;


        public void JsonObject2GoNogoTimeConfig(JObject configTime)
        {
            JsonObject2TouchTimeConfig(configTime);

            tRange_CueTimeS = new float[] { float.Parse((string)configTime["Cue Show Time Range"][0]), float.Parse((string)configTime["Cue Show Time Range"][1]) };
            tRange_NogoShowTimeS = new float[] { float.Parse((string)configTime["Nogo Show Time Range"][0]), float.Parse((string)configTime["Nogo Show Time Range"][1]) };
        }
    }
}
