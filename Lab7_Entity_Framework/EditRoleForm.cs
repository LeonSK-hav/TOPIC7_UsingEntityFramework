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
    public partial class EditRoleForm : Form
    {
        public string RoleName => txtRoleName.Text.Trim();
        public string Path => txtPath.Text.Trim();
        public string Notes => txtNotes.Text.Trim();
        public EditRoleForm()
        {
            InitializeComponent();
        }

        public EditRoleForm(string roleName, string path, string notes) : this()
        {
            txtRoleName.Text = roleName;
            txtPath.Text = path;
            txtNotes.Text = notes;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(RoleName))
            {
                MessageBox.Show("Role name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
