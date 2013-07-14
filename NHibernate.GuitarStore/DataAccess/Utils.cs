using NHibernate.AdoNet.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHibernate.GuitarStore.DataAccess
{
    public class Utils
    {
        public static string NHibernateGeneratedSQL { get; set; }

        public static int QueryCounter { get; set; }
        public static string FormatSQL()
        {
            BasicFormatter formatter = new BasicFormatter();
            return formatter.Format(NHibernateGeneratedSQL.ToLower());
        }
    }
}