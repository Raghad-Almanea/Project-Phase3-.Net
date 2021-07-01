using WaterDelivery.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Device.Location;
using WaterDelivery.ViewModels;
using System.Security.Cryptography;

namespace WaterDelivery.Controllers.Api
{ 
    public class BaseController : Controller
    {
          
 
        public DateTime TimeNow()
        {
            TimeZone localZone = TimeZone.CurrentTimeZone;
            DateTime currentDate = DateTime.Now;
            DateTime currentUTC =
            localZone.ToUniversalTime(currentDate);
            return currentUTC.AddHours(3);
        }


        public enum Order_Reservation_out
        {
            AcceptFromAdmin = 1, // 1- وتوجييه الطلب لمقدم الخدمه -- تم الموافقه من لوجه التحكم
        }

        // GET: Base
        // GET: Base
        //public readonly static string BaisUrlHoste = "https://mshrobkm.ip4s.com/";
        //public readonly static string BaisUrlCategory = "https://mshrobkm.ip4s.com/Content/Img/Category/";
        //public readonly static string BaisUrlProvider = "https://mshrobkm.ip4s.com/Content/Img/Provider/";
        //public readonly static string BaisUrlProduct = "https://mshrobkm.ip4s.com/Content/Img/Product/";


        public readonly static string BaisUrlHoste = "https://localhost:44330/";
        public readonly static string BaisUrlCategory = "https://localhost:44330/Content/Img/Category/";
        public readonly static string BaisUrlProvider = "https://localhost:44330/Content/Img/Provider/";
        public readonly static string BaisUrlProduct = "https://localhost:44330/Content/Img/Product/";


