namespace PhoneBookApp.Forms
{
    partial class UserManagementForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.dgvUsers = new System.Windows.Forms.DataGridView();
            this.btnBlock = new System.Windows.Forms.Button();
            this.btnUnblock = new System.Windows.Forms.Button();
            this.btnMakeAdmin = new System.Windows.Forms.Button();
            this.btnMakeUser = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnViewContacts = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsers)).BeginInit();
            this.SuspendLayout();

            // dgvUsers
            this.dgvUsers.AllowUserToAddRows = false;
            this.dgvUsers.Location = new System.Drawing.Point(15, 15);
            this.dgvUsers.Size = new System.Drawing.Size(770, 350);
            this.dgvUsers.TabIndex = 0;

            // Buttons
            int btnY = 380;
            int btnH = 35;
            int[] btnX = { 15, 140, 265, 390, 515 };
            string[] btnText = { "Заблокировать", "Разблокировать", "Сделать Admin", "Сделать User", "Удалить" };

            for (int i = 0; i < btnText.Length; i++)
            {
                var btn = new System.Windows.Forms.Button();
                btn.Text = btnText[i];
                btn.Location = new System.Drawing.Point(btnX[i], btnY);
                btn.Size = new System.Drawing.Size(115, btnH);
                btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                btn.BackColor = System.Drawing.Color.White;
                btn.ForeColor = System.Drawing.Color.Black;
                btn.Cursor = System.Windows.Forms.Cursors.Hand;
                this.Controls.Add(btn);
                if (i == 0) this.btnBlock = btn;
                else if (i == 1) this.btnUnblock = btn;
                else if (i == 2) this.btnMakeAdmin = btn;
                else if (i == 3) this.btnMakeUser = btn;
                else if (i == 4) this.btnDelete = btn;
            }

            // btnViewContacts
            this.btnViewContacts = new System.Windows.Forms.Button();
            this.btnViewContacts.Text = "Просмотреть контакты";
            this.btnViewContacts.Location = new System.Drawing.Point(15, 430);
            this.btnViewContacts.Size = new System.Drawing.Size(150, 35);
            this.btnViewContacts.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnViewContacts.BackColor = System.Drawing.Color.White;
            this.btnViewContacts.ForeColor = System.Drawing.Color.Black;
            this.btnViewContacts.Cursor = System.Windows.Forms.Cursors.Hand;

            // btnRefresh
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnRefresh.Text = "Обновить";
            this.btnRefresh.Location = new System.Drawing.Point(680, 430);
            this.btnRefresh.Size = new System.Drawing.Size(105, 35);
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.BackColor = System.Drawing.Color.White;
            this.btnRefresh.ForeColor = System.Drawing.Color.Black;
            this.btnRefresh.Cursor = System.Windows.Forms.Cursors.Hand;

            this.Controls.Add(this.dgvUsers);
            this.Controls.Add(this.btnViewContacts);
            this.Controls.Add(this.btnRefresh);
            this.ClientSize = new System.Drawing.Size(800, 490);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Управление пользователями";
            this.BackColor = System.Drawing.Color.White;
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsers)).EndInit();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.DataGridView dgvUsers;
        private System.Windows.Forms.Button btnBlock, btnUnblock, btnMakeAdmin, btnMakeUser, btnDelete, btnViewContacts, btnRefresh;
    }
}