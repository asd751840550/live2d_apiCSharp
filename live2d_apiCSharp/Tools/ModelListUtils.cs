using live2dAPICharp.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace live2dAPICharp.Tools
{
    public class ModelListUtils
    {   
        /// <summary>
        /// 获取模型列表
        /// </summary>
        public static ModelList GetModelList()
        {
            return FileUtils.DeserializeObjectByFile<ModelList>($"{AppContext.BaseDirectory}model_list.json");
        }

        public static List<string> GetNameByID(ModelList lstmodel, int id)
        {
            if (lstmodel != null && lstmodel.Models.Count >= id)
            {
                return lstmodel.Models[id - 1];
            }
            return null;
        }

        public static int GetIDByName(ModelList lstmodel, string Name)
        {
            int id = 0;
            foreach (var m in lstmodel.Models)
            {
                foreach (var name in m)
                {
                    if (string.Compare(name, Name, true) == 0)
                    {
                        goto END;
                    }
                }
                ++id;
            }
        END:
            return id;
        }
    }
}
