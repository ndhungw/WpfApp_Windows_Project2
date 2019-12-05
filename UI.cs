using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp_Windows_Project2
{
    public static class UI
    {
        const int startX = 71;  //vị trí bắt đầu vẽ theo trục X
        const int startY = 40;  //vị trí bắt đầu vẽ theo trục Y
        const int width = 150;   //chiều rộng mỗi ô
        const int height = 150;  //chiều dài mỗi ô
        const int margin = 4;

        public static bool isEmpty = true;
        public static bool disableAnim = false;
        private static Canvas canvas;
        static public Window window;
        private static Image[,] images;
        private static string currentImageLink;
        private static Line[,] rows;
        private static Line[,] cols;

        /// <summary>
        /// Khoi dong UI
        /// </summary>
        /// <param name="cv">Canvas</param>
        /// <param name="Rows">So dong</param>
        /// <param name="Cols">So cot</param>
        public static void Start(ref Canvas cv, Window wd ,int Rows,int Cols)
        {
            canvas = cv;
            window = wd;
            images = new Image[Rows, Cols];
        }

        /// <summary>
        /// Ve gameboard
        /// </summary>
        public static void DrawLines()
        {
            rows = new Line[images.GetLength(0) + 1, images.GetLength(1)];
            cols = new Line[images.GetLength(0) + 1, images.GetLength(1)];
            for (int i = 0; i < rows.GetLength(0); i++)
                for(int j = 0; j < rows.GetLength(1); j++)
                {
                    cols[i, j] = new Line();
                    rows[i, j] = new Line();
                    cols[i, j].Stroke = new SolidColorBrush(Colors.Black);
                    rows[i, j].Stroke = new SolidColorBrush(Colors.Black);
                    rows[i, j].StrokeThickness = 4;
                    cols[i, j].StrokeThickness = 4;
                    canvas.Children.Add(cols[i, j]);
                    canvas.Children.Add(rows[i, j]);
                    rows[i, j].X1 = (startX - 2) + j * (width + margin);
                    rows[i, j].Y1 = (startY - 2) + i * (height + margin);
                    rows[i, j].X2 = (startX - 2) + (j + 1) * (width + margin);
                    rows[i, j].Y2 = (startY - 2) + i * (height + margin);

                    cols[i, j].X1 = (startX - 2) + i * (width + margin);
                    cols[i, j].Y1 = (startY - 2) + j * (height + margin);
                    cols[i, j].X2 = (startX - 2) + i * (width + margin);
                    cols[i, j].Y2 = (startY - 2) + (j + 1) * (height + margin);
                }
        }
        public static void setLeftTopImage(Image _selectedBitmap, double left, double top)
        {
            try
            {
                Canvas.SetLeft(_selectedBitmap, left);
                Canvas.SetTop(_selectedBitmap, top);
            }
            catch (System.ArgumentNullException e)
            {
                Debug.WriteLine($"Error: {e.Message} \nĐây là trường hợp người dùng click quá nhanh" +
                    $" nên sau khi FileDialog vừa tắt thì chuột click vào cropImage ở phần giao diện" +
                    $" trong khi hình chưa được cắt xong");
            }
        }

        /// <summary>
        /// Load ma tran hinh vao class UI
        /// </summary>
        /// <param name="imgs">ma tran Image</param>
        public static void InitUIMatrix(ref Image[,] imgs)
        {
            images = imgs;
        }

        /// <summary>
        /// Set link hinh anh goc
        /// </summary>
        /// <param name="link">link hinh</param>
        public static void SetImage(string link)
        {
            currentImageLink = link;
            isEmpty = false;
        }
        

        /// <summary>
        /// Lay link hinh anh goc
        /// </summary>
        /// <returns>link hinh</returns>
        public static string GetImage()
        {
            return currentImageLink;
        }

        /// <summary>
        /// Xoa source cua cac hinh anh va tra cac hinh anh ve dung thu tu
        /// </summary>
        public static void ClearBoard()
        {
            UI.ResetBoard();
            currentImageLink = null;
            isEmpty = true;
            for (int i = 0; i < images.GetLength(0); i++)
                for (int j = 0; j < images.GetLength(1); j++)
                    images[i, j].Source = null;
        }
        /// <summary>
        /// Doi cho 2 Image
        /// </summary>
        /// <param name="point1">Vi tri cua image thu 1 trong ma tran image</param>
        /// <param name="point2">Vi tri cua image thu 2 trong ma tran image</param>
        public static void SwapPosition(Tuple<int, int> point1, Tuple<int, int> point2)
        {
            try
            {
                if (isEmpty) return;
                if (!disableAnim)
                {
                    if (point1.Item1 == point2.Item1)
                    {
                        if (point1.Item2 < point2.Item2)
                            Animation(images[point2.Item1, point2.Item2], point2, 0, -(width + margin));
                        else
                        {
                            if (point1.Item2 > point2.Item2)
                                Animation(images[point2.Item1, point2.Item2], point2, 0, +(width + margin));
                        }
                    }
                    else
                    {
                        if (point1.Item2 == point2.Item2)
                        {
                            if (point1.Item1 < point2.Item1)
                                Animation(images[point2.Item1, point2.Item2], point2, 1, -(height + margin));
                            else
                            {
                                if (point1.Item1 > point2.Item1)
                                    Animation(images[point2.Item1, point2.Item2], point2, 1, +(height + margin));
                            }
                        }
                    }
                }

                double X1 = startX + point1.Item2 * (width + margin);
                double Y1 = startY + point1.Item1 * (height + margin);
                double X2 = startX + point2.Item2 * (width + margin);
                double Y2 = startY + point2.Item1 * (height + margin);

                Canvas.SetLeft(images[point1.Item1, point1.Item2], X2);
                Canvas.SetTop(images[point1.Item1, point1.Item2], Y2);
                Canvas.SetLeft(images[point2.Item1, point2.Item2], X1);
                Canvas.SetTop(images[point2.Item1, point2.Item2], Y1);
                Image temp = images[point1.Item1, point1.Item2];

                images[point1.Item1, point1.Item2] = images[point2.Item1, point2.Item2];
                images[point2.Item1, point2.Item2] = temp;
                HighlightBlankSpot();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private static void Animation(Image image, Tuple<int, int> postion, int type, int distance)
        {
            var animation = new DoubleAnimation();

            if (type == 0)
            {

                animation.From = (postion.Item2) * (width + margin) + startX;
                animation.To = (postion.Item2) * (width + margin) + startX + distance;

            }
            else
            {
                animation.From = (postion.Item1) * (height + margin) + startY;
                animation.To = (postion.Item1 ) * (height + margin) + startY + distance;
            }

            animation.Duration = new Duration(TimeSpan.FromSeconds(0.25));

            var story = new Storyboard();
            story.Children.Add(animation);
            story.FillBehavior = FillBehavior.Stop;
            Storyboard.SetTarget(animation, image);
   
            if(type == 0)
                Storyboard.SetTargetProperty(animation, new PropertyPath(Canvas.LeftProperty));
            else
                Storyboard.SetTargetProperty(animation, new PropertyPath(Canvas.TopProperty));

            story.Begin(window);
        }
        /// <summary>
        /// Chuyen cac image trong ma tran ve dung thu tu
        /// </summary>
        public static void ResetBoard()
        {
            for (int i = 0; i < images.GetLength(0); i++)
                for (int j = 0; j < images.GetLength(1); j++)
                {
                    Tuple<int, int> tag = images[i, j].Tag as Tuple<int, int>;
                    UI.SwapPosition(tag, new Tuple<int, int>(i, j));
                    if (i == tag.Item1 && j == tag.Item2) continue;
                    else
                    {
                        j--;
                    }
                }
        }

        /// <summary>
        /// Load game o phia giao dien
        /// </summary>
        /// <param name="link">link hinh anh goc</param>
        /// <param name="matrix">ma tran du lieu</param>
        public static void LoadGame(string link,int[,] matrix)
        {
            try
            {
                UI.ApplyImage(link);
            }
            catch (Exception e) {
                throw (e); }
            UI.isEmpty = false;
            UI.currentImageLink = link;
            for (int i = 0; i < images.GetLength(0); i++)
                for (int j = 0; j < images.GetLength(1); j++)
                {
                    Tuple<int, int> tag = new Tuple<int, int>(matrix[i, j] / 3, matrix[i, j] % 3);
                    bool found = false;
                    for(int m=0;m< images.GetLength(0); m++)
                    {
                        for (int n = 0; n < images.GetLength(1); n++)
                        {
                            if ((images[m, n].Tag as Tuple<int, int>).Item1 == tag.Item1 &&
                                (images[m, n].Tag as Tuple<int, int>).Item2 == tag.Item2)
                            {
                                tag = new Tuple<int, int>(m,n);
                                found = true;
                                break;
                            }
                        }
                        if (found) break;
                    }
                        
                    UI.SwapPosition(tag, new Tuple<int, int>(i, j));
                    
                }
        }



        //------------------Cac ham phu, tro giup cho cac ham chinh
        //--------Luu y: Do cac ham phu chi duoc su dung trong class nay 
        //--------nen de private de khong bi nham lan voi cac ham chinh khi class Database duoc goi o cac class khac



        
        /// <summary>
        /// Truyen vao link hinh anh goc, ham se cat hinh va phan phoi hinh vao giao dien
        /// </summary>
        /// <param name="link">link hinh anh goc</param>
        private static void ApplyImage(string link)
        {
            var ImgSource = new BitmapImage(
                    new Uri(link, UriKind.Absolute));
            var temp = ImgSource.PixelHeight > ImgSource.PixelWidth ? ImgSource.PixelWidth : ImgSource.PixelHeight;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    //Tạo mảnh, có 9 mảnh, trừ ô [i,j] = [2,2]
                    if (!(i == 2 && j == 2))
                    {
                        //xử lí tạm thời, chỉ cắt sao cho ra vuông 3x3 (lấy phần bên trái)
                        var h = (int)(temp / 3);//chiều cao của 1 ô
                        var w = (int)(temp / 3);//chiều rộng của 1 ô
                        var rect = new Int32Rect(j * w, i * h, w, h);//tạo khung
                        var cropBitmap = new CroppedBitmap(ImgSource, rect);
                        images[i, j].Source = cropBitmap;
                    }
                    else
                    {
                        images[i, j].Source = null;
                    }
                }
            }
        }

        private static void HighlightBlankSpot()
        {
            Tuple<int, int> blankSpot = Database.GetEmptySpot();
            for(int i=0;i<rows.GetLength(0);i++)
                for(int j=0;j<rows.GetLength(1);j++)
                {
                    rows[i, j].Stroke = new SolidColorBrush(Colors.Black);
                    cols[i, j].Stroke = new SolidColorBrush(Colors.Black);
                }
            rows[blankSpot.Item1, blankSpot.Item2].Stroke = new SolidColorBrush(Colors.Red);
            rows[blankSpot.Item1 + 1, blankSpot.Item2].Stroke = new SolidColorBrush(Colors.Red);
            cols[blankSpot.Item2, blankSpot.Item1].Stroke = new SolidColorBrush(Colors.Red);
            cols[blankSpot.Item2 + 1, blankSpot.Item1].Stroke = new SolidColorBrush(Colors.Red);
        }

        public static void setZIndex(Image image)
        {

            for (int i  = 0; i< images.GetLength(0) ; i++)
            {
                for (int j = 0; j < images.GetLength(1); j++)
                {
                    Canvas.SetZIndex(images[i, j], 0);

                }
            }

            Canvas.SetZIndex(image, 1);
        }
    }
}
