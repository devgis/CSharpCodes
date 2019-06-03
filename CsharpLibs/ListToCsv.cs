using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DEVGIS.CsharpLibs
{
    /// <summary>
    /// 
    /// </summary>
    public class ListToCSV
    {
        public static string ExportToCSV<T>(List<T> list)
        {
            if (list == null || list.Count <= 0)
            {
                throw (new Exception("List is null or empty!"));
            }
            StringBuilder sb = new StringBuilder();
            string header = string.Empty;
            var properties = typeof(T).GetProperties();
            if (properties == null || properties.Length <= 0)
            {
                throw (new Exception("Properties is null!"));
            }
            for (int i = 0; i < properties.Length; i++)
            {
                if (i == 0)
                {
                    header += properties[i].Name;
                }
                else
                {
                    header += "," + properties[i].Name;
                }
                sb.AppendLine(header);
            }
            foreach (var item in list)
            {
                string rowdata = string.Empty;
                for (int i = 0; i < properties.Length; i++)
                {
                    try
                    {
                        var value = JsonConvert.SerializeObject(properties[i].GetValue(item, null));
                        value = value.TrimStart('[').TrimEnd(']').Replace(",", "；");
                        if (i == 0)
                        {
                            rowdata += value;
                        }
                        else
                        {
                            rowdata += "," + value;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (i == 0)
                        {
                            rowdata += string.Empty;
                        }
                        else
                        {
                            rowdata += ","+string.Empty;
                        }
                    }
                }
                sb.AppendLine(rowdata);
            }
            return sb.ToString();
        }
    }
}
