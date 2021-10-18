using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using TasksShared;

namespace GoNogoTask
{
    /// <summary>
    /// Interaction logic for GoNogoTask_SetupTimeWin.xaml
    /// </summary>
    public partial class GoNogoTask_SetupTimeWin : Window
    {
        private MainWindow parentMainUI;
        

        public GoNogoTask_SetupTimeWin(MainWindow parent)
        {
            InitializeComponent();

            this.parentMainUI = parent;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            parentMainUI.DisableBtnStartStop();

            LoadInitTimeData();
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            parentMainUI.ResumeBtnStartStop();
        }

        private void Btn_OK_Click(object sender, RoutedEventArgs e)
        {
            SaveTimeData();
            parentMainUI.ResumeBtnStartStop();
            this.Close();
        }

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            parentMainUI.ResumeBtnStartStop();
            this.Close();
        }

        private void LoadInitTimeData()
        {// Load Initial Time Data from parentMainUI

            GoNogoTimeConfig goNogoTimeConfig = parentMainUI.goNogoTaskConfig.Get_GoNogoTimeConfig();


            textBox_tReady_min.Text = goNogoTimeConfig.tRange_ReadyTimeS[0].ToString();
            textBox_tReady_max.Text = goNogoTimeConfig.tRange_ReadyTimeS[1].ToString(); 

            textBox_tCue_min.Text = goNogoTimeConfig.tRange_CueTimeS[0].ToString();
            textBox_tCue_max.Text = goNogoTimeConfig.tRange_CueTimeS[1].ToString();

            textBox_tNogoShow_min.Text = goNogoTimeConfig.tRange_NogoShowTimeS[0].ToString();
            textBox_tNogoShow_max.Text = goNogoTimeConfig.tRange_NogoShowTimeS[1].ToString();


            textBox_MaxReactionTime.Text = goNogoTimeConfig.t_MaxReactionTimeS.ToString();
            textBox_MaxReachTime.Text = goNogoTimeConfig.t_MaxReachTimeS.ToString();
            

            textBox_tInterTrial.Text = goNogoTimeConfig.t_InterTrialS.ToString();
            textBox_tVisFeedback.Text = goNogoTimeConfig.t_VisfeedbackShowS.ToString();
        }

        private void SaveTimeData()
        {/* ---- Save all the Set Time Information back to MainWindow Variables ----- */


            GoNogoTimeConfig goNogoTimeConfig = parentMainUI.goNogoTaskConfig.Get_GoNogoTimeConfig();

            goNogoTimeConfig.tRange_ReadyTimeS[0] = float.Parse(textBox_tReady_min.Text);
            goNogoTimeConfig.tRange_ReadyTimeS[1] = float.Parse(textBox_tReady_max.Text);


            goNogoTimeConfig.tRange_CueTimeS[0] = float.Parse(textBox_tCue_min.Text);
            goNogoTimeConfig.tRange_CueTimeS[1]= float.Parse(textBox_tCue_max.Text);

            goNogoTimeConfig.tRange_NogoShowTimeS[0] = float.Parse(textBox_tNogoShow_min.Text);
            goNogoTimeConfig.tRange_NogoShowTimeS[1] = float.Parse(textBox_tNogoShow_max.Text);

            goNogoTimeConfig.t_MaxReactionTimeS = float.Parse(textBox_MaxReactionTime.Text);
            goNogoTimeConfig.t_MaxReachTimeS = float.Parse(textBox_MaxReachTime.Text);

            goNogoTimeConfig.t_InterTrialS = float.Parse(textBox_tInterTrial.Text);
            goNogoTimeConfig.t_VisfeedbackShowS = float.Parse(textBox_tVisFeedback.Text);
        }
    }
}
