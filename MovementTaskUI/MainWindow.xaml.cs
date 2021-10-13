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
            String taskName = cbo_selectTask.SelectedItem.ToString();
            switch (taskName)
            {
                case "Selfpaced Task":
                    break;
                case "GoNogo Task":
                    try
                    {
                        using (Process taskProcess = new Process())
                        {
                            taskProcess.StartInfo.UseShellExecute = false;
                            taskProcess.StartInfo.FileName = @"..\\..\\..\\GoNogoTask\\bin\\Debug\\GoNogoTask.exe";
                            taskProcess.StartInfo.CreateNoWindow = true;
                            taskProcess.Start();
                            // This code assumes the process you are starting will terminate itself.
                            // Given that it is started without a window so you cannot terminate it
                            // on the desktop, it must terminate itself or you can do it programmatically
                            // from this application using the Kill method.
                        }
                    }
                    catch (Exception)
                    { }
                    break;
                case "COT Task":
                    break;
                default:
                    break;
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
