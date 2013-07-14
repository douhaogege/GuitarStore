using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NHibernate.Cfg;
using NHibernate.GuitarStore.DataAccess;

namespace NHibernate.GuitarStore.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {


                NHibernateBase NHB = new NHibernateBase();
                NHB.Initialize("NHibernate.GuitarStore");
                System.Console.WriteLine("NHibernate.GuitarStore assembly initialized.");
                System.Console.ReadLine();
            }
            catch (Exception ex)
            {
                string Message = ex.Message;
                if (ex.InnerException != null)
                {
                    Message += " - InnerExcepetion: " + ex.InnerException.Message;
                }
                System.Console.WriteLine();
                System.Console.WriteLine("***** ERROR *****");
                System.Console.WriteLine(Message);
                System.Console.WriteLine();
                System.Console.ReadLine();
            }
        }
    }
}
