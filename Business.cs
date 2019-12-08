using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace WpfApp_Windows_Project2
{
    public static class Business
    {
        const int startX = 71;  //vị trí bắt đầu vẽ theo trục X
        const int startY = 40;  //vị trí bắt đầu vẽ theo trục Y
        const int width = 150;   //chiều rộng mỗi ô
        const int height = 150;  //chiều dài mỗi ô
        const int margin = 4;
        const int defaultTimePlay = 180;
        public static bool isPlaying = false;
        private static int lastDirection = -1;
        private static bool isShuffling = false;

        public static Timer timer;
        private static int timePlay;
        private static Window window;

        /// <summary>
        /// Reset tro choi
        /// </summary>
        public static void StartNewGame(int Rows, int Cols)
        {
            UpdateTempData(null);
            List<string> TempData = new List<string>();
            isPlaying = true;


            Database.ConstructDatabase(Rows, Cols);
            UI.ResetBoard();

            //Shuffle
            isShuffling = true;
            Random rnd = new Random();
            int lastDirection = -1;
            int curDirection = -1;
            for (int i = 0; i < 150; i++)
            {
                curDirection = rnd.Next(1, 100) % 4 + 1;
                if (Math.Abs(curDirection - lastDirection) == 1 &&
                    (curDirection != 2 && lastDirection != 3) && (curDirection != 3 && lastDirection != 2))
                {
                    i--;
                    continue;
                }
                if (Business.DirectionalMovement(curDirection))
                {
                    string datatosave = Database.ToString();
                    datatosave += " | ";
                    switch (curDirection)
                    {
                        case 1:
                            {
                                datatosave += "2";
                                break;
                            }
                        case 2:
                            {
                                datatosave += "1";
                                break;
                            }
                        case 3:
                            {
                                datatosave += "4";
                                break;
                            }
                        default:
                            {
                                datatosave += "3";
                                break;
                            }
                    }
                    TempData.Insert(0, datatosave);
                }
                lastDirection = curDirection;
            }
            isShuffling = false;
            UpdateTempData(TempData);
            timePlay = defaultTimePlay;
            UI.changeClock(timePlay);

            if (timer != null)
                timer.Close();
            timer = new Timer();
            timer.Interval = 1000;
            timer.Elapsed += timer_Elapsed;
            timer.Start();

        }
        private static void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (isPlaying)
            {
                timePlay--;

                window.Dispatcher.Invoke(() =>
                {
                    if(timePlay >=0)
                        UI.changeClock(timePlay);

                    checkTimeUp();

                });
            }

        }

        private static void checkTimeUp()
        {
            if (timePlay <= 0)
            {
                isPlaying = false;
                UI.losingAnnouncement();
                UI.changeClock(0        );
                timer.Close();

            }
        }

        /// <summary>
        /// Khoi tao tro choi
        /// </summary>
        /// <param name="canvas">canvas</param>
        /// <param name="Rows">so dong</param>
        /// <param name="Cols">so cot</param>
        public static void InitComponents(ref Canvas canvas,Window wd, int Rows,int Cols, ref TextBlock textBlock)
        {
            window = wd;
            Database.ConstructDatabase(Rows, Cols);
            UI.Start(ref canvas, wd, Rows, Cols,ref textBlock);
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

            //Luu du lieu vao hint(temp data)
            if (isPlaying)
            {
                string datatosave = Database.ToString();
                int existOptimalPath = TraverseTempData(datatosave, GetTempData());
                if (existOptimalPath != -1) CleanTempData(existOptimalPath - 1);
                else
                {
                    datatosave += " | ";
                    switch (lastDirection)
                    {
                        case 1:
                            {
                                datatosave += "2";
                                break;
                            }
                        case 2:
                            {
                                datatosave += "1";
                                break;
                            }
                        case 3:
                            {
                                datatosave += "4";
                                break;
                            }
                        default:
                            {
                                datatosave += "3";
                                break;
                            }
                    }
                    AppendTempData(datatosave);
                }
            }
            
            UI.SwapPosition(point1, point2);
            if (Database.CheckWin() && !isShuffling && !UI.isEmpty)
            {
                timer.Close();
                UI.changeClock(0);

                //Luu diem cua nguoi choi
                String namePlayer;
                var screen = new WinNotification();
                if(screen.ShowDialog() == true)
                {
                    string Dir = $"{AppDomain.CurrentDomain.BaseDirectory}leaderboard.txt";
                    using (StreamWriter sw = File.AppendText(Dir))
                    {
                        namePlayer = screen.NamePlayer.ToString().Replace("System.Windows.Controls.TextBox: ", ""); 
                        sw.WriteLine($"{namePlayer}|{180 - timePlay}");
                    }
                }

                Business.StartNewGame(3, 3);
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
            if (!isPlaying)
                return false;

            Tuple<int, int> blankSpot = Database.GetEmptySpot();

            UI.disableAnim = true;

            if ((int)endPoint.Item1 != (int)blankSpot.Item1 || (int)endPoint.Item2 != (int)blankSpot.Item2)
            {
                UI.setLeftTopImage(selectedBitmap, (int)startPoint.Item2 * ( width + margin) + startX, (int)startPoint.Item1 * (height + margin) + startY);
                UI.disableAnim = false;
                return false;
            }

            if(startPoint.Item1 - blankSpot.Item1 == 0)
            {
                switch (startPoint.Item2 - blankSpot.Item2)
                {
                    case 1:
                        {
                            bool check = Business.DirectionalMovement(2);
                            UI.disableAnim = false;
                            return check;
                        }

                    case -1:
                        {
                            bool check = Business.DirectionalMovement(1);
                            UI.disableAnim = false;
                            return check;
                        }
                    default:
                        {
                            UI.setLeftTopImage(selectedBitmap, (int)startPoint.Item2 * (width + margin) + startX, (int)startPoint.Item1 * (height + margin) + startY);
                            UI.disableAnim = false;
                            return false;
                        }
                }
            }

            if (startPoint.Item2 - blankSpot.Item2 == 0)
            {
                switch (startPoint.Item1 - blankSpot.Item1)
                {
                    case 1:
                        {
                            bool check = Business.DirectionalMovement(4);
                            UI.disableAnim = false;
                            return check;
                        }

                    case -1:
                        {
                            bool check = Business.DirectionalMovement(3);
                            UI.disableAnim = false;
                            return check;
                        }
                    default:
                        {
                            UI.setLeftTopImage(selectedBitmap, (int)startPoint.Item2 * (width + margin) + startX, (int)startPoint.Item1 * (height + margin) + startY);
                            UI.disableAnim = false;
                            return false;
                        }
                }
            }

            UI.setLeftTopImage(selectedBitmap, (int)startPoint.Item2 * (width + margin) + startX, (int)startPoint.Item1 * (height + margin) + startY);
            UI.disableAnim = false;
            return false;
 
        }

        /// <summary>
        /// Di chuyen doi tuong rong~ theo 1 trong 4 huong
        /// </summary>
        /// <param name="direction">1:right; 2:left; 3:Down; 4:Up</param>
        /// <returns></returns>
        public static bool DirectionalMovement(int direction)
        {
            if (!isPlaying)
                return false;

            lastDirection = direction;
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

            saveFileDialog1.Filter = "txt files (*.txt)|*.txt";
            saveFileDialog1.CreatePrompt = true;
            saveFileDialog1.OverwritePrompt = true;
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.DefaultExt = "txt";
            saveFileDialog1.AddExtension = true;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == true)
            {
                List<string> matrix = Database.ExportMatrix();
                matrix.Insert(0, UI.GetImage());
                List<string> Tempdata;
                try
                {
                    Tempdata = GetTempData();
                }catch(Exception e) { Tempdata = new List<string>(); }
                matrix.Add(Tempdata.Count.ToString());
                for (int i = 0; i < Tempdata.Count; i++) matrix.Add(Tempdata[i]);

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
            openFileDialog.Filter = "txt files (*.txt)|*.txt";
            openFileDialog.FilterIndex = 1;
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

                    ClearBoard();
                    bool check = Database.ImportMatrix(matrix);
                    if (!check) throw (new Exception());
                    UI.LoadGame(link, matrix);

                    try
                    {
                        List<string> TempData = new List<string>();
                        for(int i = 0; i < int.Parse(data[4]); i++)
                        {
                            TempData.Add(data[i + 5]);
                        }
                        UpdateTempData(TempData);
                    }
                    catch(Exception e) { throw (e); }

                    isPlaying = true;

                    /*Load thoi gian*/


                    return link;
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
            isPlaying = false;
            UpdateTempData(null);
            Database.RestartDatabase();
            UI.ClearBoard();
        }

        /// <summary>
        /// Goi y nuoc di tiep theo cho nguoi dung
        /// </summary>
        public static void GiveHint()
        {
            try
            {
                List<string> Tempdata = GetTempData();
                string key = Database.ToString();
                int pos = TraverseTempData(key, Tempdata);
                if (pos == -1)
                {
                    MessageBox.Show("Hint rong!");
                    return;
                }
                string[] tokens = Tempdata[pos].Split(new string[] { " | " }, StringSplitOptions.RemoveEmptyEntries);
                CleanTempData(pos);
                DirectionalMovement(int.Parse(tokens[1]));

                timePlay -= 30;
                checkTimeUp();
                if(isPlaying)
                    UI.changeClock(timePlay);

            }
            catch (Exception e)
            {
                MessageBox.Show("Hint bi loi! Vui long reset lai board de su dung hint");
                UpdateTempData(null);
                return;
            }
        }
        public static void DeleteTempData()
        {
            string path = Directory.GetCurrentDirectory() + "\\TempData.txt";
            File.Delete(path);
        }





        //------------------Cac ham phu, tro giup cho cac ham chinh
        //--------Luu y: Do cac ham phu chi duoc su dung trong class nay 
        //--------nen de private de khong bi nham lan voi cac ham chinh khi class Database duoc goi o cac class khac





        private static void UpdateTempData(List<string> data)
        {
            string path = Directory.GetCurrentDirectory() + "\\TempData.txt";
            if (data != null) File.WriteAllLines(path, data.ToArray());
            else File.WriteAllText(path, "");
        }
        private static void AppendTempData(string datatosave)
        {
            string path = Directory.GetCurrentDirectory() + "\\TempData.txt";
            string[] data;
            try
            {
                data = File.ReadAllLines(path);
            }
            catch (IOException e) { UpdateTempData(null); return; }
            List<string> temp = GetTempData();
            temp.Insert(0, datatosave);
            UpdateTempData(temp);

        }

        private static void CleanTempData(int pos)
        {
            List<string> data = GetTempData();
            data.RemoveRange(0, pos + 1);
            UpdateTempData(data);
        }

        private static List<string> GetTempData()
        {
            string path = Directory.GetCurrentDirectory() + "\\TempData.txt";
            string[] data;
            try
            {
                data = File.ReadAllLines(path);
            }
            catch (IOException e) { throw (e); }
            List<string> result = new List<string>();
            for (int i = 0; i < data.Length; i++) result.Add(data[i]);
            return result;
        }
        private static int TraverseTempData(string key, List<string>data)
        {
            int result = -1;
            for(int i = data.Count-1; i >=0; i--)
            {
                string[] tokens = data[i].Split(new string[] { " | " },StringSplitOptions.RemoveEmptyEntries);
                if (tokens[0].CompareTo(key) == 0) return i;
            }
            return result;
        }
        
    }
}
