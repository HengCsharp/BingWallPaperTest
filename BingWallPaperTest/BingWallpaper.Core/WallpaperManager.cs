using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace BingWallpaper.Core {
    public class WallpaperManager {
        
        public string GetBingURL(int days = 0) {
            string infoUrl = $"http://cn.bing.com/HPImageArchive.aspx?idx={days}&n=1";
            CoreEngine.Current.Logger.Info(infoUrl);
            string ImageUrl;
            try {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(infoUrl);
                request.Method = "GET";request.ContentType = "text/html;charset=UTF-8";
                string xmlString;
                using(HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
                    Stream stream = response.GetResponseStream();
                    using(StreamReader streamReader = new StreamReader(stream, Encoding.UTF8)) {
                        xmlString = streamReader.ReadToEnd();
                    }
                }
                //===------
                // 定义正则表达式用来匹配标签
                Regex regImg = new Regex("<Url>(?<imgUrl>.*?)</Url>", RegexOptions.IgnoreCase);
                // 搜索匹配的字符串
                MatchCollection matches = regImg.Matches(xmlString);
                // 取得匹配项列表
                ImageUrl = "http://www.bing.com" + matches[0].Groups["imgUrl"].Value;
                //===------
                if(days == 0)//保存copyright
                {
                    CoreEngine.Current.Logger.Info("Bing查询接口：保存Image Copyright");
                    // 定义正则表达式用来匹配标签
                    Regex regCopyright = new Regex("<copyright>(?<imgCopyright>.*?)</copyright>", RegexOptions.IgnoreCase);
                    // 搜索匹配的字符串
                    MatchCollection matchesCopyright = regCopyright.Matches(xmlString);
                    // 取得匹配项列表
                    var copyright = matchesCopyright[0].Groups["imgCopyright"].Value;
                    //CoreEngine.Current.AppSetting.SetCopyright(copyright);
                    CoreEngine.Current.Logger.Info($"Bing查询接口：Copyright:{copyright}");
                }
                return ImageUrl;
            } catch {
                return null;
            }
        }

        public bool SetWallpaper(bool forceFromWeb = false) {
            var imageFolderPath = CoreEngine.Current.AppSetting.GetImagePath();
            var imageFilePath = Path.Combine(imageFolderPath, $"bing{DateTime.Now.ToString("yyyymmdd")}");

            if(forceFromWeb) {
                CoreEngine.Current.Logger.Error("设置墙纸 暂不支持从网络获取");
                return false;
            }

            try {
                SystemParametersInfo(20, 1, imageFilePath, 1);
            } catch (Exception e){
                CoreEngine.Current.Logger.Error(e, $"设置壁纸失败：系统接口调用错误");
                return false;
            }
            return true;
        }

        public Bitmap GetWallpaperImage(bool forceFromWeb = false) {
            var imageFolderPath = CoreEngine.Current.AppSetting.GetImagePath();
            var imageFilePath = Path.Combine(imageFolderPath, $"bing{DateTime.Now.ToString("yyyymmdd")}");
            CoreEngine.Current.Logger.Info($"从本地或网络获取当天最新的图片Bitmap——强制从网络获取:{(forceFromWeb ? "是" : "否")}——文件名：bing{DateTime.Now.ToString("yyyymmdd")}.jpg");
            if(forceFromWeb || !File.Exists(imageFilePath)) {
                CoreEngine.Current.Logger.Info("本地未检测到图片，启用网络下载");
                var bingUrl = GetBingURL();
                if(string.IsNullOrEmpty(bingUrl)) return null;

                var webReq = (HttpWebRequest)WebRequest.Create(bingUrl);
                webReq.Method = "GET";
                try {
                    using(var webres = webReq.GetResponse())//GetResponse
                    {
                        using(var stream = webres.GetResponseStream()) {
                            var bmpWallpaper = (Bitmap)Image.FromStream(stream);

                            if(!Directory.Exists(imageFolderPath)) {
                                Directory.CreateDirectory(imageFolderPath);
                            }
                            bmpWallpaper.Save(imageFilePath, ImageFormat.Jpeg);
                            return bmpWallpaper;

                        }
                    }
                } catch(Exception e) {
                    CoreEngine.Current.Logger.Error(e,"下载壁纸失败：网络连接失败");
                    return null;
                }
            } else {
                Bitmap bitmap;
                try {
                    bitmap = new Bitmap(imageFilePath);
                } catch(Exception e) {
                    CoreEngine.Current.Logger.Error(e, "获取本地Bitmap文件失败");
                    return null;
                }
                return bitmap;
            }
        }

        public bool DownloadWallpaper(DateTime date, out string result) => throw new NotImplementedException();

        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo")]
        public static extern int SystemParametersInfo(
            int uAction,
            int uParam,
            string lpvParm,
            int fuWinIni
        );
    }
}
