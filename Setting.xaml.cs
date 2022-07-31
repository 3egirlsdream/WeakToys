using Clipboards.Class;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
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
    /// Setting.xaml 的交互逻辑
    /// </summary>
    public partial class Setting : Window
    {
        SettingVM vm;
        public Setting()
        {
            InitializeComponent();
            vm = new SettingVM(this);
            DataContext = vm;
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
            this.DialogResult = true;
        }

        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string tag)
            {
                vm.Download(tag);
            }
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void UploadCloud(object sender, RoutedEventArgs e)
        {
            var content = Global.GetText();
            //var content = System.Text.Json.JsonSerializer.Serialize(collection);
            //var data = new
            //{
            //    CONTENT = content,
            //    USER = vm.Text
            //};

            var dic = new Dictionary<string, object>();
            dic["content"] = content;
            dic["user"] = vm.Text;
            vm.Upload(dic);
        }

        private void Sync(object sender, RoutedEventArgs e)
        {
            vm.InitCloud();
        }

        private void refreshKey(object sender, RoutedEventArgs e)
        {
            vm.Text = Global.Key();
        }
    }


    public class SettingVM : NotifyIconViewModel
    {
        Setting setting;
        public SettingVM(Setting _setting)
        {
            setting = _setting;
            Text = Global.Key();
            InitCloud();
        }


        private ObservableCollection<CloudSync> itemsSource = new ObservableCollection<CloudSync>();
        public ObservableCollection<CloudSync> ItemsSource
        {
            get => itemsSource;
            set
            {
                itemsSource = value;
                NotifyPropertyChanged(nameof(ItemsSource));
            }
        }

        private string text;
        public string Text
        {
            get => text;
            set
            {
                text = value;
                NotifyPropertyChanged(nameof(Text));
            }
        }

        public void InitCloud()
        {
            var url = Global.Http + "/api/CloudSync/GetAll?user=" + Text;
            var rst = Connections.HttpGet<List<CloudSync>>(url);
            if (rst.success)
            {
                ItemsSource = new ObservableCollection<CloudSync>(rst.data);
                foreach(var item in ItemsSource)
                {
                    item.Date = item.DATETIME.ToString("yyyy-MM-dd HH:mm:ss");
                    if (item.DATETIME == ItemsSource.Max(c => c.DATETIME))
                    {
                        item.Date += " (最新)";
                    }
                }
            }
        }

        public async void Download(string Id)
        {
            var item = ItemsSource.FirstOrDefault(c => c.ID == Id);
            item.Loading = true;
            try
            {

                var url = Global.Http + "/api/CloudSync/Get?Id=" + Id;
                var rst = await Connections.HttpGetAsync<CloudSync>(url, Encoding.Default);
                if (rst.success)
                {
                    using (FileStream fs = new FileStream(Global.LocalPath + "\\data.json", FileMode.Create))
                    {
                        StreamWriter sw = new StreamWriter(fs, Encoding.Unicode);
                        //开始写入
                        sw.Write(rst.data.CONTENT);
                        //清空缓冲区
                        sw.Flush();
                        //关闭流
                        sw.Close();
                        Global.Containers = JsonSerializer.Deserialize<ObservableCollection<Container>>(rst.data.CONTENT);
                    }
                }
                else
                {
                    throw new Exception(rst.message.content);
                }
            }
            catch(Exception ex)
            {
                Enqueue(ex.Message);
            }
            finally
            {
                Enqueue("下载完成");
                item.Loading = false;
            }
        }

        public void Upload(object post)
        {
            try
            {
                var url = Global.Http + "/api/CloudSync/Add";
                var result = Connections.Post<string>(url, post);
                if (result.success)
                {
                    Enqueue("上传成功");
                    InitCloud();
                }
                else
                {
                    throw new Exception(result.message.content);
                }
            }
            catch (Exception ex)
            {
                Enqueue(ex.Message);
            }
        }

        public void Enqueue(string str)
        {
            setting.snackbar.MessageQueue.Enqueue(str);
            setting.snackbar.IsActive = true;
        }

    }


    public class CloudSync : NotifyIconViewModel
    {
        public CloudSync()
        {
            ID = Guid.NewGuid().ToString("N").ToUpper();
        }
        public string ID { get; set; }
        public DateTime DATETIME { get; set; }
        public string CONTENT { get;set; }
        public string Date { get;set; }

        private bool loading = false;
        public bool Loading
        {
            get => loading;
            set
            {
                loading = value;
                NotifyPropertyChanged(nameof(Loading));
            }
        }
    }

}
