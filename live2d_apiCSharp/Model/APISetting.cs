using live2dAPICharp.Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace live2dAPICharp.Model
{
    public class APISetting : BaseSingleton<APISetting>
    {
        public APISetting()
        {
            ModelList = ModelListUtils.GetModelList();
            WebrootPath = AppContext.BaseDirectory;
        }

        public ModelList ModelList
        {
            get; set;
        }

        public JObject TexturesList
        {
            get;set;
        }

        public string WebrootPath
        {
            get; private set;
        }        
        
    }
}
