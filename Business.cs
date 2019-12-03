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
        const int startX = 71;  //vị trí bắt đầu vẽ theo trục X
        const int startY = 40;  //vị trí bắt đầu vẽ theo trục Y
        const int width = 150;   //chiều rộng mỗi ô
        const int height = 150;  //chiều dài mỗi ô
        const int margin = 4;

        private static bool isShuffling = false;

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
                if (Math.Abs(curDirection - lastDirection) == 1 &&
                    (curDirection != 2 && lastDirection != 3) && (curDirection != 3 && lastDirection != 2))
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
        public static void InitComponents(ref Canvas canvas,Window wd, int Rows,int Cols)
        {
            Database.ConstructDatabase(Rows, Cols);
            UI.Start(ref canvas, wd, Rows, Cols);
            UI.DrawLines();
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
        /// Thực hiện việc xử lý khi kéo thả hình ảnh
        /// </summary>
        /// <param name="selectedBitmap">Hinh duoc chon</param>
        /// <param name="startPoint">toa do bat dau keo</param>
        /// <param name="endPoint">toa do bat dau tha</param>
        /// <returns>true|| false nếu thành công hoặc thất bại</returns>
        public static bool DrapAndDrop(Image selectedBitmap, Tuple<int, int> startPoint, Tuple<int, int> endPoint)
        {
            Tuple<int, int> blankSpot = Database.GetEmptySpot();


            if ((int)endPoint.Item1 != (int)blankSpot.Item1 || (int)endPoint.Item2 != (int)blankSpot.Item2)
            {
                UI.setLeftTopImage(selectedBitmap, (int)startPoint.Item2 * ( width + margin) + startX, (int)startPoint.Item1 * (height + margin) + startY);
                return false;
            }

            if(startPoint.Item1 - blankSpot.Item1 == 0)
            {
                switch (startPoint.Item2 - blankSpot.Item2)
                {
                    case 1: return Business.DirectionalMovement(2);

                    case -1: return Business.DirectionalMovement(1);
                    default:
                        {
                            UI.setLeftTopImage(selectedBitmap, (int)startPoint.Item2 * (width + margin) + startX, (int)startPoint.Item1 * (height + margin) + startY);
                            return false;
                        }
                }
            }

            if (startPoint.Item2 - blankSpot.Item2 == 0)
            {
                switch (startPoint.Item1 - blankSpot.Item1)
                {
                    case 1: return Business.DirectionalMovement(4);

                    case -1: return Business.DirectionalMovement(3);
                    default:
                        {
                            UI.setLeftTopImage(selectedBitmap, (int)startPoint.Item2 * (width + margin) + startX, (int)startPoint.Item1 * (height + margin) + startY);
                            return false;
                        }
                }
            }

            UI.setLeftTopImage(selectedBitmap, (int)startPoint.Item2 * (width + margin) + startX, (int)startPoint.Item1 * (height + margin) + startY);
            return false;
 
        }

        /// <summary>
        /// Di chuyen doi tuong rong~ theo 1 trong 4 huong
        /// </summary>
        /// <param name="direction">1:right; 2:left; 3:Down; 4:Up</param>
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

        /// <summary>
        /// Load game
        /// </summary>
        /// <returns>Link hinh anh cua game load len, null neu load khong thanh cong</returns>
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
                    if (!File.Exists(link)) throw (new Exception());
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
                    if (!check) throw (new Exception());
                    UI.LoadGame(link, matrix);
                    return link;
                    //Load thoi gian
                }
                catch (Exception e)
                {
                    ClearBoard();
                    Exception error = new Exception("Load game khong thanh cong!");
                    throw (error);
                }
            }
            return null;
        }

        /// <summary>
        /// Clear hinh anh hien tai khoi giao dien
        /// </summary>
        public static void ClearBoard()
        {
            UI.ClearBoard();
            Database.RestartDatabase();
        }
        
    }
}
