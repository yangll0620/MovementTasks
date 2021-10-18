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
        public GoNogoTimeConfig goNogoTimeConfig = new GoNogoTimeConfig();

        [JsonProperty(PropertyName = "Target")]
        public GoNogoTargetNumPosConfig goNogoTargetNumPosConfig = new GoNogoTargetNumPosConfig();


        [JsonProperty(PropertyName = "Colors")]
        public GoNogoColorConfig goNogoColorConfig = new GoNogoColorConfig();



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


        public void SaveGoNogoMainConfig2TxtFile(string txtSavedfile)
        {
            SaveTouchJsonString2TxtFile(txtSavedfile);
            using (StreamWriter file = File.AppendText(txtSavedfile))
            {
                file.WriteLine(String.Format("{0, -40}:  {1}", "Total Trial Number Per Position Per Session", totalTrialNumPerPosSess.ToString()));
                file.WriteLine(String.Format("{0, -40}:  {1}", "noGo Trial Num Per Position Per Session", nogoTrialNumPerPosSess.ToString()));
            }
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


        public void SaveGoNogoJsonTimeString2TxtFile(string txtSavedfile)
        {
            SaveTouchJsonTimeString2TxtFile(txtSavedfile);
            using (StreamWriter file = File.AppendText(txtSavedfile))
            {
                // Save Time Settings
                file.WriteLine(String.Format("{0, -40}:  [{1} {2}]", "Cue Interface Show Time Range (s)", tRange_CueTimeS[0].ToString(), tRange_CueTimeS[1].ToString()));
                file.WriteLine(String.Format("{0, -40}:  [{1} {2}]", "Nogo Interface Show Range Time (s)", tRange_NogoShowTimeS[0].ToString(), tRange_NogoShowTimeS[1].ToString()));
            }
        }
    }

    public class GoNogoTargetNumPosConfig : TouchTargetNumPosConfig
    {
        public void JsonObject2GoNogoTargetConfig(JObject configTargetNumPos)
        {
            JsonObject2TouchGoTargetConfig(configTargetNumPos);
        }

        public void SaveGoNogoJsonTouchGoTargetNumPosString2TxtFile(string txtSavedfile)
        {
            SaveTouchJsonTouchGoTargetNumPosString2TxtFile(txtSavedfile);
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

        public void SaveGoNogoJsonColorString2TxtFile(string txtSavedfile)
        {
            SaveTouchJsonColorString2TxtFile(txtSavedfile);
            using (StreamWriter file = File.AppendText(txtSavedfile))
            {
                // Save Time Settings
                file.WriteLine(String.Format("{0, -40}:  {1}", "Crossing Cue Color", cueCrossingColorStr));
                file.WriteLine(String.Format("{0, -40}:  {1}", "Nogo Target Fill Color", nogoFillColorStr));
            }
        }

    }
}
