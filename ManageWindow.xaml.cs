using Clipboards.Class;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Clipboards
{
    /// <summary>
    /// ManageWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ManageWindow : Window
    {
        ManageVM vm;
        public ManageWindow()
        {
            InitializeComponent();
            vm = new ManageVM();
            DataContext = vm;
        }

        private void TabItem_GotFocus(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (!vm.ItemsSource.Any(c => c.ID == vm.Cur.ID))
            {
                vm.ItemsSource.Add(vm.Cur);
            }
            vm.Cur = new Container("", "");
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            using (FileStream fs = new FileStream(Global.LocalPath + "\\data.json", FileMode.Create))
            {
                StreamWriter sw = new StreamWriter(fs, Encoding.Unicode);
                var options = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All, UnicodeRanges.All),
                    WriteIndented = true
                };
                var str = System.Text.Json.JsonSerializer.Serialize(vm.ItemsSource.Select(c=> new
                {
                    Desc = c.Desc,
                    Code = c.Code
                }).ToList(), options);
                //开始写入
                sw.Write(str);
                //清空缓冲区
                sw.Flush();
                //关闭流
                sw.Close();
            }
        }

        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            if(sender is Button button && button.Tag is string tag)
            {
                var item = vm.ItemsSource.FirstOrDefault(c => c.ID == tag);
                vm.ItemsSource.Remove(item);
            }
        }
    }


    public class ManageVM : NotifyIconViewModel
    {
        public ManageVM()
        {
            ItemsSource = Global.GetTextInfo();
            Cur = new Container("", "");
        }

        private ObservableCollection<Container> itemsSource = new ObservableCollection<Container>();
        public ObservableCollection<Container> ItemsSource
        {
            get => itemsSource;
            set
            {
                itemsSource = value;
                NotifyPropertyChanged(nameof(ItemsSource));
            }
        }


        private Container cur;
        public Container Cur
        {
            get=> cur;
            set
            {
                cur = value;
                NotifyPropertyChanged(nameof(Cur));
            }
        }

        private Container selectedItem;
        public Container SelectedItem
        {
            get=> selectedItem; 
            set
            {
                selectedItem = value;
                NotifyPropertyChanged(nameof(SelectedItem));
                Cur = value;
            }
        }

    }
}
