using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PhoneBookApp.Database;
using PhoneBookApp.Models;

namespace PhoneBookApp.Forms
{
    public partial class UserManagementForm : Form
    {
        private DatabaseHelper _db;
        private User _currentAdmin;
        private List<User> _users;

        public UserManagementForm(DatabaseHelper db, User currentAdmin)
        {
            InitializeComponent();
            _db = db;
            _currentAdmin = currentAdmin;
            AddBorder(btnBlock); AddBorder(btnUnblock); AddBorder(btnMakeAdmin);
            AddBorder(btnMakeUser); AddBorder(btnDelete); AddBorder(btnViewContacts); AddBorder(btnRefresh);
            btnRefresh.Click += (s, e) => LoadUsers();
            btnBlock.Click += (s, e) => BlockUser();
            btnUnblock.Click += (s, e) => UnblockUser();
            btnMakeAdmin.Click += (s, e) => MakeAdmin();
            btnMakeUser.Click += (s, e) => MakeUser();
            btnDelete.Click += (s, e) => DeleteUser();
            btnViewContacts.Click += (s, e) => ViewContacts();
            LoadUsers();
            SetupDataGridViewStyle();
        }

        private void AddBorder(Button btn)
        {
            btn.Paint += (sender, e) =>
            {
                var rect = new Rectangle(0, 0, btn.Width - 1, btn.Height - 1);
                ControlPaint.DrawBorder(e.Graphics, rect, Color.Black, ButtonBorderStyle.Solid);
            };
        }

        private void SetupDataGridViewStyle()
        {
            dgvUsers.BackgroundColor = Color.White;
            dgvUsers.BorderStyle = BorderStyle.None;
            dgvUsers.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvUsers.GridColor = Color.Black;
            dgvUsers.EnableHeadersVisualStyles = false;
            dgvUsers.ColumnHeadersDefaultCellStyle.BackColor = Color.White;
            dgvUsers.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvUsers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvUsers.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgvUsers.DefaultCellStyle.BackColor = Color.White;
            dgvUsers.DefaultCellStyle.ForeColor = Color.Black;
            dgvUsers.DefaultCellStyle.SelectionBackColor = Color.LightGray; // лёгкое выделение, но не серый фон
            dgvUsers.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvUsers.RowHeadersVisible = false;
            dgvUsers.AllowUserToAddRows = false;
            dgvUsers.ReadOnly = true;
            dgvUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void LoadUsers()
        {
            _users = _db.GetAllUsers();
            dgvUsers.DataSource = null;
            var list = _users.Select(u => new
            {
                u.Id,
                u.Username,
                u.Email,
                u.Role,
                Status = u.IsBlocked ? "Заблокирован" : "Активен",
                Registered = u.RegistrationDate.ToShortDateString()
            }).ToList();
            dgvUsers.DataSource = list;
            if (dgvUsers.Columns["Id"] != null) dgvUsers.Columns["Id"].Visible = false;

            // Установка русских заголовков столбцов
            if (dgvUsers.Columns["Username"] != null) dgvUsers.Columns["Username"].HeaderText = "Имя";
            if (dgvUsers.Columns["Email"] != null) dgvUsers.Columns["Email"].HeaderText = "Email";
            if (dgvUsers.Columns["Role"] != null) dgvUsers.Columns["Role"].HeaderText = "Роль";
            if (dgvUsers.Columns["Status"] != null) dgvUsers.Columns["Status"].HeaderText = "Статус";
            if (dgvUsers.Columns["Registered"] != null) dgvUsers.Columns["Registered"].HeaderText = "Зарегистрирован";
        }

        private User GetSelectedUser()
        {
            if (dgvUsers.SelectedRows.Count == 0) { MessageBox.Show("Выберите пользователя."); return null; }
            int id = (int)dgvUsers.SelectedRows[0].Cells["Id"].Value;
            return _users.FirstOrDefault(u => u.Id == id);
        }

        private void BlockUser()
        {
            var u = GetSelectedUser();
            if (u == null || u.Id == _currentAdmin.Id) { MessageBox.Show("Нельзя заблокировать себя."); return; }
            if (_db.BlockUser(u.Id)) LoadUsers();
        }

        private void UnblockUser()
        {
            var u = GetSelectedUser();
            if (u == null) return;
            if (_db.UnblockUser(u.Id)) LoadUsers();
        }

        private void MakeAdmin()
        {
            var u = GetSelectedUser();
            if (u == null || u.Id == _currentAdmin.Id) return;
            if (_db.ChangeUserRole(u.Id, "Admin")) LoadUsers();
        }

        private void MakeUser()
        {
            var u = GetSelectedUser();
            if (u == null || u.Id == _currentAdmin.Id) return;
            if (_db.ChangeUserRole(u.Id, "User")) LoadUsers();
        }

        private void DeleteUser()
        {
            var u = GetSelectedUser();
            if (u == null || u.Id == _currentAdmin.Id) return;
            if (MessageBox.Show($"Удалить {u.Username} и все его контакты?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                if (_db.DeleteUser(u.Id)) LoadUsers();
        }

        private void ViewContacts()
        {
            var u = GetSelectedUser();
            if (u == null) return;
            var contacts = _db.GetContactsByUserId(u.Id);
            if (!contacts.Any()) { MessageBox.Show("Нет контактов."); return; }
            var f = new Form { Text = $"Контакты: {u.Username}", Size = new Size(700, 400), StartPosition = FormStartPosition.CenterParent, BackColor = Color.White };
            var dgv = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor = Color.White, BorderStyle = BorderStyle.None };
            dgv.DataSource = contacts.Select(c => new { c.FirstName, c.LastName, c.Phone, c.Email }).ToList();
            f.Controls.Add(dgv);
            f.ShowDialog();
        }
    }
}