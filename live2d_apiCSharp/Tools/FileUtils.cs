using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace live2dAPICharp.Tools
{
    public class FileUtils
    {
        public static void AllFilesNameLower(string path)
        {
            if(Directory.Exists(path))
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                FileInfo[] files = dir.GetFiles();
                foreach(var f in files)
                {
                    string filename = Path.GetFileName(f.FullName);
                    File.Move(f.FullName, Path.Combine(filename.Replace('-', ' ').ToLower(), f.DirectoryName));
                }
                foreach(var d in dir.GetDirectories())
                {
                    AllFilesNameLower(d.FullName);
                }
            }
        }

        public static T DeserializeObjectByFile<T>(string filapath)
        {
            T retobj = default(T);
            try
            {
                string json = File.ReadAllText(filapath);
                retobj = JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                //todo log
            }
            return retobj;
        }
    }
}
