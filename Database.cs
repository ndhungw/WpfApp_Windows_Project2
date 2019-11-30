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
        
        public Database(int Cols, int Rows)
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

    }
}
