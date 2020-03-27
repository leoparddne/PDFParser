using Microsoft.Win32;
using Parser;
using System;
using System.Windows;
using System.Windows.Forms;

namespace Viewer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        string filePath
        {
            get
            {
                return txtFileName.Text.Trim();
            }
        }
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFile = new Microsoft.Win32.OpenFileDialog();
            openFile.Filter = "(*.pdf)|*.pdf";

            if ((bool)openFile.ShowDialog())
            {
                txtFileName.Text = openFile.FileName;
            }
        }

        private void btnParse_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                //txtResult.Text = SpireParser.GetAllText(filePath);
                txtResult.Text = ITextSharpParser.ReadAllText(filePath);
            }
        }
        private void btnPageText_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                int page = Convert.ToInt32(txtPage.Text.Trim());

                //txtResult.Text = SpireParser.GetAllText(filePath);
                txtResult.Text = ITextSharpParser.ReadPage(filePath, page);
            }
        }

        private void btnSavePageImage_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                try
                {
                    int page = Convert.ToInt32(txtPage.Text.Trim());
                    //索引从0开始
                    SpireParser.SavePageImage(filePath, page-1, txtImageSavePath.Text.Trim());
                }
                catch (Exception ex)
                {
                    txtResult.Text = ex.Message;
                }
            }
        }

        private void btnSaveAllImage_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                try
                {
                    SpireParser.SaveAllImage(filePath, txtImageSavePath.Text.Trim());

                    txtResult.Text = "success";
                }
                catch (Exception ex)
                {
                    txtResult.Text = ex.Message;
                }
            }
        }

        private void btnImagePath_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtImageSavePath.Text = fbd.SelectedPath;
            }
        }
    }
}
