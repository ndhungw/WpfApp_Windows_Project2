using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            try
            {
                Canvas.SetLeft(_selectedBitmap, left);
                Canvas.SetTop(_selectedBitmap, top);
            }
            catch (System.ArgumentNullException e)
            {
                Debug.WriteLine($"Error: {e.Message} \nĐây là trường hợp người dùng click quá nhanh" +
                    $" nên sau khi FileDialog vừa tắt thì chuột click vào cropImage ở phần giao diện" +
                    $" trong khi hình chưa được cắt xong");
            }
        }

    }
}
