using NHibernate;
using NHibernate.Cfg;
using NHibernate.SqlCommand;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using log4net;
using NHibernate.Event;

namespace NHibernate.GuitarStore.DataAccess
{
    public class NHibernateBase
    {
        private static NHibernate.Cfg.Configuration Configuration { get; set; }
        protected static ISessionFactory SessionFactory { get; set; }
        private static ISession session = null;
        private static IStatelessSession statelessSession = null;
        private static string SerializedConfiguration = ConfigurationManager.AppSettings["SerializedFilename"];
        private static ILog log = LogManager.GetLogger("NHBase.SQL");

        public NHibernateBase()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        public static NHibernate.Cfg.Configuration ConfigureNHibernate(string assembly)
        {
            if (Configuration == null)
            {
                Configuration = new NHibernate.Cfg.Configuration();
                Configuration.AddAssembly(assembly);

                FileStream file = File.Open(SerializedConfiguration, FileMode.Create);
                IFormatter bf = new BinaryFormatter();
                bf.Serialize(file, Configuration);
                file.Close();
            }
            Configuration = LoadConfigurationFromFile();

            //拦截器，输出HQL
            Configuration.SetInterceptor(new SQLInterceptor());
            //日志记录
            log.Info("NHibernate config sucessful");

            return Configuration;
        }
        /// <summary>
        /// 实例化NHibernate
        /// </summary>
        /// <param name="assembly">程序集命名空间</param>
        public void Initialize(string assembly)
        {
            Configuration = ConfigureNHibernate(assembly);
            //注册监听事件
            Configuration.EventListeners.PostDeleteEventListeners = new IPostDeleteEventListener[] { new AuditDeleteEvent() };

            SessionFactory = Configuration.BuildSessionFactory();
        }

        /// <summary>
        /// Validating a Serialized Configuration
        /// </summary>
        private static bool IsConfigurationFileValid
        {
            get
            {
                try
                {

                    Assembly assembly = Assembly.Load("Nhibernate.GuitarStore");
                    FileInfo configInfo = new FileInfo(SerializedConfiguration);
                    FileInfo asmInfo = new FileInfo(assembly.Location);
                    return configInfo.LastWriteTime >= asmInfo.LastWriteTime;
                }
                catch (Exception ex)
                {
                    //log.Debug(ex.Message);
                    return false;
                }
            }
        }

        private static NHibernate.Cfg.Configuration LoadConfigurationFromFile()
        {
            if (!IsConfigurationFileValid) return null;
            try
            {
                using (FileStream file =
                File.Open(SerializedConfiguration, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    return (NHibernate.Cfg.Configuration)bf.Deserialize(file);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }



        public static ISession Session
        {
            get
            {
                if (session == null)
                {
                    session = SessionFactory.OpenSession();
                }
                return session;
            }
        }
        public static IStatelessSession StatelessSession
        {
            get
            {
                if (statelessSession == null)
                {
                    statelessSession = SessionFactory.OpenStatelessSession();
                }
                return statelessSession;
            }
        }

        public IList<T> ExecuteICriteria<T>()
        {
            using (ITransaction transaction = Session.BeginTransaction())
            {
                try
                {
                    IList<T> result = Session.CreateCriteria(typeof(T)).List<T>();
                    transaction.Commit();
                    return result;
                }
                catch (Exception ex)
                {
                    //log.Info(ex.Message);
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }

    public class SQLInterceptor : EmptyInterceptor, IInterceptor
    {
        SqlString IInterceptor.OnPrepareStatement(NHibernate.SqlCommand.SqlString sql)
        {
            Utils.NHibernateGeneratedSQL = sql.ToString();
            Utils.QueryCounter++;
            System.Diagnostics.Trace.WriteLine("============"+sql);
            return sql;
        }
    }


    #region 事件event

    public class AuditDeleteEvent : IPostDeleteEventListener
    {
        private static ILog log = LogManager.GetLogger("NHBase.SQL");
        public void OnPostDelete(PostDeleteEvent @event)
        {
            log.Info(@event.Id.ToString() + " 已经被删除.");
        }
    }
    #endregion 
}