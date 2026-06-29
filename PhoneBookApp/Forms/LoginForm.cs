using System;
using System.Drawing;
using System.Windows.Forms;
using PhoneBookApp.Database;

namespace PhoneBookApp.Forms
{
    public partial class LoginForm : Form
    {
        private DatabaseHelper _db;

        public LoginForm()
        {
            InitializeComponent();
            _db = new DatabaseHelper();
            // Добавляем рамки для кнопок (в дизайнере нельзя задать границу, делаем в коде)
            AddBorder(btnLogin);
            AddBorder(btnRegister);
            chkShowPassword.CheckedChanged += (s, e) => txtPassword.PasswordChar = chkShowPassword.Checked ? '\0' : '●';
            btnLogin.Click += BtnLogin_Click;
            btnRegister.Click += (s, e) => new RegisterForm(_db).ShowDialog();
        }

        private void AddBorder(Button btn)
        {
            btn.Paint += (sender, e) =>
            {
                var rect = new Rectangle(0, 0, btn.Width - 1, btn.Height - 1);
                ControlPaint.DrawBorder(e.Graphics, rect, Color.Black, ButtonBorderStyle.Solid);
            };
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmail.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Заполните все поля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var user = _db.AuthenticateUser(txtEmail.Text.Trim(), txtPassword.Text);
            if (user != null)
            {
                var main = new MainForm(user, _db);
                main.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Неверный email/пароль или аккаунт заблокирован.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}