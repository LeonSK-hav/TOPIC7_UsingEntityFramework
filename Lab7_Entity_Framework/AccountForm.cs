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
    public partial class AccountForm : Form
    {
        public AccountForm()
        {
            InitializeComponent();
            dgvAccounts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAccounts.MultiSelect = false;

            LoadRolesList();
            LoadAccounts();
            SetupContextMenu();
        }

        private void AccountForm_Load(object sender, EventArgs e)
        {

        }

        private void LoadRolesList()
        {
            lbRolesFilter.Items.Clear();
            using (var db = new RestaurantContext())
            {
                var roles = db.Roles.OrderBy(r => r.RoleName).ToList();
                foreach (var r in roles)
                {
                    lbRolesFilter.Items.Add(r);
                }
            }
        }

        private void LoadAccounts(string nameFilter = "", int? roleIdFilter = null)
        {
            dgvAccounts.Rows.Clear();
            using (var db = new RestaurantContext())
            {
                // Lấy danh sách Account
                var query = db.Accounts.AsQueryable();

                // Lọc theo tên nếu có
                if (!string.IsNullOrEmpty(nameFilter))
                    query = query.Where(a => a.AccountName.Contains(nameFilter) || a.FullName.Contains(nameFilter));

                var list = query.ToList();

                foreach (var a in list)
                {
                    // Lấy danh sách Role của account này
                    var roles = db.RoleAccounts
                                  .Where(ra => ra.AccountName == a.AccountName)
                                  .Join(db.Roles, ra => ra.RoleID, r => r.Id, (ra, r) => r.RoleName)
                                  .ToList();

                    // Nếu có filter theo RoleId
                    if (roleIdFilter.HasValue && !db.RoleAccounts.Any(ra => ra.AccountName == a.AccountName && ra.RoleID == roleIdFilter.Value))
                        continue;

                    // Thêm row vào DataGridView
                    dgvAccounts.Rows.Add(
                        a.AccountName,
                        a.FullName,
                        a.Email,
                        a.Tell,
                        a.DateCreated.ToString("yyyy-MM-dd"),
                        (a.Status.HasValue && a.Status.Value) ? "Active" : "Inactive",
                        string.Join(", ", roles)
                    );
                }
            }
        }


        private void btnSreach_Click(object sender, EventArgs e)
        {
            int? roleId = null;
            if (lbRolesFilter.SelectedItem != null && lbRolesFilter.SelectedItem is Role r)
                roleId = r.Id;

            LoadAccounts(txtSearchName.Text.Trim(), roleId);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {

            using (var dlg = new EditAccountForm())
            {
                if (dlg.ShowDialog(this) != DialogResult.OK) return;

                using (var db = new RestaurantContext())
                {
                    var acc = new Account
                    {
                        AccountName = dlg.AccountName,
                        FullName = dlg.FullName,
                        Email = dlg.Email,
                        Tell = dlg.Tell,
                        Status = true,
                        DateCreated = DateTime.Now
                    };
                    db.Accounts.Add(acc);
                    db.SaveChanges();

                    // gán Role
                    foreach (var role in dlg.SelectedRoles)
                    {
                        db.RoleAccounts.Add(new RoleAccount
                        {
                            RoleID = role.Id,
                            AccountName = acc.AccountName,
                            Actived = true
                        });
                    }
                    db.SaveChanges();
                }
                LoadAccounts();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if(dgvAccounts.SelectedRows.Count == 0) return;
            string accName = dgvAccounts.SelectedRows[0].Cells[0].Value.ToString();

            using (var db = new RestaurantContext())
            {
                var acc = db.Accounts.Find(accName);
                if (acc == null) return;

                var roles = db.RoleAccounts.Where(ra => ra.AccountName == acc.AccountName)
                                           .Select(ra => ra.Role)
                                           .ToList();

                using (var dlg = new EditAccountForm(acc, roles))
                {
                    if (dlg.ShowDialog(this) != DialogResult.OK) return;

                    acc.FullName = dlg.FullName;
                    acc.Email = dlg.Email;
                    acc.Tell = dlg.Tell;

                    // cập nhật role: xóa cũ, thêm mới
                    var oldRoles = db.RoleAccounts.Where(ra => ra.AccountName == acc.AccountName).ToList();
                    db.RoleAccounts.RemoveRange(oldRoles);
                    foreach (var role in dlg.SelectedRoles)
                    {
                        db.RoleAccounts.Add(new RoleAccount
                        {
                            AccountName = acc.AccountName,
                            RoleID = role.Id,
                            Actived = true
                        });
                    }

                    db.SaveChanges();
                }
            }
            LoadAccounts();
        }

        private void btnResetPassword_Click(object sender, EventArgs e)
        {
            if (dgvAccounts.SelectedRows.Count == 0) return;
            string accName = dgvAccounts.SelectedRows[0].Cells[0].Value.ToString();

            using (var db = new RestaurantContext())
            {
                var acc = db.Accounts.Find(accName);
                if (acc == null) return;
                acc.Password = null;
                acc.Status = false;
                db.SaveChanges();
            }
            LoadAccounts();
            MessageBox.Show("Password reset! Account is inactive.");
        }

        private void SetupContextMenu()
        {
            ContextMenuStrip cms = new ContextMenuStrip();
            cms.Items.Add("Xóa tài khoản", null, (s, e) =>
            {
                btnResetPassword_Click(s, e);
            });

            cms.Items.Add("Xem danh sách vai trò", null, (s, e) =>
            {
                if (dgvAccounts.SelectedRows.Count == 0) return;
                string accName = dgvAccounts.SelectedRows[0].Cells[0].Value.ToString();
                using (var frm = new AccountRolesForm(accName))
                {
                    frm.ShowDialog();
                }
            });

            dgvAccounts.ContextMenuStrip = cms;
        }
    }
}
