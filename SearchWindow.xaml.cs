using WeakToys.Class;
using NHotkey;
using NHotkey.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WeakToys
{
    /// <summary>
    /// SearchWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SearchWindow : Window
    {
        SearchVM vm;
        public SearchWindow()
        {
            InitializeComponent();
            mainText.Focus();
            mainText.SelectAll();
            vm = new SearchVM(this);
            DataContext = vm;
            Global.Containers = Global.GetTextInfo();
            Deactivated += SearchWindow_Deactivated;
            HotkeyManager.Current.AddOrReplace("Increment", Key.Space, ModifierKeys.Control, OnIncrementOrDecrement);
        }

        private void SearchWindow_Deactivated(object sender, EventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        { 
            var win = new ManageWindow();
            //win.Owner = this;
            win.ShowDialog();
            Global.Containers = Global.GetTextInfo();
        }

        private void OnIncrementOrDecrement(object sender, HotkeyEventArgs e)
        {
            this.Visibility = Visibility.Visible;
            this.Focus();
            e.Handled = true;
        }
    }

    public class SearchVM : NotifyIconViewModel
    {
        SearchWindow window;
        public SearchVM(SearchWindow _window)
        {
            window = _window;
            Visibility = Visibility.Collapsed;
        }

        private string text;
        public string Text
        {
            get=> text;
            set
            {
                text = value;
                NotifyPropertyChanged(nameof(Text));
                LastClick = DateTime.Now;
                Search(value);
            }
        }

        private ObservableCollection<Container> itemsSource = new ObservableCollection<Container>();
        public ObservableCollection<Container> ItemsSource
        {
            get => itemsSource;
            set
            {
                itemsSource = value;
                NotifyPropertyChanged(nameof(ItemsSource));
                Visibility = value.Any() ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public DateTime LastClick { get; set; }
        public async void Search(string txt, bool f = true)
        {
            if (string.IsNullOrEmpty(txt))
            {
                ItemsSource = new ObservableCollection<Container>();
                return;
            }
            if (f)
            {
                await Task.Delay(1000);
                if ((DateTime.Now - LastClick).TotalSeconds < 1)
                {
                    return;
                }
            }

            var result = Global.Containers.Where(c => c.Desc.Contains(txt) || c.Code.Contains(txt)).Take(5);
            ItemsSource = new ObservableCollection<Container>(result);
        }

        private Visibility visibility = Visibility.Collapsed;
        public Visibility Visibility
        {
            get => visibility;
            set
            {
                visibility = value;
                NotifyPropertyChanged(nameof(Visibility));
            }
        }



        public SimpleCommand CmdEnter => new SimpleCommand()
        {
            ExecuteDelegate=x =>
            {
                Search(Text, false);
            },
            CanExecuteDelegate = o => true
        };
    }


    public class Container : NotifyIconViewModel
    {
        public Container()
        {
            ID = Guid.NewGuid().ToString("N").ToUpper();
        }

        public Container(string code, string desc)
        {
            Code = code;
            Desc = desc;
            ID = Guid.NewGuid().ToString("N").ToUpper();
        }
        [JsonIgnore]
        public string ID { get;set; }

        private string code;
        public string Code
        {
            get => code;
            set
            {
                code = value;
                NotifyPropertyChanged(nameof(Code));
            }
        }

        private string desc;
        public string Desc
        {
            get => desc;
            set
            {
                desc = value;
                NotifyPropertyChanged(nameof(Desc));
            }
        }

        private string tag;
        public string Tag
        {
            get => tag;
            set
            {
                tag = value;
                NotifyPropertyChanged(nameof(Tag));
                if (!string.IsNullOrEmpty(value))
                {
                    Tags = new ObservableCollection<string>(value.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries));
                }
                else
                {
                    Tags = new ObservableCollection<string>();
                }
            }
        }

        private ObservableCollection<string> tags = new ObservableCollection<string>();

        [JsonIgnore]
        public ObservableCollection<string> Tags
        {
            get => tags;
            set
            {
                tags = value;
                NotifyPropertyChanged(nameof(Tags));
            }
        }

    }
}
