namespace PhoneBookApp.Forms
{
    partial class ContactForm
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
            this.lblFirstName = new System.Windows.Forms.Label();
            this.txtFirstName = new System.Windows.Forms.TextBox();
            this.lblLastName = new System.Windows.Forms.Label();
            this.txtLastName = new System.Windows.Forms.TextBox();
            this.lblPhone = new System.Windows.Forms.Label();
            this.txtPhone = new System.Windows.Forms.TextBox();
            this.lblEmail = new System.Windows.Forms.Label();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.lblAddress = new System.Windows.Forms.Label();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.lblNote = new System.Windows.Forms.Label();
            this.txtNote = new System.Windows.Forms.TextBox();
            this.lblTags = new System.Windows.Forms.Label();
            this.clbTags = new System.Windows.Forms.CheckedListBox();
            this.lblBirthday = new System.Windows.Forms.Label();
            this.dtpBirthday = new System.Windows.Forms.DateTimePicker();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();

            int y = 20;
            int labelX = 20, fieldX = 145, fieldWidth = 320;

            this.lblFirstName.AutoSize = true;
            this.lblFirstName.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblFirstName.ForeColor = System.Drawing.Color.Black;
            this.lblFirstName.Location = new System.Drawing.Point(labelX, y);
            this.lblFirstName.Text = "Имя *:";

            this.txtFirstName.Location = new System.Drawing.Point(fieldX, y - 3);
            this.txtFirstName.Size = new System.Drawing.Size(fieldWidth, 23);
            this.txtFirstName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            y += 40;

            this.lblLastName.AutoSize = true;
            this.lblLastName.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblLastName.ForeColor = System.Drawing.Color.Black;
            this.lblLastName.Location = new System.Drawing.Point(labelX, y);
            this.lblLastName.Text = "Фамилия:";
            this.txtLastName.Location = new System.Drawing.Point(fieldX, y - 3);
            this.txtLastName.Size = new System.Drawing.Size(fieldWidth, 23);
            this.txtLastName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            y += 40;

            this.lblPhone.AutoSize = true;
            this.lblPhone.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblPhone.ForeColor = System.Drawing.Color.Black;
            this.lblPhone.Location = new System.Drawing.Point(labelX, y);
            this.lblPhone.Text = "Телефон:";
            this.txtPhone.Location = new System.Drawing.Point(fieldX, y - 3);
            this.txtPhone.Size = new System.Drawing.Size(fieldWidth, 23);
            this.txtPhone.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            y += 40;

            this.lblEmail.AutoSize = true;
            this.lblEmail.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblEmail.ForeColor = System.Drawing.Color.Black;
            this.lblEmail.Location = new System.Drawing.Point(labelX, y);
            this.lblEmail.Text = "Email:";
            this.txtEmail.Location = new System.Drawing.Point(fieldX, y - 3);
            this.txtEmail.Size = new System.Drawing.Size(fieldWidth, 23);
            this.txtEmail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            y += 40;

            this.lblAddress.AutoSize = true;
            this.lblAddress.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblAddress.ForeColor = System.Drawing.Color.Black;
            this.lblAddress.Location = new System.Drawing.Point(labelX, y);
            this.lblAddress.Text = "Адрес:";
            this.txtAddress.Location = new System.Drawing.Point(fieldX, y - 3);
            this.txtAddress.Size = new System.Drawing.Size(fieldWidth, 23);
            this.txtAddress.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            y += 40;

            this.lblNote.AutoSize = true;
            this.lblNote.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblNote.ForeColor = System.Drawing.Color.Black;
            this.lblNote.Location = new System.Drawing.Point(labelX, y);
            this.lblNote.Text = "Заметка:";
            this.txtNote.Location = new System.Drawing.Point(fieldX, y - 3);
            this.txtNote.Size = new System.Drawing.Size(fieldWidth, 60);
            this.txtNote.Multiline = true;
            this.txtNote.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            y += 75;

            this.lblTags.AutoSize = true;
            this.lblTags.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblTags.ForeColor = System.Drawing.Color.Black;
            this.lblTags.Location = new System.Drawing.Point(labelX, y);
            this.lblTags.Text = "Теги:";
            this.clbTags.Location = new System.Drawing.Point(fieldX, y - 3);
            this.clbTags.Size = new System.Drawing.Size(200, 80);
            this.clbTags.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            y += 95;

            this.lblBirthday.AutoSize = true;
            this.lblBirthday.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblBirthday.ForeColor = System.Drawing.Color.Black;
            this.lblBirthday.Location = new System.Drawing.Point(labelX, y);
            this.lblBirthday.Text = "День рождения:";

            // ИЗМЕНЕНО: Координата X сдвинута на 160 (чуть левее, чем 180, но сохраняя отступ от надписи)
            this.dtpBirthday.Location = new System.Drawing.Point(160, y - 3);
            this.dtpBirthday.Size = new System.Drawing.Size(150, 23);
            this.dtpBirthday.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpBirthday.CustomFormat = "dd.MM.yyyy";
            y += 40;

            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.BackColor = System.Drawing.Color.White;
            this.btnSave.ForeColor = System.Drawing.Color.Black;
            this.btnSave.Location = new System.Drawing.Point(fieldX, y);
            this.btnSave.Size = new System.Drawing.Size(130, 35);
            this.btnSave.Text = "Сохранить";
            this.btnSave.Cursor = System.Windows.Forms.Cursors.Hand;

            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.BackColor = System.Drawing.Color.White;
            this.btnCancel.ForeColor = System.Drawing.Color.Black;
            this.btnCancel.Location = new System.Drawing.Point(fieldX + 150, y);
            this.btnCancel.Size = new System.Drawing.Size(130, 35);
            this.btnCancel.Text = "Отмена";
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;

            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 550);
            this.Controls.Add(this.lblFirstName);
            this.Controls.Add(this.txtFirstName);
            this.Controls.Add(this.lblLastName);
            this.Controls.Add(this.txtLastName);
            this.Controls.Add(this.lblPhone);
            this.Controls.Add(this.txtPhone);
            this.Controls.Add(this.lblEmail);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.lblAddress);
            this.Controls.Add(this.txtAddress);
            this.Controls.Add(this.lblNote);
            this.Controls.Add(this.txtNote);
            this.Controls.Add(this.lblTags);
            this.Controls.Add(this.clbTags);
            this.Controls.Add(this.lblBirthday);
            this.Controls.Add(this.dtpBirthday);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Контакт";
            this.BackColor = System.Drawing.Color.White;
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblFirstName, lblLastName, lblPhone, lblEmail, lblAddress, lblNote, lblBirthday, lblTags;
        private System.Windows.Forms.TextBox txtFirstName, txtLastName, txtPhone, txtEmail, txtAddress, txtNote;
        private System.Windows.Forms.DateTimePicker dtpBirthday;
        private System.Windows.Forms.CheckedListBox clbTags;
        private System.Windows.Forms.Button btnSave, btnCancel;
    }
}