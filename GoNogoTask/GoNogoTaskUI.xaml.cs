using System;
using System.Windows;
using System.IO;
using swf = System.Windows.Forms;
using sd = System.Drawing;
using TasksShared;
using Newtonsoft.Json;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows.Input;

namespace GoNogoTask
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly string defaultSaveFolder = @"C:\\";

        private string taskName;
        public string serialPortIO8_name = "";
        public string file_saved;


        public GoNogoTaskConfig goNogoTaskConfig;

        private bool BtnStartState, BtnStopState;

        swf.Screen presentTouchScreen, taskUIScreen;

        GoNogoTask_PresentWin goNogoTask_PresentWin;

        int blockNum;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            taskName = "GoNogoTask";

            // Get the mainUI and touch Screen
            taskUIScreen = ScreenDetect.TaskUIScreen();
            presentTouchScreen = ScreenDetect.TaskPresentTouchScreen();
            
            // TaskUI shown in taskUIScreen
            this.Top = taskUIScreen.Bounds.Top;
            this.Left = taskUIScreen.Bounds.Left;


            // Check serial Port IO8 Connection
            CheckIO8Connection();


            // Load Default Config File
            goNogoTaskConfig = new GoNogoTaskConfig();
            LoadConfigFile("defaultConfig");

            ShowMainConfig();


            if (String.Equals(serialPortIO8_name, ""))
            {
                btn_start.IsEnabled = false;
            }
            else
            {
                btn_start.IsEnabled = true;
            }
        }


        private void CheckIO8Connection()
        {
            // locate serial Port Name
            serialPortIO8_name = SerialPortIO8Manipulate.Locate_serialPortIO8Name();
            if (String.Equals(serialPortIO8_name, ""))
            {
                btn_start.IsEnabled = false;
                btn_comReconnect.Visibility = Visibility.Visible;
                btn_comReconnect.IsEnabled = true;
                textblock_comState.Visibility = Visibility.Visible;

                run_comState.Text = "Can't Find the COM Port for DLP-IO8!";
                run_comState.Background = new SolidColorBrush(Colors.Orange);
                run_comState.Foreground = new SolidColorBrush(Colors.Red);
                run_instruction.Text = "Please connect it correctly and reCheck!";
                run_instruction.Background = new SolidColorBrush(Colors.Orange);
                run_instruction.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                btn_comReconnect.Visibility = Visibility.Hidden;
                btn_comReconnect.IsEnabled = false;
                run_comState.Text = "Found the COM Port for DLP-IO8!";
                run_comState.Background = new SolidColorBrush(Colors.White);
                run_comState.Foreground = new SolidColorBrush(Colors.Green);
                run_instruction.Text = "Can start trials now";
                run_instruction.Background = new SolidColorBrush(Colors.White);
                run_instruction.Foreground = new SolidColorBrush(Colors.Green);
            }
        }

        private void LoadConfigFile(string configFile)
        {/*Load Config File .json 
            configFile == '': load the default Config File
            */


            // Read the Config. File and convert to JsonObject
            if (String.Equals(configFile, "defaultConfig"))
            {
                string movementTaskFolder = Path.GetFullPath(@"..\\..\\..\\");
                configFile = Path.Combine(movementTaskFolder, taskName, "Resources", "ConfigFiles", "defaultConfig.json");
            }
            goNogoTaskConfig.LoadJsonFile2GoNogoConfig(configFile);


            // Set the default savefolder, audios
            if (String.Equals(goNogoTaskConfig.savedFolder, "default"))
                goNogoTaskConfig.savedFolder = defaultSaveFolder + taskName;

            if (String.Equals(goNogoTaskConfig.audioFile_Correct, "default"))
            {
                string movementTaskFolder = Path.GetFullPath(@"..\\..\\..\\");
                goNogoTaskConfig.audioFile_Correct = Path.Combine(movementTaskFolder, taskName, "Resources", "Audios", "Correct.wav"); 
            }

            if (String.Equals(goNogoTaskConfig.audioFile_Error, "default"))
            {
                string movementTaskFolder = Path.GetFullPath(@"..\\..\\..\\");
                goNogoTaskConfig.audioFile_Error = Path.Combine(movementTaskFolder, taskName, "Resources", "Audios", "Error.wav");
            }
        }

        private void SaveConfigFile(string configFile)
        {/*Load Config File .json 
            configFile == '': load the default Config File
            */

            // Write to Json file
            string json = JsonConvert.SerializeObject(goNogoTaskConfig, Formatting.Indented);
            File.WriteAllText(configFile, json);
        }

        private void ShowMainConfig()
        {//Config into the Main Interface 

            textBox_NHPName.Text = goNogoTaskConfig.NHPName;
            textBox_totalTrialNumPerPosSess.Text = goNogoTaskConfig.totalTrialNumPerPosSess.ToString();
            textBox_nogoTrialNumPerPosSess.Text = goNogoTaskConfig.nogoTrialNumPerPosSess.ToString();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void MenuItem_SetupColors(object sender, RoutedEventArgs e)
        {
            GoNogoTask_SetupColorsWin Win_SetupColors = new GoNogoTask_SetupColorsWin(this);

            swf.Screen showScreen = ScreenDetect.TaskUIScreen();
            sd.Rectangle Rect_showScreen = showScreen.Bounds;
            Win_SetupColors.Top = Rect_showScreen.Top;
            Win_SetupColors.Left = Rect_showScreen.Left;

            // Set Owner
            Win_SetupColors.Owner = this;

            Win_SetupColors.Show();
        }

        private void MenuItem_SetupTime(object sender, RoutedEventArgs e)
        {
            GoNogoTask_SetupTimeWin Win_SetupTime = new GoNogoTask_SetupTimeWin(this);
 
            swf.Screen showScreen = ScreenDetect.TaskUIScreen();
            sd.Rectangle Rect_showScreen = showScreen.Bounds;
            Win_SetupTime.Top = Rect_showScreen.Top;
            Win_SetupTime.Left = Rect_showScreen.Left;

            // Set Owner
            Win_SetupTime.Owner = this;

            Win_SetupTime.Show();
        }

        private void MenuItem_SetupTarget(object sender, RoutedEventArgs e)
        {
            GoNogoTask_SetupTargetWin Win_SetupTarget = new GoNogoTask_SetupTargetWin(this);
 
            swf.Screen showScreen = ScreenDetect.TaskUIScreen();
            sd.Rectangle Rect_showScreen = showScreen.Bounds;
            Win_SetupTarget.Top = Rect_showScreen.Top;
            Win_SetupTarget.Left = Rect_showScreen.Left;

            // Set Owner
            Win_SetupTarget.Owner = this;

            Win_SetupTarget.Show();
        }

        private void MenuItem_SetupSaveFolderAudio(object sender, RoutedEventArgs e)
        {
            GoNogoTask_SetupSavefolderAudiosWin Win_SetupSavefolderAudios = new GoNogoTask_SetupSavefolderAudiosWin(this);

            swf.Screen showScreen = ScreenDetect.TaskUIScreen();
            sd.Rectangle Rect_showScreen = showScreen.Bounds;
            Win_SetupSavefolderAudios.Top = Rect_showScreen.Top;
            Win_SetupSavefolderAudios.Left = Rect_showScreen.Left;

            // Set Owner
            Win_SetupSavefolderAudios.Owner = this;

            Win_SetupSavefolderAudios.Show();
        }

        private void Btn_comReconnect_Click(object sender, RoutedEventArgs e)
        {
            CheckIO8Connection();
        }

        private void btnLoadConf_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog
            {

                // Set filter for file extension and default file extension 
                DefaultExt = ".json",
                Filter = "Json Files|*.json"
            };

            Nullable<bool> result = openFileDlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string configFile = openFileDlg.FileName;
                LoadConfigFile(configFile);
            }
        }


        private void MenuItem_SaveConf_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveFileDlg = new Microsoft.Win32.SaveFileDialog
            {
                // Set filter for file extension and default file extension 
                DefaultExt = ".json",
                Filter = "Json Files|*.json",
                FileName = "config"
            };

            Nullable<bool> result = saveFileDlg.ShowDialog();
            if (result == true)
            {
                // Open document 
                string saveConfigFile = saveFileDlg.FileName;
                SaveConfigFile(saveConfigFile);
            }
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            goNogoTask_PresentWin.Hide();
            goNogoTask_PresentWin.IsEnabled = false;

            btn_start.IsEnabled = false;
            btn_stop.IsEnabled = true;
            btn_resume.IsEnabled = true;
            btn_pause.IsEnabled = false;
        }

        private void btnResume_Click(object sender, RoutedEventArgs e)
        {
            goNogoTask_PresentWin.Show();
            goNogoTask_PresentWin.IsEnabled = true;

            btn_start.IsEnabled = false;
            btn_stop.IsEnabled = true;
            btn_resume.IsEnabled = false;
            btn_pause.IsEnabled = true;
        }

        private void Btn_stop_Click(object sender, RoutedEventArgs e)
        {
            goNogoTask_PresentWin.Show();
            goNogoTask_PresentWin.IsEnabled = true;

            btn_start.IsEnabled = true;
            btn_stop.IsEnabled = false;
            btn_resume.IsEnabled = false;
            btn_pause.IsEnabled = false;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            Presentation_Start();
        }

        private void Presentation_Start()
        {
            InputBlockDialog inputDialog = new InputBlockDialog("Input Block Number:")
            {
                Top = taskUIScreen.Bounds.Top,
                Left = taskUIScreen.Bounds.Left,
                Owner = this
            };
            if (inputDialog.ShowDialog() == true)
            {
                blockNum = int.Parse(inputDialog.Answer);

                // save all the Input parameters
                saveTaskInf2Savedfile(blockNum);

                btn_start.IsEnabled = false;
                btn_stop.IsEnabled = true;
                btn_resume.IsEnabled = false;
                btn_pause.IsEnabled = true;


                // Show the taskpresent Window on the Touch Screen
                goNogoTask_PresentWin = new GoNogoTask_PresentWin(this)
                {
                    Top = presentTouchScreen.Bounds.Top,
                    Left = presentTouchScreen.Bounds.Left,
                    Name = taskName + "_Win",
                    Owner = this
                };


                // Start the Task
                goNogoTask_PresentWin.Show();
                goNogoTask_PresentWin.Present_Start();
            }
            else
            {
                MessageBox.Show("No Block Number");
            }
        }

        private void Presentation_Stop()
        {

        }

        private void Presentation_Pause()
        {

        }

        private void Presentation_Resume()
        {

        }

        private void saveTaskInf2Savedfile(int blockNum)
        {
            DateTime time_now = DateTime.Now;

            // if saved_folder not exist, created!
            string savedFolder = goNogoTaskConfig.savedFolder;
            if (String.Equals(savedFolder, "default"))
                savedFolder = defaultSaveFolder + taskName;

            if (Directory.Exists(savedFolder) == false)
            {
                Directory.CreateDirectory(savedFolder);
            }

            string filename_saved = textBox_NHPName.Text + time_now.ToString("-yyyyMMdd-HHmmss") + "-Block" + blockNum.ToString() + ".txt";
            file_saved = Path.Combine(savedFolder, filename_saved);

            using (StreamWriter file = new StreamWriter(file_saved))
            {
                file.WriteLine("Date: " + time_now.ToString("MM/dd/yyyy hh:mm:ss tt"));
                file.WriteLine("NHP Name: " + textBox_NHPName.Text);
                file.WriteLine("Task: " + taskName);
                file.WriteLine("\n");


                file.WriteLine(String.Format("{0, -40}:  {1}", "Screen Resolution(Pixal)", presentTouchScreen.Bounds.Width.ToString() + " x " + presentTouchScreen.Bounds.Height.ToString()));
                file.WriteLine(String.Format("{0, -40}:  {1}", "CM to Pixal Ratio", Utility.ratioCM2Pixal.ToString()));
                file.WriteLine(String.Format("{0, -40}:  {1}", "Inch to Pixal Ratio", Utility.ratioIn2Pixal.ToString()));
                file.WriteLine("\n");
                file.WriteLine("\n");
            }

            using (StreamWriter file = File.AppendText(file_saved))
                file.WriteLine("\nPresentation Settings:");
            goNogoTaskConfig.SaveGoNogoMainConfig2TxtFile(file_saved);

            using (StreamWriter file = File.AppendText(file_saved))
                file.WriteLine("\nTarget Position Settings:");
            goNogoTaskConfig.goNogoTargetNumPosConfig.SaveGoNogoJsonTouchGoTargetNumPosString2TxtFile(file_saved);


            using (StreamWriter file = File.AppendText(file_saved))
                file.WriteLine("\nColor Settings:");
            goNogoTaskConfig.goNogoColorConfig.SaveGoNogoJsonColorString2TxtFile(file_saved);

            using (StreamWriter file = File.AppendText(file_saved))
                file.WriteLine("\nTime Settings:");
            goNogoTaskConfig.goNogoTimeConfig.SaveGoNogoJsonTimeString2TxtFile(file_saved);
        }


        public void DisableBtnStartStop()
        {
            BtnStartState = btn_start.IsEnabled;
            BtnStopState = btn_stop.IsEnabled;
            btn_start.IsEnabled = false;
            btn_stop.IsEnabled = false;
        }

        private void TextBox_totalTrialNumPerPosSess_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            goNogoTaskConfig.totalTrialNumPerPosSess = int.Parse(textBox_totalTrialNumPerPosSess.Text);
        }

        private void TextBox_nogoTrialNumPerPosSess_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            goNogoTaskConfig.nogoTrialNumPerPosSess = int.Parse(textBox_nogoTrialNumPerPosSess.Text);
        }

        private void TextBox_NHPName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            goNogoTaskConfig.NHPName = textBox_NHPName.Text;
        }

        public void ResumeBtnStartStopStatus()
        {
            btn_start.IsEnabled = BtnStartState;
            btn_stop.IsEnabled = BtnStopState;
        }


        // Following codes for Checking Specific Key is Pressed 
        void InitializeHook()
        {
            var windowHelper = new WindowInteropHelper(this);
            var windowSource = HwndSource.FromHwnd(windowHelper.Handle);

            windowSource.AddHook(MessagePumpHook);
        }
        void UninitializeHook()
        {
            var windowHelper = new WindowInteropHelper(this);
            var windowSource = HwndSource.FromHwnd(windowHelper.Handle);

            windowSource.RemoveHook(MessagePumpHook);
        }

        IntPtr MessagePumpHook(IntPtr handle, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == TaskHotKeys.WM_HOTKEY)
            {
                if ((int)wParam == TaskHotKeys.HotKeyId_Start)
                {
                    // The hotkey has been pressed, do something!
                    Presentation_Start();
                    handled = true;
                }
                else if ((int)wParam == TaskHotKeys.HotKeyId_Stop)
                {
                    // The hotkey has been pressed, do something!
                    Presentation_Stop();

                    handled = true;
                }
                else if ((int)wParam == TaskHotKeys.HotKeyId_Pause)
                {
                    // The hotkey has been pressed, do something!
                    Presentation_Pause();

                    handled = true;
                }
                else if ((int)wParam == TaskHotKeys.HotKeyId_Resume)
                {
                    // The hotkey has been pressed, do something!
                    Presentation_Resume();

                    handled = true;
                }
            }

            return IntPtr.Zero;
        }
        void InitializeHotKey(Key key, int hotkeyId)
        {
            var windowHelper = new WindowInteropHelper(this);

            // You can specify modifiers such as SHIFT, ALT, CONTROL, and WIN.
            // Remember to use the bit-wise OR operator (|) to join multiple modifiers together.
            uint modifiers = (uint)ModifierKeys.None;

            // We need to convert the WPF Key enumeration into a virtual key for the Win32 API!
            uint virtualKey = (uint)KeyInterop.VirtualKeyFromKey(key);
            TaskHotKeys.RegisterHotKey(windowHelper.Handle, hotkeyId, modifiers, virtualKey);

        }
        void UninitializeHotKey(int hotkeyId)
        {
            var windowHelper = new WindowInteropHelper(this);
            TaskHotKeys.UnregisterHotKey(windowHelper.Handle, hotkeyId);
        }

        private void MenuItem_About(object sender, RoutedEventArgs e)
        {
            GoNogoTask_AboutWin Win_About = new GoNogoTask_AboutWin(this);
            Win_About.Show();
        }

        private void MenuItem_Documentation(object sender, RoutedEventArgs e)
        {
            
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            InitializeHook();

            InitializeHotKey(TaskHotKeys.key_Start, TaskHotKeys.HotKeyId_Start);
        }
    }
}
