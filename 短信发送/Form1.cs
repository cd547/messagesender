using System;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Web;
using System.Net;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace 短信发送
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

       

        private void Form1_Load(object sender, EventArgs e)
        {
            //get access_token
            this.textBox_access_token.Text = IsExistAccess_Token(this.textBox_corpid.Text, this.textBox_corpsecret.Text);
        }

        private static Access_token GetAccess_token(string corpid, string secret)
        {
      
            string strUrl = "https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid=" + corpid + "&corpsecret=" + secret;
            Access_token mode = new Access_token();
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(strUrl);
            req.Method = "GET";
            using (WebResponse wr = req.GetResponse())
            {
                HttpWebResponse myResponse = (HttpWebResponse)req.GetResponse();
                StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
                string content = reader.ReadToEnd();//在这里对Access_token 赋值  
                Access_token token = new Access_token();
                token = Newtonsoft.Json.JsonConvert.DeserializeObject<Access_token>(content);
                mode.access_token = token.access_token;
                mode.expires_in = token.expires_in;
            }
            return mode;
        }

        /// <summary>
        /// 获取Access_token值
        /// </summary>
        /// <returns></returns>
        public static string IsExistAccess_Token(string corpid, string secret)
        {
            string Token = string.Empty;
            DateTime YouXRQ;
            // 读取XML文件中的数据，并显示出来 ，注意文件路径  
            string filepath = Directory.GetCurrentDirectory()+ "/XMLFile1.xml";
            StreamReader str = new StreamReader(filepath, System.Text.Encoding.UTF8);
            XmlDocument xml = new XmlDocument();
            xml.Load(str);
            str.Close();
            str.Dispose();
            Token = xml.SelectSingleNode("xml").SelectSingleNode("Access_Token").InnerText;
            YouXRQ = Convert.ToDateTime(xml.SelectSingleNode("xml").SelectSingleNode("Access_YouXRQ").InnerText);
            if (DateTime.Now > YouXRQ)
            {
                DateTime _youxrq = DateTime.Now;
                Access_token mode = GetAccess_token(corpid, secret);
                xml.SelectSingleNode("xml").SelectSingleNode("Access_Token").InnerText = mode.access_token;
                _youxrq = _youxrq.AddSeconds(int.Parse(mode.expires_in));
                xml.SelectSingleNode("xml").SelectSingleNode("Access_YouXRQ").InnerText = _youxrq.ToString();
                xml.Save(filepath);
                Token = mode.access_token;
            }
            return Token;
        }

        public class GetAppInof
        {
            public string name { get; set; }
            public string square_logo_url { get; set; }
            public string description { get; set; }
            public allow_userinfos allow_userinfos { get; set; }


        }

        public class allow_userinfos
        {
            List<User> user1 = new List<User>();
            public List<User> user { get { return user1; } set { user1 = value; } }
        }

        public class User
        {
            public string userid { get; set; }
            public string name { get; set; }
            public string position { get; set; }
            public string mobile { get; set; }
            public string gender { get; set; }
        }



        //获取应用信息
        private static GetAppInof GetAgent_info(string access_token, string agentid)
        {
            string strUrl = "https://qyapi.weixin.qq.com/cgi-bin/agent/get?access_token=" + access_token + "&agentid=" + agentid;
            GetAppInof mode = new GetAppInof();
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(strUrl);
            req.Method = "GET";
            using (WebResponse wr = req.GetResponse())
            {
                HttpWebResponse myResponse = (HttpWebResponse)req.GetResponse();
                StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
                string content = reader.ReadToEnd();// 赋值  
                GetAppInof token = new GetAppInof();
                token = Newtonsoft.Json.JsonConvert.DeserializeObject<GetAppInof>(content);
                mode.name = token.name;
                mode.square_logo_url = token.square_logo_url;
                mode.description = token.description;
                mode.allow_userinfos = token.allow_userinfos;
            }
            return mode;
        }

        //获取成员姓名https://qyapi.weixin.qq.com/cgi-bin/user/get?access_token=ACCESS_TOKEN&userid=USERID
        private User GetUser(string access_token, string userid)
        {
            string strUrl = "https://qyapi.weixin.qq.com/cgi-bin/user/get?access_token=" + access_token + "&userid=" + userid;
            User mode = new User();
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(strUrl);
            req.Method = "GET";
            using (WebResponse wr = req.GetResponse())
            {
                HttpWebResponse myResponse = (HttpWebResponse)req.GetResponse();
                StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
                string content = reader.ReadToEnd();// 赋值  
                User token = new User();
                token = Newtonsoft.Json.JsonConvert.DeserializeObject<User>(content);
                mode.name = token.name;
                mode.gender = token.gender;
                mode.mobile = token.mobile;
            }
            return mode;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            this.textBox_access_token.Text= IsExistAccess_Token(this.textBox_corpid.Text, this.textBox_corpsecret.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            GetAppInof getappinfo = GetAgent_info(IsExistAccess_Token(this.textBox_corpid.Text, this.textBox_corpsecret.Text), "1000003");
            this.textBox1.Text = "name:" + getappinfo.name;
            this.textBox1.Text += "\r\n allow_userinfos:\r\n";
            int n = getappinfo.allow_userinfos.user.Count;
            for (int i = 0; i < n; i++)
            {
                string username = GetUser(IsExistAccess_Token(this.textBox_corpid.Text, this.textBox_corpsecret.Text), getappinfo.allow_userinfos.user[i].userid).name;
                this.textBox1.Text += "[user"+(i+1).ToString() +":"+getappinfo.allow_userinfos.user[i].userid+":"+ username + "]\r\n";

            }

        }
    }


}
