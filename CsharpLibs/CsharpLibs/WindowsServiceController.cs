using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DEVGIS.CsharpLibs
{
    public class WindowsServiceController
    {
        // POST: api/WindowsService
        public WinServiceResult Post(WinServiceParam value)
        {
            WinServiceResult rs = new WinServiceResult();
            if (value == null)
            {
                rs.OK = false;
                rs.ErrorInfo = "Need post json data from http body(Content-type: application/json) such as {\"host\":\"hostnameorip\",\"servicename\":\"svcname\",\"username\":\"user\",\"password\":\"000\"}";
                return rs;
            }
            try
            {
                ConnectionOptions op = new ConnectionOptions();
                if(!string.IsNullOrEmpty(value.username))
                {
                    op.Username = value.username;
                    op.Password =value.password;
                }

                ManagementScope scope = new ManagementScope(string.Format(@"\\{0}\root\cimv2", value.host), op); //144.77.110.58
                scope.Connect();
                ManagementPath path = new ManagementPath("Win32_Service");
                ManagementClass services;
                services = new ManagementClass(scope, path, null);

                string s = string.Empty;
                foreach (ManagementObject service in services.GetInstances())
                {
                    if (service.GetPropertyValue("Name").ToString().Equals(value.servicename))
                    {
                        rs.OK = true;
                        rs.ServiceStat = service.GetPropertyValue("State").ToString();
                        return rs;
                    }
                }
            }
            catch (Exception ex)
            {
                rs.OK = false;
                rs.ErrorInfo = ex.Message;
            }
            return rs;
        }
    }
    
        public class WinServiceParam
    {
        public string host
        {
            get;
            set;
        }

        public string servicename
        {
            get;
            set;
        }

        public string username
        {
            get;
            set;
        }

        public string password
        {
            get;
            set;
        }
    }
    
    public class WinServiceResult
    {
        public bool OK
        {
            get;
            set;
        }

        public string ServiceStat
        {
            get;
            set;
        }

        public string ErrorInfo
        {
            get;
            set;
        }
    }
}
