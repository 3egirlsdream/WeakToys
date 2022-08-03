using WeakToys.Class;
using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NHotkey.Wpf;
using NHotkey;

namespace WeakToys
{
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainVM vm;
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                vm = new MainVM(this);
                this.DataContext = vm;
                Top = System.Windows.SystemParameters.PrimaryScreenHeight - this.Height - 50;
                Left = SystemParameters.PrimaryScreenWidth - this.Width - 20;
                Loaded += MainWindow_Loaded;
                MouseLeave += MainWindow_MouseLeave;

            }
            catch (Exception ex)
            {
                LogHelper.Instance._logger.Error(ex.Message);
            }
        }

        private void MainWindow_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }

        private void MainWindow_Deactivated(object sender, EventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
            HotkeyManager.Current.AddOrReplace("Increment", Key.C, ModifierKeys.Control, OnIncrementOrDecrement);
        }


        private void OnIncrementOrDecrement(object sender, HotkeyEventArgs e)
        {
            var clip = new ClipboardItem();
            //文本内容检测
            if (System.Windows.Clipboard.ContainsText())
            {
                clip.Type = ClipboardType.Text;
                clip.Text = System.Windows.Clipboard.GetText().Trim();
                //做进一步操作
            }
            else if (Clipboard.ContainsImage())
            {
                //clip.Type = ClipboardType.Image;
                //using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                //{
                //    var img = Clipboard.GetImage();
                //    BitmapEncoder encoder = new BmpBitmapEncoder();
                //    encoder.Frames.Add(BitmapFrame.Create(img));
                //    encoder.Save(ms);
                //    var map = new System.Drawing.Bitmap(ms);
                //    var path = System.Environment.CurrentDirectory + "\\cache\\" + img.GetHashCode() + ".png";
                //    map.Save(path);
                //    clip.Image = path;
                //}
            }
            if (vm.ClipboardsItems.All(c => c.HashCode != clip.HashCode) && clip.HashCode != 0)
            {
                vm.ClipboardsItems.Insert(0, clip);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            try
            {
            }
            catch(Exception ex)
            {
                LogHelper.Instance._logger.Error(ex.Message);
            }
            base.OnClosed(e);
        }

        private void MoveWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                WindowState = WindowState.Normal;
                DragMove();
            }
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }
        
        /// 
        /// 所有控件初始化完成后调用
        /// 
        /// 
        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
        }

    }
}
