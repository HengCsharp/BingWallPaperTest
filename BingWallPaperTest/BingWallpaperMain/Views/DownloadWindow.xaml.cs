using BingWallpaper.Core;
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
using BingWallpaper.Core.Utilities;

namespace BingWallPaperTest.Views {
    /// <summary>
    /// Interaction logic for DownloadWindow.xaml
    /// </summary>
    public partial class DownloadWindow : Window {
        public DownloadWindow() {
            InitializeComponent();
            Init();
        }

        private void Init() {
            dpStart.DisplayDateStart = DateTime.Now.AddDays(-6);
            dpStart.DisplayDateEnd = DateTime.Now;
            dpEnd.DisplayDateStart = DateTime.Now.AddDays(-6);
            dpEnd.DisplayDateEnd = DateTime.Now;
            dpStart.SelectedDate = DateTime.Now.AddDays(-6);
            dpEnd.SelectedDate = DateTime.Now;
            var localImgCount = new FileUtil().GetLocalImagesUrl().Count;
            tbPicCount.Text = $"本地：{localImgCount}张";
            tBoxDetail.Text = "*由于Bing接口限制，仅支持下载七天内图片。\r\n";
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        private void btnDownload_Click(object sender, RoutedEventArgs e) {
            if(null == dpStart.SelectedDate || null == dpEnd.SelectedDate) {
                ShowLog("警告：日期不能为空\r\n");
                return;
            }
            var start = dpStart.SelectedDate.Value;
            var end = dpEnd.SelectedDate.Value;
            if(start > end) {
                ShowLog($"警告：开始日期不能大于结束日期\r\n");
                return;
            }
            btnDownload.Content = "正在下载";
            btnDownload.IsEnabled = false;
            Task.Run(() => Download(start, end));
        }

        private void Download(DateTime start, DateTime end) {
            var interval = new TimeSpan(end.Ticks - start.Ticks).Days + 1;
            var pr = 1f;
            for( DateTime date = start; date <= end; date = date.AddDays(1), pr++) {
                var isSuccess = CoreEngine.Current.DownloadWallpaperImage(date, out string result);
                this.Dispatcher.Invoke(() => {
                    ShowLog($"{result}：{date.ToString("yyyymmdd")}.jpg\r\n");
                    var process = Convert.ToInt32((pr / interval) * 100);
                    pbDownload.Value = process;
                    tbProcess.Text = $"{process}%";
                });
            }

            this.Dispatcher.Invoke(new Action(() => {
                btnDownload.Content = "开始下载";
                btnDownload.IsEnabled = true;
                ShowLog("下载完成\r\n");
                var localImgCount = new FileUtil().GetLocalImagesUrl().Count;
                tbPicCount.Text = $"本地：{localImgCount}张";
            }));
        }

        private void HeadBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            DragMove();
        }

        public void ShowLog(string msg) {
            tBoxDetail.AppendText(msg);
            tBoxDetail.ScrollToEnd();
        }
    }
}
