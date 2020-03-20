using Newtonsoft.Json;
using SourceAFIS.Simple;
using System;
using System.Collections.Specialized;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace BiometriaTawaCSharp
{
    public class Utilidad<T> where T:class
    {
        public static T GetJson(T clase,string url) {
            try {
                var info = new WebClient().DownloadString(url);
                clase = JsonConvert.DeserializeObject<T>(info);
            }
            catch (Exception ex) {
                throw ex;
            }

            return clase;
        }

        public static Fingerprint ExtraerTemplate(string huellaBase64) {
            AfisEngine Afis = new AfisEngine();
            Fingerprint Huella = new Fingerprint();
            Huella.AsBitmap = new Bitmap(Base64StringToBitmap(huellaBase64));
            Person person = new Person();
            person.Fingerprints.Add(Huella);
            Afis.Extract(person);
            return new Fingerprint { Template = person.Fingerprints[0].Template };
        }

        public static SQLiteConnection ConnSqlite(string BdSqlite)
        {
            var db = new SQLiteConnection(
                string.Format("Data Source={0};Version=3;", BdSqlite)
            );

            db.Open();

            return db;
        }

        public static Bitmap ByteToBitmap(byte[] blob)
        {
            MemoryStream mStream = new MemoryStream();
            byte[] pData = blob;
            mStream.Write(pData, 0, Convert.ToInt32(pData.Length));
            Bitmap bm = new Bitmap(mStream, false);
            mStream.Dispose();
            return bm;
        }

        public static byte[] ReduceBytes(byte[] inputBytes, int jpegQuality = 50)
        {
            Image image;
            byte[] outputBytes;
            using (var inputStream = new MemoryStream(inputBytes))
            {
                image = Image.FromStream(inputStream);
                var jpegEncoder = ImageCodecInfo.GetImageDecoders()
                  .First(c => c.FormatID == ImageFormat.Jpeg.Guid);
                var encoderParameters = new EncoderParameters(1);
                encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, jpegQuality);
                using (var outputStream = new MemoryStream())
                {
                    image.Save(outputStream, jpegEncoder, encoderParameters);
                    outputBytes = outputStream.ToArray();
                }
            }

            return outputBytes;
        }

        public static string convertByteToString(byte[] dato)
        {
            try
            {
                return Convert.ToBase64String(dato);
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public static string ConverFisicImageToBase64(string Path) {
            using (Image image = Image.FromFile(Path))
            {
                using (MemoryStream m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    byte[] imageBytes = m.ToArray();

                    // Convert byte[] to Base64 String
                    string base64String = Convert.ToBase64String(imageBytes);
                    return base64String;
                }
            }
        }

        public static byte[] GetByteArrayFromFisicImage(string Path)
        {
            using (Image image = Image.FromFile(Path))
            {
                using (MemoryStream m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    return m.ToArray();
                }
            }
        }

        public static byte[] convertStringToByte(string b64Str)
        {
            try
            {
                return Convert.FromBase64String(b64Str);
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public static byte[] imgToByteConverter(Image inImg)
        {
            ImageConverter imgCon = new ImageConverter();
            return (byte[])imgCon.ConvertTo(inImg, typeof(byte[]));
        }

        public static string ConvertBiteToBase64(Byte[] bytes) {
            return Convert.ToBase64String(bytes, 0, bytes.Length);
        }

        public static Image byteArrayToImage(byte[] byteArrayIn)
        {
            using (MemoryStream mStream = new MemoryStream(byteArrayIn))
            {
                return Image.FromStream(mStream);
            }
        }


        public static string ImageToBase64(Image image) {

                using (MemoryStream m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    byte[] imageBytes = m.ToArray();

                    // Convert byte[] to Base64 String
                    string base64String = Convert.ToBase64String(imageBytes);
                    return base64String;
                }
        }

        public static Bitmap Base64StringToBitmap(string base64String)
        {
            Bitmap bmpReturn = null;
            //Convert Base64 string to byte[]

            string dummyData = base64String.Trim().Replace(" ", "+");
            if (dummyData.Length % 4 > 0)
                dummyData = dummyData.PadRight(dummyData.Length + 4 - dummyData.Length % 4, '=');

            byte[] byteBuffer = Convert.FromBase64String(dummyData);
            MemoryStream memoryStream = new MemoryStream(byteBuffer);

            memoryStream.Position = 0;

            bmpReturn = (Bitmap)Bitmap.FromStream(memoryStream);

            memoryStream.Close();
            memoryStream = null;
            byteBuffer = null;

            return bmpReturn;
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
                if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet || nic.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
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
