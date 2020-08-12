using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace live2dAPICharp.Model
{
    public class ModelList
    {
        public List<List<string>> Models
        { 
            get; set;
        } = new List<List<string>>();

        public List<string> Messages
        {
            get; set;
        } = new List<string>();

    }
}
