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
    internal class WallpaperManager {
        private static readonly HttpClient _httpClient = new HttpClient() {
            Timeout = TimeSpan.FromSeconds(30)
        };
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

        public bool SetWallpaper(bool forceFromWeb = false) => throw new NotImplementedException();

        // preserve existing sync API while implementing a robust async flow
        public Bitmap GetWallpaperImage(bool forceFromWeb = false) {
            try {
                return GetWallpaperImageAsync(forceFromWeb).GetAwaiter().GetResult();
            } catch(Exception ex) {
                CoreEngine.Current.Logger.Error(ex, "GetWallpaperImage failed (sync wrapper)");
                return null;
            }
        }

        public async Task<Bitmap> GetWallpaperImageAsync(bool forceFromWeb = false) {
            var imageFolderPath = CoreEngine.Current.AppSetting.GetImagePath();
            var imageFileName = $"bing{DateTime.Now:yyyyMMdd}.jpg";
            var imageFilePath = Path.Combine(imageFolderPath, imageFileName);
            CoreEngine.Current.Logger.Info($"从本地或网络获取当天最新的图片Bitmap——强制从网络获取:{(forceFromWeb ? "是" : "否")}——文件名：{imageFileName}");

            // If local file exists and not forcing web, try load and return
            if(!forceFromWeb && File.Exists(imageFilePath)) {
                try {
                    var bytes = await File.ReadAllBytesAsync(imageFilePath).ConfigureAwait(false);
                    using var ms = new MemoryStream(bytes);
                    using var loaded = new Bitmap(ms);
                    // Clone to detach from underlying stream
                    var clone = new Bitmap(loaded);
                    return clone;
                } catch(Exception ex) {
                    CoreEngine.Current.Logger.Error(ex, "获取本地Bitmap文件失败，尝试从网络下载");
                    // fall through to attempt network download
                }
            }

            // Download from web
            CoreEngine.Current.Logger.Info("本地未检测到图片或已请求强制下载，启用网络下载");
            var bingUrl = GetBingURL();
            if(string.IsNullOrEmpty(bingUrl)) {
                CoreEngine.Current.Logger.Info("未能获取 Bing 图像 URL");
                return null;
            }

            try {
                using var response = await _httpClient.GetAsync(bingUrl, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                if(!response.IsSuccessStatusCode) {
                    CoreEngine.Current.Logger.Error($"下载失败，HTTP 状态码: {response.StatusCode}");
                    return null;
                }

                using var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                using var memoryStream = new MemoryStream();
                await responseStream.CopyToAsync(memoryStream).ConfigureAwait(false);
                var data = memoryStream.ToArray();

                // Create Bitmap clone from bytes to avoid stream dependency
                using(var msForBitmap = new MemoryStream(data))
                using(var bmpFromStream = new Bitmap(msForBitmap)) {
                    var resultBitmap = new Bitmap(bmpFromStream);

                    // Ensure directory exists and save atomically
                    try {
                        if(!Directory.Exists(imageFolderPath)) {
                            Directory.CreateDirectory(imageFolderPath);
                        }

                        var tempFile = Path.Combine(imageFolderPath, $"{Guid.NewGuid():N}.tmp");
                        await File.WriteAllBytesAsync(tempFile, data).ConfigureAwait(false);
                        // atomic replace (overwrite) if target exists
                        File.Move(tempFile, imageFilePath, overwrite: true);
                    } catch(Exception exFile) {
                        CoreEngine.Current.Logger.Error(exFile, "保存壁纸到本地失败，但仍返回下载的 Bitmap");
                        // still return the bitmap even if saving failed
                    }

                    return resultBitmap;
                }
            } catch(Exception e) {
                CoreEngine.Current.Logger.Error(e, "下载壁纸失败：网络连接失败");
                return null;
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
