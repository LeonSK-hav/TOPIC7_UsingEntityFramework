using Lab7_Entity_Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab7_Entity_Framework
{
    public class RoleAccount
    {
        public int Id { get; set; }              // new PK
        public int RoleID { get; set; }
        public string AccountName { get; set; }  // can now be null if desired
        public bool Actived { get; set; }
        public string Notes { get; set; }

        public virtual Role Role { get; set; }
        public virtual Account Account { get; set; }
    }
}
