using live2dAPICharp.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace live2dAPICharp.Tools
{
    public class TexttureUtils
    {
        public static JArray GetTextureName(string modelName, int modelTexturesId)
        {
            JObject ret = GetList(modelName);
            JArray array = null;
            if (ret != null)
            {
                string json = ret["textures"][modelTexturesId - 1].ToString();
                if (!json.Contains('[') && !json.Contains(']'))
                {
                    json = json.Insert(0, "[\"");
                    json = json.Insert(json.Length, "\"]");
                }
                array = JArray.Parse(json);
            }
            return array == null ? null : array;
        }

        public static JObject GetList(string modelName)
        {
            string cachepath = $"{APISetting.Instance.WebrootPath}model/{modelName}/textures.cache";
            JArray ret;
            if (File.Exists(cachepath))
            {
                ret = FileUtils.DeserializeObjectByFile<JArray>(cachepath);
            }
            else
            {
                ret = GetTextures(modelName);
                if(ret != null)
                {
                    File.WriteAllText(cachepath, JsonConvert.SerializeObject(ret));
                }
            }
            if(ret == null)
            {
                return null;
            }
            else
            {
                JObject tmpret = new JObject();
                tmpret["textures"] = ret;
                return tmpret;
            }
        }

        public static JArray GetTextures(string modelName)
        {
            JArray ret;
            string modeldir = $"{APISetting.Instance.WebrootPath}model/{modelName}/";
            if (File.Exists($"{modeldir}textures_order.json"))
            {
                List<List<string>> srclist = new List<List<string>>();
                //List<List<string>> mergelist = new List<List<string>>();
                JObject torder = FileUtils.DeserializeObjectByFile<JObject>($"{modeldir}textures_order.json");
                foreach (var dirlist in torder)
                {
                    List<List<string>> lsttmp = new List<List<string>>();
                    List<List<string>> lstswap = new List<List<string>>();
                    foreach (var dir in dirlist.Value)
                    {
                        string childtexture = dir.Value<string>();
                        if (!string.IsNullOrEmpty(childtexture))
                        {
                            DirectoryInfo dinfo = new DirectoryInfo($"{modeldir}{childtexture}");
                            foreach (var file in dinfo.GetFiles())
                            {
                                List<string> tmp = new List<string>();
                                tmp.Add($"{childtexture}/{Path.GetFileName(file.FullName)}");
                                lstswap.Add(tmp);
                            }
                        }
                        lsttmp = TexttureUtils.MergeList_SameSubscript<string>(lsttmp, lstswap);
                    }
                    srclist = TexttureUtils.MergeList<string>(lsttmp, srclist);
                }
                ret = JArray.FromObject(srclist);
            }
            else
            {
                DirectoryInfo dinfo = new DirectoryInfo($"{modeldir}textures/");
                List<string> tmp = new List<string>();
                foreach (var file in dinfo.GetFiles())
                {
                    tmp.Add($"textures/{Path.GetFileName(file.FullName)}");
                }
                ret = JArray.FromObject(tmp);
            }
            return ret;
        }

        public static List<List<T>> MergeList_SameSubscript<T>(List<List<T>> lst1, List<List<T>> lst2)
        {
            List<List<T>> lstret = new List<List<T>>();
            for(int idx = 0; idx < Math.Max(lst1.Count, lst2.Count); ++idx)
            {
                if (idx < Math.Min(lst1.Count, lst2.Count))
                {
                    lstret.Add(lst1[idx].Concat(lst2[idx]).ToList());
                }
                else
                {
                    lstret.Add(new List<T>().Concat((lst1.Count > lst2.Count ? lst1[idx] : lst2[idx])).ToList());
                }
            }
            return lstret;
        }

        public static List<List<T>> MergeList<T>(List<List<T>> lst1, List<List<T>> lst2)
        {
            List<List<T>> lstret = null;
            if(lst1.Count == 0 || lst2.Count == 0)
            {
                lstret = lst1.Count > lst2.Count ? lst1 : lst2;
                goto END;
            }
            else
            {
                lstret = new List<List<T>>();
            }
            for (int idx = 0; idx < lst1.Count; ++idx)
            {
                for(int w = 0; w < lst2.Count; ++w)
                {
                    lstret.Add(lst1[idx].Concat(lst2[w]).ToList());
                }
            }
        END:
            return lstret;
        }
    }
}
