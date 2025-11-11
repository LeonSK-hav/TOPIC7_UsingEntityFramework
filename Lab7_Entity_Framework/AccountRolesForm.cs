using Lab7_Entity_Framework.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab7_Entity_Framework
{
    public partial class AccountRolesForm : Form
    {
        public AccountRolesForm(string accountName)
        {
            InitializeComponent();
            LoadRoles(accountName);
        }
        private void LoadRoles(string accountName)
        {
            using (var db = new RestaurantContext())
            {
                var roles = db.RoleAccounts
                              .Where(ra => ra.AccountName == accountName)
                              .Select(ra => ra.Role.RoleName)
                              .ToList();
                lstRoles.Items.Clear();
                foreach (var r in roles)
                    lstRoles.Items.Add(r);
            }
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
