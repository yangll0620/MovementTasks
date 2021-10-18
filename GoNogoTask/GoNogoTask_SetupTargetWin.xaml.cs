using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections;
using TasksShared;
using sd = System.Drawing;
using System.Reflection;
using System.Windows.Shapes;

namespace GoNogoTask
{
    /// <summary>
    /// Interaction logic for GoNogoTask_SetupTargetWin.xaml
    /// </summary>
    public partial class GoNogoTask_SetupTargetWin : Window
    {

        private MainWindow parentMainUI;

        ArrayList optPosString_List;
        private TextBox editBox_Pos;
        int indexSelected = 0;

        Window Win_allTargets;

        public GoNogoTask_SetupTargetWin(MainWindow parent)
        {
            InitializeComponent();

            this.parentMainUI = parent;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            parentMainUI.DisableBtnStartStop(); 
            //optPosString_List = new ArrayList();

            LoadInitTargetData();
        }


        private void LoadInitTargetData()
        {

            GoNogoTargetNumPosConfig goNogoTargetNumPosConfig = parentMainUI.goNogoTaskConfig.Get_GoNogoTargetNumPosConfig();

            // Fill in target DiaInch and No of Positions
            textBox_targetDiaInch.Text = goNogoTargetNumPosConfig.targetDiaInch.ToString();
            textBox_targetNoOfPositions.Text = goNogoTargetNumPosConfig.targetNoOfPositions.ToString();

            // Fill in Pos List Box
            optPosString_List = new ArrayList();
            UpdatePosListBox(goNogoTargetNumPosConfig.optPostions_OCenter_List);


            // Editable TextBox for changing position
            editBox_Pos = new TextBox();
            editBox_Pos.Name = "editBox_Pos";
            editBox_Pos.Width = 0;
            editBox_Pos.Height = 0;
            editBox_Pos.Visibility = Visibility.Hidden;
            editBox_Pos.Text = "";
            editBox_Pos.Background = new SolidColorBrush(Colors.Beige);
            editBox_Pos.Foreground = new SolidColorBrush(Colors.Blue);
            Grid_setupTarget.Children.Add(editBox_Pos);
            Grid_setupTarget.RegisterName(editBox_Pos.Name, editBox_Pos);
            Grid_setupTarget.UpdateLayout();
        }


        private void UpdatePosListBox(List<int[]> optPostions_OCenter_List)
        {/*
                Generate the optional X, Y Positions (origin in center)

                Store into class member parent.optPostions_OCenter_List
                and Show on the control listBox_Positions
            */

            // Binding with listBox_Position
            optPosString_List.Clear();
            foreach (int[] xyPos in optPostions_OCenter_List)
            {
                optPosString_List.Add(xyPos[0].ToString() + ", " + xyPos[1].ToString());
            }
            listBox_Positions.ItemsSource = null;
            listBox_Positions.ItemsSource = optPosString_List;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            parentMainUI.ResumeBtnStartStop();
        }

        private void ListBox_Positions_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            indexSelected = listBox_Positions.SelectedIndex;
            CreateEditBox(sender);
        }

        private void CreateEditBox(object sender)
        {
            // Get the position and width/height of selected Item
            ListBoxItem lbi = (ListBoxItem)listBox_Positions.ItemContainerGenerator.ContainerFromItem(listBox_Positions.SelectedItem);
            Point pt = lbi.TransformToAncestor(this).Transform(new Point(0, 0));

            double delta = 3;
            editBox_Pos.HorizontalAlignment = HorizontalAlignment.Left;
            editBox_Pos.VerticalAlignment = VerticalAlignment.Top;
            editBox_Pos.Margin = new Thickness(pt.X + delta, pt.Y + delta, 0, 0);
            editBox_Pos.Width = lbi.ActualWidth;
            editBox_Pos.Height = lbi.ActualHeight;

            editBox_Pos.Visibility = Visibility.Visible;
            editBox_Pos.Focus();

            editBox_Pos.Text = (string)lbi.Content;
            Grid_setupTarget.UpdateLayout();

            editBox_Pos.KeyDown += new KeyEventHandler(this.EditOver);

        }


