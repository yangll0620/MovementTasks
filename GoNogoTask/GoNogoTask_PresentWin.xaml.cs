using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using swf = System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TasksShared;

namespace GoNogoTask
{
    /// <summary>
    /// Interaction logic for GoNogoTask_PresentWin.xaml
    /// </summary>
    public partial class GoNogoTask_PresentWin : Window
    {
        // --------------enumerate ----------------------//
        public enum TargetType
        {
            Nogo,
            Go,
        }

        public enum TrialExeResult
        {
            readyWaitTooShort,
            cueWaitTooShort,
            goReactionTimeToolong,
            goReachTimeToolong,
            goHit,
            goClose,
            goMiss,
            nogoMoved,
            nogoSuccess
        }

        public enum ScreenTouchState
        {
            Idle,
            Touched
        }

        private enum GoTargetTouchState
        {
            goHit, // at least one finger inside the circleGo
            goMissed // touched with longer distance 
        }

        private enum PressedStartpad
        {
            No,
            Yes
        }

        private enum GiveJuicerState
        {
            No,
            CorrectGiven
        }

        private enum StartPadHoldState
        {
            HoldEnough,
            HoldTooShort
        }


        MainWindow parentMainUI;

        // objects of Go cirle, nogo Rectangle, lines of the crossing, and two white points
        Ellipse circleGo;
        Rectangle rectNogo;
        Crossing crossing;

        // diameter for crossing, circle, and square
        int targetDiameterPixal;

        string file_saved;
        System.Media.SoundPlayer player_Correct, player_Error;

        private List<int[]> optPostions_OTopLeft_List;
        private int targetPosNum;
        private int totalTrialNumPerPosSess, nogoTrialNumPerPosSess;

        // Wait Time Range for Each Event, and Max Reaction and Reach Time
        float[] tRange_ReadyTimeS, tRange_CueTimeS, tRange_NogoShowTimeS;
        float tMax_ReactionTimeMS, tMax_ReachTimeMS;
        int t_VisfeedbackShowMS, t_InterTrialMS, t_JuicerCorrectGivenMS;


        // ColorBrushes 
        private SolidColorBrush brush_goCircleFill, brush_nogoRectFill, brush_CueCrossing;
        private SolidColorBrush brush_BKWaitTrialStart, brush_BKTrial;
        private SolidColorBrush brush_CorrectFill, brush_CorrOutline, brush_ErrorFill, brush_ErrorOutline;


        // Varaibles for Current Session, Trials Left in Current Session (Realtime Feedback)
        int currSessi, trialNLeftInSess;
        //TargetExeFeedback_List: totalGoTrials, successGoTrials, totalNogoTrials, successNogoTrials
        private List<int[]> TargetExeFeedback_List;


        Thread thread_ReadWrite_IO8;

        // commands for setting dig out high/low for channels
        static string cmdDigIn1 = "A";
        static string cmdHigh3 = "3";
        static string cmdLow3 = "E";

        // Channel 1 Digital Input for Startpad, Channel 3 for Juicer 
        static string startpad_In = cmdDigIn1;
        static string codeHigh_JuicerPin = cmdHigh3, codeLow_JuicerPin = cmdLow3;

        static int startpad_DigIn_Pressed = 0;
        static int startpad_DigIn_Unpressed = 1;


        static string Code_InitState = "0000";
        static string Code_TouchTriggerTrial = "1110";
        static string Code_ReadyShown = "0110";
        static string Code_ReadyWaitTooShort = "0011";
        static string Code_GoTargetShown = "1010";
        static string Code_GoReactionTooLong = "1100";
        static string Code_GoReachTooLong = "1011";
        static string Code_GoTouched = "1101";
        static string Code_GoTouchedHit = "0100";
        static string Code_GoTouchedMiss = "1000";
        static string Code_CueShown = "0001";
        static string Code_CueWaitTooShort = "0101";
        static string Code_noGoTargetShown = "0010";
        static string Code_noGoEnoughTCorrectFeedback = "0111";
        static string Code_noGoLeftEarlyErrorFeedback = "1001";


        string TDTCmd_InitState, TDTCmd_TouchTriggerTrial, TDTCmd_ReadyShown, TDTCmd_ReadyWaitTooShort;
        string TDTCmd_GoTargetShown, TDTCmd_GoReactionTooLong, TDTCmd_GoReachTooLong, TDTCmd_GoTouched, TDTCmd_GoTouchedHit, TDTCmd_GoTouchedMiss;
        string TDTCmd_CueShown, TDTCmd_CueWaitTooShort;
        string TDTCmd_noGoTargetShown, TDTCmd_noGoEnoughTCorrectFeedback, TDTCmd_noGoLeftEarlyErrorFeedback;

        // Global stopwatch
        Stopwatch globalWatch;

        // Stop Watch for recording the time interval between the first touchpoint and the last touchpoint within One Touch
        Stopwatch tpoints1TouchWatch;
        // the Max Duration for One Touch (ms)
        long tMax_1Touch = 40;


        SerialPort serialPort_IO8;


        GiveJuicerState giveJuicerState;
        PressedStartpad pressedStartpad;
        StartPadHoldState startpadHoldstate; // hold states for Ready, Cue Interfaces
        TrialExeResult trialExeResult;

        // Variables for Various Time Points during trials
        long timePoint_StartpadTouched, timePoint_StartpadLeft;
        long timePoint_Interface_ReadyOnset, timePoint_Interface_CueOnset, timePoint_Interface_TargetOnset;


        // Target Information (posIndex, goNogoType) List for Each Trial Per Session
        private List<int[]> trialTargetInfo_PerSess_List;

        // list storing the position/Timepoint of the touch points when touched down
        List<double[]> downPoints_Pos = new List<double[]>();

        // list storing the position, touched and left Timepoints of the touch points
        // one element: [point_id, touched_timepoint, touched_x, touched_y, left_timepoint, left_x, left_y]
        List<double[]> touchPoints_PosTime = new List<double[]>();


        public GoNogoTask_PresentWin(MainWindow parentWin)
        {
            InitializeComponent();

            parentMainUI = parentWin;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;

            NewVariables();

            GetSetupParameters();

            Generate_IO8EventTDTCmd();
        }

