using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
#if DOTNETFX
using System.Windows.Forms;
#endif

namespace DEVGIS.CsharpLibs
{
    public class Loger2
    {

        public static void WriteLog(string Message)
        {
            if (System.Configuration.ConfigurationManager.AppSettings["Log"] != null
               && "TRUE".Equals(System.Configuration.ConfigurationManager.AppSettings["Log"].ToUpper()))
            {
                string filePath = string.Empty;
#if DOTNETFX
                filePath = Path.Combine(Application.StartupPath, "Logs");
#endif
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

                    File.AppendAllLines(Path.Combine(filePath, DateTime.Now.ToString("yyyy-MM-dd") + ".log"), list, Encoding.Default);
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