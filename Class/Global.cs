using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Security.Cryptography;

namespace Clipboards
{
    internal class Global
    {
        public static string LocalPath = AppDomain.CurrentDomain.BaseDirectory;

        public static ObservableCollection<Container> GetTextInfo()
        {
            var filePath = LocalPath + "\\data.json";

            using(var fs = new FileStream(filePath, FileMode.Open))
            {
                var sr = new StreamReader(fs, Encoding.Default);
                var content = sr.ReadToEnd();
                sr.Close();
                if (string.IsNullOrEmpty(content))
                {
                    return new ObservableCollection<Container>();
                }
                var result = System.Text.Json.JsonSerializer.Deserialize<ObservableCollection<Container>>(content);
                foreach(var item in result)
                {
                    item.ID = Guid.NewGuid().ToString("N").ToUpper();
                }
                return result;
            }
        }

        public static string GetText()
        {
            var filePath = LocalPath + "\\data.json";

            using (var fs = new FileStream(filePath, FileMode.Open))
            {
                var sr = new StreamReader(fs, Encoding.Default);
                var content = sr.ReadToEnd();
                sr.Close();
                return content;
            }
        }

        public static string Http { get; set; } = "https://localhost:44389";



        public static string UUID()
        {
            string code = null;
            SelectQuery query = new SelectQuery("select * from Win32_ComputerSystemProduct");
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
            {
                foreach (var item in searcher.Get())
                {
                    using (item) code = item["UUID"].ToString();
                }
            }
            return code;
        }
        public static string Key()
        {
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.UTF8.GetBytes(UUID().Replace("-", "") + "CATLNMSL"));
                var strResult = BitConverter.ToString(result);
                string result3 = strResult.Replace("-", "");
                return result3;
            }
        }

        public static ObservableCollection<Container> Containers = new ObservableCollection<Container>();
    }
}
