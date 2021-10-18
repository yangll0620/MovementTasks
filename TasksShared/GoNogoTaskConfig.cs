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

        [JsonProperty(PropertyName = "Target")]
        private GoNogoTargetNumPosConfig goNogoTargetNumPosConfig = new GoNogoTargetNumPosConfig();


        [JsonProperty(PropertyName = "Colors")]
        private GoNogoColorConfig goNogoColorConfig = new GoNogoColorConfig();



        public void LoadJsonFile2GoNogoConfig(string configFile)
        {

            using (StreamReader r = new StreamReader(configFile))
            {
                string jsonStr = r.ReadToEnd();
                dynamic config = JsonConvert.DeserializeObject(jsonStr);

                JsonString2GoNogoMainConfig(config);


                // Times Setup
                var configTime = config["Times"];
                goNogoTimeConfig.JsonObject2GoNogoTimeConfig(configTime);

                // Target Num & Pos Setup
                var configTargetNumPos = config["Target"];
                goNogoTargetNumPosConfig.JsonObject2GoNogoTargetConfig(configTargetNumPos);

                // Color
                var configColor = config["Colors"];
                goNogoColorConfig.JsonString2GoNogoColorConfig(configColor);
            }
        }


        public void JsonString2GoNogoMainConfig(JObject config)
        {
            JsonString2TouchMainConfig(config);

            totalTrialNumPerPosSess = (int)config["Total Trial Num Per Position Per Session"];
            nogoTrialNumPerPosSess = (int)config["noGo Trial Num Per Position Per Session"];
        }

        public GoNogoTimeConfig Get_GoNogoTimeConfig()
        {
            return goNogoTimeConfig;
        }


        public GoNogoTargetNumPosConfig Get_GoNogoTargetNumPosConfig()
        {
            return goNogoTargetNumPosConfig;
        }


        public GoNogoColorConfig Get_GoNogoColorConfig()
        {
            return goNogoColorConfig;
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

    public class GoNogoTargetNumPosConfig : TouchTargetNumPosConfig
    {
        public void JsonObject2GoNogoTargetConfig(JObject configTargetNumPos)
        {
            JsonObject2TouchGoTargetConfig(configTargetNumPos);
        }
    }


    public class GoNogoColorConfig: TouchColorConfig
    {

        [JsonProperty(PropertyName = "noGo Fill Color")]
        public string nogoFillColorStr;

        [JsonProperty(PropertyName = "Cue Crossing Color")]
        public string cueCrossingColorStr;

        public void JsonString2GoNogoColorConfig(JObject configColors)
        {
            JsonString2TouchColorConfig(configColors);

            nogoFillColorStr = (string)configColors["noGo Fill Color"];
            cueCrossingColorStr = (string)configColors["Cue Crossing Color"];
        }

    }
}
