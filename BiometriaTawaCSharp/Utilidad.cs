using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BiometriaTawaCSharp
{
    public class Utilidad<T> where T:class
    {
        public static T GetJson(T clase,string url) {
            try {
                var info = new WebClient().DownloadString(url);
                clase = JsonConvert.DeserializeObject<T>(info);
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }

            return clase;
        }
    }
}
