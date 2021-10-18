using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;


namespace TasksShared
{

    public class BaseMoveTaskConfig
    {
        [JsonProperty(PropertyName = "NHP Name")]
        public string NHPName;

        [JsonProperty(PropertyName = "saved folder")]
        public string savedFolder;

        [JsonProperty(PropertyName = "audioFile_Correct")]
        public string audioFile_Correct;

        [JsonProperty(PropertyName = "audioFile_Error")]
        public string audioFile_Error;



        // Time Related Variables
        private BaseTimeConfig baseTimeConfig = new BaseTimeConfig();

        // Color Related Variables
        private BaseColorConfig baseColorConfig = new BaseColorConfig();



        public void LoadJsonFile2BaseConfig(string configFile)
        {
            using (StreamReader r = new StreamReader(configFile))
            {
                string jsonStr = r.ReadToEnd();
                dynamic config = JsonConvert.DeserializeObject(jsonStr);


                JsonString2BaseMainConfig(config);

                // Times Sections
                var configTime = config["Times"];
                baseTimeConfig.JsonString2BaseTimeConfig(configTime);


                // Color
                var configColor = config["Colors"];
                baseColorConfig.JsonString2BaseColorConfig(configColor);
            }
        }

        public void JsonString2BaseMainConfig(JObject config)
        {
            NHPName = (string)config["NHP Name"];
            savedFolder = (string)config["saved folder"];
            audioFile_Correct = (string)config["audioFile_Correct"];
            audioFile_Error = (string)config["audioFile_Error"];
        }

        public void SaveBaseJsonString2TxtFile(string txtSavedfile)
        {
            using (StreamWriter file = File.AppendText(txtSavedfile))
            {
                file.WriteLine(String.Format("{0, -40}:  {1}", "saved folder", savedFolder));
                file.WriteLine(String.Format("{0, -40}:  {1}", "audioFile_Correct", audioFile_Correct));
                file.WriteLine(String.Format("{0, -40}:  {1}", "audioFile_Error", audioFile_Error));
            }
        }
    }


    public class BaseTimeConfig
    {
        [JsonProperty(PropertyName = "Max Reach Time")]
        public float t_MaxReachTimeS;

        [JsonProperty(PropertyName = "Visual Feedback Show Time")]
        public float t_VisfeedbackShowS;

        [JsonProperty(PropertyName = "Inter Trials Time")]
        public float t_InterTrialS;

        [JsonProperty(PropertyName = "Juice Correct Given Time")]
        public float t_JuicerCorrectGivenS;


        public void JsonString2BaseTimeConfig(JObject configTime)
        {
            t_MaxReachTimeS = float.Parse((string)configTime["Max Reach Time"]);
            t_InterTrialS = float.Parse((string)configTime["Inter Trials Time"]);
            t_VisfeedbackShowS = float.Parse((string)configTime["Visual Feedback Show Time"]);
            t_JuicerCorrectGivenS = float.Parse((string)configTime["Juice Correct Given Time"]);
        }

        public void SaveBaseJsonTimeString2TxtFile(string txtSavedfile)
        {
            using (StreamWriter file = File.AppendText(txtSavedfile))
            {
                // Save Time Settings
                file.WriteLine(String.Format("{0, -40}:  {1}", "Max Reach Time (s)", t_MaxReachTimeS.ToString()));
                file.WriteLine(String.Format("{0, -40}:  {1}", "Inter-Trial Time (s)", t_InterTrialS.ToString()));
                file.WriteLine(String.Format("{0, -40}:  {1}", "Visual Feedback Time (s)", t_VisfeedbackShowS.ToString()));
                file.WriteLine(String.Format("{0, -40}:  {1}", "Correct Given Juicer Time (s)", t_JuicerCorrectGivenS.ToString()));
            }
        }
    }


    public class BaseColorConfig
    {
        [JsonProperty(PropertyName = "Wait Trial Start Background")]
        public string BKWaitTrialColorStr;

        [JsonProperty(PropertyName = "Trial Background")]
        public string BKTrialColorStr;

        public void JsonString2BaseColorConfig(JObject configColors)
        {
            BKWaitTrialColorStr = (string)configColors["Wait Trial Start Background"];
            BKTrialColorStr = (string)configColors["Trial Background"];
        }

        public void SaveBaseJsonString2TxtFile(string txtSavedfile)
        {
            using (StreamWriter file = File.AppendText(txtSavedfile))
            {
                file.WriteLine(String.Format("{0, -40}:  {1}", "Wait Trial Start Background", BKWaitTrialColorStr));
                file.WriteLine(String.Format("{0, -40}:  {1}", "Trial Background", BKTrialColorStr));
            }
        }
    }
}