        private void Generate_IO8EventTDTCmd()
        {
            TDTCmd_InitState = SerialPortIO8Manipulate.Convert2_IO8EventCmd_Bit5to8(Code_InitState);
            TDTCmd_TouchTriggerTrial = SerialPortIO8Manipulate.Convert2_IO8EventCmd_Bit5to8(Code_TouchTriggerTrial);
            TDTCmd_ReadyShown = SerialPortIO8Manipulate.Convert2_IO8EventCmd_Bit5to8(Code_ReadyShown);
            TDTCmd_ReadyWaitTooShort = SerialPortIO8Manipulate.Convert2_IO8EventCmd_Bit5to8(Code_ReadyWaitTooShort);
            TDTCmd_GoTargetShown = SerialPortIO8Manipulate.Convert2_IO8EventCmd_Bit5to8(Code_GoTargetShown);
            TDTCmd_GoReactionTooLong = SerialPortIO8Manipulate.Convert2_IO8EventCmd_Bit5to8(Code_GoReactionTooLong);
            TDTCmd_GoReachTooLong = SerialPortIO8Manipulate.Convert2_IO8EventCmd_Bit5to8(Code_GoReachTooLong);
            TDTCmd_GoTouched = SerialPortIO8Manipulate.Convert2_IO8EventCmd_Bit5to8(Code_GoTouched);
            TDTCmd_GoTouchedHit = SerialPortIO8Manipulate.Convert2_IO8EventCmd_Bit5to8(Code_GoTouchedHit);
            TDTCmd_GoTouchedMiss = SerialPortIO8Manipulate.Convert2_IO8EventCmd_Bit5to8(Code_GoTouchedMiss);
            TDTCmd_CueShown = SerialPortIO8Manipulate.Convert2_IO8EventCmd_Bit5to8(Code_CueShown);
            TDTCmd_CueWaitTooShort = SerialPortIO8Manipulate.Convert2_IO8EventCmd_Bit5to8(Code_CueWaitTooShort);
            TDTCmd_noGoTargetShown = SerialPortIO8Manipulate.Convert2_IO8EventCmd_Bit5to8(Code_noGoTargetShown);
            TDTCmd_noGoEnoughTCorrectFeedback = SerialPortIO8Manipulate.Convert2_IO8EventCmd_Bit5to8(Code_noGoEnoughTCorrectFeedback);
            TDTCmd_noGoLeftEarlyErrorFeedback = SerialPortIO8Manipulate.Convert2_IO8EventCmd_Bit5to8(Code_noGoLeftEarlyErrorFeedback);
        }

