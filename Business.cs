using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace WpfApp_Windows_Project2
{
    public static class Business
    {
        public static Image CropImage(BitmapImage bitmapImage, Int32Rect int32Rect, int width, int height)
        {
            var cropBitmap = new CroppedBitmap(bitmapImage, int32Rect);
            var cropImage = new Image();
            cropImage.Stretch = Stretch.Fill;
            cropImage.Width = width;
            cropImage.Height = height;
            cropImage.Source = cropBitmap;
            return cropImage;
        }

        public static void setLeftTopImage( Image _selectedBitmap, double left, double top)
        {
            Canvas.SetLeft(_selectedBitmap, left);
            Canvas.SetTop(_selectedBitmap, top);
        }


    }
}
