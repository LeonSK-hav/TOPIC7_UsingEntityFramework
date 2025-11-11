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
    public partial class RoleForm : Form
    {
        public RoleForm()
        {
            InitializeComponent();
            // Ensure the TreeView selection triggers account loading
            tvwRoles.AfterSelect += tvwRoles_AfterSelect;
        }

        private void LoadRoles()
        {
            tvwRoles.Nodes.Clear();
            using (var db = new RestaurantContext())
            {
                var roles = db.Roles.OrderBy(r => r.RoleName).ToList();
                foreach (var role in roles)
                {
                    var node = new TreeNode(role.RoleName)
                    {
                        Tag = role,
                        Name = role.Id.ToString()
                    };
                    tvwRoles.Nodes.Add(node);
                }
            }
            if (tvwRoles.Nodes.Count > 0) tvwRoles.SelectedNode = tvwRoles.Nodes[0];
        }

        private void RoleForm_Load(object sender, EventArgs e)
        {
            LoadRoles();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadRoles();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var dlg = new EditRoleForm())
            {
                if (dlg.ShowDialog(this) != DialogResult.OK) return;
                using (var db = new RestaurantContext())
                {
                    var role = new Role
                    {
                        RoleName = dlg.RoleName,
                        Path = dlg.Path,
                        Notes = dlg.Notes
                    };
                    db.Roles.Add(role);
                    db.SaveChanges();
                    LoadRoles();
                    SelectRole(role.Id);
                }
            }
        }
        private void SelectRole(int roleId)
        {
            foreach (TreeNode node in tvwRoles.Nodes)
            {
                if (node.Name == roleId.ToString())
                {
                    tvwRoles.SelectedNode = node;
                    node.EnsureVisible();
                    break;
                }
            }
        }

        private void LoadAccountsForSelectedRole()
        {
            lvwAccounts.Items.Clear();
            var node = tvwRoles.SelectedNode;
            if (node == null || !(node.Tag is Role)) return;
            var role = node.Tag as Role;
            using (var db = new RestaurantContext())
            {
                var users = db.RoleAccounts
                    .Where(ra => ra.RoleID == role.Id)
                    .GroupJoin(
                        db.Accounts,
                        ra => ra.AccountName,
                        a => a.AccountName,
                        (ra, accs) => new { ra, accs })
                    .SelectMany(
                        x => x.accs.DefaultIfEmpty(),
                        (x, a) => new
                        {
                            AccountName = x.ra.AccountName,
                            AccountKey = a != null ? a.AccountName : null,
                            FullName = a != null ? a.FullName : null,
                            Email = a != null ? a.Email : null,
                            Tell = a != null ? a.Tell : null,
                            DateCreated = a != null ? (DateTime?)a.DateCreated : null,
                            Actived = x.ra.Actived,
                            RoleAccountNotes = x.ra.Notes
                        })
                    .OrderBy(u => u.AccountName)
                    .ToList();

                foreach (var u in users)
                {
                    var item = new ListViewItem(u.AccountName);
                    item.SubItems.Add(u.FullName ?? "");
                    item.SubItems.Add(u.Email ?? "");
                    item.SubItems.Add(u.Tell ?? "");
                    item.SubItems.Add(u.DateCreated?.ToString("yyyy-MM-dd") ?? "");
                    item.SubItems.Add(u.Actived ? "Yes" : "No");
                    item.SubItems.Add(u.RoleAccountNotes ?? "");
                    item.Tag = u;
                    lvwAccounts.Items.Add(item);
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {

            var node = tvwRoles.SelectedNode;
            if (node == null || !(node.Tag is Role)) return;
            var roleTag = node.Tag as Role;
            using (var db = new RestaurantContext())
            {
                var role = db.Roles.Find(roleTag.Id);
                if (role == null) return;
                using (var dlg = new EditRoleForm(role.RoleName, role.Path, role.Notes))
                {
                    if (dlg.ShowDialog(this) != DialogResult.OK) return;
                    role.RoleName = dlg.RoleName;
                    role.Path = dlg.Path;
                    role.Notes = dlg.Notes;
                    db.SaveChanges();
                    LoadRoles();
                    SelectRole(role.Id);
                }
            }
        }

        private void tvwRoles_AfterSelect(object sender, TreeViewEventArgs e)
        {
            LoadAccountsForSelectedRole();
        }
    }
}
