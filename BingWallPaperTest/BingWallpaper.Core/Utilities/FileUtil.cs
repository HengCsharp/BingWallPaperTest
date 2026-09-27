using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BingWallpaper.Core.Utilities {
    /// <summary>
    /// 文件管理工具
    /// </summary>
    public class FileUtil {
        public List<string> GetLocalImagesUrl() {
            var list = new List<string>();
            var imgFolderPath = CoreEngine.Current.AppSetting.GetImagePath();
            DirectoryInfo root = new DirectoryInfo(imgFolderPath);
            FileInfo[] files = root.GetFiles();
            foreach(FileInfo file in files) {
                var path = file.FullName;
                if(Path.GetExtension(path) == ".jpg") {
                    list.Add(path);
                }
            }
            return list;
        }
    }
}
