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

namespace TasksShared
{
    /// <summary>
    /// Interaction logic for InputBlockDialog.xaml
    /// </summary>
    public partial class InputBlockDialog : Window
    {
        public InputBlockDialog(string question)
        {
            InitializeComponent();
            label_Question.Content = question;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        public string Answer
        {
            get { return textbox_Answer.Text; }
        }
    }
}
