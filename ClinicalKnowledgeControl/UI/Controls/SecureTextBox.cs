using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClinicalKnowledgeControl.UI.Controls
{
    public class SecureTextBox : TextBox
    {
        public SecureTextBox()
        {
            this.ShortcutsEnabled = false;
            this.ReadOnly = true;
            this.BackColor = System.Drawing.SystemColors.Window;
        }

        protected override void WndProc(ref Message m)
        {
            // Блокируем правый клик
            if (m.Msg == 0x0204) // WM_CONTEXTMENU
                return;
            base.WndProc(ref m);
        }
    }
}
