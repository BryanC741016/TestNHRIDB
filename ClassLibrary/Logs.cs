using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public static class Logs
    {
        private static Encoding _encode = Encoding.GetEncoding(65001);
        public static void WriteLog(string path,string str)
        {
            string file = DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
            string newPath = Path.Combine(path, file);
            if (!File.Exists(newPath)) {
                File.Create(newPath).Close();
            }

            string now = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            using (StreamWriter outputFile = new StreamWriter(newPath, true, _encode))
            {
               outputFile.WriteLine(now+":"+str);
                outputFile.Close();
            }
        }

        public static bool GetFind(string path,string str,int num) {
            if (!System.IO.File.Exists(path)) {
                return false;
            }
            string all = ReadAll(path);
            if (Regex.Matches(all, str).Count >= num) {
                return true;
            }
            return false;
        }

        public static string ReadAll(string path) {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            StreamReader file = new StreamReader(fs, _encode, true);

            string allStr = file.ReadToEnd();

            file.Close();
            fs.Close();

            return allStr;
        }
    }
}
