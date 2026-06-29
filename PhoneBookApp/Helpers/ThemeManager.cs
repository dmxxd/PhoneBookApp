using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PhoneBookApp.Helpers
{
    public static class ThemeManager
    {
        public static bool IsDark { get; private set; } = false;

        public static void ToggleTheme(Form form)
        {
            IsDark = !IsDark;
            ApplyTheme(form);
        }

        public static void ApplyTheme(Form form)
        {
            Color backColor = IsDark ? Color.FromArgb(32, 32, 32) : Color.FromArgb(240, 242, 245);
            Color panelBackColor = IsDark ? Color.FromArgb(45, 45, 48) : Color.White;
            Color foreColor = IsDark ? Color.White : Color.Black;
            Color secondaryFore = IsDark ? Color.LightGray : Color.DimGray;

            form.BackColor = backColor;
            foreach (Control c in GetAllControls(form))
            {
                if (c is Panel || c is Form)
                    c.BackColor = c == form ? backColor : panelBackColor;
                if (c is Label)
                    c.ForeColor = foreColor;
                if (c is TextBox)
                {
                    c.BackColor = IsDark ? Color.FromArgb(64, 64, 64) : Color.White;
                    c.ForeColor = foreColor;
                }
                if (c is Button btn && btn.FlatStyle == FlatStyle.Flat && btn.BackColor != Color.Transparent)
                {
                    // сохраняем оригинальные цвета для цветных кнопок
                    if (!IsDark && btn.BackColor == Color.FromArgb(64, 64, 64))
                        btn.BackColor = Color.FromArgb(52, 152, 219);
                    else if (IsDark && btn.BackColor == Color.FromArgb(52, 152, 219))
                        btn.BackColor = Color.FromArgb(64, 64, 64);
                }
                if (c is DataGridView dgv)
                {
                    dgv.BackgroundColor = IsDark ? Color.FromArgb(64, 64, 64) : Color.White;
                    dgv.GridColor = IsDark ? Color.FromArgb(80, 80, 80) : Color.FromArgb(236, 240, 241);
                    dgv.DefaultCellStyle.BackColor = IsDark ? Color.FromArgb(64, 64, 64) : Color.White;
                    dgv.DefaultCellStyle.ForeColor = foreColor;
                }
                if (c is FlowLayoutPanel flp)
                    flp.BackColor = panelBackColor;
            }
        }

        private static IEnumerable<Control> GetAllControls(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                yield return c;
                foreach (Control child in GetAllControls(c))
                    yield return child;
            }
        }
    }
}
