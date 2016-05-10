using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace WareDealer.Helper
{
    /// <summary>
    /// 密码加密解密操作相关类
    /// </summary>
    public class PassWordHelper
    {
        
        private PassWordHelper()
        {

        }

        private static PassWordHelper _passHelper;

        public static PassWordHelper GetInstance()
        {
            return _passHelper ?? (_passHelper = new PassWordHelper());
        }

        #region MD5 - 32 加密

        /// <summary>
        /// MD5 - 32加密
        /// </summary>
        /// <param name="source">待加密字段</param>
        /// <returns></returns>
        public string Md5(string source)
        {
            MD5 md5 = MD5.Create();
            byte[] btStr = Encoding.UTF8.GetBytes(source);
            byte[] hashStr = md5.ComputeHash(btStr);
            StringBuilder pwd = new StringBuilder();
            foreach (byte bStr in hashStr) { pwd.Append(bStr.ToString("x2")); }
            return pwd.ToString();
        }

        /// <summary>
        /// 加盐MD5 -32 加密
        /// </summary>
        /// <param name="source">待加密字段</param>
        /// <param name="salt">盐巴字段</param>
        /// <returns></returns>
        public string Md5Salt(string source, string salt)
        {
            return "";
            //return salt.IsEmpty() ? source.Md5() : (source + "『" + salt + "』").Md5();
        }

        #endregion

        #region DES 加密解密

        /// <summary>
        /// DES 字符串型加密
        /// </summary>
        /// <param name="source">待加密字段</param>
        /// <param name="keyVal">8位密钥值</param>
        /// <param name="ivVal">8位加密辅助向量</param>
        /// <returns>类似：xQ969nexy964SXhkTuekUQ==</returns>
        public string DesStr(string source, string keyVal, string ivVal)
        {
            try
            {
                byte[] btKey = Encoding.UTF8.GetBytes(keyVal.Length > 8 ? keyVal.Substring(0, 8) : keyVal);
                byte[] btIv = Encoding.UTF8.GetBytes(ivVal.Length > 8 ? ivVal.Substring(0, 8) : ivVal);
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                using (MemoryStream ms = new MemoryStream())
                {
                    byte[] inData = Encoding.UTF8.GetBytes(source);
                    try
                    {
                        using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(btKey, btIv), CryptoStreamMode.Write))
                        {
                            cs.Write(inData, 0, inData.Length);
                            cs.FlushFinalBlock();
                        }
                        return Convert.ToBase64String(ms.ToArray());
                    }
                    catch
                    {
                        return source;
                    }
                }
            }
            catch { return "DES加密出错"; }
        }

        /// <summary>
        /// DES 字符串型解密
        /// </summary>
        /// <param name="source">待解密字段</param>
        /// <param name="keyVal">8位密钥值</param>
        /// <param name="ivVal">8位加密辅助向量</param>
        /// <returns></returns>
        public string UnDesStr(string source, string keyVal, string ivVal)
        {
            byte[] btKey = Encoding.UTF8.GetBytes(keyVal.Length > 8 ? keyVal.Substring(0, 8) : keyVal);
            byte[] btIv = Encoding.UTF8.GetBytes(ivVal.Length > 8 ? ivVal.Substring(0, 8) : ivVal);
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] inData = Convert.FromBase64String(source);
                try
                {
                    using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(btKey, btIv), CryptoStreamMode.Write))
                    {
                        cs.Write(inData, 0, inData.Length);
                        cs.FlushFinalBlock();
                    }
                    return Encoding.UTF8.GetString(ms.ToArray());
                }
                catch
                {
                    return source;
                }
            }  
        }

        /// <summary>
        /// DES MAC地址型加密
        /// </summary>
        /// <param name="source">待加密字段</param>
        /// <param name="keyVal">8位密钥值</param>
        /// <param name="ivVal">8位加密辅助向量</param>
        /// <returns></returns>
        public string DesMac(string source, string keyVal, string ivVal)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(source);
                var des = new DESCryptoServiceProvider { Key = Encoding.ASCII.GetBytes(keyVal.Length > 8 ? keyVal.Substring(0, 8) : keyVal), IV = Encoding.ASCII.GetBytes(ivVal.Length > 8 ? ivVal.Substring(0, 8) : ivVal) };
                var desencrypt = des.CreateEncryptor();
                byte[] result = desencrypt.TransformFinalBlock(data, 0, data.Length);
                return BitConverter.ToString(result);
            }
            catch { return "转换出错！"; }
        }

        /// <summary>
        /// DES MAC地址型解密
        /// </summary>
        /// <param name="source">待解密字段</param>
        /// <param name="keyVal">8位密钥值</param>
        /// <param name="ivVal">8位加密辅助向量</param>
        /// <returns></returns>
        public string UnDesMac(string source, string keyVal, string ivVal)
        {
            try
            {
                string[] sInput = source.Split("-".ToCharArray());
                byte[] data = new byte[sInput.Length];
                for (int i = 0; i < sInput.Length; i++)
                {
                    data[i] = byte.Parse(sInput[i], NumberStyles.HexNumber);
                }
                var des = new DESCryptoServiceProvider { Key = Encoding.ASCII.GetBytes(keyVal.Length > 8 ? keyVal.Substring(0, 8) : keyVal), IV = Encoding.ASCII.GetBytes(ivVal.Length > 8 ? ivVal.Substring(0, 8) : ivVal) };
                var desencrypt = des.CreateDecryptor();
                byte[] result = desencrypt.TransformFinalBlock(data, 0, data.Length);
                return Encoding.UTF8.GetString(result);
            }
            catch { return "解密出错！"; }
        }

        #endregion

        #region RSA 加密解密

        //密钥对
        private const string PublicRsaKey = @"<RSAKeyValue><Modulus>x</Modulus><Exponent>e</Exponent></RSAKeyValue>";
        private const string PrivateRsaKey = @"<RSAKeyValue><Modulus>x</Modulus><Exponent>e</Exponent><P>p</P><Q>q</Q><DP>dp</DP><DQ>dq</DQ><InverseQ>iq</InverseQ><D>d</D></RSAKeyValue>";

        /// <summary>
        /// RSA 加密
        /// </summary>
        /// <param name="source">待加密字段</param>
        /// <returns></returns>
        public string Rsa(string source)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(PublicRsaKey);
            var cipherbytes = rsa.Encrypt(Encoding.UTF8.GetBytes(source), true);
            return Convert.ToBase64String(cipherbytes);
        }

        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="source">待解密字段</param>
        /// <returns></returns>
        public string UnRsa(string source)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(PrivateRsaKey);
            var cipherbytes = rsa.Decrypt(Convert.FromBase64String(source), true);
            return Encoding.UTF8.GetString(cipherbytes);
        }

        #endregion
    }
}
