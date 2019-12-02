using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
        private static Canvas canvas;
        private static Image[,] images;
        private static string currentImageLink;
        /// <summary>
        /// Khoi dong UI
        /// </summary>
        /// <param name="cv">Canvas</param>
        /// <param name="Rows">So dong</param>
        /// <param name="Cols">So cot</param>
        public static void Start(ref Canvas cv,int Rows,int Cols)
        {
            canvas = cv;
            images = new Image[Rows, Cols];
        }

        /// <summary>
        /// Ve gameboard
        /// </summary>
        public static void DrawLines()
        {
            for (int i = 0; i <= images.GetLength(0); i++)
            {
                Line row = new Line();
                Line col = new Line();
                col.Stroke = new SolidColorBrush(Colors.Black);
                row.Stroke = new SolidColorBrush(Colors.Black);
                row.StrokeThickness = 4;
                col.StrokeThickness = 4;
                canvas.Children.Add(col);
                canvas.Children.Add(row);
                row.X1 = (startX-2);
                row.Y1 = (startY - 2) + i * (height + margin);
                row.X2 = (startX - 2) + 3 * (width + margin);
                row.Y2 = (startY - 2) + i * (height + margin);

                col.X1 = (startX - 2) + i * (width + margin);
                col.Y1 = (startY - 2);
                col.X2 = (startX - 2) + i * (width + margin);
                col.Y2 = (startY - 2) + 3 * (height + margin);
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
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        /// <summary>
        /// Chuyen cac image trong ma tran ve dung thu tu
        /// </summary>
        public static void ResetBoard()
        {
            for (int i = 0; i < images.GetLength(0); i++)
                for (int j = 0; j < images.GetLength(1); j++)
                {
                    Tuple<int,int> tag=images[i,j].Tag as Tuple<int,int>;
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
            UI.ClearBoard();
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
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    //Tạo mảnh, có 9 mảnh, trừ ô [i,j] = [2,2]
                    if (!(i == 2 && j == 2))
                    {
                        var temp = ImgSource.Height > ImgSource.Width ? ImgSource.Width : ImgSource.Height;
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
    }
}
