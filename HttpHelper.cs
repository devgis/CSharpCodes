using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Test.Common
{
    public class HttpHelper
    {
       

        public static string SendRequest(Service service, out HttpStatusCode httpStatusCode)
        {
            httpStatusCode = HttpStatusCode.NotImplemented;
            if (service == null)
                return string.Empty;

            string responseContent = string.Empty;
            try
            {
                Encoding encoding = Encoding.Default;
                if (!string.IsNullOrEmpty(service.Encoding))
                {
                    encoding = Encoding.GetEncoding(service.Encoding);
                }
                byte[] byteArray = encoding.GetBytes(service.PARAMS);
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(service.URL));
                webReq.Host = "ccc";
                webReq.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 10.0; Win64; x64; Trident/7.0; .NET4.0C; .NET4.0E; .NET CLR 2.0.50727; .NET CLR 3.0.30729; .NET CLR 3.5.30729)";
                webReq.Method = "POST";
                if (!string.IsNullOrEmpty(service.METHOD))
                {
                    service.Method = service.METHOD;
                }

                if (!string.IsNullOrEmpty(service.UserName) && !string.IsNullOrEmpty(service.UserPassword))
                {
                    webReq.Credentials = new NetworkCredential(service.UserName, service.UserPassword);
                }
                //webReq.Headers.Add("Authorization", "Negotiate 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");
                
                webReq.ContentType = "application/x-www-form-urlencoded";
                if (!string.IsNullOrEmpty(service.ContentType))
                {
                    webReq.ContentType = service.ContentType;
                }
                webReq.ContentLength = byteArray.Length;
                if (byteArray != null && byteArray.Length > 0)
                {
                    using (Stream reqStream = webReq.GetRequestStream())
                    {
                        reqStream.Write(byteArray, 0, byteArray.Length);//Whrite Param
                        //reqStream.Close();
                    }
                }

                using (HttpWebResponse response = (HttpWebResponse)webReq.GetResponse())
                {
                    httpStatusCode = response.StatusCode;
                    //deal the result
                    using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.Default))
                    {
                        responseContent = sr.ReadToEnd().ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return responseContent;
        }
        public static bool SendPing(Service service,out string status,int timeout=1000)
        {
            Ping ping = new Ping();
            PingReply reply = ping.Send(service.URL, timeout);
            status = reply.Status.ToString();
            if (reply.Status == IPStatus.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
