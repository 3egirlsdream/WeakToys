using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace WeakToys
{
    /// <summary>
    /// NotifyBox.xaml 的交互逻辑
    /// </summary>
    public partial class NotifyBox : Window
    {
        Action<NotifyBox> action;
        public NotifyBox(Action<NotifyBox> action)
        {
            InitializeComponent();
            ShowInTaskbar = false;
            Topmost = true;
            Loaded += Load_box;
            this.action = action;
        }

        private void Load_box(object sender, RoutedEventArgs e)
        {
            var page = sender as NotifyBox;

            DoubleAnimation animation = new DoubleAnimation();
            animation.From = SystemParameters.WorkArea.Right;
            animation.To = SystemParameters.WorkArea.Right - 200;
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(500));
            page.BeginAnimation(Window.LeftProperty, animation);

            Task.Run(() =>
            {
                this.action?.Invoke(this);
            }).ContinueWith((t) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.Close();
                });
            });
        }
    }
}