        private void EditOver(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    string[] strxy = editBox_Pos.Text.Split(',');
                    parentMainUI.goNogoTaskConfig.Get_GoNogoTargetNumPosConfig().optPostions_OCenter_List[indexSelected] = new int[] { int.Parse(strxy[0]), int.Parse(strxy[1]) };

                    optPosString_List[indexSelected] = editBox_Pos.Text;
                    listBox_Positions.ItemsSource = null;
                    listBox_Positions.ItemsSource = optPosString_List;
                    editBox_Pos.Visibility = Visibility.Hidden;
                }
                catch
                {
                    editBox_Pos.Text = "";
                }
            }
        }

        private void Btn_OK_Click(object sender, RoutedEventArgs e)
        {
            SaveTargetData();
            parentMainUI.ResumeBtnStartStop();
            this.Close();
        }

        private void SaveTargetData()
        {/* ---- Save all the Set Target Information back to MainWindow Variables ----- */
            GoNogoTargetNumPosConfig targetConfig = parentMainUI.goNogoTaskConfig.Get_GoNogoTargetNumPosConfig();
            targetConfig.targetDiaInch = float.Parse(textBox_targetDiaInch.Text);
            targetConfig.targetNoOfPositions = int.Parse(textBox_targetNoOfPositions.Text);


            // Extract parent.optPostions_OCenter_List from optPosString_List
            targetConfig.optPostions_OCenter_List.Clear();
            for (int i = 0; i < optPosString_List.Count; i++)
            {
                try
                {
                    string xyPosString = (string)optPosString_List[i];
                    string[] strxy = xyPosString.Split(',');
                    targetConfig.optPostions_OCenter_List.Add(new int[] { int.Parse(strxy[0]), int.Parse(strxy[1]) });
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private void Btn_GenDefaultOptPos_Click(object sender, RoutedEventArgs e)
        {
            int targetNoOfPositions = int.Parse(textBox_targetNoOfPositions.Text);
            GenOptPos(targetNoOfPositions);
        }

        private void GenOptPos(int targetNoOfPositions)
        {// Generate optional Positions based on targetNoOfPositions

            int targetRadius = Utility.Inch2Pixal(float.Parse(textBox_targetDiaInch.Text)) / 2;

            sd.Rectangle Rect_presentScreen = ScreenDetect.TaskPresentationScreen().Bounds;
            int shorter = (Rect_presentScreen.Height > Rect_presentScreen.Width) ? Rect_presentScreen.Width : Rect_presentScreen.Height;
            int radius = (shorter / 2) - targetRadius;
            List<int[]> optPostions_OCenter_List = Utility.GenDefaultPositions_GoNogoTask(targetNoOfPositions, radius);
            UpdatePosListBox(optPostions_OCenter_List);
        }

        private void Btn_ClosePositions_Click(object sender, RoutedEventArgs e)
        {
            Win_allTargets.Close();

            btn_CheckPositions.IsEnabled = true;
            btn_ClosePositions.IsEnabled = false;
            listBox_Positions.IsEnabled = true;
            textBox_targetNoOfPositions.IsEnabled = true;
            textBox_targetDiaInch.IsEnabled = true;
        }

        private void Btn_CheckPositions_Click(object sender, RoutedEventArgs e)
        {
            GoNogoColorConfig colorConfig = parentMainUI.goNogoTaskConfig.Get_GoNogoColorConfig();
            Color BKColor = (Color)(typeof(Colors).GetProperty(colorConfig.BKTrialColorStr) as PropertyInfo).GetValue(null, null);
            Color targetColor = (Color)(typeof(Colors).GetProperty(colorConfig.goFillColorStr) as PropertyInfo).GetValue(null, null);

            // Get the target diameter from textBox_targetDiaCM
            int targetDiaPixal = Utility.Inch2Pixal(float.Parse(textBox_targetDiaInch.Text));

            // if a new targetNoOfPositions
            if (optPosString_List.Count != int.Parse(textBox_targetNoOfPositions.Text))
            {
                GenOptPos(int.Parse(textBox_targetNoOfPositions.Text));
            }

            Win_allTargets = ShowAllTargets(targetDiaPixal, optPosString_List, targetColor, BKColor);

            btn_CheckPositions.IsEnabled = false;
            btn_ClosePositions.IsEnabled = true;
            listBox_Positions.IsEnabled = false;
            textBox_targetNoOfPositions.IsEnabled = false;
            textBox_targetDiaInch.IsEnabled = false;
        }

        private Window ShowAllTargets(int targetDiaPixal, ArrayList optPosString_List, Color targetColor, Color BKColor)
        {/* 
            Show all the targets 

            Args:
                targetDiaPixal: Target Diameter in Pixal

                postions_OriginCenter_List: x,y Position for Each Target (Origin in Screen Center)

                targetColor: the target Color

                BKColor: the Background Color
            */


            //Show the Win_allTargets on the Touch Screen
            Window Win_allTargets = new Window();
            sd.Rectangle Rect_presentScreen = ScreenDetect.TaskPresentationScreen().Bounds;
            Win_allTargets.Top = Rect_presentScreen.Top;
            Win_allTargets.Left = Rect_presentScreen.Left;


            // Show Background
            Win_allTargets.Background = new SolidColorBrush(BKColor);
            Win_allTargets.WindowState = WindowState.Maximized;
            Win_allTargets.Name = "childWin_ShowAllTargets";
            Win_allTargets.WindowStyle = WindowStyle.None;
            Win_allTargets.Show();



            // Add a Grid
            Grid wholeGrid = new Grid();
            wholeGrid.Height = Win_allTargets.ActualHeight;
            wholeGrid.Width = Win_allTargets.ActualWidth;
            Win_allTargets.Content = wholeGrid;
            wholeGrid.UpdateLayout();

            // Close Button
            Button btn_Close = new Button();
            btn_Close.Width = 100;
            btn_Close.Height = 25;
            btn_Close.VerticalAlignment = VerticalAlignment.Top;
            btn_Close.HorizontalAlignment = HorizontalAlignment.Right;
            btn_Close.Margin = new Thickness(0, 0, 0, 0);
            btn_Close.Content = "Close";
            btn_Close.Click += new RoutedEventHandler(Btn_ClosePositions_Click);
            wholeGrid.Children.Add(btn_Close);

            // Extract postions_OriginCenter_List from optPosString_List
            List<int[]> postions_OriginCenter_List = new List<int[]>();
            for (int i = 0; i < optPosString_List.Count; i++)
            {
                try
                {
                    string xyPosString = (string)optPosString_List[i];
                    string[] strxy = xyPosString.Split(',');
                    postions_OriginCenter_List.Add(new int[] { int.Parse(strxy[0]), int.Parse(strxy[1]) });
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            // Show All Targets
            foreach (int[] cPoint_Pos_OCenter in postions_OriginCenter_List)
            {
                // Change the cPoint  into Top Left Coordinate System
                sd.Rectangle Rect_touchScreen = ScreenDetect.TaskPresentationScreen().Bounds;
                int[] cPoint_Pos_OTopLeft = new int[] { cPoint_Pos_OCenter[0] + Rect_touchScreen.Width / 2, cPoint_Pos_OCenter[1] + Rect_touchScreen.Height / 2 };

                Ellipse circle = ShapeManipulate.Create_Circle((double)targetDiaPixal, new SolidColorBrush(targetColor));
                ShapeManipulate.Move_Circle_OTopLeft(circle, cPoint_Pos_OTopLeft);

                wholeGrid.Children.Add(circle);
            }
            wholeGrid.UpdateLayout();

            Win_allTargets.Owner = this;

            return Win_allTargets;
        }


        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            parentMainUI.ResumeBtnStartStop();
            this.Close();
        }

        private void ListBox_Positions_KeyDown(object sender, KeyEventArgs e)
        {
            indexSelected = listBox_Positions.SelectedIndex;
            if (e.Key == Key.F2)
                CreateEditBox(sender);
        }
    }
}
