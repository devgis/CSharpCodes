using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Web;

namespace HFSIFrameLib.COM
{
    public class Loger
    {

        public static void WriteLog(string Message)
        {
            if (System.Configuration.ConfigurationManager.AppSettings["Log"] != null
               && "TRUE".Equals(System.Configuration.ConfigurationManager.AppSettings["Log"].ToUpper()))
            {
                string filePath = Path.Combine(Application.StartupPath, "Logs");
                if (HttpContext.Current != null && HttpContext.Current.Server != null && Directory.Exists(HttpContext.Current.Server.MapPath("/")))
                {
                    filePath = Path.Combine(HttpContext.Current.Server.MapPath("/"), "Logs");
                }
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                List<string> list = new List<string>();
                list.Add(string.Format("时间:{0}----------------------------------------------------------------------", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                list.Add(Message);

                try
                {
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }

                    File.AppendAllLines(Path.Combine(filePath, DateTime.Now.ToString("yyyy-MM-dd-HH") + ".log"), list, Encoding.Default);
                }
                catch
                { }
            }
        }

        public static void WriteLog(Exception ex)
        {
            if (System.Configuration.ConfigurationManager.AppSettings["Log"] == null
              || !"TRUE".Equals(System.Configuration.ConfigurationManager.AppSettings["Log"].ToUpper()))
            {
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("[ERRORMESSAGE]:{0}", ex.Message));
            sb.AppendLine(string.Format("[INNEREXCEPTION]:{0}", ex.InnerException));
            sb.AppendLine(string.Format("[SOURCE]:{0}", ex.Source));
            sb.AppendLine(string.Format("[STACKTRACE]:{0}", ex.StackTrace));
            WriteLog(sb.ToString());
        }
    }
}
