using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace live2dAPICharp.Model
{
    public class BaseSingleton<T> where T : new()
    {
        private static object _lockobj = new object();
        private static T _instance;

        public static T Instance
        {
            get
            {
                lock (_lockobj)
                {
                    if (_instance == null)
                    {
                        _instance = new T();
                    }
                }
                return _instance;
            }
        }

    }
}
