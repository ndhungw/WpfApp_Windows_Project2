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
using System.Windows.Threading;

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
        const int startX = 71;  //vị trí bắt đầu vẽ theo trục X
        const int startY = 40;  //vị trí bắt đầu vẽ theo trục Y
        const int width = 150;   //chiều rộng mỗi ô
        const int height = 150;  //chiều dài mỗi ô
        const int margin = 4;
        BitmapImage baseimage = new BitmapImage(new Uri("Images/BaseImage.jpg", UriKind.Relative));


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Business.InitComponents(ref canvas, this, Rows, Cols, ref TimerTextBlock);
            Image[,] images = new Image[Rows, Cols];

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    images[i, j] = new Image();
                    images[i,j].Width = width;
                    images[i, j].Height = height;
                    images[i, j].Source = null;
                    canvas.Children.Add(images[i, j]);
                    UI.setLeftTopImage(images[i, j], startX + j * (width + margin), startY + i * (height + margin));

                    //Events
                    images[i, j].MouseLeftButtonDown += CropImage_MouseLeftButtonDown;
                    images[i, j].PreviewMouseLeftButtonUp += CropImage_PreviewMouseLeftButtonUp;
                    images[i, j].Tag = new Tuple<int, int>(i, j);
                    
                }
            }
            UI.InitUIMatrix(ref images);
        }


        bool _isDragging = false;
        Image _selectedBitmap = null;
        Point _lastPosition;
        Tuple<int, int> startMove;

        private void CropImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var imageCrop = sender as Image;
            UI.setZIndex(imageCrop);

            var position = e.GetPosition(this);
            int i = (int)(position.Y - startY - Header.ActualHeight) / (height + 2);
            int j = ((int)position.X - startX) / (width + 2);
            if (Business.isPlaying == false)
                return;
            
            if (_isDragging == true )
            {
                Tuple<int, int> newPosition = new Tuple<int, int>(i, j);
                Business.DrapAndDrop(_selectedBitmap, startMove, newPosition);
                _isDragging = false;
                return;
            }

            _isDragging = true;
            _selectedBitmap = sender as Image;
            _lastPosition = e.GetPosition(this);//vị trước khi được kéo đi nơi khác
            startMove = new Tuple<int, int>(i, j);
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(this);
            int i = (int)(position.Y - startY - Header.ActualHeight) / (height + 2);
            int j = ((int)position.X - startX ) / (width + 2);
            //this.Title = $"{i} - {j}";

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
            if (_isDragging == false)
                return;

            _isDragging = false;
            var position = e.GetPosition(this);

            int i = (int)(position.Y - startY - Header.ActualHeight) / (height + 2);
            int j = ((int)position.X - startX) / (width + 2);

            Tuple<int, int> newPosition = new Tuple<int, int>(i, j);
            Business.DrapAndDrop(_selectedBitmap, startMove, newPosition);

            var image = sender as Image;
            image.ReleaseMouseCapture();

        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
        }


        private void New_MenuItem_Click(object sender, RoutedEventArgs e)
        {

            /*Làm mới game*/
            Business.StartNewGame(Rows, Cols);
            
        }

        private void Save_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Business.SaveGame();
        }

        private void Load_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            string link=null;
            try
            {
                link = Business.LoadGame();
            }
            catch(Exception err)
            {
                previewImage.Source = baseimage;
                MessageBox.Show(err.Message);
            }
            if (link == null) return;
            var ImgSource = new BitmapImage(
                    new Uri(link, UriKind.Absolute));
            previewImage.Source = ImgSource;
        }

        private void Browserbtn_Click(object sender, RoutedEventArgs e)
        {
            var screen = new OpenFileDialog();
            screen.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            screen.FilterIndex = 1;
            screen.DefaultExt = "png";
            if (screen.ShowDialog() == true)
            {
                int[,] matrix = new int[Rows, Cols];
                for (int i = 0; i < Rows; i++)
                {
                    for (int j = 0; j < Cols; j++)
                    {
                        matrix[i, j] = i * 3 + j;//0, 1, 2, ..., 8
                    }
                }
                try
                {
                    Business.ClearBoard();
                    UI.LoadGame(screen.FileName, matrix);
                    previewImage.Source = new BitmapImage(new Uri(screen.FileName, UriKind.Absolute));
                }
                catch(Exception err) {
                    previewImage.Source = baseimage;
                    Business.ClearBoard();
                    MessageBox.Show(err.Message);
                    return;
                }

                Business.StartNewGame(Rows,Cols);
            }
        }

        //public void timer_Tick(object sender, EventArgs e)
        //{
        //    if (Business.isPlaying == true)
        //    {
        //        TimeSpan res = DateTime.Now.Subtract(Business.TimeStart);
        //        TimerTextBlock.Text = res.ToString(@"hh\:mm\:ss");
        //    }
        //    else
        //    {
        //        Business.timer.Stop();
        //        TimerTextBlock.Text = "00:00:00";
        //        Business.TimeStart = DateTime.Now;
        //    }
        //}

        private void Leaderboard_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Leaderboard f = new Leaderboard();
            f.ShowDialog();
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

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Business.ClearBoard();
            previewImage.Source = baseimage;
        }
        
        private void HintBtn_Click(object sender, RoutedEventArgs e)
        {
            Business.GiveHint();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Business.DeleteTempData();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                Business.DirectionalMovement(2);
            }
            if (e.Key == Key.Right)
            {
                Business.DirectionalMovement(1);
            }
            if (e.Key == Key.Down)
            {
                Business.DirectionalMovement(3);
            }
            if (e.Key == Key.Up)
            {
                Business.DirectionalMovement(4);
            }

        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            var screen = new Option();
            screen.Show();
        }
    }
}
