using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BingWallpaper.Core.Utilities {
    public class AppSettingOperation {
        public string GetImagePath() {
            return Path.Combine(CoreEngine.Current.AppRootDirection, "Image");
        }
    }
}
