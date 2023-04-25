using NLog.LayoutRenderers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WeakToys.Class
{
    public class NotifyIconViewModel : ValidationBase
    {
        /// <summary>
        /// 如果窗口没显示，就显示窗口
        /// </summary>
        public SimpleCommand ShowWindowCommand
        {
            get
            {
                return new SimpleCommand
                {
                    CanExecuteDelegate = x => Application.Current.MainWindow != null && Application.Current.MainWindow.Visibility != Visibility.Visible,
                    ExecuteDelegate = o =>
                    {
                        Application.Current.MainWindow.Visibility = Visibility.Visible;
                    }
                };
            }
        }

        /// <summary>
        /// 隐藏窗口
        /// </summary>
        public SimpleCommand HideWindowCommand
        {
            get
            {
                return new SimpleCommand
                {
                    ExecuteDelegate = x => Application.Current.MainWindow.Visibility = Visibility.Collapsed,
                    CanExecuteDelegate = o => Application.Current.MainWindow != null
                };
            }
        }


        public SimpleCommand OpenSettingCommand
        {
            get
            {
                return new SimpleCommand
                {
                    ExecuteDelegate = x =>
                    {
                        var setting = new Setting();
                        setting.ShowDialog();
                    },
                    CanExecuteDelegate = o => Application.Current.MainWindow != null,
                };
            }
        }


        private bool isChecked = AutoStart.IsExistKey(System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName);
        public bool IsChecked
        {
            get => isChecked;
            set
            {
                isChecked = value;
                NotifyPropertyChanged(nameof(IsChecked));
                AutoStart.SetMeStart(value);
            }
        }


        /// <summary>
        /// 关闭软件
        /// </summary>
        public SimpleCommand ExitApplicationCommand
        {
            get
            {
                return new SimpleCommand { ExecuteDelegate = x => Application.Current.Shutdown() };
            }
        }

        public SimpleCommand ExcuteCmdCommand => new SimpleCommand()
        {
            ExecuteDelegate = o =>
            {
                Action<NotifyBox> rs = new Action<NotifyBox>((notifyBox)=>
                {
                    var txt = Global.GetBatText();
                    var ls = txt.Split(new string[] { ";", "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    var cmd = new Cmd();
                    var result = cmd.RunCmd(ls.Skip(2).ToList());
                    var from = ls[0];
                    var folder = new DirectoryInfo(from);
                    CopyFile(folder, ls[1]);
                    notifyBox.Dispatcher.Invoke(() =>
                    {
                        notifyBox.txt.Content = "执行完成";
                    });
                    MessageBox.Show(result);
                });
                NotifyBox notifyBox = new NotifyBox(rs);
                notifyBox.ShowDialog();
                
            },
            CanExecuteDelegate = x => true
        };

        private void CopyFile(DirectoryInfo Folders, string lastpath)
        {
            //首先复制目录下的文件
            foreach (FileInfo fileInfo in Folders.GetFiles())
            {
                if (fileInfo.Exists)
                {
                    string filename = fileInfo.FullName.Substring(fileInfo.FullName.LastIndexOf('\\'));
                    var p = lastpath + filename;
                    if(File.Exists(p)) File.Delete(p);

                    fileInfo.CopyTo(p, true);
                }
            }

            //其次复制目录下的文件夹，并且进行遍历
            foreach (DirectoryInfo Folder in Folders.GetDirectories())
            {
                string Foldername = Folder.FullName.Substring(Folder.FullName.LastIndexOf('\\'));
                //复制后文件夹目录
                string copypath = lastpath + Foldername;
                //创建文件夹
                if (!Directory.Exists(copypath))
                    Directory.CreateDirectory(copypath);
                //将目录加深，遍历子目录中的文件
                lastpath = copypath;
                //子目录递归调用，遍历子目录
                CopyFile(Folder, lastpath);
                //上一个子目录中归来，还原目录深度，循环至下一子目录
                lastpath = lastpath.Substring(0, lastpath.LastIndexOf('\\'));
            }
        }
    }
}
