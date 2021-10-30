using System;
using System.Windows;
using System.Windows.Forms;

namespace GoNogoTask
{
    /// <summary>
    /// Interaction logic for GoNogoTask_SetupSavefolderAudiosWin.xaml
    /// </summary>
    public partial class GoNogoTask_SetupSavefolderAudiosWin : Window
    {

        private MainWindow parentMainUI;
        public GoNogoTask_SetupSavefolderAudiosWin(MainWindow parent)
        {
            InitializeComponent();

            this.parentMainUI = parent;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            parentMainUI.DisableBtnStartStop();
            LoadInitSavefolderAudiosData();
        }

        private void LoadInitSavefolderAudiosData()
        {

            // Fill in saveFolder, audioFile_Correct and audioFile_Error Texts
            textBox_savedFolder.Text = parentMainUI.goNogoTaskConfig.savedFolder;
            textBox_audioFile_Correct.Text = parentMainUI.goNogoTaskConfig.audioFile_Correct;
            textBox_audioFile_Error.Text = parentMainUI.goNogoTaskConfig.audioFile_Error;
        }

        private void Btn_OK_Click(object sender, RoutedEventArgs e)
        {
            SaveSavefolderAudiosData();
            parentMainUI.ResumeBtnStartStopStatus();
            this.Close();
        }

        private void SaveSavefolderAudiosData()
        {/* ---- Save all the Set Information back to MainWindow Variables ----- */

            parentMainUI.goNogoTaskConfig.savedFolder = textBox_savedFolder.Text;
            parentMainUI.goNogoTaskConfig.audioFile_Correct = textBox_audioFile_Correct.Text;
            parentMainUI.goNogoTaskConfig.audioFile_Error = textBox_audioFile_Error.Text;
        }

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            parentMainUI.ResumeBtnStartStopStatus();
            this.Close();
        }

        private void Btn_SelectSavefolder_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDlg = new FolderBrowserDialog();
            folderBrowserDlg.Description = "Select the directory for saving data";

            DialogResult result = folderBrowserDlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                textBox_savedFolder.Text = folderBrowserDlg.SelectedPath;
            }
        }

        private string Select_AudioFile_Dlg(string title)
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
            openFileDlg.FileName = "Document";
            openFileDlg.DefaultExt = ".wav";
            openFileDlg.Filter = "Audio Files (.wav)|*.wav";
            openFileDlg.Title = title;
            Nullable<bool> result = openFileDlg.ShowDialog();

            string filename = "";
            if (result == true)
                filename = openFileDlg.FileName;

            return filename;
        }

        private void Btn_Select_AudioFile_Correct_Click(object sender, RoutedEventArgs e)
        {
            textBox_audioFile_Correct.Text = Select_AudioFile_Dlg("Selecting an Audio for Correct Trials");
        }

        private void Btn_Select_AudioFile_Error_Click(object sender, RoutedEventArgs e)
        {
            textBox_audioFile_Correct.Text = Select_AudioFile_Dlg("Selecting an Audio for Error Trials");
        }
    }
}
