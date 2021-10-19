using System;
using System.Windows;
using System.IO;
using swf = System.Windows.Forms;
using sd = System.Drawing;
using TasksShared;
using Newtonsoft.Json;
using System.Reflection;

namespace GoNogoTask
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string taskName;
        public string serialPortIO8_name;


        public GoNogoTaskConfig goNogoTaskConfig;

        private bool BtnStartState, BtnStopState;

        string file_saved;

        swf.Screen presentTouchScreen;


        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            taskName = "GoNogoTask";

            // Task UI
            swf.Screen taskUIScreen = ScreenDetect.TaskUIScreen();
            sd.Rectangle Rect_showTaskUIScreen = taskUIScreen.Bounds;
            this.Top = Rect_showTaskUIScreen.Top;
            this.Left = Rect_showTaskUIScreen.Left;


            // Check serial Port IO8 Connection
            //CheckIO8Connection();


            // Load Default Config File
            goNogoTaskConfig = new GoNogoTaskConfig();
            LoadConfigFile("defaultConfig");

            ShowMainConfig();

            // Get the touch Screen
            presentTouchScreen = ScreenDetect.TaskPresentTouchScreen();
            
            //Rect_presentTouchScreen = presentTouchScreen.Bounds;
            /*if (textBox_NHPName.Text != "" && serialPortIO8_name != null)
            {
                btn_start.IsEnabled = true;
                btn_stop.IsEnabled = false;
            }
            else
            {
                btn_start.IsEnabled = false;
                btn_stop.IsEnabled = false;
            }*/
            btn_start.IsEnabled = true;
        }


        private void LoadConfigFile(string configFile)
        {/*Load Config File .json 
            configFile == '': load the default Config File
            */


            System.Windows.MessageBox.Show(Directory.GetCurrentDirectory());

            // Read the Config. File and convert to JsonObject
            if (String.Equals(configFile, "defaultConfig"))
            {
                string movementTaskFolder = Path.GetFullPath(@"..\\..\\..\\");
                configFile = Path.Combine(movementTaskFolder, taskName, "Resources", "ConfigFiles", "defaultConfig.json");
            }
            goNogoTaskConfig.LoadJsonFile2GoNogoConfig(configFile);


            // Set the default savefolder, audios
            if (String.Equals(goNogoTaskConfig.savedFolder, "default"))
                goNogoTaskConfig.savedFolder = @"C:\\" + taskName;

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

        }

        private void btnLoadConf_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            openFileDlg.DefaultExt = ".json";
            openFileDlg.Filter = "Json Files|*.json";

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

        }

        private void btnResume_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_stop_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            // save all the Input parameters
            saveTaskInf2Savedfile();
        }

        private void saveTaskInf2Savedfile()
        {
            DateTime time_now = DateTime.Now;

            // if saved_folder not exist, created!
            string savedFolder = goNogoTaskConfig.savedFolder;
            if (String.Equals(savedFolder, "default"))
                savedFolder = @"C:\\" + taskName;

            if (Directory.Exists(savedFolder) == false)
            {
                System.IO.Directory.CreateDirectory(savedFolder);
            }

            string filename_saved = textBox_NHPName.Text + time_now.ToString("-yyyyMMdd-HHmmss") + ".txt";
            file_saved = System.IO.Path.Combine(savedFolder, filename_saved);

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

        public void ResumeBtnStartStop()
        {
            btn_start.IsEnabled = BtnStartState;
            btn_stop.IsEnabled = BtnStopState;
        }
    }
}
