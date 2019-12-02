using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
            if (Database.CheckWin()&&!isShuffling&&!UI.isEmpty)
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

        /// <summary>
        /// Luu trang thai game
        /// </summary>
        public static void SaveGame()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog1.CreatePrompt = true;
            saveFileDialog1.OverwritePrompt = true;
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.DefaultExt = "txt";
            saveFileDialog1.AddExtension = true;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == true)
            {
                List<string> matrix = Database.ExportMatrix();
                matrix.Insert(0, UI.GetImage());

                //Save thoi gian

                File.WriteAllLines(saveFileDialog1.FileName, matrix.ToArray());
                MessageBox.Show("Game saved");
            }
        }

        public static string LoadGame()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string[] data = File.ReadAllLines(openFileDialog.FileName);
                    string link = data[0];
                    if (!File.Exists(link)) throw (new Exception("File hinh anh khong hop le"));
                    int[,] matrix = new int[3, 3];
                    for (int i = 0; i < 3; i++)
                    {
                        string[] tokens = data[i + 1].Split(' ');
                        for (int j = 0; j < 3; j++)
                        {
                            matrix[i, j] = int.Parse(tokens[j]);
                        }
                    }
                    bool check = Database.ImportMatrix(matrix);
                    if (!check) throw (new Exception("Ma tran khong hop le"));
                    UI.LoadGame(link, matrix);
                    return link;
                    //Load thoi gian
                }
                catch (IOException e)
                {
                    MessageBox.Show("Invalid File!");
                    return null;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    return null;
                }
            }
            return null;
        }
    }
}
