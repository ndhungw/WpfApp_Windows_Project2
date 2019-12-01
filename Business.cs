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

        public static Point getEmptyIndex()
        {
            Point emptyIndex = new Point();

            for (int i = 0; i < Database.rows; i++)
                for (int j = 0; j < Database.cols; j++)
                    if (Database._a[i, j] == 0)
                    {
                        emptyIndex.X = i;
                        emptyIndex.Y = j;
                    }

            return emptyIndex;
        }

        public static void changeIndex(int newI, int newJ, int oldI, int oldJ)
        {
            Database._a[newI, newJ] = 1;
            Database._a[oldI, oldJ] = 0;
            Database._images[newI, newJ] = Database._images[oldI, oldJ];
            Database._images[oldI, oldJ] = null;
        }

        public static bool checkLegallyMoving(int newI, int newJ, int oldI, int oldJ)
        {
            if (newI != oldI && newJ != oldJ)
                return false;

            if (newI - oldI > 1 || newI - oldI < -1)
                return false;

            if (newJ - oldJ > 1 || newJ - oldJ < -1)
                return false;

            return true;
        }

        //public static bool checkMouseLocateOnPicture()
    }
}
