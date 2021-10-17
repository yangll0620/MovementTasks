using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;


namespace MovementTaskUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbo_selectTask.SelectedIndex = 0;
        }
        private void Btn_OK_Click(object sender, RoutedEventArgs e)
        {
            string taskShowName = cbo_selectTask.SelectedItem.ToString();
            string taskPrjName;
            switch (taskShowName)
            {
                case "Selfpaced Task":
                    taskPrjName = "SelfpacedTask";
                    break;
                case "GoNogo Task":
                    taskPrjName = "GoNogoTask";
                    break;
                case "COT Task":
                    taskPrjName = "COTTask";
                    break;
                default:
                    taskPrjName = "";
                    break;
            }

            // Open Selected Task
            string movementTaskFolder = Path.GetFullPath(@"..\\..\\..\\");
            string debugSubPath = Path.Combine("bin", "Debug");
            string taskExeFileName = Path.Combine(movementTaskFolder, taskPrjName, debugSubPath, taskPrjName + ".exe");
            try
            {
                using (Process taskProcess = new Process())
                {
                    taskProcess.StartInfo.UseShellExecute = false;
                    taskProcess.StartInfo.FileName = taskExeFileName;
                    taskProcess.StartInfo.CreateNoWindow = true;
                    taskProcess.Start();
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Can't Open Task");
            }
        }
    }


    class TaskOptions : ObservableCollection<string>
    {
        public TaskOptions()
        {
            Add("GoNogo Task");
            Add("COT Task");
            Add("Selfpaced Task");
        }
    }
}
