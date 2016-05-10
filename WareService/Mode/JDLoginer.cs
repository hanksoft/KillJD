using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WareDealer.Mode
{
    public class JDLoginer
    {
        /// <summary>
        /// 当前帐号临时编号
        /// </summary>
        public string uuid { get; set; }
        public string machineNet { get; set; }
        public string machineCpu { get; set; }
        public string machineDisk { get; set; }
        /// <summary>
        /// 随机数
        /// </summary>
        public string r { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string eid { get; set; }
        /// <summary>
        /// sessionId
        /// </summary>
        public string fp { get; set; }
        /// <summary>
        /// token
        /// </summary>
        public string _t { get; set; }
        /// <summary>
        /// key键
        /// </summary>
        public string tname { get; set; }
        /// <summary>
        /// key值
        /// </summary>
        public string tvalue { get; set; }
        /// <summary>
        /// 登录名
        /// </summary>
        public string loginname { get; set; }
        /// <summary>
        /// 登录密码
        /// </summary>
        public string nloginpwd { get; set; }
        /// <summary>
        /// 登录密码
        /// </summary>
        public string loginpwd { get; set; }
        /// <summary>
        /// 记住账号信息
        /// </summary>
        public string chkRememberMe { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        public string authcode { get; set; }
        /// <summary>
        /// 网站本地缓存数据
        /// </summary>
        public string cookies { get; set; }
    }
}
