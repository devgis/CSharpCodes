public class Services
    {
        public DataTable Query(string where = "")
        {
            string sql = "select * from services where (DELETED<>1 or DELETED is null)";
            if (!string.IsNullOrEmpty(where))
            {
                sql += " and " + where;
            }
            return AccessHelper.Query(sql);
        }

        public bool Add(Service service)
        {
            string sql = "insert into SERVICES(ID,ServiceType, NAME, URL, ContentType, Encoding, PARAMS, REMARKS, UserName, UserPassword,DELETED)VALUES(?,?,?,?,?,?,?,?,?,?,0)";
            OleDbParameter[] parms = new OleDbParameter[] {
                new OleDbParameter("ID",OleDbType.VarChar),
                new OleDbParameter("ServiceType",OleDbType.VarChar),
                new OleDbParameter("NAME",OleDbType.VarChar),
                new OleDbParameter("URL",OleDbType.VarChar),
                new OleDbParameter("ContentType",OleDbType.VarChar),
                new OleDbParameter("Encoding",OleDbType.VarChar),
                new OleDbParameter("PARAMS",OleDbType.VarChar),
                new OleDbParameter("REMARKS",OleDbType.VarChar),
                new OleDbParameter("UserName",OleDbType.VarChar),
                new OleDbParameter("UserPassword",OleDbType.VarChar)
            };
            parms[0].Value = service.ID;
            parms[1].Value = service.ServiceType;
            parms[2].Value = service.NAME;
            parms[3].Value = service.URL;
            parms[4].Value = service.ContentType;
            parms[5].Value = service.Encoding;
            parms[6].Value = service.PARAMS;
            parms[7].Value = service.REMARKS;
            parms[8].Value = service.UserName;
            parms[9].Value = service.UserPassword;
            return AccessHelper.Excute(sql, parms);
        }

        public bool Edit(Service service)
        {
            string sql = "update SERVICES set ServiceType=?,NAME=?, URL=?, ContentType=?, Encoding=?, PARAMS=?, REMARKS=?, UserName=?, UserPassword=? where ID=?";
            OleDbParameter[] parms = new OleDbParameter[] {
                new OleDbParameter("ServiceType",OleDbType.VarChar),
                new OleDbParameter("NAME",OleDbType.VarChar),
                new OleDbParameter("URL",OleDbType.VarChar),
                new OleDbParameter("ContentType",OleDbType.VarChar),
                new OleDbParameter("Encoding",OleDbType.VarChar),
                new OleDbParameter("PARAMS",OleDbType.VarChar),
                new OleDbParameter("REMARKS",OleDbType.VarChar),
                new OleDbParameter("UserName",OleDbType.VarChar),
                new OleDbParameter("UserPassword",OleDbType.VarChar),
                new OleDbParameter("ID",OleDbType.VarChar)
            };
            
            parms[0].Value = service.ServiceType;
            parms[1].Value = service.NAME;
            parms[2].Value = service.URL;
            parms[3].Value = service.ContentType;
            parms[4].Value = service.Encoding;
            parms[5].Value = service.PARAMS;
            parms[6].Value = service.REMARKS;
            parms[7].Value = service.UserName;
            parms[8].Value = service.UserPassword;
            parms[9].Value = service.ID;
            return AccessHelper.Excute(sql, parms);
        }
       
        public bool Delete(string id)
        {
            string sql = "update SERVICES set DELETED=1 where ID=?";
            OleDbParameter[] parms = new OleDbParameter[] {
                new OleDbParameter("ID",OleDbType.VarChar)
            };

            parms[0].Value = id;
            return AccessHelper.Excute(sql, parms);
        }
    }