        private void PrepBef_Present()
        {

            // create a serial Port IO8 instance, and open it
            
            try
            {
                serialPort_IO8.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Message", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            // Thread for Read and Write IO8
            thread_ReadWrite_IO8 = new Thread(new ThreadStart(Thread_ReadWrite_IO8));


            // init a global stopwatch
            globalWatch = new Stopwatch();
            tpoints1TouchWatch = new Stopwatch();

            // Init Trial Information
            Update_FeedbackTrialsInformation();

            //Write Trial Setup Information
            //Write_TrialSetupInformation();
        }

        private void Thread_ReadWrite_IO8()
        {/* Thread for reading/writing serial port IO8*/

            Stopwatch startpadReadWatch = new Stopwatch();
            long startpadReadInterval = 30;

            try
            {
                serialPort_IO8.WriteLine(codeLow_JuicerPin);
            }
            catch (InvalidOperationException) { }

            startpadReadWatch.Start();
            while (serialPort_IO8.IsOpen)
            {
                try
                {
                    // ----- Juicer Control
                    if (giveJuicerState == GiveJuicerState.CorrectGiven)
                    {

                        serialPort_IO8.WriteLine(codeHigh_JuicerPin);
                        Thread.Sleep(t_JuicerCorrectGivenMS);
                        serialPort_IO8.WriteLine(codeLow_JuicerPin);
                        giveJuicerState = GiveJuicerState.No;
                    }
                    //--- End of Juicer Control

                    //--- Startpad Read
                    if (startpadReadWatch.ElapsedMilliseconds >= startpadReadInterval)
                    {
                        serialPort_IO8.WriteLine(startpad_In);

                        // Read the Startpad Voltage
                        string str_Read = serialPort_IO8.ReadExisting();

                        // Restart the startpadReadWatch
                        startpadReadWatch.Restart();

                        // parse the start pad voltage 
                        string[] str_DigIn = str_Read.Split();

                        if (!String.IsNullOrEmpty(str_DigIn[0]))
                        {
                            int digIn = int.Parse(str_DigIn[0]);

                            if (digIn == startpad_DigIn_Pressed && pressedStartpad == PressedStartpad.No)
                            {/* time point from notouched state to touched state */

                                pressedStartpad = PressedStartpad.Yes;
                            }
                            else if (digIn == startpad_DigIn_Unpressed && pressedStartpad == PressedStartpad.Yes)
                            {/* time point from touched state to notouched state */

                                // the time point for leaving startpad
                                timePoint_StartpadLeft = globalWatch.ElapsedMilliseconds;
                                pressedStartpad = PressedStartpad.No;
                            }
                        }
                    }
                }
                catch (InvalidOperationException) { }
            }

            startpadReadWatch.Stop();
        }

        private void Create_Shapes()
        {// Create necessary elements: go circle, nogo rect, and one crossing
            circleGo = ShapeManipulate.Create_Circle(targetDiameterPixal);
            rectNogo = ShapeManipulate.Create_NogoRect(targetDiameterPixal, targetDiameterPixal);
            crossing = ShapeManipulate.Create_OneCrossing(targetDiameterPixal);
        }

        private void NewVariables()
        {
            Create_Shapes();

            // Color Brushes
            brush_BKWaitTrialStart = new SolidColorBrush();
            brush_BKTrial = new SolidColorBrush();
            brush_goCircleFill = new SolidColorBrush();
            brush_nogoRectFill = new SolidColorBrush();
            brush_CueCrossing = new SolidColorBrush();
            brush_CorrectFill = new SolidColorBrush();
            brush_CorrOutline = new SolidColorBrush();
            brush_ErrorFill = new SolidColorBrush();
            brush_ErrorOutline = new SolidColorBrush();


            // Feedback Audios Setup
            player_Correct = new System.Media.SoundPlayer(Properties.Resources.Correct);
            player_Error = new System.Media.SoundPlayer(Properties.Resources.Error);


            // TargetExeFeedback_List
            targetPosNum = parentMainUI.goNogoTaskConfig.goNogoTargetNumPosConfig.targetNoOfPositions;
            TargetExeFeedback_List = new List<int[]>();
            for (int i = 0; i < targetPosNum; i++)
            {
                TargetExeFeedback_List.Add(new int[] { 0, 0, 0, 0 });
            }

            // serialPort_IO8
            serialPort_IO8 = new SerialPort
            {
                PortName = parentMainUI.serialPortIO8_name,
                BaudRate = 115200
            };

            // trialTargetInfo_PerSess_List Storing trial Target Info [posIndex, GoNogoIndex]
            trialTargetInfo_PerSess_List = Create_TrialTargetInfo_List(totalTrialNumPerPosSess, nogoTrialNumPerPosSess, targetPosNum);


            // optPostions_OTopLeft_List: optional positions
            optPostions_OTopLeft_List = new List<int[]>();
            for (int i = 0; i < targetPosNum; i++)
            {
                optPostions_OTopLeft_List.Add(new int[] { 0, 0});
            }
        }



        private List<int[]> Create_TrialTargetInfo_List(int trialNumPerPos, int nogoTrialNumPerPos, int posNum)
        {/*
            Parameters:
                trialNumPerPos: total Trial Number Per Position(trialNum = goTrialNum + nogoTrialNum)
                nogoTrialNumPerPos: nogo Trial Number
                posNum: total Position Number

            Return:
                trialTargetInfo_List: Each Trial Target Information List ([posIndex, gonogoIndex]), gonogoIndex = 0(nogo), 1(go)
                e.g. trialNumPerPos = 5; nogoTrialNumPerPos = 2; posNum = 3;
                [   [0, 0], [0, 0], [0, 1], [0, 1], [0, 1],
                    [1, 0], [1, 0], [1, 1], [1, 1], [1, 1],
                    [2, 0], [2, 0], [2, 1], [2, 1], [2, 1]
                ], 
            */

            List<int[]> trialTargetInfo_List = new List<int[]>();

            for (int posi = 0; posi < posNum; posi++)
            {
                for (int goNogoi = 0; goNogoi < trialNumPerPos; goNogoi++)
                {
                    int goNogoIndex = (int)((goNogoi < nogoTrialNumPerPos) ? TargetType.Nogo : TargetType.Go);
                    trialTargetInfo_List.Add(new int[] { posi, goNogoIndex });
                }

            }

            return trialTargetInfo_List;
        }

        private void GetSetupParameters()
        {/* get the setup from the parent interface */


            GoNogoTaskConfig goNogoTaskConfig = parentMainUI.goNogoTaskConfig;

            totalTrialNumPerPosSess = goNogoTaskConfig.totalTrialNumPerPosSess;
            nogoTrialNumPerPosSess = goNogoTaskConfig.nogoTrialNumPerPosSess;

            file_saved = parentMainUI.file_saved;

            // TargetNumPos Setup
            GoNogoTargetNumPosConfig goNogoTargetNumPosConfig = goNogoTaskConfig.goNogoTargetNumPosConfig;
            targetDiameterPixal = Utility.Inch2Pixal(goNogoTargetNumPosConfig.targetDiaInch);
            targetPosNum = goNogoTargetNumPosConfig.optPostions_OCenter_List.Count;
            swf.Screen touchScreen = ScreenDetect.TaskPresentTouchScreen();
            int width = touchScreen.Bounds.Width, height = touchScreen.Bounds.Height;
            for (int i = 0; i < targetPosNum; i++)
            {
                int[] xy_OCenter = goNogoTargetNumPosConfig.optPostions_OCenter_List[i];

                optPostions_OTopLeft_List[i] = ShapeManipulate.ConvertXY_OCenter2TopLeft(xy_OCenter, width, height);
            }


            // Time Setup
            GoNogoTimeConfig goNogoTimeConfig = goNogoTaskConfig.goNogoTimeConfig;
            tRange_ReadyTimeS = goNogoTimeConfig.tRange_ReadyTimeS;
            tRange_CueTimeS = goNogoTimeConfig.tRange_CueTimeS;
            tRange_NogoShowTimeS = goNogoTimeConfig.tRange_NogoShowTimeS;
            tMax_ReactionTimeMS = goNogoTimeConfig.t_MaxReactionTimeS * 1000;
            tMax_ReachTimeMS = goNogoTimeConfig.t_MaxReachTimeS * 1000;
            t_InterTrialMS = (int)(goNogoTimeConfig.t_InterTrialS * 1000);
            t_VisfeedbackShowMS = (int)(goNogoTimeConfig.t_VisfeedbackShowS * 1000);
            t_JuicerCorrectGivenMS = (int)(goNogoTimeConfig.t_JuicerCorrectGivenS * 1000);


            // Colors Setup
            GoNogoColorConfig goNogoColorConfig = goNogoTaskConfig.goNogoColorConfig;
            brush_BKWaitTrialStart.Color = (Color)(typeof(Colors).GetProperty(goNogoColorConfig.BKWaitTrialColorStr) as PropertyInfo).GetValue(null, null);
            brush_BKTrial.Color = (Color)(typeof(Colors).GetProperty(goNogoColorConfig.BKTrialColorStr) as PropertyInfo).GetValue(null, null);
            brush_goCircleFill.Color = (Color)(typeof(Colors).GetProperty(goNogoColorConfig.goFillColorStr) as PropertyInfo).GetValue(null, null);
            brush_nogoRectFill.Color = (Color)(typeof(Colors).GetProperty(goNogoColorConfig.nogoFillColorStr) as PropertyInfo).GetValue(null, null);
            brush_CueCrossing.Color = (Color)(typeof(Colors).GetProperty(goNogoColorConfig.cueCrossingColorStr) as PropertyInfo).GetValue(null, null);
            brush_CorrectFill.Color = (Color)(typeof(Colors).GetProperty(goNogoColorConfig.CorrFillColorStr) as PropertyInfo).GetValue(null, null);
            brush_CorrOutline.Color = (Color)(typeof(Colors).GetProperty(goNogoColorConfig.CorrOutlineColorStr) as PropertyInfo).GetValue(null, null);
            brush_ErrorFill.Color = (Color)(typeof(Colors).GetProperty(goNogoColorConfig.ErrorFillColorStr) as PropertyInfo).GetValue(null, null);
            brush_ErrorOutline.Color = (Color)(typeof(Colors).GetProperty(goNogoColorConfig.ErrorOutlineColorStr) as PropertyInfo).GetValue(null, null);

            // Feedback Audios
            if (String.Compare(goNogoTaskConfig.audioFile_Correct, "default", true) != 0)
            {
                player_Correct.SoundLocation = goNogoTaskConfig.audioFile_Correct;
            }
            if (String.Compare(goNogoTaskConfig.audioFile_Error, "default", true) != 0)
            {
                player_Error.SoundLocation = goNogoTaskConfig.audioFile_Error;
            }

            // 
            circleGo.Fill = brush_goCircleFill;
            rectNogo.Fill = brush_nogoRectFill;
            crossing.Stroke = brush_CueCrossing;
        }

        private void Update_FeedbackTrialsInformation()
        {/* Init the Feedback Trial Information in the Mainwindow */

            trialNLeftInSess = totalTrialNumPerPosSess * targetPosNum;

            // Update Main Window Feedback 
            parentMainUI.textBox_feedback_currSessioni.Text = currSessi.ToString();
            parentMainUI.textBox_feedback_TrialsLeftCurrSess.Text = trialNLeftInSess.ToString();

            int[] targetExeFeedback;
            targetExeFeedback = TargetExeFeedback_List[0];
            parentMainUI.textBox_feedback_Targ0TotalGo.Text = targetExeFeedback[0].ToString();
            parentMainUI.textBox_feedback_Targ0SuccessGo.Text = targetExeFeedback[1].ToString();
            parentMainUI.textBox_feedback_Targ0TotalNogo.Text = targetExeFeedback[2].ToString();
            parentMainUI.textBox_feedback_Targ0SuccessNogo.Text = targetExeFeedback[3].ToString();
            targetExeFeedback = TargetExeFeedback_List[1];
            parentMainUI.textBox_feedback_Targ1TotalGo.Text = targetExeFeedback[0].ToString();
            parentMainUI.textBox_feedback_Targ1SuccessGo.Text = targetExeFeedback[1].ToString();
            parentMainUI.textBox_feedback_Targ1TotalNogo.Text = targetExeFeedback[2].ToString();
            parentMainUI.textBox_feedback_Targ1SuccessNogo.Text = targetExeFeedback[3].ToString();
            targetExeFeedback = TargetExeFeedback_List[2];
            parentMainUI.textBox_feedback_Targ2TotalGo.Text = targetExeFeedback[0].ToString();
            parentMainUI.textBox_feedback_Targ2SuccessGo.Text = targetExeFeedback[1].ToString();
            parentMainUI.textBox_feedback_Targ2TotalNogo.Text = targetExeFeedback[2].ToString();
            parentMainUI.textBox_feedback_Targ2SuccessNogo.Text = targetExeFeedback[3].ToString();
        }


        long timestamp_0;
        bool PresentTrial;
        // selected Target Position Index for Current Presented Trial
        int currTrialTargetPosInd;
        TargetType targetType;

        // t_Ready, t_Cue and t_noGoShow for Each Trial
        List<float> t_Ready_List = new List<float>();
        List<float> t_Cue_List = new List<float>();
        List<float> t_noGoShow_List = new List<float>();

        public async void Present_Start()
        {
            try
            {
                float t_Cue, t_Ready, t_noGoShow;
                int[] pos_Taget_OTopLeft;
                int totalTrialNumPerSess = totalTrialNumPerPosSess * targetPosNum;

                // Present Each Trial
                globalWatch.Restart();
                thread_ReadWrite_IO8.Start();
                timestamp_0 = DateTime.Now.Ticks;
                int totalTriali = 0;
                PresentTrial = true;
                while (PresentTrial)
                {
                    currSessi++;

                    // Write Current Session number 
                    using (StreamWriter file = File.AppendText(file_saved))
                    {
                        file.WriteLine("\n\n");
                        file.WriteLine(String.Format("{0, -40}: {1}", "Session", currSessi.ToString()));
                    }


                    ShuffleTrials_GenRandomTime();

                    int sessTriali = 0;
                    trialNLeftInSess = totalTrialNumPerSess - sessTriali;
                    while (sessTriali < trialTargetInfo_PerSess_List.Count)
                    {
                        // Extract trial parameters
                        int[] targetInfo = trialTargetInfo_PerSess_List[sessTriali];
                        currTrialTargetPosInd = targetInfo[0];
                        pos_Taget_OTopLeft = optPostions_OTopLeft_List[currTrialTargetPosInd];
                        targetType = (TargetType)targetInfo[1];
                        t_Cue = t_Cue_List[sessTriali];
                        t_Ready = t_Ready_List[sessTriali];
                        t_noGoShow = t_noGoShow_List[sessTriali];

                        try
                        {
                            serialPort_IO8.WriteLine(TDTCmd_InitState);
                        }
                        catch (InvalidOperationException) { }



                        /*----- WaitStartTrial Interface ------*/
                        pressedStartpad = PressedStartpad.No;
                        await Interface_WaitStartTrial();

                        if (PresentTrial == false)
                        {
                            break;
                        }

                        /*-------- Trial Interfaces -------*/
                        try
                        {
                            // Ready Interface
                            await Interface_Ready(t_Ready);

                            if (PresentTrial == false)
                            {
                                break;
                            }

                            // Cue Interface
                            await Interface_Cue(t_Cue, pos_Taget_OTopLeft);

                            if (PresentTrial == false)
                            {
                                break;
                            }

                            // Go or noGo Target Interface
                            sessTriali++;
                            if (targetType == TargetType.Go)
                            {
                                await Interface_Go(pos_Taget_OTopLeft);
                                if (PresentTrial == false)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                await Interface_noGo(t_noGoShow, pos_Taget_OTopLeft);
                                if (PresentTrial == false)
                                {
                                    break;
                                }
                            }

                            Remove_All();
                        }
                        catch (TaskCanceledException)
                        {
                            Remove_All();
                        }

                        try
                        {
                            serialPort_IO8.WriteLine(TDTCmd_InitState);
                        }
                        catch (InvalidOperationException) { }


                        // totalTriali including trials fail during Ready and Cue Phases
                        totalTriali++;
                        trialNLeftInSess = totalTrialNumPerSess - sessTriali;
                        Update_FeedbackTrialsInformation();

                        /*-------- Write Trial Information ------*/
                        List<String> strExeSubResult = new List<String>();
                        strExeSubResult.Add("readyWaitTooShort");
                        strExeSubResult.Add("cueWaitTooShort");
                        strExeSubResult.Add("goReactionTimeToolong");
                        strExeSubResult.Add("goReachTimeToolong");
                        strExeSubResult.Add("goMiss");
                        strExeSubResult.Add("goSuccess");
                        strExeSubResult.Add("nogoMoved");
                        strExeSubResult.Add("nogoSuccess");
                        String strExeFail = "Failed";
                        String strExeSuccess = "Success";
                        using (StreamWriter file = File.AppendText(file_saved))
                        {
                            decimal ms2sRatio = 1000;

                            if (totalTriali > 1)
                            { // Startpad touched in trial i+1 treated as the return point as in trial i        

                                file.WriteLine(String.Format("{0, -40}: {1}", "Returned to Startpad TimePoint", (timePoint_StartpadTouched / ms2sRatio).ToString()));
                            }


                            /* Current Trial Written Inf*/
                            file.WriteLine("\n");

                            // Trial Num
                            file.WriteLine(String.Format("{0, -40}: {1}", "TrialNum", totalTriali.ToString()));

                            // the timepoint when touching the startpad to initial a new trial
                            file.WriteLine(String.Format("{0, -40}: {1}", "Startpad Touched TimePoint", (timePoint_StartpadTouched / ms2sRatio).ToString()));

                            // Start Interface showed TimePoint
                            file.WriteLine(String.Format("{0, -40}: {1}", "Ready Start TimePoint", (timePoint_Interface_ReadyOnset / ms2sRatio).ToString()));

                            // Ready Time
                            file.WriteLine(String.Format("{0, -40}: {1}", "Ready Interface Time", t_Ready.ToString()));


                            // Various Cases
                            if (trialExeResult == TrialExeResult.readyWaitTooShort)
                            {// case: ready WaitTooShort

                                // Left startpad early during ready
                                file.WriteLine(String.Format("{0, -40}: {1}", "Startpad Left TimePoint", (timePoint_StartpadLeft / ms2sRatio).ToString()));

                                // trial exe result : success or fail
                                file.WriteLine(String.Format("{0, -40}: {1}, {2}", "Trial Result", strExeFail, strExeSubResult[0]));
                            }
                            else if (trialExeResult == TrialExeResult.cueWaitTooShort)
                            {// case: Cue WaitTooShort

                                // Cue Interface Timepoint, Cue Time and Left startpad early during Cue
                                file.WriteLine(String.Format("{0, -40}: {1}", "Cue Start TimePoint", (timePoint_Interface_CueOnset / ms2sRatio).ToString()));
                                file.WriteLine(String.Format("{0, -40}: {1}", "Cue Interface Time", t_Cue.ToString()));
                                file.WriteLine(String.Format("{0, -40}: {1}", "Startpad Left TimePoint", (timePoint_StartpadLeft / ms2sRatio).ToString()));

                                // trial exe result : success or fail
                                file.WriteLine(String.Format("{0, -40}: {1}, {2}", "Trial Result", strExeFail, strExeSubResult[1]));
                            }
                            else if (trialExeResult == TrialExeResult.goReactionTimeToolong)
                            {// case : goReactionTimeToolong 

                                // Cue Interface Timepoint and Cue Time
                                file.WriteLine(String.Format("{0, -40}: {1}", "Cue Start TimePoint", (timePoint_Interface_CueOnset / ms2sRatio).ToString()));
                                file.WriteLine(String.Format("{0, -40}: {1}", "Cue Interface Time", t_Cue.ToString()));

                                // Target Interface Timepoint, Target type: Go, and Target position index: 0 (1, 2)
                                file.WriteLine(String.Format("{0, -40}: {1}", "Target Start TimePoint", (timePoint_Interface_TargetOnset / ms2sRatio).ToString()));
                                file.WriteLine(String.Format("{0, -40}: {1}", "TargetType", targetType.ToString()));
                                file.WriteLine(String.Format("{0, -40}: {1}", "TargetPositionIndex", currTrialTargetPosInd.ToString()));


                                // trial exe result : success or fail
                                file.WriteLine(String.Format("{0, -40}: {1}, {2}", "Trial Result", strExeFail, strExeSubResult[2]));
                            }
                            else if (trialExeResult == TrialExeResult.goReachTimeToolong)
                            {// case : goReachTimeToolong

                                // Cue Interface Timepoint and Cue Time
                                file.WriteLine(String.Format("{0, -40}: {1}", "Cue Start TimePoint", (timePoint_Interface_CueOnset / ms2sRatio).ToString()));
                                file.WriteLine(String.Format("{0, -40}: {1}", "Cue Interface Time", t_Cue.ToString()));

                                // Target Interface Timepoint, Target type: Go, and Target position index: 0 (1, 2)
                                file.WriteLine(String.Format("{0, -40}: {1}", "Target Start TimePoint", (timePoint_Interface_TargetOnset / ms2sRatio).ToString()));
                                file.WriteLine(String.Format("{0, -40}: {1}", "TargetType", targetType.ToString()));
                                file.WriteLine(String.Format("{0, -40}: {1}", "TargetPositionIndex", currTrialTargetPosInd.ToString()));
                                // Target interface:  Left Startpad Time Point
                                file.WriteLine(String.Format("{0, -40}: {1}", "Startpad Left TimePoint", (timePoint_StartpadLeft / ms2sRatio).ToString()));


                                // trial exe result : success or fail
                                file.WriteLine(String.Format("{0, -40}: {1}, {2}", "Trial Result", strExeFail, strExeSubResult[3]));
                            }
                            else if (trialExeResult == TrialExeResult.goClose | trialExeResult == TrialExeResult.goHit | trialExeResult == TrialExeResult.goMiss)
                            {// case: Go success (goClose or goHit) or goMiss

                                // Cue Interface Timepoint and Cue Time
                                file.WriteLine(String.Format("{0, -40}: {1}", "Cue Start TimePoint", (timePoint_Interface_CueOnset / ms2sRatio).ToString()));
                                file.WriteLine(String.Format("{0, -40}: {1}", "Cue Interface Time", t_Cue.ToString()));

                                // Target Interface Timepoint, Target type: Go, and Target position index: 0 (1, 2)
                                file.WriteLine(String.Format("{0, -40}: {1}", "Target Start TimePoint", (timePoint_Interface_TargetOnset / ms2sRatio).ToString()));
                                file.WriteLine(String.Format("{0, -40}: {1}", "TargetType", targetType.ToString()));
                                file.WriteLine(String.Format("{0, -40}: {1}", "TargetPositionIndex", currTrialTargetPosInd.ToString()));

                                // Target interface:  Left Startpad Time Point
                                file.WriteLine(String.Format("{0, -40}: {1}", "Startpad Left TimePoint", (timePoint_StartpadLeft / ms2sRatio).ToString()));

                                //  Target interface:  touched  timepoint and (x, y position) of all touch points
                                for (int pointi = 0; pointi < touchPoints_PosTime.Count; pointi++)
                                {
                                    double[] downPoint = touchPoints_PosTime[pointi];

                                    // touched pointi touchpoint
                                    file.WriteLine(String.Format("{0, -40}: {1, -40}", "Touch Point " + pointi.ToString() + " TimePoint", ((decimal)downPoint[1] / ms2sRatio).ToString()));

                                    // touched pointi position
                                    file.WriteLine(String.Format("{0, -40}: {1}", "Touch Point " + pointi.ToString() + " XY Position", downPoint[2].ToString() + ", " + downPoint[3].ToString()));

                                }

                                //  Target interface:  left timepoint and (x, y position) of all touch points
                                for (int pointi = 0; pointi < touchPoints_PosTime.Count; pointi++)
                                {
                                    double[] downPoint = touchPoints_PosTime[pointi];

                                    // left pointi touchpoint
                                    file.WriteLine(String.Format("{0, -40}: {1, -40}", "Left Point " + pointi.ToString() + " TimePoint", ((decimal)downPoint[4] / ms2sRatio).ToString()));

                                    // left pointi position
                                    file.WriteLine(String.Format("{0, -40}: {1}", "Left Point " + pointi.ToString() + " XY Position", downPoint[5].ToString() + ", " + downPoint[6].ToString()));
                                }


                                // trial exe result : success or fail
                                if (trialExeResult == TrialExeResult.goMiss)
                                    file.WriteLine(String.Format("{0, -40}: {1}, {2}", "Trial Result", strExeFail, strExeSubResult[4]));
                                else
                                    file.WriteLine(String.Format("{0, -40}: {1}, {2}", "Trial Result", strExeSuccess, strExeSubResult[5]));

                            }
                            else if (trialExeResult == TrialExeResult.nogoMoved)
                            { // case: noGo moved 

                                // Cue Interface Timepoint and Cue Time
                                file.WriteLine(String.Format("{0, -40}: {1}", "Cue Start TimePoint", (timePoint_Interface_CueOnset / ms2sRatio).ToString()));
                                file.WriteLine(String.Format("{0, -40}: {1}", "Cue Interface Time", t_Cue.ToString()));

                                // Cue Interface Timepoint, Target type: Go, and Target position index: 0 (1, 2)
                                file.WriteLine(String.Format("{0, -40}: {1}", "Target Start TimePoint", (timePoint_Interface_TargetOnset / ms2sRatio).ToString()));
                                file.WriteLine(String.Format("{0, -40}: {1}", "TargetType", targetType.ToString()));
                                file.WriteLine(String.Format("{0, -40}: {1}", "TargetPositionIndex", currTrialTargetPosInd.ToString()));

                                // Target nogo interface show time
                                file.WriteLine(String.Format("{0, -40}: {1}", "Nogo Interface Show Time", t_noGoShow.ToString()));

                                // Target interface:  Left Startpad Time Point
                                file.WriteLine(String.Format("{0, -40}: {1}", "Startpad Left TimePoint", (timePoint_StartpadLeft / ms2sRatio).ToString()));



                                // trial exe result : success or fail
                                file.WriteLine(String.Format("{0, -40}: {1}, {2}", "Trial Result", strExeFail, strExeSubResult[6]));

                            }
                            else if (trialExeResult == TrialExeResult.nogoSuccess)
                            { // case: noGo success 

                                // Cue Interface Timepoint and Cue Time
                                file.WriteLine(String.Format("{0, -40}: {1}", "Cue Start TimePoint", (timePoint_Interface_CueOnset / ms2sRatio).ToString()));
                                file.WriteLine(String.Format("{0, -40}: {1}", "Cue Interface Time", t_Cue.ToString()));

                                // Cue Interface Timepoint, Target type: Go, and Target position index: 0 (1, 2)
                                file.WriteLine(String.Format("{0, -40}: {1}", "Target Start TimePoint", (timePoint_Interface_TargetOnset / ms2sRatio).ToString()));
                                file.WriteLine(String.Format("{0, -40}: {1}", "TargetType", targetType.ToString()));
                                file.WriteLine(String.Format("{0, -40}: {1}", "TargetPositionIndex", currTrialTargetPosInd.ToString()));
                                // Target nogo interface show time
                                file.WriteLine(String.Format("{0, -40}: {1}", "Nogo Interface Show Time", t_noGoShow.ToString()));


                                // trial exe result : success or fail
                                file.WriteLine(String.Format("{0, -40}: {1}, {2}", "Trial Result", strExeSuccess, strExeSubResult[7]));
                            }

                        }
                    }
                }

                // Detect the return to startpad timepoint for the last trial
                pressedStartpad = PressedStartpad.No;
                try
                {
                    await Wait_Return2StartPad(1);
                }
                catch (TaskCanceledException)
                {
                    using (StreamWriter file = File.AppendText(file_saved))
                    {
                        file.WriteLine(String.Format("{0, -40}: {1}", "Returned to Startpad TimePoint", timePoint_StartpadTouched.ToString()));
                    }
                }

                // save the summary of exp
                SaveSummaryofExp();
            }
            catch(TaskCanceledException)
            {

            }
        }


        private void Remove_All()
        {
            crossing.Visibility = Visibility.Hidden;
            crossing.IsEnabled = false;
            circleGo.Visibility = Visibility.Hidden;
            circleGo.IsEnabled = false;
            rectNogo.Visibility = Visibility.Hidden;
            rectNogo.IsEnabled = false;
            wholePresentGrid.UpdateLayout();
        }


        private Task Wait_EnoughTouch(float t_EnoughTouch)
        {
            /* 
             * Wait for Enough Touch Time
             * 
             * Input: 
             *    t_EnoughTouch: the required Touch time (s)  
             */

            Task task = null;

            // start a task and return it
            return Task.Run(() =>
            {
                Stopwatch touchedWatch = new Stopwatch();
                touchedWatch.Restart();

                while (PresentTrial && pressedStartpad == PressedStartpad.Yes && startpadHoldstate != StartPadHoldState.HoldEnough)
                {
                    if (touchedWatch.ElapsedMilliseconds >= t_EnoughTouch * 1000)
                    {/* touched with enough time */
                        startpadHoldstate = StartPadHoldState.HoldEnough;
                    }
                }
                touchedWatch.Stop();
                if (startpadHoldstate != StartPadHoldState.HoldEnough)
                {
                    throw new TaskCanceledException(task);
                }

            });
        }

        private Task Interface_WaitStartTrial()
        {
            /* task for WaitStart interface
             * 
             * Wait for Touching Startpad to trigger a new Trial
             */

            Remove_All();
            wholePresentGrid.Background = brush_BKWaitTrialStart;
            //myGridBorder.BorderBrush = brush_BDWaitTrialStart;

            Task task_WaitStart = Task.Run(() =>
            {
                while (PresentTrial && pressedStartpad == PressedStartpad.No) ;


                if (pressedStartpad == PressedStartpad.Yes)
                {
                    // the time point for startpad touched
                    serialPort_IO8.WriteLine(TDTCmd_TouchTriggerTrial);
                    timePoint_StartpadTouched = globalWatch.ElapsedMilliseconds;
                }

            });

            return task_WaitStart;
        }


        private async Task Interface_Ready(float t_Ready)
        {/* task for Ready interface:
            Show the Ready Interface while Listen to the state of the startpad. 
            * 
            * Output:
            *   startPadHoldstate_Ready = 
            *       StartPadHoldState.HoldEnough (if startpad is touched lasting t_Ready)
            *       StartPadHoldState.HoldTooShort (if startpad is released before t_Ready) 
            */

            try
            {
                wholePresentGrid.Background = brush_BKTrial;
                serialPort_IO8.WriteLine(TDTCmd_ReadyShown);
                timePoint_Interface_ReadyOnset = globalWatch.ElapsedMilliseconds;

                // Wait Startpad Hold Enough Time
                startpadHoldstate = StartPadHoldState.HoldTooShort;
                await Wait_EnoughTouch(t_Ready);

            }
            catch (TaskCanceledException)
            {
                // trial execute result: waitReadyTooShort 
                serialPort_IO8.WriteLine(TDTCmd_ReadyWaitTooShort);
                trialExeResult = TrialExeResult.readyWaitTooShort;

                Task task = null;
                throw new TaskCanceledException(task);
            }
            catch (InvalidOperationException) { }
        }


        public async Task Interface_Cue(float t_Cue, int[] crossingPos_OTopLeft)
        {/* task for Cue Interface 
            Show the Cue Interface while Listen to the state of the startpad. 
            
            Args:
                t_Cue: Cue interface showes duration(s)
                crossingPos_OTopLeft: the center X, Y position of the one crossing, Origin at Screen TopLeft

            * Output:
            *   startPadHoldstate_Cue = 
            *       StartPadHoldState.HoldEnough (if startpad is touched lasting t_Cue)
            *       StartPadHoldState.HoldTooShort (if startpad is released before t_Cue) 
            */

            try
            {
                //myGrid.Children.Clear();
                Remove_All();

                // Show the crossing at onecrossingPos_OCenter 
                crossing.Show_Crossing_OTopLeft(crossingPos_OTopLeft);
                //Show_OneCrossing(onecrossingPos_OCenter);

                serialPort_IO8.WriteLine(TDTCmd_CueShown);
                timePoint_Interface_CueOnset = globalWatch.ElapsedMilliseconds;


                // wait target cue for several seconds
                startpadHoldstate = StartPadHoldState.HoldTooShort;
                await Wait_EnoughTouch(t_Cue);

            }
            catch (TaskCanceledException)
            {

                // Audio Feedback
                player_Error.Play();

                // trial execute result: waitCueTooShort 
                serialPort_IO8.WriteLine(TDTCmd_CueWaitTooShort);
                trialExeResult = TrialExeResult.cueWaitTooShort;


                Task task = null;
                throw new TaskCanceledException(task);
            }

        }

        ScreenTouchState screenTouchstate;
        private async Task Interface_Go(int[] targetPos_OTopLeft)
        {/* task for Go Interface: Show the Go Interface while Listen to the state of the startpad.
            * 1. If Reaction time < Max Reaction Time or Reach Time < Max Reach Time, end up with long reaction or reach time ERROR Interface
            * 2. Within proper reaction time && reach time, detect the touch point and end up with hit, near and miss.
            
            * Args:
            *    targetPos_OTopLeft: the Center Position of the Target, Origin in TopLeft

            * Output:
            *   startPadHoldstate_Cue = 
            *       StartPadHoldState.HoldEnough (if startpad is touched lasting t_Cue)
            *       StartPadHoldState.HoldTooShort (if startpad is released before t_Cue) 
            */

            try
            {
                // Remove the Crossing and Show the Go Circle
                crossing.Hidden_Crossing();
                ShapeManipulate.Show_Circle_OTopLeft(circleGo, targetPos_OTopLeft, brush_goCircleFill);

                // Increased Total Go Trial Number of currTrialTargetPosInd
                TargetExeFeedback_List[currTrialTargetPosInd][0]++;

                // go target Onset Time Point
                timePoint_Interface_TargetOnset = globalWatch.ElapsedMilliseconds;
                serialPort_IO8.WriteLine(TDTCmd_GoTargetShown);

                // Wait for Reaction within tMax_ReactionTime
                pressedStartpad = PressedStartpad.Yes;
                await Wait_Reaction();

                // Wait for Touch within tMax_ReachTime and Calcuate the gotargetTouchstate
                screenTouchstate = ScreenTouchState.Idle;
                await Wait_Reach();


                /*---- Go Target Touch States ----*/
                if (gotargetTouchstate == GoTargetTouchState.goHit)
                {/*Hit */

                    Feedback_GoCorrect_Hit();

                    // Increased Success Go Trial Number of currTrialTargetPosInd
                    TargetExeFeedback_List[currTrialTargetPosInd][1]++;

                    // trial execute result: goHit 
                    trialExeResult = TrialExeResult.goHit;

                }
                else if (gotargetTouchstate == GoTargetTouchState.goMissed)
                {/* touch missed*/

                    Feedback_GoERROR_Miss();

                    // trial execute result: goMiss 
                    trialExeResult = TrialExeResult.goMiss;
                }

                await Task.Delay(t_VisfeedbackShow);
            }
            catch (TaskCanceledException)
            {
                Interface_GoERROR_LongReactionReach();
                await Task.Delay(t_VisfeedbackShow);
                throw new TaskCanceledException("Not Reaction Within the Max Reaction Time.");
            }

        }



        private Task Wait_Reaction()
        {/* Wait for Reaction within tMax_ReactionTime */

            // start a task and return it
            return Task.Run(() =>
            {
                Stopwatch waitWatch = new Stopwatch();
                waitWatch.Start();
                while (PresentTrial && pressedStartpad == PressedStartpad.Yes)
                {
                    if (waitWatch.ElapsedMilliseconds >= tMax_ReactionTimeMS)
                    {/* No release Startpad within tMax_ReactionTime */
                        waitWatch.Stop();

                        serialPort_IO8.WriteLine(TDTCmd_GoReactionTooLong);
                        trialExeResult = TrialExeResult.goReactionTimeToolong;


                        throw new TaskCanceledException("No Reaction within the Max Reaction Time");
                    }
                }
                waitWatch.Stop();
            });
        }

        private Task Wait_Reach()
        {/* Wait for Reach within tMax_ReachTime*/

            return Task.Run(() =>
            {
                Stopwatch waitWatch = new Stopwatch();
                waitWatch.Start();
                while (PresentTrial && screenTouchstate == ScreenTouchState.Idle)
                {
                    if (waitWatch.ElapsedMilliseconds >= tMax_ReachTimeMS)
                    {/*No Screen Touched within tMax_ReachTime*/
                        waitWatch.Stop();

                        serialPort_IO8.WriteLine(TDTCmd_GoReachTooLong);
                        trialExeResult = TrialExeResult.goReachTimeToolong;


                        throw new TaskCanceledException("No Reach within the Max Reach Time");
                    }
                }
                downPoints_Pos.Clear();
                touchPoints_PosTime.Clear();
                waitWatch.Restart();
                while (waitWatch.ElapsedMilliseconds <= tMax_1Touch) ;
                waitWatch.Stop();
                calc_GoTargetTouchState();
            });
        }

        GoTargetTouchState gotargetTouchstate;
        // Center Point and Radius of CircleGo (in Pixal)
        Point circleGo_centerPoint_Pixal;
        double circleGo_Radius_Pixal;

        private void calc_GoTargetTouchState()
        {/* Calculate GoTargetTouchState  
            1. based on the Touch Down Positions in  List downPoints_Pos and circleGo_centerPoint
            2. Assign the calculated target touch state to the GoTargetTouchState variable gotargetTouchstate
            */

            double distance;
            gotargetTouchstate = GoTargetTouchState.goMissed;
            while (downPoints_Pos.Count > 0)
            {
                // always deal with the point at 0
                Point touchp = new Point(downPoints_Pos[0][0], downPoints_Pos[0][1]);

                // distance between the touchpoint and the center of the circleGo
                distance = Point.Subtract(circleGo_centerPoint_Pixal, touchp).Length;


                if (distance <= circleGo_Radius_Pixal)
                {// Hit 

                    serialPort_IO8.WriteLine(TDTCmd_GoTouchedHit);
                    gotargetTouchstate = GoTargetTouchState.goHit;

                    downPoints_Pos.Clear();
                    break;
                }

                // remove the downPoint at 0
                downPoints_Pos.RemoveAt(0);
            }

            if (gotargetTouchstate == GoTargetTouchState.goMissed)
            {
                serialPort_IO8.WriteLine(TDTCmd_GoTouchedMiss);
            }
            downPoints_Pos.Clear();
        }

        private void ShuffleTrials_GenRandomTime()
        {/* ---- 
            1. shuffle trials, i.e Shuffle trialTargetInfo_PerSess_List
            2. Generate the random t_Ready, t_Cue, t_noGoShow for each trial, stored in t_Ready_List, t_Cue_List, t_noGoShow_List;
             */

            // Shuffle trialTargetInfo_PerSess_List
            trialTargetInfo_PerSess_List = Utility.ShuffleListMember(trialTargetInfo_PerSess_List);


            // generate a random t_Ready and t_Cue, and and them into t_Ready_List and t_Cue_List individually
            Random rand = new Random();
            for (int i = 0; i < trialTargetInfo_PerSess_List.Count; i++)
            {
                t_Cue_List.Add(Utility.TransferToRange((float)rand.NextDouble(), tRange_CueTimeS[0], tRange_CueTimeS[1]));
                t_Ready_List.Add(Utility.TransferToRange((float)rand.NextDouble(), tRange_ReadyTimeS[0], tRange_ReadyTimeS[1]));
                t_noGoShow_List.Add(Utility.TransferToRange((float)rand.NextDouble(), tRange_NogoShowTimeS[0], tRange_NogoShowTimeS[1]));
            }
        }

    }
}
