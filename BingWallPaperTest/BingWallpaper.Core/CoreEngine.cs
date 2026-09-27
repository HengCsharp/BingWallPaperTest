using BingWallpaper.Core.Utilities;
using NLog;
using System.Drawing;
namespace BingWallpaper.Core {
    public class CoreEngine {
        #region 单例模式
        private static object _lockObj = new object();
        private static CoreEngine _instance = new CoreEngine();
        public static CoreEngine Current {
            get {
                lock(_lockObj) {
                    if(_instance == null) {
                        _instance = new CoreEngine();
                    }
                    return _instance;
                }
            }
        }
        #endregion
        public AppSettingOperation AppSetting { get; private set; } = new AppSettingOperation();
        public string AppRootDirection { get; private set; } = AppDomain.CurrentDomain.BaseDirectory;
        /// <summary>
        /// 日志管理器
        /// </summary>
        public Logger Logger = LogManager.GetCurrentClassLogger();

        public void SetWallpaperAsync(bool forceFromWeb = false) { 
            throw new NotImplementedException();
        }

        public void SetWallpaper(bool forceFromWeb = false) {
            Current.Logger.Info($"设置墙纸");
            new WallpaperManager().SetWallpaper();
        }

        public Bitmap GetWallpaperImage(bool forceFromWeb = false) {
            Current.Logger.Info($"U获取桌面壁纸Bitmap");
            return new WallpaperManager().GetWallpaperImage(forceFromWeb);
        }

        public bool DownloadWallpaperImage(DateTime date, out string result) {
            throw new NotImplementedException();
        }
    }
}
