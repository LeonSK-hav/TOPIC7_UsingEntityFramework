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
    public partial class EditAccountForm : Form
    {
        public string AccountName => txtName.Text.Trim();
        public string FullName => txtFullName.Text.Trim();
        public string Email => txtEmail.Text.Trim();
        public string Tell => txtTell.Text.Trim();
        public List<Role> SelectedRoles => lbRoles.SelectedItems.Cast<Role>().ToList();
        public EditAccountForm()
        {
            InitializeComponent();
            LoadRoles();
        }

        public EditAccountForm(Account acc, List<Role> assignedRoles) : this()
        {
            txtName.Text = acc.AccountName;
            txtName.Enabled = false;
            txtFullName.Text = acc.FullName;
            txtEmail.Text = acc.Email;
            txtTell.Text = acc.Tell;

            // tick các role đã gán
            for (int i = 0; i < lbRoles.Items.Count; i++)
            {
                var role = lbRoles.Items[i] as Role;
                if (assignedRoles.Any(r => r.Id == role.Id))
                    lbRoles.SetSelected(i, true);
            }
        }

        private void LoadRoles()
        {
            lbRoles.Items.Clear();
            using (var db = new RestaurantContext())
            {
                var roles = db.Roles.OrderBy(r => r.RoleName).ToList();
                foreach (var r in roles)
                    lbRoles.Items.Add(r);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(AccountName))
            {
                MessageBox.Show("Account name is required");
                return;
            }
            DialogResult = DialogResult.OK;
        }

        public override string ToString()
        {
            return AccountName;
        }
    }
}
