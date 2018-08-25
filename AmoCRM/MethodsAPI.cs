using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Xml;

namespace AmoCRM
{
    public class MethodsAPI
    {
        public enum TaskType
        {
            ring = 1,
            meeting,
            writeAletter
        }

        public enum ElementType
        {
            contact = 1,
            deal = 2,
            company = 3,
            buyer = 12
        }
        public static CookieCollection Authorize(string Subdomen, string UserLogin, string UserHash)
        {
            try
            {
                var URL = "https://" + Subdomen + "/private/api/auth.php";
                var parametrs = "USER_LOGIN=" + UserLogin + "&USER_HASH=" + UserHash;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.CookieContainer = new CookieContainer();

                byte[] EncodedPostParams = Encoding.GetEncoding(1251).GetBytes(parametrs);
                request.ContentLength = EncodedPostParams.Length;
                request.GetRequestStream().Write(EncodedPostParams, 0, EncodedPostParams.Length);
                request.GetRequestStream().Close();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                XmlDocument docXML = new XmlDocument();
                docXML.LoadXml(responseString);
                var auth = docXML.DocumentElement.ChildNodes[0].InnerText == "true";

                if (auth)
                {
                    return request.CookieContainer.GetCookies(request.RequestUri);
                }
                else
                {
                    return new CookieCollection();
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public static List<int> AddTask(CookieCollection cookie, string subdomen, List<TaskCRM> TaskList)
        {
            var URL = "https://" + subdomen + "/api/v2/tasks";
            List<int> idList = new List<int>();
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.CookieContainer = new CookieContainer();
                httpWebRequest.CookieContainer.Add(cookie);

                string dataJson = "{\"add\": [";
                foreach (TaskCRM item in TaskList)
                {
                    dataJson += item.SerializeItem();
                }
                dataJson = dataJson.Replace("}{", "},{") + "]}";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(dataJson);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    if (!String.IsNullOrEmpty(result))
                    {
                        JObject jObject = JObject.Parse(result);
                        var items = jObject["_embedded"]["items"];
                        foreach (JToken token in items)
                        {
                            idList.Add(token["id"].ToObject<int>());
                        }
                    }
                }
                return idList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static List<TaskCRM> GetTasks(CookieCollection cookie, string subdomen)
        {
            try
            {
                var URL = "https://" + subdomen + "/api/v2/tasks";
                return GetTaskList(cookie, URL);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static List<TaskCRM> GetTasks(CookieCollection cookie, string subdomen, int idTask, int element_id,
                                                                  int responsible_user_id, int element_type)
        {
            try
            {
                var URL = "https://" + subdomen + "/api/v2/tasks?id=" + idTask + "&element_id=" + element_id +
                          "&responsible_user_id=" + responsible_user_id + "&element_type=" + element_type;
                return GetTaskList(cookie, URL);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public static List<TaskCRM> GetTasks(CookieCollection cookie, string subdomen, int idTask, int limit_rows, int limit_offset,
                                                                    int element_id, int responsible_user_id, int element_type)
        {
            try
            {
                var URL = "https://" + subdomen + "/api/v2/tasks?id =" + idTask + "&limit_rows=" + limit_rows + "&limit_offset="
                        + limit_offset + "&element_id=" + element_id + "&responsible_user_id=" + responsible_user_id + "&element_type=" + element_type;
                return GetTaskList(cookie, URL);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        static List<TaskCRM> GetTaskList(CookieCollection cookie, string URL)
        {
            List<TaskCRM> taskList = new List<TaskCRM>();
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            httpWebRequest.CookieContainer = new CookieContainer();
            httpWebRequest.CookieContainer.Add(cookie);

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                if (!String.IsNullOrEmpty(result))
                {
                    JObject jObject = JObject.Parse(result);
                    var items = jObject["_embedded"]["items"];
                    foreach (JToken item in items)
                    {
                        taskList.Add(new JavaScriptSerializer().Deserialize<TaskCRM>(JsonConvert.SerializeObject(item)));
                    }
                }

            }
            return taskList;
        }

        public static DateTime ToDateTime(long timestamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            if (timestamp == 0)
            {
                return new DateTime();
            }
            return dateTime = dateTime.AddSeconds(timestamp).ToLocalTime();
        }

        public static long ToTimeStamp(DateTime date)
        {
            DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            if (date == DateTime.MinValue)
            {
                return 0;
            }
            return (long)Math.Floor((date.ToUniversalTime() - unixStart).TotalSeconds);
        }

        public static long DateTimeToTimeStamp(DateTime date, TimeSpan time)
        {
            DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            if (date == DateTime.MinValue)
            {
                return 0;
            }
            date.Add(time);
            return (long)Math.Floor((date.ToUniversalTime() - unixStart).TotalSeconds);

        }
    }
}