        #region Validtion
        public static string GetFormNumber()
        {
            Random rnd = new Random();
            int code = 1234; //rnd.Next(1000, 9999);

            return code.ToString();
        }
        //valid for email
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    var domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
        // convert arabic number to english 
        public static string toEnglishNumber(string input)
        {
            string EnglishNumbers = "";

            for (int i = 0; i < input.Length; i++)
            {
                if (Char.IsDigit(input[i]))
                {
                    EnglishNumbers += char.GetNumericValue(input, i);
                }
                else
                {
                    EnglishNumbers += input[i].ToString();
                }
            }
            return EnglishNumbers;
        }
        // check if format is valid
        public static bool IsDateValid(string Date, string format)
        {
            DateTime dt;
            if (DateTime.TryParseExact(Date, format, null, DateTimeStyles.None, out dt))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
        #region date
        static List<DateTime> GetDatesBetween(DateTime startDate, DateTime endDate)
        {

            List<DateTime> allDates = new List<DateTime>();

            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                allDates.Add(date);
            return allDates;

        }

        #endregion

        #region othor
        public static string creatMessage(string lang, string textAr, string textEn)
        {
            if (lang == "ar")
            {
                return textAr;
            }
            else
            {
                return textEn;
            }

        }
        public static void SendPushNotification(int user_id, int order_id, int order_type, string msg, int type = 0)
        {
            try
            {
                ApplicationDbContext db = new ApplicationDbContext();
                var devide_ids = (from st in db.Device_Id where st.fk_user == user_id select st).ToList();
                var user_type = (from st in db.Provider where st.id == user_id select st.device_type).SingleOrDefault();


                foreach (var item in devide_ids)
                {
                    string applicationID = "AAAAiazOQCw:APA91bEajIjiXagMzf1BCB68M6cmUxciDJmWAzacfCk4NZhm6J0oYHTNHG8tX5HxkZJqx5fQKwaUNkzJraq_IZH8ONo5qe9TYzpPE3DFkBI0blJy2hnaLBlFPkiIONnC7ojGuwiKAToZ";
                    string senderId = "591309717548";
                    string deviceId = item.device_id;
                    WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                    tRequest.Method = "post";
                    tRequest.ContentType = "application/json";
                    if (user_type == "ios")
                    {
                        var data = new
                        {
                            to = deviceId,

                            notification = new
                            {
                                body = msg,
                                title = "Laptops",
                                sound = "Enabled",
                                priority = "high",
                                type = type,
                                order_id = order_id,
                                order_type = order_type


                            }
                        };
                        var serializer = new JavaScriptSerializer();
                        var json = serializer.Serialize(data);
                        Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                        tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));
                        tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                        tRequest.ContentLength = byteArray.Length;
                        using (Stream dataStream = tRequest.GetRequestStream())
                        {
                            dataStream.Write(byteArray, 0, byteArray.Length);
                            using (WebResponse tResponse = tRequest.GetResponse())
                            {
                                using (Stream dataStreamResponse = tResponse.GetResponseStream())
                                {
                                    using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                    {
                                        String sResponseFromServer = tReader.ReadToEnd();
                                        string str = sResponseFromServer;
                                    }
                                }
                            }
                        }
                    }

                    else
                    {
                        var data = new
                        {
                            to = deviceId,
                            notification = new
                            {
                                body = msg,
                                title = "Laptops",
                                sound = "Enabled",
                                priority = "high",
                                type = type,
                                order_id = order_id,
                                order_type = order_type


                            },
                            data = new
                            {
                                body = msg,
                                title = "رُديـنـا",
                                sound = "Enabled",
                                priority = "high",
                                type = type,
                                order_id = order_id,
                                order_type = order_type


                            }
                        };
                        var serializer = new JavaScriptSerializer();
                        var json = serializer.Serialize(data);
                        Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                        tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));
                        tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                        tRequest.ContentLength = byteArray.Length;
                        using (Stream dataStream = tRequest.GetRequestStream())
                        {
                            dataStream.Write(byteArray, 0, byteArray.Length);
                            using (WebResponse tResponse = tRequest.GetResponse())
                            {
                                using (Stream dataStreamResponse = tResponse.GetResponseStream())
                                {
                                    using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                    {
                                        String sResponseFromServer = tReader.ReadToEnd();
                                        string str = sResponseFromServer;
                                    }
                                }
                            }
                        }
                    }
                }





            }
            catch (Exception ex)
            {
                string str = ex.Message;

            }
        }
        #endregion

        #region sms

        //mobily
        static public string SendMessage(string msg, string numbers)
        {
            //int temp = '0';

            HttpWebRequest req = (HttpWebRequest)
            WebRequest.Create("http://www.mobily.ws/api/msgSend.php");
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            string postData = "mobile=966503444442" + "&password=" + "444442" + "&numbers=" + numbers + "&sender=" + "MTA.SA" + "&msg=" + msg + "&applicationType=68&lang=3";

            // string postData = "mobile=966569051984" + "&password=" + "ASD569051984" + "&numbers=" + numbers + "&sender=" + "Qatif Cars" + "&msg=" + msg + "&applicationType=68&lang=3";
            req.ContentLength = postData.Length;

            StreamWriter stOut = new
            StreamWriter(req.GetRequestStream(),
            System.Text.Encoding.ASCII);
            stOut.Write(postData);
            stOut.Close();
            // Do the request to get the response
            string strResponse;
            StreamReader stIn = new StreamReader(req.GetResponse().GetResponseStream());
            strResponse = stIn.ReadToEnd();
            stIn.Close();
            return strResponse;
        }
        public string SendMessageText(string msg, string numbers, string code)
        {
            //int temp = '0';

            HttpWebRequest req = (HttpWebRequest)
            WebRequest.Create("http://www.mobily.ws/api/msgSend.php");
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            string postData = "mobile=966503444442" + "&password=" + "444442" + "&numbers=" + numbers + "&sender=" + "MTA.SA" + "&msg=" + ConvertToUnicode(msg + " " + code) + "&applicationType=59";
            req.ContentLength = postData.Length;

            StreamWriter stOut = new
            StreamWriter(req.GetRequestStream(),
            System.Text.Encoding.ASCII);
            stOut.Write(postData);
            stOut.Close();
            // Do the request to get the response
            string strResponse;
            StreamReader stIn = new StreamReader(req.GetResponse().GetResponseStream());
            strResponse = stIn.ReadToEnd();
            stIn.Close();
            return strResponse;
        }

        private string ConvertToUnicode(string val)
        {
            string msg2 = string.Empty;

            for (int i = 0; i < val.Length; i++)
            {
                msg2 += convertToUnicode(System.Convert.ToChar(val.Substring(i, 1)));
            }

            return msg2;
        }

        private string convertToUnicode(char ch)
        {
            System.Text.UnicodeEncoding class1 = new System.Text.UnicodeEncoding();
            byte[] msg = class1.GetBytes(System.Convert.ToString(ch));

            return fourDigits(msg[1] + msg[0].ToString("X"));
        }

        private string fourDigits(string val)
        {
            string result = string.Empty;

            switch (val.Length)
            {
                case 1: result = "000" + val; break;
                case 2: result = "00" + val; break;
                case 3: result = "0" + val; break;
                case 4: result = val; break;
            }

            return result;
        }
        #endregion
        #region Distance
        public static List<dataorder> GetAllNearestFamousPlaces(double currentLatitude, double currentLongitude, string km, List<dataorder> data)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            int kmt = int.Parse(km);
            List<dataorder> Caldistance = new List<dataorder>();
            var query = (from c in data

                         select c).ToList();
            foreach (var place in query)
            {
                double distance = Distance(currentLatitude, currentLongitude, Convert.ToDouble(place.lat_from), Convert.ToDouble(place.lng_from));
                if (distance <= kmt)         //nearbyplaces which are within 25 kms  50 w 70
                {
                    dataorder dist = new dataorder();
                    dist.type = place.type;
                    dist.lat_from = place.lat_from;
                    dist.lng_from = place.lng_from;
                    dist.ID = place.ID;
                    dist.order_num = place.order_num;
                    Caldistance.Add(dist);
                }
            }

            return Caldistance;
        }

        private static double Distance(double lat1, double lon1, double lat2, double lon2)
        {
            double theta = lon1 - lon2;
            double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
            dist = Math.Acos(dist);
            dist = rad2deg(dist);
            dist = (dist * 60 * 1.1515) / 0.6213711922;
            // dist = (dist  * 1.609344);   //miles to kms
            return (dist);
        }

        static double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        static double rad2deg(double rad)
        {
            return (rad * 180.0 / Math.PI);
        }
        #endregion



        public int GetUserId()
        {

            string Values = string.Empty;
            int UserId;

            HttpCookie reqCookies = Request.Cookies["userInfo"];
            if (reqCookies != null)
            {
               Values = Decrypt(reqCookies["UserId"].ToString()); 
                
                //Values = reqCookies["UserId"].ToString();
            }


            bool success = Int32.TryParse(Values, out UserId);
            if (success)
                UserId = Convert.ToInt32(Values);
            else
                UserId = 0;

            return UserId;
        }


        public static string encrypt(string encryptString)
        {
            string EncryptionKey = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            byte[] clearBytes = Encoding.Unicode.GetBytes(encryptString);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
        });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    encryptString = Convert.ToBase64String(ms.ToArray());
                }
            }
            return encryptString;
        }

        public static string Decrypt(string cipherText)
        {
            string EncryptionKey = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
                      0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
                          });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        public List<CityViewModel> GetCitiess()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var Cities = db.City.Where(x => x.is_active == true).Select(x => new CityViewModel
                {
                    Id = x.Id,
                    city_name = x.name
                }).ToList();
                return Cities;
            }

        }
        public class DistanceModel
        {
            public int ID { get; set; }
            public string lat_from { get; set; }
            public string lng_from { get; set; }
            public int type { get; set; }

        }
        public class dataorder
        {
            public int ID { get; set; }
            public int order_num { get; set; }
            public string lat_from { get; set; }
            public string lng_from { get; set; }
            public string lat_to { get; set; }
            public string lng_to { get; set; }
            public string address_from { get; set; }

            public string address_to { get; set; }
            public int type { get; set; }
        }


        public static double GetDistance(double sLatitude, double sLongitude, double eLatitude, double eLongitude)
        {
            try
            {
                var sCoord = new GeoCoordinate(sLatitude, sLongitude);
                var eCoord = new GeoCoordinate(eLatitude, eLongitude);

                double dd = sCoord.GetDistanceTo(eCoord) / 1000;
                double distance = Math.Round(dd, 1, MidpointRounding.ToEven);
                return distance;

            }
            catch (Exception)
            {

                return 10;
            }
        }
    }
}