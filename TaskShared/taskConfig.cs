using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace TaskShared
{
    public class taskConfig
    {
        public string NHPName;
        public string savedFolder;
        public string audioFile_Correct, audioFile_Error;


        // Time Related Variables
        public float[] tRange_ReadyTimeS;
        public float tMax_ReactionTimeS, tMax_ReachTimeS, t_VisfeedbackShowS, t_InterTrialS;
        public float t_JuicerCorrectGivenS;

        // Target Related Variables
        public float targetDiaInch { get; set; }
        public int targetNoOfPositions;
        public int targetDiaPixal;
        public List<int[]> optPostions_OCenter_List;


        // Color Related Variables
        public string BKWaitTrialColorStr, BKTrialColorStr;
        public string CorrFillColorStr, CorrOutlineColorStr, ErrorFillColorStr, ErrorOutlineColorStr;

        public void JSonFile2BaseConfig(string configFile)
        {
            using (StreamReader r = new StreamReader(configFile))
            {
                string jsonStr = r.ReadToEnd();
                dynamic config = JsonConvert.DeserializeObject(jsonStr);

                NHPName = config["NHP Name"];
                savedFolder = (string)config["saved folder"];
                audioFile_Correct = (string)config["audioFile_Correct"];
                audioFile_Error = (string)config["audioFile_Error"];

                // Times Sections
                var configTime = config["Times"];
                tRange_ReadyTimeS = new float[] { float.Parse((string)configTime["Ready Show Time Range"][0]), float.Parse((string)configTime["Ready Show Time Range"][1]) };
                tMax_ReactionTimeS = float.Parse((string)configTime["Max Reaction Time"]);
                tMax_ReachTimeS = float.Parse((string)configTime["Max Reach Time"]);
                t_InterTrialS = float.Parse((string)configTime["Inter Trials Time"]);
                t_VisfeedbackShowS = float.Parse((string)configTime["Visual Feedback Show Time"]);
                t_JuicerCorrectGivenS = float.Parse((string)configTime["Juice Correct Given Time"]);


                // Color Sections
                var configColors = config["Colors"];
                BKWaitTrialColorStr = configColors["Wait Trial Start Background"];
                BKTrialColorStr = configColors["Trial Background"];
                CorrFillColorStr = configColors["Correct Fill"];
                CorrOutlineColorStr = configColors["Correct Outline"];
                ErrorFillColorStr = configColors["Error Fill"];
                ErrorOutlineColorStr = configColors["Error Outline"];


                // Target Sections
                var configTarget = config["Target"];
                targetDiaInch = float.Parse((string)configTarget["Target Diameter (Inch)"]);
                targetNoOfPositions = configTarget["Target No of Positions"];
                optPostions_OCenter_List = new List<int[]>();
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
}
