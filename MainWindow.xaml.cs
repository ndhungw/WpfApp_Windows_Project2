using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp_Windows_Project2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        const int Rows = 3; //số dòng mặc định
        const int Cols = 3; //số cột mặc định

        int[,] _a;  //Mảng dữ liệu
        const int startX = 30;  //vị trí bắt đầu vẽ theo trục X
        const int startY = 30;  //vị trí bắt đầu vẽ theo trục Y
        const int width = 100;   //chiều rộng mỗi ô
        const int height = 100;  //chiều dài mỗi ô

        public object Cavas { get; private set; }

        private void NewGameInit()
        {
            ////Tạo giao diện
            ////Vẽ đường dọc
            //for (int i = 1; i < Rows; i++)
            //{
            //    var verticalLine = new Line();
            //    verticalLine.StrokeThickness = 1;
            //    verticalLine.Stroke = new SolidColorBrush(Colors.Black);
            //    canvas.Children.Add(verticalLine);

            //    verticalLine.X1 = startX + i * width;
            //    verticalLine.X2 = startX + i * width;

            //    verticalLine.Y1 = startY;
            //    verticalLine.Y2 = startY + Rows * height;
            //}

            ////Vẽ đường ngang
            //for (int i = 1; i < Cols; i++)
            //{
            //    var horizontalLine = new Line();
            //    horizontalLine.StrokeThickness = 1;
            //    horizontalLine.Stroke = new SolidColorBrush(Colors.Black);
            //    canvas.Children.Add(horizontalLine);

            //    horizontalLine.X1 = startX;
            //    horizontalLine.X2 = startX + Cols * width;

            //    horizontalLine.Y1 = startY + i * height;
            //    horizontalLine.Y2 = startY + i * height;
            //}

            /*Tạo dữ liệu trò chơi*/
            //Tạo ma trận Rows x Cols
            _a = new int[Rows, Cols];
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    _a[i, j] = i * 3 + j;//0, 1, 2, ..., 8
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            NewGameInit();

            var screen = new OpenFileDialog();

            if(screen.ShowDialog() == true)
            {
                var ImgSource = new BitmapImage(
                    new Uri(screen.FileName, UriKind.Absolute));
                Debug.WriteLine($"W: {ImgSource.Width} - H: {ImgSource.Height}");
                previewImage.Width = 300;
                previewImage.Height = 240;
                previewImage.Source = ImgSource;
                Canvas.SetLeft(previewImage, startX + Rows * width + startX);
                Canvas.SetTop(previewImage, startY);

                //Bắt đầu cắt thành 9 mảnh
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        //Tạo mảnh, có 9 mảnh, trừ ô [i,j] = [2,2]
                        if (!(i == 2 && j == 2))
                        {
                            //xử lí tạm thời, chỉ cắt sao cho ra vuông 3x3 (lấy phần bên trái)
                            var h = (int)ImgSource.Height / 3;//chiều cao của 1 ô
                            var w = (int)ImgSource.Height / 3;//chiều rộng của 1 ô
                            var rect = new Int32Rect(j * w, i * h, w, h);//tạo khung

                            //cắt hình đưa vào khung
                            var cropBitmap = new CroppedBitmap(ImgSource, rect);
                            var cropImage = new Image();
                            cropImage.Stretch = Stretch.Fill;
                            cropImage.Width = width;
                            cropImage.Height = height;
                            cropImage.Source = cropBitmap;
                            canvas.Children.Add(cropImage);
                            Canvas.SetLeft(cropImage, startX + j * (width + 2));
                            Canvas.SetTop(cropImage, startY + i * (height + 2));

                            //Events
                            cropImage.MouseLeftButtonDown += CropImage_MouseLeftButtonDown;
                            cropImage.PreviewMouseLeftButtonUp += CropImage_PreviewMouseLeftButtonUp;
                            cropImage.Tag = new Tuple<int, int>(i, j);
                        }
                    }
                }
            }
        }

        bool _isDragging = false;
        Image _selectedBitmap = null;
        Point _lastPosition;

        private void CropImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isDragging = true;
            _selectedBitmap = sender as Image;
            _lastPosition = e.GetPosition(this);//vị trước khi được kéo đi nơi khác
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(this);
            int i = ((int)position.Y - startY) / height;
            int j = ((int)position.X - startX) / width;

            this.Title = $"{position.X} - {position.Y}, a[{i}][{j}]";


            if (_isDragging)
            {
                if (i < Rows && j < Cols)//kiểm tra điều kiện còn nằm trong vùng của thao tác kéo thả
                {
                    var dx = position.X - _lastPosition.X;
                    var dy = position.Y - _lastPosition.Y;

                    var lastLeft = Canvas.GetLeft(_selectedBitmap);
                    var lastTop = Canvas.GetTop(_selectedBitmap);
                    Canvas.SetLeft(_selectedBitmap, lastLeft + dx);
                    Canvas.SetTop(_selectedBitmap, lastTop + dy);

                    _lastPosition = position;

                }
            }
        }

        private void CropImage_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            var position = e.GetPosition(this);

            int x = (int)(position.X - startX) / (width + 2) * (width + 2) + startX;
            int y = (int)(position.Y - startY) / (height + 2) * (height + 2) + startY;

            Canvas.SetLeft(_selectedBitmap, x);
            Canvas.SetTop(_selectedBitmap, y);

            var image = sender as Image;
            var (i, j) = image.Tag as Tuple<int, int>;

            MessageBox.Show($"{i} - {j}");
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //var position = e.GetPosition(this);

            //int i = ((int)position.Y - startY) / height;
            //int j = ((int)position.X - startX) / width;

            //this.Title = $"{position.X} - {position.Y}, a[{i}][{j}]";

            //var img = new Image();
            //img.Width = 30;
            //img.Height = 30;
            //img.Source = new BitmapImage(
            //    new Uri("circle.png", UriKind.Relative));
            //canvas.Children.Add(img);

            //Canvas.SetLeft(img, startX + j * width);
            //Canvas.SetTop(img, startY + i * height);
        }

        private void previewImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var animation = new DoubleAnimation();
            animation.From = 200;
            animation.To = 300;
            animation.Duration = new Duration(TimeSpan.FromSeconds(1));
            animation.AutoReverse = true;
            animation.RepeatBehavior = RepeatBehavior.Forever;

            var story = new Storyboard();
            story.Children.Add(animation);
            Storyboard.SetTargetName(animation, previewImage.Name);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Canvas.LeftProperty));
            story.Begin(this);
        }



        private void New_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            /*Kiểm tra có dữ liệu mà chưa lưu hay không/

            /*Làm mới game*/
            canvas.Children.Clear();
            NewGameInit();

            MessageBox.Show("New game");
        }

        private void Save_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            //const string SaveFileName = "save.txt";
            //var writer = new StreamWriter(SaveFileName);

            ////Lưu lượt đi hiện tại
            //writer.WriteLine(isXTurn ? "X" : "O");

            ////Lưu ma trận dữ liệu trò chơi
            //for (int i = 0; i < Rows; i++)
            //{
            //    for (int j = 0; j < Cols; j++)
            //    {
            //        writer.Write(_a[i, j] + " ");
            //    }
            //    writer.WriteLine();
            //}
            //writer.Close();

            //MessageBox.Show("Game is saved");
        }

        private void Load_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            //const string Seperator = " ";
            //var screen = new OpenFileDialog();

            //if (screen.ShowDialog() == true)
            //{
            //    var LoadFileName = screen.FileName;
            //    StreamReader reader = new StreamReader(LoadFileName);
            //    var firstLine = reader.ReadLine();

            //    isXTurn = (firstLine == "X");

            //    for (int rowIndex = 0; rowIndex < Rows; rowIndex++)
            //    {
            //        //string[] tokens = reader.ReadLine().Split(new string[] { Seperator }, StringSplitOptions.RemoveEmptyEntries);
            //        string[] tokens = reader.ReadLine().Split(new string[] { Seperator }, StringSplitOptions.None);

            //        //Model
            //        for (int colIndex = 0; colIndex < Cols; colIndex++)
            //        {
            //            _a[rowIndex, colIndex] = int.Parse(tokens[colIndex]);
            //            /* Vẽ X hoặc Y lên giao diện tại vị trí _a[i,j] */
            //            if (_a[rowIndex, colIndex] != 0)
            //            {
            //                var imgMark = new Image();
            //                imgMark.Width = width;
            //                imgMark.Height = height;

            //                if (_a[rowIndex, colIndex] == 1)//  Vị trí của X
            //                {
            //                    imgMark.Source = new BitmapImage(new Uri(imgXSource, UriKind.Relative));
            //                }
            //                else//  Vị trí của O
            //                {
            //                    imgMark.Source = new BitmapImage(new Uri(imgOSource, UriKind.Relative));
            //                }

            //                canvas.Children.Add(imgMark);
            //                Canvas.SetLeft(imgMark, startX + colIndex * width);
            //                Canvas.SetTop(imgMark, startY + rowIndex * height);
            //            }
            //        }
            //    }
            //    reader.Close();
            //    MessageBox.Show("Game is loaded!");
            //}
        }

    }
}
