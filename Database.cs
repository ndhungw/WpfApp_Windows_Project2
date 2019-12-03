using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp_Windows_Project2
{
    public class Database
    {
        static private int[,] _a; 
        
        public static void ConstructDatabase(int Cols, int Rows)
        {
            _a = new int[Rows, Cols];

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    _a[i, j] = i * 3 + j;//0, 1, 2, ..., 8
                }
            }
        }

        /// <summary>
        /// Reset lai ma tran database
        /// </summary>
        public static void RestartDatabase()
        {
            for (int i = 0; i < _a.GetLength(0); i++)
            {
                for (int j = 0; j < _a.GetLength(1); j++)
                {
                    _a[i, j] = i * 3 + j;//0, 1, 2, ..., 8
                }
            }
        }
        /// <summary>
        /// Hoan doi vi tri 2 phan tu trong mang
        /// </summary>
        /// <param name="x">Doi tuong swap thu 1(dong,cot)</param>
        /// <param name="y">Doi tuong swap thu 2(dong,cot)</param>
        /// <returns>true neu swap thanh cong, false neu that bai</returns>
        public static bool SwapPosition(Tuple<int,int> x, Tuple<int,int> y)
        {
            try
            {
                if (!IsValidMove(x, y)) return false;
                int temp = _a[x.Item1, x.Item2];
                _a[x.Item1, x.Item2] = _a[y.Item1, y.Item2];
                _a[y.Item1, y.Item2] = temp;
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Kiem tra dieu kien thang
        /// </summary>
        /// <returns>true neu thang, false neu chua thang</returns>
        public static bool CheckWin()
        {
            for(int i=0;i<_a.GetLength(0);i++)
                for(int j = 0; j < _a.GetLength(1); j++)
                {
                    if(_a[i, j] != i * 3 + j)
                    {
                        return false;
                    }
                }
            return true;
        }

        /// <summary>
        /// Thay doi matrix database
        /// </summary>
        /// <param name="matrix">Mang 2 chieu mang gia tri moi cua database</param>
        /// <returns>true neu thanh cong, false neu that bai</returns>
        public static bool ImportMatrix(int[,] matrix)
        {
            //Kiem tra tinh xac thuc cua ma tran truyen vao
            try
            {
                if (matrix.GetLength(0) != _a.GetLength(0) || matrix.GetLength(1) != _a.GetLength(1)) return false;
                List<int> template = GetTemplate();
                for (int i = 0; i < _a.GetLength(0); i++)
                    for (int j = 0; j < _a.GetLength(1); j++)
                    {
                        if (!template.Contains(matrix[i, j])) return false;
                        template.Remove(matrix[i, j]);
                    }
            }
            catch (Exception e) { return false; }
            //Truyen ma tran vao database
            for (int i = 0; i < _a.GetLength(0); i++)
                for (int j = 0; j < _a.GetLength(1); j++)
                    _a[i, j] = matrix[i, j];
            return true;
        }
        /// <summary>
        /// Xuat ma tran database duoi dang moi chuoi cac string de luu vao file txt
        /// </summary>
        /// <returns>Chuoi string voi moi phan tu la mot dong cua ma tran database</returns>
        public static List<string> ExportMatrix()
        {
            List<string> result = new List<string>();
            for (int i = 0; i < _a.GetLength(0); i++)
            {
                string temp = "";
                for (int j = 0; j < _a.GetLength(1); j++)
                {
                    temp += _a[i, j].ToString();
                    if (j != _a.GetLength(1) - 1) temp += " ";
                }
                result.Add(temp);
            }
            return result;
        }

        /// <summary>
        /// Lay diem trong' trong ma tran
        /// </summary>
        /// <returns>Toa do diem trong, null neu khong tim duoc(ma tran sai)</returns>
        public static Tuple<int, int> GetEmptySpot()
        {
            for (int i = 0; i < _a.GetLength(0); i++)
                for (int j = 0; j < _a.GetLength(1); j++)
                    if (_a[i, j] == 8)
                    {
                        return new Tuple<int, int>(i, j);
                    }

            return null;
        }

        public static string ToString()
        {
            string result = "";
            for (int i = 0; i < _a.GetLength(0); i++)
            {
                for (int j = 0; j < _a.GetLength(1); j++)
                {
                    result += _a[i, j].ToString();
                    if (i != _a.GetLength(0) - 1 || j != _a.GetLength(1) - 1) result += " ";
                }
            }
            return result;
        }



        //------------------Cac ham phu, tro giup cho cac ham chinh
        //--------Luu y: Do cac ham phu chi duoc su dung trong class nay 
        //--------nen de private de khong bi nham lan voi cac ham chinh khi class Database duoc goi o cac class khac




        /// <summary>
        /// Clone ma tran database thanh 1 List<int>
        /// </summary>
        /// <returns>ma tran database duoi dang List<int></returns>
        private static List<int> GetTemplate()
        {
            try
            {
                List<int> result = new List<int>();
                for (int i = 0; i < _a.GetLength(0); i++)
                    for (int j = 0; j < _a.GetLength(1); j++)
                    {
                        result.Add(_a[i, j]);
                    }
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Kiem tra viec swap vi tri 2 doi tuong trong ma tran co hop le khong
        /// </summary>
        /// <param name="x">doi tuong thu 1</param>
        /// <param name="y">doi tuong thu 2</param>
        /// <returns>true neu hop le, false neu khong hop le</returns>
        private static bool IsValidMove(Tuple<int,int>x,Tuple<int,int> y)
        {
            int xDistance = Math.Abs(x.Item1 - y.Item1);
            int yDistance = Math.Abs(x.Item2 - y.Item2);
            bool result = true;
            result = (IsValidPosition(x.Item1, x.Item2) && IsValidPosition(y.Item1, y.Item2));
            if (result) result = ((xDistance == 1 && yDistance == 0) || (xDistance == 0 && yDistance == 1));
            return result;
        }

        /// <summary>
        /// Kiem tra xem toa do(x,y) co phai la toa do cua 1 doi tuong trong ma tran khong
        /// </summary>
        /// <param name="x">Dong</param>
        /// <param name="y">Cot</param>
        /// <returns>true neu toa do hop le, false neu khong</returns>
        private static bool IsValidPosition(int x, int y)
        {
            return (x >= 0 && x < _a.GetLength(0) && y >= 0 && y < _a.GetLength(1));
        }
    }
}
