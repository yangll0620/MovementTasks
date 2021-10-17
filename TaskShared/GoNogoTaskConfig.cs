using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TaskShared
{
    public class GoNogoTaskConfig : taskConfig
    {
        public int totalTrialNumPerPosSess, nogoTrialNumPerPosSess;
        public float[] tRange_CueTimeS, tRange_NogoShowTimeS;
        public string goFillColorStr, nogoFillColorStr, cueCrossingColorStr;

        public void JSonFile2Config(string configFile)
        {
            JSonFile2BaseConfig(configFile);

            using (StreamReader r = new StreamReader(configFile))
            {
                string jsonStr = r.ReadToEnd();
                dynamic config = JsonConvert.DeserializeObject(jsonStr);

                totalTrialNumPerPosSess = config["Total Trial Num Per Position Per Session"];
                nogoTrialNumPerPosSess = (int)config["noGo Trial Num Per Position Per Session"];

                var configTime = config["Times"];
                tRange_CueTimeS = new float[] { float.Parse((string)configTime["Cue Show Time Range"][0]), float.Parse((string)configTime["Cue Show Time Range"][1]) };
                tRange_NogoShowTimeS = new float[] { float.Parse((string)configTime["Nogo Show Time Range"][0]), float.Parse((string)configTime["Nogo Show Time Range"][1]) };

                var configColors = config["Colors"];
                goFillColorStr = configColors["Go Fill Color"];
                nogoFillColorStr = configColors["noGo Fill Color"];
                cueCrossingColorStr = configColors["Cue Crossing Color"];
            }
        }
    }
}
