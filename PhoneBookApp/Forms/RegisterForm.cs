using System;
using System.Drawing;
using System.Windows.Forms;
using PhoneBookApp.Database;

namespace PhoneBookApp.Forms
{
    public partial class RegisterForm : Form
    {
        private DatabaseHelper _db;

        public RegisterForm(DatabaseHelper db)
        {
            InitializeComponent();
            _db = db;
            AddBorder(btnRegister);
            AddBorder(btnCancel);
            btnRegister.Click += BtnRegister_Click;
            btnCancel.Click += (s, e) => Close();
        }

        private void AddBorder(Button btn)
        {
            btn.Paint += (sender, e) =>
            {
                var rect = new Rectangle(0, 0, btn.Width - 1, btn.Height - 1);
                ControlPaint.DrawBorder(e.Graphics, rect, Color.Black, ButtonBorderStyle.Solid);
            };
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string email = txtEmail.Text.Trim();
            string pass = txtPassword.Text;
            string confirm = txtConfirm.Text;

            if (string.IsNullOrWhiteSpace(username)) { MessageBox.Show("Введите имя."); return; }
            if (string.IsNullOrWhiteSpace(email)) { MessageBox.Show("Введите email."); return; }
            if (pass.Length < 4) { MessageBox.Show("Пароль должен быть не менее 4 символов."); return; }
            if (pass != confirm) { MessageBox.Show("Пароли не совпадают."); return; }
            try { var addr = new System.Net.Mail.MailAddress(email); if (addr.Address != email) throw new Exception(); }
            catch { MessageBox.Show("Некорректный email."); return; }

            if (_db.RegisterUser(username, email, pass))
            {
                MessageBox.Show("Регистрация успешна!");
                DialogResult = DialogResult.OK;
                Close();
            }
            else MessageBox.Show("Пользователь с таким email уже существует.");
        }
    }
}