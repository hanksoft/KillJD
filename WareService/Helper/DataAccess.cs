using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;
using System.Threading;
using System.Data;

namespace DataAccess
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class SqliteConn
    {
        private bool m_disposed;
        private static Dictionary<String, SQLiteConnection> connPool = new Dictionary<string, SQLiteConnection>();
        private static Dictionary<String, ReaderWriterLock> rwl = new Dictionary<String, ReaderWriterLock>();
        private static readonly SqliteConn instance = new SqliteConn();
        private static string DEFAULT_NAME = "LOCAL";

        #region Init
        // 使用单例，解决初始化与销毁时的问题
        private SqliteConn()
        {
            rwl.Add("LOCAL", new ReaderWriterLock());
            rwl.Add("DB1", new ReaderWriterLock());
            connPool.Add("LOCAL", CreateConn("\\local.db"));
            connPool.Add("DB1", CreateConn("\\db1.db"));
            Console.WriteLine("INIT FINISHED");
        }

        private static SQLiteConnection CreateConn(string dbName)
        {
            SQLiteConnection _conn = new SQLiteConnection();
            try
            {
                string pstr = "pwd";
                SQLiteConnectionStringBuilder connstr = new SQLiteConnectionStringBuilder();
                connstr.DataSource = Environment.CurrentDirectory + dbName;
                _conn.ConnectionString = connstr.ToString();
                _conn.SetPassword(pstr);
                _conn.Open();
                return _conn;
            }
            catch (Exception exp)
            {
                Console.WriteLine("===CONN CREATE ERR====\r\n{0}", exp.ToString());
                return null;
            }
        }
        #endregion

        #region Destory
        // 手动控制销毁，保证数据完整性
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                if (disposing)
                {
                    // Release managed resources
                    Console.WriteLine("关闭本地DB连接...");
                    CloseConn();
                }
                // Release unmanaged resources
                m_disposed = true;
            }
        }

        ~SqliteConn()
        {
            Dispose(false);
        }

        public void CloseConn()
        {
            foreach (KeyValuePair<string, SQLiteConnection> item in connPool)
            {
                SQLiteConnection _conn = item.Value;
                String _connName = item.Key;
                if (_conn != null && _conn.State != ConnectionState.Closed)
                {
                    try
                    {
                        _conn.Close();
                        _conn.Dispose();
                        _conn = null;
                        Console.WriteLine("Connection {0} Closed.", _connName);
                    }
                    catch (Exception exp)
                    {
                        Console.WriteLine("严重异常: 无法关闭本地DB {0} 的连接。", _connName);
                        exp.ToString();
                    }
                    finally
                    {
                        _conn = null;
                    }
                }
            }
        }
        #endregion

        #region GetConn
        public static SqliteConn GetInstance()
        {
            return instance;
        }

        public SQLiteConnection GetConnection(string name)
        {
            SQLiteConnection _conn = connPool[name];

            try
            {
                if (_conn != null)
                {
                    Console.WriteLine("TRY GET LOCK");
                    //加锁，直到释放前，其它线程无法得到conn
                    rwl[name].AcquireWriterLock(3000);
                    Console.WriteLine("LOCK GET");
                    return _conn;
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine("===GET CONN ERR====\r\n{0}", exp.StackTrace);
            }
            return null;
        }

        public void ReleaseConn(string name)
        {
            try
            {
                //释放
                Console.WriteLine("RELEASE LOCK");
                rwl[name].ReleaseLock();
            }
            catch (Exception exp)
            {
                Console.WriteLine("===RELEASE CONN ERR====\r\n{0}", exp.StackTrace);
            }
        }

        public SQLiteConnection GetConnection()
        {
            return GetConnection(DEFAULT_NAME);
        }

        public void ReleaseConn()
        {
            ReleaseConn(DEFAULT_NAME);
        }
        #endregion
    }

}