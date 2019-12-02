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
        private static bool isShuffling = false;

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

        /// <summary>
        /// Reset tro choi
        /// </summary>
        public static void StartNewGame(int Rows, int Cols)
        {
            Database.ConstructDatabase(Rows, Cols);
            UI.ResetBoard();
            //Shuffle
            isShuffling = true;
            Random rnd = new Random();
            int lastDirection = -1;
            int curDirection = -1;
            for (int i = 0; i < 150; i++)
            {
                curDirection = rnd.Next(1, 4);
                if(Math.Abs(curDirection-lastDirection)==1 &&
                    (curDirection!=2 && lastDirection!=3) && (curDirection != 3 && lastDirection != 2))
                {
                    i--;
                    continue;
                }
                Business.DirectionalMovement(curDirection);
                lastDirection = curDirection;
            }
            isShuffling = false;
        }

        /// <summary>
        /// Khoi tao tro choi
        /// </summary>
        /// <param name="canvas">canvas</param>
        /// <param name="Rows">so dong</param>
        /// <param name="Cols">so cot</param>
        public static void InitComponents(ref Canvas canvas,int Rows,int Cols)
        {
            Database.ConstructDatabase(Rows, Cols);
            UI.Start(ref canvas, Rows, Cols);
        }
        /// <summary>
        /// Thuc hien hoan vi 2 doi tuong hinh anh
        /// </summary>
        /// <param name="point1">toa do cua doi tuong thu 1</param>
        /// <param name="point2">toa do cua doi tuong thu 2</param>
        /// <returns>true neu thanh cong, false neu khong thanh cong</returns>
        public static bool Move(Tuple<int,int> point1, Tuple<int,int> point2)
        {
            bool check = Database.SwapPosition(point1, point2);
            if (!check) return check;
            UI.SwapPosition(point1, point2);
            if (Database.CheckWin()&&!isShuffling)
            {
                //Luu diem cua nguoi choi
                MessageBox.Show("You won!");
                Business.StartNewGame(3,3);
            }
            return check;
        }

        /// <summary>
        /// Di chuyen doi tuong rong~ theo 1 trong 4 huong
        /// </summary>
        /// <param name="direction">1:left; 2:right; 3:Up; 4:Down</param>
        /// <returns></returns>
        public static bool DirectionalMovement(int direction)
        {
            Tuple<int, int> blankSpot = Database.GetEmptySpot();
            switch (direction)
            {
                case 1:
                    {
                        Tuple<int, int> Destination = new Tuple<int, int>(blankSpot.Item1, blankSpot.Item2 - 1);
                        return Move(blankSpot, Destination);
                    }
                case 2:
                    {
                        Tuple<int, int> Destination = new Tuple<int, int>(blankSpot.Item1, blankSpot.Item2 + 1);
                        return Move(blankSpot, Destination);
                    }
                case 3:
                    {
                        Tuple<int, int> Destination = new Tuple<int, int>(blankSpot.Item1 - 1, blankSpot.Item2);
                        return Move(blankSpot, Destination);
                    }
                case 4:
                    {
                        Tuple<int, int> Destination = new Tuple<int, int>(blankSpot.Item1 + 1, blankSpot.Item2);
                        return Move(blankSpot, Destination);
                    }
                default:return false;
            }
        }
    }
}
