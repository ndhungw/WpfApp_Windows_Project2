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
        const int startX = 30;  //vị trí bắt đầu vẽ theo trục X
        const int startY = 30;  //vị trí bắt đầu vẽ theo trục Y
        const int width = 150;   //chiều rộng mỗi ô
        const int height = 150;  //chiều dài mỗi ô
        
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Business.InitComponents(ref canvas,Rows,Cols);
        }

        bool _isDragging = false;
        Image _selectedBitmap = null;
        Point _lastPosition;
        Tuple<int, int> startMove;

        private void CropImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
            var position = e.GetPosition(this);
            int i = (int)(position.Y - startY - Header.ActualHeight) / (height + 2);
            int j = ((int)position.X - startX) / (width + 2);
            _isDragging = true;
            _selectedBitmap = sender as Image;
            _lastPosition = e.GetPosition(this);//vị trước khi được kéo đi nơi khác
            startMove = new Tuple<int, int>(i, j);
            //MessageBox.Show($"{i} - {j}");
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(this);
            int i = (int)(position.Y - startY - Header.ActualHeight) / (height + 2);
            int j = ((int)position.X - startX ) / (width + 2);
            this.Title = $"{i} - {j}";

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
            }

        }

        private void CropImage_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            var position = e.GetPosition(this);

            int i = (int)(position.Y - startY - Header.ActualHeight) / (height + 2);
            int j = ((int)position.X - startX) / (width + 2);

            Tuple<int, int> newPosition = new Tuple<int, int>(i, j);
            Business.DrapAndDrop(_selectedBitmap, startMove, newPosition);
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

            /*Làm mới game*/
            Business.StartNewGame(Rows, Cols);
            
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
                UI.ClearBoard();
                var ImgSource = new BitmapImage(
                    new Uri(screen.FileName, UriKind.Absolute));

                previewImage.Source = ImgSource;

                Image[,] images = new Image[Rows, Cols];
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

                            images[i, j] = cropImage;
                        }
                        else
                        {
                            var cropImage = new Image();
                            cropImage.Width = width;
                            cropImage.Height = height;
                            cropImage.Source = null;
                            canvas.Children.Add(cropImage);
                            UI.setLeftTopImage(cropImage, startX + j * (width + 2), startY + i * (height + 2));
                            cropImage.Tag = new Tuple<int, int>(i, j);

                            images[i, j] = cropImage;
                        }
                    }
                }
                UI.SetImages(ref images);
                Business.StartNewGame(Rows,Cols);
            }
        }

        private void Leaderboard_MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Up_Btn_Click(object sender, RoutedEventArgs e)
        {
            Business.DirectionalMovement(4);
        }

        private void Left_Btn_Click(object sender, RoutedEventArgs e)
        {
            Business.DirectionalMovement(2);
        }

        private void Down_Btn_Click(object sender, RoutedEventArgs e)
        {
            Business.DirectionalMovement(3);
        }

        private void Right_Btn_Click(object sender, RoutedEventArgs e)
        {
            Business.DirectionalMovement(1);
        }
    }
}
