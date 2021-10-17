using System;
using System.Windows;
using System.IO;
using swf = System.Windows.Forms;
using sd = System.Drawing;
using TasksShared;


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

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            taskName = "GoNogoTask";

            // Task UI
            swf.Screen taskUIScreen = Utility.TaskUIScreen();
            sd.Rectangle Rect_showTaskUIScreen = taskUIScreen.Bounds;
            this.Top = Rect_showTaskUIScreen.Top;
            this.Left = Rect_showTaskUIScreen.Left;


            // Check serial Port IO8 Connection
            //CheckIO8Connection();


            // Load Default Config File
            goNogoTaskConfig = new GoNogoTaskConfig();
            LoadConfigFile("defaultConfig");

            ShowMainConfig();
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
        }


        private void LoadConfigFile(string configFile)
        {/*Load Config File .json 
            configFile == '': load the default Config File
            */


            MessageBox.Show(Directory.GetCurrentDirectory());

            // Read the Config. File and convert to JsonObject
            if (String.Equals(configFile, "defaultConfig"))
            {
                string movementTaskFolder = Path.GetFullPath(@"..\\..\\..\\");
                configFile = Path.Combine(movementTaskFolder, taskName, "Resources", "ConfigFiles", "defaultConfig.json");
            }
            goNogoTaskConfig.LoadJsonFile2GoNogoConfig(configFile);
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

        }

        private void MenuItem_SetupTime(object sender, RoutedEventArgs e)
        {
            GoNogoTask_SetupTimeWin Win_SetupTime = new GoNogoTask_SetupTimeWin(this);
 
            swf.Screen showScreen = Utility.TaskUIScreen();
            sd.Rectangle Rect_showScreen = showScreen.Bounds;
            Win_SetupTime.Top = Rect_showScreen.Top;
            Win_SetupTime.Left = Rect_showScreen.Left;

            // Set Owner
            Win_SetupTime.Owner = this;

            Win_SetupTime.Show();
        }

        private void MenuItem_SetupTarget(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_SetupSaveFolderAudio(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_comReconnect_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnLoadConf_Click(object sender, RoutedEventArgs e)
        {

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

        }

        private void MenuItem_SaveConf_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
