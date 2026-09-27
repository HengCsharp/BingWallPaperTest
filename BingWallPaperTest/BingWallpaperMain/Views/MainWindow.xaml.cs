using BingWallpaper.Core;
using Microsoft.Win32;
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
        private bool _isPackUp = true;
        
        public MainWindow()
        {
            InitializeComponent();
            InitializeUI();
        }

        #region 初始化

        /// <summary>
        /// 初始化界面
        /// </summary>
        private void InitializeUI() {
            PackUp(_isPackUp);
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
            ImgPreview.Source = new WPFSupportFormat().ChangeBitmapToImageSource(bitmap);
        }
        #endregion

        #region 窗体事件
        private void Window_StateChanged(object sender, EventArgs e) {
            var W = (Window)sender;
            var state = W.WindowState;
            if(state == WindowState.Normal) {
                BtnMaxi.Content = (char)0xEF2E;
                BtnMaxi.ToolTip = "Maxi";
                _isWindowNormal = true;
            } else if(state == WindowState.Maximized) {
                BtnMaxi.Content = (char)0xEF2F;
                BtnMaxi.ToolTip = "BackNormal";
                _isWindowNormal = false;
            }
        }

        private void Window_Closed(object sender, EventArgs e) {

        }
        #endregion

        #region 成员事件

        #region 左上方工具栏
        private void BtnPackUp_Click(object sender, RoutedEventArgs e) {
            PackUp(_isPackUp);
        }

        private void BtnOpenImageFolder_Click(object sender, RoutedEventArgs e) {
            //Process.Start("explorer.exe", imgFolderPath);
            var imgFolderPath = CoreEngine.Current.AppSetting.GetImagePath();
            var dialog = new OpenFileDialog();
            dialog.InitialDirectory = imgFolderPath;
            dialog.Filter = "图片文件|*.jpg;";
            dialog.Multiselect = false;

            if(dialog.ShowDialog() == true) {
                string imgPath = dialog.FileName;
                CoreEngine.Current.Logger.Info($"从选中的文件-{imgPath}中设置为背景图片");
                Bitmap bitmap = new Bitmap(imgPath);
                ImgPreview.Source = new WPFSupportFormat().ChangeBitmapToImageSource(bitmap);

            } else {
                CoreEngine.Current.Logger.Debug($"打开图片对话窗失败");
            }
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
        #endregion

        #region 右上方按钮
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

        #region 下方控件
        private void cbImageSize_SelectionChanged(object sender, SelectionChangedEventArgs e) {

        }

        private void cbWallpaperStyle_SelectionChanged(object sender, SelectionChangedEventArgs e) {

        }

        private void btnReflush_Click(object sender, RoutedEventArgs e) {
            SetAppBackground(true);
        }

        private void btnSetWallpaper_Click(object sender, RoutedEventArgs e) {
            CoreEngine.Current.SetWallpaper();
        }
        #endregion

        #region 其它

        /// <summary>
        /// 背景图片拖拽事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImgPreview_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            DragMove();
        }
        #endregion

        #endregion

        #region 成员方法

        private void PackUp(bool isPackUp) {
            _isPackUp = !_isPackUp;
            foreach (var chil in SpToolBar.Children)
            {
                var ctlEle = chil as Grid;
                if(ctlEle.Tag != null && ctlEle.Tag.ToString() == "Unpack") continue;
                ctlEle.Visibility = isPackUp ? Visibility.Collapsed : Visibility.Visible;
            }
            BtnPackUp.Content = isPackUp ? ((char)0xF0D6).ToString() : ((char)0xF0D5).ToString();
            BtnPackUp.ToolTip = isPackUp ? "展开" : "收起";
        }

        #endregion
    }
}