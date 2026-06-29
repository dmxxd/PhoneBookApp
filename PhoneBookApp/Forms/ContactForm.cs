using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PhoneBookApp.Database;
using PhoneBookApp.Models;

namespace PhoneBookApp.Forms
{
    public partial class ContactForm : Form
    {
        private int _userId;
        private DatabaseHelper _db;
        private Contact _editingContact;

        public ContactForm(int userId, DatabaseHelper db, Contact contact)
        {
            InitializeComponent();
            _userId = userId;
            _db = db;
            _editingContact = contact;
            LoadTags();
            if (contact != null) LoadContactData();
            AddBorder(btnSave);
            AddBorder(btnCancel);
            btnSave.Click += BtnSave_Click;
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

        private void LoadTags()
        {
            clbTags.Items.Clear();
            var allTags = _db.GetAllTags();
            foreach (var tag in allTags) clbTags.Items.Add(tag, false);
        }

        private void LoadContactData()
        {
            txtFirstName.Text = _editingContact.FirstName;
            txtLastName.Text = _editingContact.LastName;
            txtPhone.Text = _editingContact.Phone;
            txtEmail.Text = _editingContact.Email;
            txtAddress.Text = _editingContact.Address;
            txtNote.Text = _editingContact.Note;
            if (_editingContact.Birthday.HasValue) dtpBirthday.Value = _editingContact.Birthday.Value;
            for (int i = 0; i < clbTags.Items.Count; i++)
                if (_editingContact.TagNames.Contains(clbTags.Items[i].ToString()))
                    clbTags.SetItemChecked(i, true);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                MessageBox.Show("Введите имя.");
                return;
            }

            var contact = new Contact
            {
                Id = _editingContact?.Id ?? 0,
                UserId = _userId,
                FirstName = txtFirstName.Text.Trim(),
                LastName = txtLastName.Text.Trim(),
                Phone = txtPhone.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Address = txtAddress.Text.Trim(),
                Note = txtNote.Text.Trim(),
                Birthday = dtpBirthday.Checked ? dtpBirthday.Value : (DateTime?)null,
                TagNames = clbTags.CheckedItems.Cast<string>().ToList()
            };

            bool ok = _editingContact == null ? _db.AddContact(contact) : _db.UpdateContact(contact);
            if (ok) { DialogResult = DialogResult.OK; Close(); }
            else MessageBox.Show("Ошибка сохранения.");
        }
    }
}