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
        const int width = 150;   //chiều rộng mỗi ô
        const int height = 150;  //chiều dài mỗi ô

        public object Cavas { get; private set; }

        private void NewGameInit()
        {
            /*Tạo dữ liệu trò chơi*/
            //Tạo ma trận Rows x Cols
            Database.rows = Rows;
            Database.cols = Cols;
            Database._a = new int[Rows, Cols];
            Database._images = new Image[Rows, Cols];
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            NewGameInit();
        }

        static bool _isDragging = false;
        Image _selectedBitmap = null;
        Tuple<int, int> _oldIndexSelectedBitmap = null;
        Tuple<double, double> _distance = null;
        Point _lastPosition;
        int count = 0;

        private void CropImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            count++;
            if(count == 2)
            {
                UI.setLeftTopImage(_selectedBitmap, _oldIndexSelectedBitmap.Item2 * (width + 2) + startX, _oldIndexSelectedBitmap.Item1 * (height + 2) + startY);
                _isDragging = false;
            }

            var position = e.GetPosition(this);
            int i = ((int)position.Y - startY) / ( height + 2 );
            int j = ((int)position.X - startX) / ( width + 2 );

            _isDragging = true;
            _selectedBitmap = sender as Image;
            _oldIndexSelectedBitmap = new Tuple<int, int>(i, j);

            _distance = new Tuple<double, double>(position.Y - (i * width + startY), position.X - (j * height + startX));
            _lastPosition = e.GetPosition(this);//vị trước khi được kéo đi nơi khác
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(this);
            int i = ((int)position.Y - startY) / ( height + 2 );
            int j = ((int)position.X - startX) / ( width + 2 );

            if (_isDragging)
            {
                if (i < Rows && j < Cols)//kiểm tra điều kiện còn nằm trong vùng của thao tác kéo thả
                {
                    var dx = position.X - _lastPosition.X;
                    var dy = position.Y - _lastPosition.Y;

                    var lastLeft = Canvas.GetLeft(_selectedBitmap);
                    var lastTop = Canvas.GetTop(_selectedBitmap);
                    UI.setLeftTopImage(_selectedBitmap, lastLeft + dx, lastTop + dy);

                    _lastPosition = position;
                }
                else
                {
                    //if (position.Y - (_lastPosition.Y - _distance.Item1) > height || position.X - (_lastPosition.X - _distance.Item2) > width)
                    //{
                    //    UI.setLeftTopImage(_selectedBitmap, _oldIndexSelectedBitmap.Item2 * (width + 2) + startX, _oldIndexSelectedBitmap.Item1 * (height + 2) + startY);
                    //    _isDragging = false;
                    //}
                        
                }
                Title = "Dang keo";
            }

        }

        private void CropImage_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            count = 0;
            _isDragging = false;
            var position = e.GetPosition(this);
       
            Point currentIndex = new Point();
            currentIndex.X = (int)(position.Y - startY) / (height + 2);
            currentIndex.Y = (int)(position.X - startX) / (width + 2);

            if (currentIndex.X < Rows && currentIndex.Y < Cols && Database._a[(int)currentIndex.X, (int)currentIndex.Y] == 0 && Business.checkLegallyMoving((int)currentIndex.X, (int)currentIndex.Y, _oldIndexSelectedBitmap.Item1, _oldIndexSelectedBitmap.Item2)) 
            {
                int left = (int)currentIndex.Y * (width + 2) + startX;
                int top = (int)currentIndex.X * (height + 2) + startY;

                UI.setLeftTopImage(_selectedBitmap,(double)left, (double)top);
                Business.changeIndex((int)currentIndex.X, (int)currentIndex.Y, _oldIndexSelectedBitmap.Item1, _oldIndexSelectedBitmap.Item2);
            }
            else
            {
                UI.setLeftTopImage(_selectedBitmap, _oldIndexSelectedBitmap.Item2 * (width +2) + startX, _oldIndexSelectedBitmap.Item1 * (height + 2) + startY);
            }

            var image = sender as Image;
            var (i, j) = image.Tag as Tuple<int, int>;

            //MessageBox.Show($"{i} - {j}");
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //var position = e.GetPosition(this);

            //int i = ((int)position.Y - startY) / height;
            //int j = ((int)position.X - startX) / width;

            //this.Title = $"{position.X} - {position.Y}, a[{i}][{j}]";
        }

        private void previewImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //var animation = new DoubleAnimation();
            //animation.From = 200;
            //animation.To = 300;
            //animation.Duration = new Duration(TimeSpan.FromSeconds(1));
            //animation.AutoReverse = true;
            //animation.RepeatBehavior = RepeatBehavior.Forever;

            //var story = new Storyboard();
            //story.Children.Add(animation);
            //Storyboard.SetTargetName(animation, previewImage.Name);
            //Storyboard.SetTargetProperty(animation, new PropertyPath(Canvas.LeftProperty));
            //story.Begin(this);
        }



        private void New_MenuItem_Click(object sender, RoutedEventArgs e)
        {

            /*Làm mới game*/
            canvas.Children.Clear();
            NewGameInit();

            MessageBox.Show("New game");
        }

        private void Save_MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Load_MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Browserbtn_Click(object sender, RoutedEventArgs e)
        {
            var screen = new OpenFileDialog();

            if (screen.ShowDialog() == true)
            {
                var ImgSource = new BitmapImage(
                    new Uri(screen.FileName, UriKind.Absolute));

                previewImage.Source = ImgSource;

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

                            var cropImage = Business.CropImage(ImgSource, rect, width, height);
                            canvas.Children.Add(cropImage);
                            UI.setLeftTopImage(cropImage, startX + j * (width + 2), startY + i * (height + 2));

                            //Events
                            cropImage.MouseLeftButtonDown += CropImage_MouseLeftButtonDown;
                            cropImage.PreviewMouseLeftButtonUp += CropImage_PreviewMouseLeftButtonUp;
                            cropImage.Tag = new Tuple<int, int>(i, j);

                            Database._images[i, j] = cropImage;
                            Database._a[i, j] = 1;
                        }
                    }
                }
            }
        }

        private void Leaderboard_MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void topbtn_Click(object sender, RoutedEventArgs e)
        {
            Point emptyIndex = Business.getEmptyIndex();

            if ((int)(emptyIndex.X) + 1 >= Rows)
                return;

            var left = (int)emptyIndex.Y * (width + 2) + startX;
            var top = (int)emptyIndex.X * (height + 2) + startY;

            UI.setLeftTopImage(Database._images[(int)(emptyIndex.X) + 1, (int)(emptyIndex.Y) ], left, top);
            Business.changeIndex((int)(emptyIndex.X), (int)(emptyIndex.Y), (int)(emptyIndex.X) + 1, (int)(emptyIndex.Y) );

        }

        private void leftbtn_Click(object sender, RoutedEventArgs e)
        {
            Point emptyIndex = Business.getEmptyIndex();

            if ((int)(emptyIndex.Y) + 1 >= Cols)
                return;

            var left = (int)emptyIndex.Y * (width + 2) + startX;
            var top = (int)emptyIndex.X * (height + 2) + startY;

            UI.setLeftTopImage(Database._images[(int)(emptyIndex.X), (int)(emptyIndex.Y) + 1], left, top);
            Business.changeIndex((int)(emptyIndex.X), (int)(emptyIndex.Y), (int)(emptyIndex.X), (int)(emptyIndex.Y) + 1);
        }

        private void downbtn_Click(object sender, RoutedEventArgs e)
        {
            Point emptyIndex = Business.getEmptyIndex();

            if ((int)(emptyIndex.X) - 1 < 0)
                return;

            var left = (int)emptyIndex.Y * (width + 2) + startX;
            var top = (int)emptyIndex.X * (height + 2) + startY;

            UI.setLeftTopImage(Database._images[(int)(emptyIndex.X) - 1, (int)(emptyIndex.Y)], left, top);
            Business.changeIndex((int)(emptyIndex.X), (int)(emptyIndex.Y), (int)(emptyIndex.X) - 1, (int)(emptyIndex.Y));
        }

        private void rightbtn_Click(object sender, RoutedEventArgs e)
        {
            Point emptyIndex = Business.getEmptyIndex();

            if ((int)(emptyIndex.Y) - 1 < 0)
                return;

            var left = (int)emptyIndex.Y * (width + 2) + startX;
            var top = (int)emptyIndex.X * (height + 2) + startY;

            UI.setLeftTopImage(Database._images[(int)(emptyIndex.X), (int)(emptyIndex.Y) - 1], left, top);
            Business.changeIndex((int)(emptyIndex.X), (int)(emptyIndex.Y), (int)(emptyIndex.X), (int)(emptyIndex.Y) - 1);
        }
    }
}
