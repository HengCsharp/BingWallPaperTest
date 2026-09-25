using BingWallpaper.Core;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BingWallPaperTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isWindowNormal = true;
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SetAppBackground(bool force = false, bool showSuccess = false) {
            Bitmap bitmap = CoreEngine.Current.GetWallpaperImage(force);
            if (bitmap == null) {
                CoreEngine.Current.Logger.Info("获取图片资源失败");
                //Alert.Show("获取图片资源失败", AlertTheme.Error);
                return;
            }
            CoreEngine.Current.Logger.Info("获取图片资源成功");
            //tbImageCopyright.ToolTip = tbImageCopyright.Text = CoreEngine.Current.AppSetting.GetCopyright;
            //ImgPreview.Source = new WPFSupportFormat().ChangeBitmapToImageSource(bitmap);
        }
        private void ImgPreview_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            throw new NotImplementedException();
        }

        private void BtnPackUp_Click(object sender, RoutedEventArgs e) {
            throw new NotImplementedException();
        }

        private void BtnOpenImageFolder_Click(object sender, RoutedEventArgs e) {
            throw new NotImplementedException();
        }

        private void BtnDownload_Click(object sender, RoutedEventArgs e) {
            throw new NotImplementedException();
        }

        private void BtnOpenSetting_Click(object sender, RoutedEventArgs e) {
            throw new NotImplementedException();
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e) {

        }

        private void BtnAbout_Click(object sender, RoutedEventArgs e) {

        }

        #region RightUp Buttons
        private void BtnClose_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        private void BtnMaxi_Click(object sender, RoutedEventArgs e) {
            if(_isWindowNormal) {
                WindowState = WindowState.Maximized;
            } else {
                WindowState = WindowState.Normal;
            }
        }

        private void BtnMini_Click(object sender, RoutedEventArgs e) {
            WindowState = WindowState.Minimized;
        }

        private void BtnFeedback_Click(object sender, RoutedEventArgs e) {
            var proc = new Process();
            proc.StartInfo.FileName = "https://forms.office.com/Pages/ResponsePage.aspx?id=DQSIkWdsW0yxEjajBLZtrQAAAAAAAAAAAAO__cwTRKlUOUFMOVJHRlhTMDhZUDRRU05YMzlHOTFDNy4u";
            proc.Start();
        }
        #endregion

        private void cbImageSize_SelectionChanged(object sender, SelectionChangedEventArgs e) {

        }

        private void cbWallpaperStyle_SelectionChanged(object sender, SelectionChangedEventArgs e) {

        }

        private void btnReflush_Click(object sender, RoutedEventArgs e) {

        }

        private void btnSetWallpaper_Click(object sender, RoutedEventArgs e) {

        }
        #region Window events
        private void Window_StateChanged(object sender, EventArgs e) {
            var W = (Window)sender;
            var state = W.WindowState;
            if(state == WindowState.Normal) 
            {
                BtnMaxi.Content = (char)0xEF2E;
                BtnMaxi.ToolTip = "Maxi";
                _isWindowNormal = true;
            } 
            else if(state == WindowState.Maximized) 
            {
                BtnMaxi.Content = (char)0xEF2F;
                BtnMaxi.ToolTip = "BackNormal";
                _isWindowNormal = false;
            }
        }
        #endregion

        private void Window_Closed(object sender, EventArgs e) {

        }
    }
}