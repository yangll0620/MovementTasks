using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    /// Interaction logic for GoNogoTask_SetupColorsWin.xaml
    /// </summary>
    public partial class GoNogoTask_SetupColorsWin : Window
    {
        private MainWindow parentMainUI;
        public GoNogoTask_SetupColorsWin(MainWindow parent)
        {
            InitializeComponent();

            this.parentMainUI = parent;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            parentMainUI.DisableBtnStartStop();

            LoadInitColorsData();
        }

        private void LoadInitColorsData()
        {
            //Data binding the Color ComboBoxes
            cbo_goColor.ItemsSource = typeof(Colors).GetProperties();
            cbo_nogoColor.ItemsSource = typeof(Colors).GetProperties();
            cbo_cueColor.ItemsSource = typeof(Colors).GetProperties();
            cbo_BKWaitTrialColor.ItemsSource = typeof(Colors).GetProperties();
            cbo_BKTrialColor.ItemsSource = typeof(Colors).GetProperties();
            cbo_CorrFillColor.ItemsSource = typeof(Colors).GetProperties();
            cbo_CorrOutlineColor.ItemsSource = typeof(Colors).GetProperties();
            cbo_ErrorFillColor.ItemsSource = typeof(Colors).GetProperties();
            cbo_ErrorOutlineColor.ItemsSource = typeof(Colors).GetProperties();


            // Set Default Selected Item
            GoNogoColorConfig colorsConfig = parentMainUI.goNogoTaskConfig.goNogoColorConfig;
            cbo_goColor.SelectedItem = typeof(Colors).GetProperty(colorsConfig.goFillColorStr);
            cbo_nogoColor.SelectedItem = typeof(Colors).GetProperty(colorsConfig.nogoFillColorStr);
            cbo_cueColor.SelectedItem = typeof(Colors).GetProperty(colorsConfig.cueCrossingColorStr);
            cbo_BKWaitTrialColor.SelectedItem = typeof(Colors).GetProperty(colorsConfig.BKWaitTrialColorStr);
            cbo_BKTrialColor.SelectedItem = typeof(Colors).GetProperty(colorsConfig.BKTrialColorStr);
            cbo_CorrFillColor.SelectedItem = typeof(Colors).GetProperty(colorsConfig.CorrFillColorStr);
            cbo_CorrOutlineColor.SelectedItem = typeof(Colors).GetProperty(colorsConfig.CorrOutlineColorStr);
            cbo_ErrorFillColor.SelectedItem = typeof(Colors).GetProperty(colorsConfig.ErrorFillColorStr);
            cbo_ErrorOutlineColor.SelectedItem = typeof(Colors).GetProperty(colorsConfig.ErrorOutlineColorStr);
        }

        private void Btn_OK_Click(object sender, RoutedEventArgs e)
        {
            SaveColorsData();
            parentMainUI.ResumeBtnStartStopStatus();
            this.Close();
        }

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            parentMainUI.ResumeBtnStartStopStatus();
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            parentMainUI.ResumeBtnStartStopStatus();
        }

        private void SaveColorsData()
        { /* ---- Save all the Select Colors Information back to MainWindow Color Strings ----- */

            GoNogoColorConfig colorsConfig = parentMainUI.goNogoTaskConfig.goNogoColorConfig;
            colorsConfig.goFillColorStr = (cbo_goColor.SelectedItem as PropertyInfo).Name;
            colorsConfig.nogoFillColorStr = (cbo_nogoColor.SelectedItem as PropertyInfo).Name;
            colorsConfig.cueCrossingColorStr = (cbo_cueColor.SelectedItem as PropertyInfo).Name;
            colorsConfig.BKWaitTrialColorStr = (cbo_BKWaitTrialColor.SelectedItem as PropertyInfo).Name;
            colorsConfig.BKTrialColorStr = (cbo_BKTrialColor.SelectedItem as PropertyInfo).Name;
            colorsConfig.CorrFillColorStr = (cbo_CorrFillColor.SelectedItem as PropertyInfo).Name;
            colorsConfig.CorrOutlineColorStr = (cbo_CorrOutlineColor.SelectedItem as PropertyInfo).Name;
            colorsConfig.ErrorFillColorStr = (cbo_ErrorFillColor.SelectedItem as PropertyInfo).Name;
            colorsConfig.ErrorOutlineColorStr = (cbo_ErrorOutlineColor.SelectedItem as PropertyInfo).Name;
        }
    }
}
