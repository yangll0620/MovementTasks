using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MovementTasks
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            cbo_selectTask.SelectedIndex = 0;

            try
            {
                // Get the current directory.
                string path = Directory.GetCurrentDirectory();
                textBlock_1.Text = path;
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
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
                        using (Process myProcess = new Process())
                        {
                            myProcess.StartInfo.UseShellExecute = false;
                            // You can start any process, HelloWorld is a do-nothing example.
                            myProcess.StartInfo.FileName = "H:\\My Drive\\NMRC_umn\\Projects\\ProjDev\\GoNogoTaskDev\\GononGoTask_wpf\\GonoGoTask_wpfVer\\bin\\Release\\GonoGoTask_wpfVer.exe";
                            myProcess.StartInfo.CreateNoWindow = true;
                            myProcess.Start();
                            // This code assumes the process you are starting will terminate itself.
                            // Given that it is started without a window so you cannot terminate it
                            // on the desktop, it must terminate itself or you can do it programmatically
                            // from this application using the Kill method.
                        }
                    }
                    catch (Exception)
                    {}
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
