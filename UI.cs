using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WpfApp_Windows_Project2
{
    public static class UI
    {
        public static void setLeftTopImage(Image _selectedBitmap, double left, double top)
        {
            Canvas.SetLeft(_selectedBitmap, left);
            Canvas.SetTop(_selectedBitmap, top);
        }

    }
}
