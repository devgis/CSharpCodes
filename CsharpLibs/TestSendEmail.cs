using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace DEVGIS.CsharpLibs
{
    public class TestSendEmail
    {
        static void Main(string[] args)
        {

            System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
            msg.To.Add("devgis@qq.com");
            /*
             * msg.CC.Add("c@c.com");
             * msg.CC.Add("c@c.com");可以抄送给多人
             */
            msg.From = new MailAddress("邮箱", "AlphaWu", System.Text.Encoding.UTF8);
            /* 上面3个参数分别是发件人地址（可以随便写），发件人姓名，编码*/
            msg.Subject = "这是测试邮件html";//邮件标题            
            msg.SubjectEncoding = System.Text.Encoding.UTF8;//邮件标题编码
            msg.Body = "<a  href='http://www.devgis.com'>devgis</a>";//邮件内容
            msg.BodyEncoding = System.Text.Encoding.UTF8;//邮件内容编码
            msg.IsBodyHtml = true;//是否是HTML邮件
            msg.Priority = MailPriority.High;//邮件优先级
            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential("邮箱", "密码");
            //在zj.com注册的邮箱和密码
            client.Host = "smtp.163.com";
            object userState = msg;
            try
            {
                client.SendAsync(msg, userState);
                //简单一点儿可以client.Send(msg);
                Console.WriteLine("发送成功");
            }
            catch (System.Net.Mail.SmtpException ex)
            {
                Console.WriteLine("发送邮件出错:"+ ex.Message);
            }
            Console.Read();
        }
    }
}
