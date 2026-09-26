using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BingWallPaperTest {
    public class WPFSupportFormat {
        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteObject(IntPtr hObject);

        public ImageSource ChangeBitmapToImageSource(Bitmap bitmap) {
            if(bitmap == null) return null;

            IntPtr hBitmap = IntPtr.Zero;
            try {
                hBitmap = bitmap.GetHbitmap(); // obtain HBITMAP from System.Drawing.Bitmap
                var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
                bitmapSource.Freeze(); // make it cross-thread accessible / immutable
                return bitmapSource;
            } finally {
                if(hBitmap != IntPtr.Zero) {
                    DeleteObject(hBitmap); // avoid GDI handle leak
                }
            }
        }
    }
}