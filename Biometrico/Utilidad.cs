using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
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

        public static Bitmap ByteToImage(byte[] blob)
        {
            MemoryStream mStream = new MemoryStream();
            byte[] pData = blob;
            mStream.Write(pData, 0, Convert.ToInt32(pData.Length));
            Bitmap bm = new Bitmap(mStream, false);
            mStream.Dispose();
            return bm;
        }

        //public static void SendJson(int id, byte[] huella) {
        // Post Json
        public string PostRequestJson(string endpoint, string json)
        {
            // Create string to hold JSON response
            string jsonResponse = string.Empty;

            using (var client = new WebClient())
            {
                try
                {
                    client.UseDefaultCredentials = true;
                    client.Headers.Add("Content-Type:application/json");
                    client.Headers.Add("Accept:application/json");
                    var uri = new Uri(endpoint);
                    var response = client.UploadString(uri, "POST", json);
                    jsonResponse = response;
                }
                catch (WebException ex)
                {
                    // Http Error
                    if (ex.Status == WebExceptionStatus.ProtocolError)
                    {
                        HttpWebResponse wrsp = (HttpWebResponse)ex.Response;
                        var statusCode = (int)wrsp.StatusCode;
                        var msg = wrsp.StatusDescription;
                        //throw new HttpException(statusCode, msg);
                    }
                    else
                    {
                        //throw new HttpException(500, ex.Message);
                    }
                }
            }

            return jsonResponse;
        }

        //Post
        public static string EnviarHuella(string endpoint, NameValueCollection requestParams)
        {
            // Create string to hold JSON response
            string jsonResponse = string.Empty;

            using (var client = new WebClient())
            {
                try
                {
                    client.UseDefaultCredentials = true;
                    client.Headers.Add("Content-Type:application/x-www-form-urlencoded");
                    client.Headers.Add("Accept:application/json");
                    var uri = new Uri(endpoint);
                    var response = client.UploadValues(uri, "POST", requestParams);
                    jsonResponse = Encoding.ASCII.GetString(response);
                }
                catch (WebException ex)
                {
                    // Http Error
                    if (ex.Status == WebExceptionStatus.ProtocolError)
                    {
                        HttpWebResponse wrsp = (HttpWebResponse)ex.Response;
                        var statusCode = (int)wrsp.StatusCode;
                        var msg = wrsp.StatusDescription;
                        //throw new HttpException(statusCode, msg);
                    }
                    else
                    {
                        //throw new HttpException(500, ex.Message);
                    }
                }
            }

            return jsonResponse;
        }

        public static PhysicalAddress GetMacAddress()
        {
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                // Only consider Ethernet network interfaces
                if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                    nic.OperationalStatus == OperationalStatus.Up)
                {
                    return nic.GetPhysicalAddress();
                }
            }
            return null;
        }

        public static string GetIp()
        {
            IPHostEntry host;

            string localIP = "";

            host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    localIP = ip.ToString();
                }

            }

            return localIP;
        }

    }
}
