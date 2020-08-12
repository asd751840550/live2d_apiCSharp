using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using live2dAPICharp.Model;
using live2dAPICharp.Tools;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace live2dAPICharp.Controllers
{
    [EnableCors("any")]
    [Route("api/[controller]")]
    [ApiController]
    //[Produces("application/json")]
    public class HomeController : ControllerBase
    {
        [HttpGet("{*path}")]
        public IActionResult GetPathData(string path)
        {
            byte[] content = new byte[2048];
            List<byte> lstcontent = new List<byte>();
            try
            {
                int offset = 0;
                
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    while (fs.Length - offset > 0)
                    {
                        offset += fs.Read(content, 0, Math.Min(2048, (int)fs.Length - offset));
                        lstcontent.AddRange(content);
                    }
                }
            }
            catch (Exception ex)
            {
                content = null;
            }
            return new FileContentResult(lstcontent.ToArray(), "*/*");
        }
        //[HttpGet("live2d.js")]
        //public string Get()
        //{
        //    return System.IO.File.ReadAllText($"{APISetting.Instance.WebrootPath}lib/js/live2d.js");
        //}

        [HttpGet("GetModel")]
        public IActionResult Get(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                return new JsonResult("error") {StatusCode = 302 };
            }
            string[] ids = id.Split('-');
            int modelid = int.Parse(ids[0]);
            int modelTexttureid = 0;
            JObject jsonobj;
            List<string> modelName = ModelListUtils.GetNameByID(APISetting.Instance.ModelList, modelid);
            string name = string.Empty;
            try
            {
                if (int.TryParse(ids[1], out int tmpid))
                {
                    modelTexttureid = tmpid;
                }
                if (modelName.Count > 1)
                {
                    name = modelTexttureid > 0 ? modelName[modelTexttureid - 1] : modelName[0];
                    jsonobj = FileUtils.DeserializeObjectByFile<JObject>($"{APISetting.Instance.WebrootPath}model/{name}/index.json");
                }
                else
                {
                    name = modelName[0];
                    jsonobj = FileUtils.DeserializeObjectByFile<JObject>($"{APISetting.Instance.WebrootPath}model/{name}/index.json");
                    if (modelTexttureid > 0)
                    {

                        JArray modelTexttureName = TexttureUtils.GetTextureName(name, modelTexttureid);
                        if (modelTexttureName != null)
                        {
                            jsonobj["textures"] = modelTexttureName;
                        }
                    }
                }
                //textures 路径
                for (int idx = 0; idx < jsonobj["textures"].Count(); ++idx)
                {
                    jsonobj["textures"][idx] = $"{APISetting.Instance.WebrootPath}model/{name}/{jsonobj["textures"][idx]}";
                }

                jsonobj["model"] = $"{APISetting.Instance.WebrootPath}model/{name}/{jsonobj["model"]}";
                if (jsonobj["pose"] != null)
                {
                    jsonobj["pose"] = $"{APISetting.Instance.WebrootPath}model/{name}/{jsonobj["pose"]}";
                }
                if (jsonobj["physics"] != null)
                {
                    jsonobj["physics"] = $"{APISetting.Instance.WebrootPath}model/{name}/{jsonobj["physics"]}";
                }
                if (jsonobj["motions"] != null)
                {
                    foreach (var idx in jsonobj["motions"])
                    {
                        foreach (var v1 in idx)
                        {
                            string v1path = v1.Path.Split('.')[v1.Path.Split('.').Length - 1];
                            for (int v2 = 0; v2 < v1.Count(); ++v2)
                            {
                                foreach (var v3 in jsonobj["motions"][v1path][v2])
                                {
                                    string v3path = v3.Path.Split('.')[v3.Path.Split('.').Length - 1];
                                    if (string.Compare("file", v3path) == 0 || string.Compare("sound", v3path) == 0)
                                        jsonobj["motions"][v1path][v2][v3path] = $"{APISetting.Instance.WebrootPath}model/{name}/{jsonobj["motions"][v1path][v2][v3path]}";
                                }

                            }
                        }
                    }
                    //for(int idx = 0; idx < jsonobj["motions"].Count(); ++idx)
                    //{                        
                    //    for (int w = 0; w < jsonobj["motions"][idx]?.Count(); ++idx)
                    //    {
                    //        jsonobj["motions"][idx][w] = $"{APISetting.Instance.WebrootPath}model/{name}/${jsonobj["motions"][idx][w]}";
                    //    }
                    //}
                }
                if (jsonobj["expressions"] != null)
                {
                    for (int idx = 0; idx < jsonobj["expressions"].Count(); ++idx)
                    {
                        jsonobj["expressions"][idx] = $"{APISetting.Instance.WebrootPath}model/{name}/{jsonobj["expressions"][idx]}";
                    }
                }
            }
            catch(Exception ex)
            {
                return new JsonResult("error") { StatusCode = 500 };
            }
            //Response.ContentType = "application/json;charset=utf-8";
            //Response.Headers.Add("access-control-allow-orgin", "*");
            //tmpret = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(tmpret));
            return new JsonResult(jsonobj, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii, Formatting = Formatting.None });
            //return new JsonResult(jsonobj);
            // new JsonResult(HtmlEncoder.Create().Encode(tmpret));// new JsonResult(tmpret) { StatusCode = 200};
        }

        [HttpGet("switch")]
        public IActionResult SwitchModel(string id)
        {
            if(!int.TryParse(id, out int modelid))
            {
                return new JsonResult("error") { StatusCode = 302 };
            }
            else
            {
                JObject jobj = new JObject();
                if(modelid >= APISetting.Instance.ModelList.Models.Count)
                {
                    modelid = 1;
                }
                else
                {
                    ++modelid;
                }
                jobj["model"] = new JObject();
                jobj["model"]["id"] = modelid;
                jobj["model"]["name"] = APISetting.Instance.ModelList.Models[modelid - 1][0];
                jobj["model"]["message"] = APISetting.Instance.ModelList.Messages[modelid - 1];
                return new JsonResult(jobj, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii, Formatting = Formatting.None });
            }    
        }

        [HttpGet("rand_textures")]
        public IActionResult SwitchTexture(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                return new JsonResult("error") { StatusCode = 302 };
            }
            string[] ids = id.Split('-');
            int modelid = 0;
            int modelTexttureid = 0;
            int modelnewid = 0;
            JObject textturelist = new JObject();
            List<string> modelName = new List<string>();
            try
            {
                modelid = int.Parse(ids[0]);
                modelTexttureid = int.Parse(ids[1]);
                modelName = ModelListUtils.GetNameByID(APISetting.Instance.ModelList, modelid);
                if (modelName.Count > 1)
                {
                    textturelist["textures"] = JArray.FromObject(modelName);
                }
                else
                {
                    textturelist = TexttureUtils.GetList(modelName[0]);
                }

                if (textturelist != null)
                {
                    if (textturelist["textures"].Count() <= 1)
                    {
                        modelnewid = 1;
                    }
                    else
                    {
                        modelnewid = modelTexttureid;
                        while (modelnewid == modelTexttureid)
                        {
                            Random rd = new Random();
                            modelnewid = rd.Next(0, textturelist["textures"].Count() - 1) + 1;
                        }
                    }
                }
                else
                {
                    return new JsonResult("error") { StatusCode = 302 };
                }
            }
            catch(Exception ex)
            { }
            JObject jobj = new JObject();
            jobj["textures"] = new JObject();
            jobj["textures"]["id"] = modelnewid;
            jobj["textures"]["name"] = textturelist["textures"][modelnewid - 1];
            jobj["textures"]["model"] = modelName.Count > 1 ? modelName[modelnewid - 1] : modelName[0];
            return new JsonResult(jobj, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii, Formatting = Formatting.None });
        }
    }
}
