using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHibernate.GuitarStore.Common
{
    public class Guitar
    {
        //private Guid guid;
        //private string p;

        //public Guitar(Guid guid, string p)
        //{
        //    // TODO: Complete member initialization
        //    this.guid = guid;
        //    this.p = p;
        //}
        public virtual Guid Id { get; set; }
        public virtual string Type { get; set; }
        IList<Inventory> Inventory { get; set; }


    }
}
