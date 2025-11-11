using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab7_Entity_Framework.Models
{
    public class Account
    {
        public string AccountName { get; set; } // PK, maps to RoleAccount.AccountName

        public string Password { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Tell { get; set; }
        public DateTime DateCreated { get; set; }

        public bool? Status { get; set; }

    }
}
