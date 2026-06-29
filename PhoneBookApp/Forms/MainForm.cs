using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PhoneBookApp.Database;
using PhoneBookApp.Models;
using PhoneBookApp.Helpers;

namespace PhoneBookApp.Forms
{
    public partial class MainForm : Form
    {
        private User _currentUser;
        private DatabaseHelper _db;
        private List<Contact> _currentContacts;
        private FlowLayoutPanel flowContacts;
        private TextBox txtSearch;
        private Button btnFilterTags;
        private Button btnAdd;
        private List<string> _selectedTags = new List<string>();
        private bool darkMode = false;
        private Panel sidePanel;
        private Panel topPanel;

        public MainForm(User user, DatabaseHelper db)
        {
            InitializeComponent();
            _currentUser = user;
            _db = db;
            BuildUI();
            LoadContacts();
            CheckBirthdays();

            тёмнаяТемаToolStripMenuItem.Click += (s, e) => ToggleTheme();
            выйтиToolStripMenuItem.Click += (s, e) => { var l = new LoginForm(); l.Show(); this.Close(); };
        }

        private void BuildUI()
        {
            // Верхняя панель
            topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                Padding = new Padding(10),
                BackColor = darkMode ? Color.Black : Color.White
            };

            txtSearch = new TextBox
            {
                Width = 280,
                Location = new Point(10, 15),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = darkMode ? Color.Black : Color.White,
                ForeColor = darkMode ? Color.White : Color.Black
            };
            txtSearch.TextChanged += (s, e) => SearchTimer();
            txtSearch.Enter += (s, e) => { if (txtSearch.Text == "Поиск...") txtSearch.Text = ""; };
            txtSearch.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txtSearch.Text)) txtSearch.Text = "Поиск..."; };
            txtSearch.Text = "Поиск...";
            txtSearch.ForeColor = darkMode ? Color.Gray : Color.Gray;

            btnFilterTags = new Button
            {
                Text = "Фильтр по тегам",
                Location = new Point(310, 13),
                Width = 120,
                Height = 30,
                FlatStyle = FlatStyle.Flat,
                BackColor = darkMode ? Color.Black : Color.White,
                ForeColor = darkMode ? Color.White : Color.Black,
                Font = new Font("Segoe UI", 9),
                Cursor = Cursors.Hand
            };
            btnFilterTags.Click += (s, e) => ShowTagFilter();
            AddBorder(btnFilterTags);

            topPanel.Controls.Add(txtSearch);
            topPanel.Controls.Add(btnFilterTags);

            // Боковая панель
            sidePanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 220,
                BackColor = darkMode ? Color.Black : Color.White,
                Padding = new Padding(10)
            };

            btnAdd = CreateSideButton("Добавить контакт");
            btnAdd.Click += (s, e) => AddContact();
            sidePanel.Controls.Add(btnAdd);
            sidePanel.Controls.Add(CreateSeparator());

            if (_currentUser.Role == "Admin")
            {
                sidePanel.Controls.Add(CreateSpacer(8));
                var btnAdmin = CreateSideButton("Управление пользователями");
                btnAdmin.Click += (s, e) => new UserManagementForm(_db, _currentUser).ShowDialog();
                sidePanel.Controls.Add(btnAdmin);
                sidePanel.Controls.Add(CreateSeparator());
            }

            flowContacts = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(15),
                BackColor = darkMode ? Color.Black : Color.White
            };

            this.Controls.Add(flowContacts);
            this.Controls.Add(sidePanel);
            this.Controls.Add(topPanel);
            ApplyTheme();
        }

        private void AddBorder(Button btn)
        {
            btn.Paint += (sender, e) =>
            {
                var rect = new Rectangle(0, 0, btn.Width - 1, btn.Height - 1);
                ControlPaint.DrawBorder(e.Graphics, rect, darkMode ? Color.DarkGray : Color.Black, ButtonBorderStyle.Solid);
            };
        }

        private Button CreateSideButton(string text)
        {
            var btn = new Button
            {
                Text = text,
                Dock = DockStyle.Top,
                Height = 40,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = darkMode ? Color.White : Color.Black,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0),
                Font = new Font("Segoe UI", 9),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 0, 5)
            };
            AddBorder(btn);
            return btn;
        }

        private Panel CreateSeparator()
        {
            return new Panel
            {
                Dock = DockStyle.Top,
                Height = 1,
                BackColor = darkMode ? Color.DarkGray : Color.Black,
                Margin = new Padding(0, 5, 0, 5)
            };
        }

        private Panel CreateSpacer(int height)
        {
            return new Panel { Dock = DockStyle.Top, Height = height, BackColor = Color.Transparent };
        }

        private Timer searchTimer;
        private void SearchTimer()
        {
            if (searchTimer == null)
            {
                searchTimer = new Timer { Interval = 500 };
                searchTimer.Tick += (s, e) => { searchTimer.Stop(); LoadContacts(); };
            }
            searchTimer.Stop();
            searchTimer.Start();
        }

        private void ShowTagFilter()
        {
            var allTags = _db.GetAllTags();
            if (allTags.Count == 0) return;
            var f = new Form
            {
                Text = "Фильтр по тегам",
                Size = new Size(300, 400),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.White
            };
            var clb = new CheckedListBox { Dock = DockStyle.Fill, CheckOnClick = true, BorderStyle = BorderStyle.FixedSingle };
            foreach (var tag in allTags) clb.Items.Add(tag);
            var btnOk = new Button
            {
                Text = "Применить",
                Dock = DockStyle.Bottom,
                Height = 35,
                BackColor = Color.White,
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat
            };
            btnOk.Paint += (s, e) => ControlPaint.DrawBorder(e.Graphics, new Rectangle(0, 0, btnOk.Width - 1, btnOk.Height - 1), Color.Black, ButtonBorderStyle.Solid);
            btnOk.Click += (s, e) => { _selectedTags = clb.CheckedItems.Cast<string>().ToList(); LoadContacts(); f.Close(); };
            f.Controls.Add(clb);
            f.Controls.Add(btnOk);
            f.ShowDialog();
        }

        private void LoadContacts()
        {
            string search = txtSearch.Text == "Поиск..." ? "" : txtSearch.Text;
            _currentContacts = _db.SearchContacts(_currentUser.Id, search, _selectedTags);
            DisplayContacts();
        }

        private void DisplayContacts()
        {
            flowContacts.Controls.Clear();
            if (_currentContacts.Count == 0)
            {
                var lblEmpty = new Label
                {
                    Text = "Нет контактов. Нажмите 'Добавить контакт'.",
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill,
                    Font = new Font("Segoe UI", 12),
                    ForeColor = darkMode ? Color.White : Color.Black
                };
                flowContacts.Controls.Add(lblEmpty);
                return;
            }

            foreach (var c in _currentContacts)
            {
                var card = CreateContactCard(c);
                flowContacts.Controls.Add(card);
            }
        }

        private Panel CreateContactCard(Contact c)
        {
            var card = new Panel
            {
                Width = flowContacts.Width - 30,
                Height = 110,
                BackColor = darkMode ? Color.Black : Color.White,
                Margin = new Padding(0, 0, 0, 10),
                Padding = new Padding(10)
            };
            card.Paint += (s, e) =>
            {
                var rect = new Rectangle(0, 0, card.Width - 1, card.Height - 1);
                ControlPaint.DrawBorder(e.Graphics, rect, darkMode ? Color.DarkGray : Color.Black, ButtonBorderStyle.Solid);
            };

            // Используем TableLayoutPanel для жёсткого разделения текста и кнопок
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3,
                Padding = new Padding(0),
                Margin = new Padding(0),
                BackColor = Color.Transparent
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 85F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));

            var lblName = new Label
            {
                Text = $"{c.FirstName} {c.LastName}",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = darkMode ? Color.White : Color.Black
            };
            var lblPhone = new Label
            {
                Text = string.IsNullOrEmpty(c.Phone) ? "—" : c.Phone,
                Font = new Font("Segoe UI", 9),
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = darkMode ? Color.White : Color.Black
            };
            var lblTags = new Label
            {
                Text = c.TagNames.Any() ? string.Join(", ", c.TagNames.Take(2)) : "",
                Font = new Font("Segoe UI", 8),
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = darkMode ? Color.White : Color.Black
            };

            var btnEdit = new Button
            {
                Text = "Изменить",
                FlatStyle = FlatStyle.Flat,
                BackColor = darkMode ? Color.Black : Color.White,
                ForeColor = darkMode ? Color.White : Color.Black,
                Dock = DockStyle.Top,
                Height = 30,
                Margin = new Padding(0, 0, 0, 5),
                Cursor = Cursors.Hand,
                Tag = c
            };
            btnEdit.Paint += (s, e) =>
            {
                var rect = new Rectangle(0, 0, btnEdit.Width - 1, btnEdit.Height - 1);
                ControlPaint.DrawBorder(e.Graphics, rect, darkMode ? Color.DarkGray : Color.Black, ButtonBorderStyle.Solid);
            };
            btnEdit.Click += (s, e) => EditContact(c);

            var btnDelete = new Button
            {
                Text = "Удалить",
                FlatStyle = FlatStyle.Flat,
                BackColor = darkMode ? Color.Black : Color.White,
                ForeColor = darkMode ? Color.White : Color.Black,
                Dock = DockStyle.Top,
                Height = 30,
                Cursor = Cursors.Hand,
                Tag = c
            };
            btnDelete.Paint += (s, e) =>
            {
                var rect = new Rectangle(0, 0, btnDelete.Width - 1, btnDelete.Height - 1);
                ControlPaint.DrawBorder(e.Graphics, rect, darkMode ? Color.DarkGray : Color.Black, ButtonBorderStyle.Solid);
            };
            btnDelete.Click += (s, e) => DeleteContact(c);

            var rightPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
            rightPanel.Controls.Add(btnEdit);
            rightPanel.Controls.Add(btnDelete);
            btnEdit.Location = new Point(0, 0);
            btnDelete.Location = new Point(0, 35);

            layout.Controls.Add(lblName, 0, 0);
            layout.Controls.Add(lblPhone, 0, 1);
            layout.Controls.Add(lblTags, 0, 2);
            layout.Controls.Add(rightPanel, 1, 0);
            layout.SetRowSpan(rightPanel, 3);

            card.Controls.Add(layout);
            return card;
        }

        private void EditContact(Contact c)
        {
            var cf = new ContactForm(_currentUser.Id, _db, c);
            if (cf.ShowDialog() == DialogResult.OK) LoadContacts();
        }

        private void DeleteContact(Contact c)
        {
            if (MessageBox.Show($"Удалить контакт {c.FirstName} {c.LastName}?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (_db.DeleteContact(c.Id, _currentUser.Id)) LoadContacts();
                else MessageBox.Show("Ошибка при удалении.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddContact()
        {
            var cf = new ContactForm(_currentUser.Id, _db, null);
            if (cf.ShowDialog() == DialogResult.OK) LoadContacts();
        }

        private void CheckBirthdays()
        {
            var today = DateTime.Today;
            var end = today.AddDays(3);
            var birthdays = _db.GetContactsWithBirthdayInRange(today, end);
            if (birthdays.Any())
            {
                string msg = "Ближайшие дни рождения:\n" +
                    string.Join("\n", birthdays.Select(c => $"{c.FirstName} {c.LastName} — {c.Birthday:dd MMMM}"));
                MessageBox.Show(msg, "Напоминание", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ToggleTheme()
        {
            darkMode = !darkMode;
            ApplyTheme();
            LoadContacts();
        }

        private void ApplyTheme()
        {
            Color backColor = darkMode ? Color.Black : Color.White;
            Color foreColor = darkMode ? Color.White : Color.Black;
            Color panelBack = darkMode ? Color.Black : Color.White;
            Color borderColor = darkMode ? Color.DarkGray : Color.Black;

            this.BackColor = backColor;
            topPanel.BackColor = panelBack;
            sidePanel.BackColor = panelBack;
            flowContacts.BackColor = panelBack;

            txtSearch.BackColor = panelBack;
            txtSearch.ForeColor = foreColor;
            btnFilterTags.BackColor = panelBack;
            btnFilterTags.ForeColor = foreColor;

            foreach (Control c in sidePanel.Controls)
            {
                if (c is Button btn)
                {
                    btn.BackColor = Color.Transparent;
                    btn.ForeColor = foreColor;
                    btn.Invalidate();
                }
                if (c is Panel sep && sep.Height == 1)
                    sep.BackColor = borderColor;
            }

            foreach (Control card in flowContacts.Controls)
            {
                if (card is Panel p)
                {
                    p.BackColor = panelBack;
                    p.Invalidate();
                    foreach (Control child in p.Controls)
                    {
                        if (child is Label lbl) lbl.ForeColor = foreColor;
                        if (child is Button btn)
                        {
                            btn.BackColor = darkMode ? Color.Black : Color.White;
                            btn.ForeColor = foreColor;
                            btn.Invalidate();
                        }
                    }
                }
            }
        }
    }
}
