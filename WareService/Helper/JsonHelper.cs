using System;
using System.Collections.Generic;
using System.Text;
using AjaxPro;
using System.IO;
using System.ComponentModel;
using System.Diagnostics;

namespace WareDealer.Helper
{
    /// <summary>
    /// JSON助手
    /// </summary>
    [DisplayName("JSON助手")]
    public static  class JsonHelper
    {
        #region 读取Json文件，产生对象

        /// <summary>
        /// 读取Json文件，产生对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="filePath">Json文件路径</param>
        /// <returns>对象</returns>
        public static T LoadJson<T>(string filePath)
        {
            return (T)LoadJson(filePath, typeof(T));
        }
               
        /// <summary>
        /// 读取Json文件，产生对象
        /// </summary>
        /// <param name="filePath">Json文件路径</param>
        /// <param name="type">对象类型</param>
        /// <returns>对象</returns>
        public static object LoadJson(string filePath, Type type)
        {
            if (System.IO.File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath, Encoding.Default);
                return FromJson(json, type);
            }
            else
            {
                return null;
            }
        }

        #endregion 读取Json文件，产生对象

        #region 以Json文件格式保存对象

        /// <summary>
        /// 以Json文件格式保存对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="filePath">Json文件</param>
        /// <param name="obj">对象</param>
        public static void SaveJson<T>(string filePath, T obj)
        {
            SaveJson(filePath, obj, typeof(T));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <param name="obj"></param>
        /// <param name="isFormat"></param>
        public static void SaveJson<T>(string filePath, T obj, bool isFormat)
        {
            string json = ToJson<T>(obj, isFormat);
            File.WriteAllText(filePath, json, Encoding.UTF8);
        }

       
        /// <summary>
        /// 以Json文件格式保存对象
        /// </summary>
        /// <param name="filePath">Json文件路径</param>
        /// <param name="obj">对象</param>
        /// <param name="type">对象类型</param>
        public static void SaveJson(string filePath, object obj, Type type)
        {
            string json = ToJson(obj, type);
            File.WriteAllText(filePath, json,Encoding.UTF8);
        }

        #endregion 以Json文件格式保存对象

        /// <summary>
        /// 将 JSON 文本转换为 JavaScript 对象
        /// </summary>
        /// <param name="json">JSON 文本</param>
        /// <returns>JavaScript 对象</returns>
        public static JavaScriptObject ToJavaScriptObj(string json)
        {
            return FromJson<JavaScriptObject>(json);
        }

        /// <summary>
        /// 将对象转换为 JavaScript 对象
        /// </summary>
        /// <typeparam name="T">原始对象类型</typeparam>
        /// <param name="obj">原始对象</param>
        /// <returns>目标对象</returns>
        public static IJavaScriptObject ToJavaScriptObj<T>(T obj)
        {
            return ToJavaScriptObj(obj, typeof(T));
        }

        /// <summary>
        /// 将对象转换为 JavaScript 对象
        /// </summary>
        /// <param name="obj">原始对象</param>
        /// <param name="type">原始对象类型</param>
        /// <returns>目标对象</returns>
        public static IJavaScriptObject ToJavaScriptObj(object obj, Type type)
        {
            try
            {
                return FromJson<IJavaScriptObject>(ToJson(obj, type));
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 将对象转换为 JSON 文本
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>JSON 文本</returns>
        public static string ToJson<T>(T obj)
        {
            return ToJson(obj, typeof(T));
        }

        /// <summary>
        /// 将对象转换为 JSON 文本
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="type">类型</param>
        /// <returns>JSON 文本</returns>
        public static string ToJson(object obj, Type type)
        {
            return JavaScriptSerializer.Serialize(obj);
        }

        /// <summary>
        /// 将对象转换为 JSON 文本
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="obj">对象</param>
        /// <param name="isFormat">是否格式化</param>
        /// <returns>JSON 文本</returns>
        public static string ToJson<T>(T obj, bool isFormat)
        {
            return ToJson(obj, isFormat, typeof(T));
        }
        //xc 添加 2013-7-1 11:37:20 该方法为老方法，暂时废弃
        private static string oldFormatJson(string s)
        {
            int dip = 0;
            string s2 = "";
            string t = "";
            bool isInString = false;//sha
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (c == '"')
                {
                    if (i == 0 || s[i - 1] != '\\')
                    {
                        if (isInString == true)
                        {
                            isInString = false;
                        }
                        else
                        {
                            isInString = true;
                        }
                    }
                }
                if (isInString)
                {
                    s2 = s2 + c;
                    continue;
                }
                if (c == '{')
                {
                    s2 = s2 + "\r\n" + t + "{\r\n";

                    dip++;
                    t = "";
                    for (int j = 0; j < dip; j++)
                    {
                        t = t + "\t";
                    }

                    s2 = s2 + t;
                }
                else if (c == '}')
                {
                    dip--;
                    t = "";
                    for (int j = 0; j < dip; j++)
                    {
                        t = t + "\t";
                    }


                    s2 = s2 + "\r\n" + t + "}\r\n";



                    s2 = s2 + t;
                }
                else if (c == '[')
                {
                    s2 = s2 + "\r\n" + t + "[\r\n";

                    dip++;
                    t = "";
                    for (int j = 0; j < dip; j++)
                    {
                        t = t + "\t";
                    }

                    s2 = s2 + t;
                }
                else if (c == ']')
                {

                    dip--;
                    t = "";
                    for (int j = 0; j < dip; j++)
                    {
                        t = t + "\t";
                    }

                    s2 = s2 + "\r\n" + t + "]\r\n";

                    s2 = s2 + t;
                }
                else if (c == ',')
                {
                    s2 = s2 + ",\r\n" + t;
                }
                else
                {
                    s2 = s2 + c;
                }


            }
            return s2;
        }
        //xc 添加 2013-7-1 11:37:32
        private static string FormatJson(string json)
        {
            StringBuilder sb = new StringBuilder();
            int sj = 0;//缩进制表符个数
            bool outOfQuotes = true; //是否是json字符
            char prevChar = ' ';//上一个字符
            foreach (char item in json)
            {
                if (item == '\"' && prevChar != '\\')
                {
                    outOfQuotes = !outOfQuotes;
                }
                else if ((item == ']' || item == '}') && outOfQuotes)
                {
                    sb.Append("\r\n");
                    sj--;
                    for (int i = 0; i < sj; i++)
                    {
                        sb.Append("\t");
                    }
                }
                sb.Append(item);
                if ((item == ',' || item == '[' || item == '{') && outOfQuotes)
                {
                    sb.Append("\r\n");
                    if (item != ',')
                    {
                        sj++;
                    }
                    for (int i = 0; i < sj; i++)
                    {
                        sb.Append("\t");
                    }
                }
                prevChar = item;
            }
            return sb.ToString();
        }
        /// <summary>
        /// 将对象转换为 JSON 文本
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="isFormat">是否格式化</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static string ToJson(object obj, bool isFormat, Type type)
        {
            string s = ToJson(obj, type);
            if (isFormat)
            {
                //xc 修改 2013-7-1 9:55:57
                return FormatJson(s);
            }
            return s;
        }

        /// <summary>
        /// 将 JSON 文本转换为 指定类型的对象
        /// </summary>
        /// <typeparam name="T">指定类型</typeparam>
        /// <param name="json">JSON 文本</param>
        /// <returns>对象</returns>
        public static T FromJson<T>(string json)
        {
            return JavaScriptDeserializer.DeserializeFromJson<T>(json);
        }

        /// <summary>
        /// 将 JSON 文本转换为 指定类型的对象
        /// </summary>
        /// <param name="json">JSON 文本</param>
        /// <param name="type">指定类型</param>
        /// <returns>对象</returns>
        public static object FromJson(string json, Type type)
        {
            return JavaScriptDeserializer.DeserializeFromJson(json, type);
        }

        //public T Deserialize<T>(string json)
        //{
        //    T obj = Activator.CreateInstance<T>();
        //    using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
        //    {
        //        DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
        //        return (T)serializer.ReadObject(ms);
        //    }
        //}
        /// <summary>
        /// 将从数据库里查询出的表格Json字符转换成datatable
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static System.Data.DataTable Json2DataTable(string json)
        {
            try
            {
                AjaxPro.JavaScriptArray arrayS =
                AjaxPro.JavaScriptDeserializer.DeserializeFromJson<AjaxPro.JavaScriptArray>(json);
                System.Data.DataTable dt = new System.Data.DataTable();
                bool isInitDt = false;
                foreach (JavaScriptObject item in arrayS)
                {
                    if (!isInitDt)
                    {
                        foreach (string key in item.Keys)
                        {
                            System.Data.DataColumn fNameColumn = new System.Data.DataColumn();
                            fNameColumn.DataType = System.Type.GetType("System.String");
                            fNameColumn.ColumnName = key;
                            fNameColumn.DefaultValue = "";
                            dt.Columns.Add(fNameColumn);
                        }
                        isInitDt = true;
                    }
                    System.Data.DataRow dr = dt.NewRow();
                    foreach (string key in item.Keys)
                    {
                        //dr[key] = item[key].Value;
                        dr[key] = item[key].Value.Trim('\"');
                    }
                    dt.Rows.Add(dr);
                }
                return dt;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public static string DataTable2Json(System.Data.DataTable dt)
        {
            JavaScriptArray jsa = new JavaScriptArray();
            foreach (System.Data.DataRow item in dt.Rows)
            {
                JavaScriptObject jso = new JavaScriptObject();
                foreach (System.Data.DataColumn dc in dt.Columns)
                {
                    JavaScriptString jss = new JavaScriptString(item[dc.ColumnName].ToString().Trim('\"'));
                    jso.Add(dc.ColumnName, jss);
                }
                jsa.Add(jso);
            }
            return JavaScriptSerializer.Serialize(jsa);
        }

        #region JSON覆盖: OrJson

        /// <summary>
        /// 用sub中的内容覆盖main中的内容
        /// </summary>
        /// <param name="strMain"></param>
        /// <param name="strSub"></param>
        /// <returns></returns>
        public static string OrJson(string strMain, string strSub)
        {
            return OrJson(strMain, strSub, false);
        }

        /// <summary>
        /// 用sub中的内容覆盖main中的内容
        /// </summary>
        /// <param name="strMain"></param>
        /// <param name="strSub"></param>
        /// <param name="isFormat"></param>
        /// <returns></returns>
        public static string OrJson(string strMain, string strSub, bool isFormat)
        {
            try
            {
                IJavaScriptObject jsMain = FromJson<IJavaScriptObject>(strMain);
                IJavaScriptObject jsSub = FromJson<IJavaScriptObject>(strSub);

                if (jsSub == null || !(jsSub is JavaScriptObject || jsSub is JavaScriptArray))
                {
                    throw new Exception("基于字符串的JSON覆盖操作的Sub必须是JSON对象！");
                }


                IJavaScriptObject jsNew = OrJson(jsMain, jsSub);

                return ToJson<IJavaScriptObject>(jsNew, isFormat);
            }
            catch (Exception ex)
            {
                //return strMain;
                throw new Exception("覆盖数据错误！", ex);
            }
        }

        /// <summary>
        /// 用sub中的内容覆盖main中的内容
        /// </summary>
        /// <param name="jsMain"></param>
        /// <param name="jsSub"></param>
        /// <returns></returns>
        public static IJavaScriptObject OrJson(IJavaScriptObject jsMain, IJavaScriptObject jsSub)
        {
            if (jsSub == null)  //jsSub为NULL
            {
                //直接覆盖输出
                return null;
            }
            else if (
                jsSub is JavaScriptNumber   //jsSub为数字类型
                || jsSub is JavaScriptBoolean   //jsSub为布尔类型
                || jsSub is JavaScriptString    //jsSub为文本类型
                )
            {
                //基本数据类型直接复制输出
                return jsSub;
            }
            else if (jsSub is JavaScriptArray)  //jsSub为数组类型
            {

                JavaScriptArray jsNew = new JavaScriptArray();

                if (jsMain is JavaScriptArray)
                {
                    JavaScriptArray jaSub = jsSub as JavaScriptArray;
                    JavaScriptArray jaMain = jsMain as JavaScriptArray;

                    for (int i = 0; i < jaSub.Count; i++)
                    {
                        IJavaScriptObject q;
                        if (i < jaMain.Count && jaMain[i] != null && jaSub[i] != null && jaMain[i].GetType() == jaSub[i].GetType())
                        {
                            q = OrJson(jaMain[i], jaSub[i]);
                        }
                        else
                        {
                            q = jaSub[i];
                        }

                        jsNew.Add(q);
                    }

                    if (jaMain.Count > jaSub.Count)
                    {
                        for (int j = jaSub.Count; j < jaMain.Count; j++)
                        {
                            jsNew.Add(jaMain[j]);
                        }
                    }
                }
                else
                {
                    JavaScriptArray jaSub = jsSub as JavaScriptArray;
                    for (int i = 0; i < jaSub.Count; i++)
                    {
                        jsNew.Add(jaSub[i]);
                    }
                }
                return jsNew;
            }
            else if (jsSub is JavaScriptObject) //jsSub为对象类型
            {
                JavaScriptObject jsNew = new JavaScriptObject();

                if (jsMain is JavaScriptObject)
                {
                    JavaScriptObject joSub = jsSub as JavaScriptObject;
                    JavaScriptObject joMain = jsMain as JavaScriptObject;

                    foreach (string k in joSub.Keys)
                    {
                        if (joMain.Contains(k) && joSub[k] != null && joMain[k] != null && joSub[k].GetType() == joMain[k].GetType())
                        {
                            IJavaScriptObject q = OrJson(joMain[k], joSub[k]);
                            jsNew.Add(k, q);
                        }
                        else
                        {
                            jsNew.Add(k, joSub[k]);
                        }
                    }

                    foreach (string k in joMain.Keys)
                    {
                        if (!jsNew.Contains(k))
                        {
                            jsNew.Add(k, joMain[k]);
                        }
                    }
                }
                else
                {
                    JavaScriptObject joSub = jsSub as JavaScriptObject;
                    foreach (string k in joSub.Keys)
                    {
                        jsNew.Add(k, joSub[k]);
                    }

                }
                return jsNew;
            }
            else
            {
                //未知的jsSub类型,不修改jsMain
                return jsMain;
            }
        }

        #endregion JSON覆盖

    }
}
