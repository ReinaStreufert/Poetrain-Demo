using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace poetrain.UI.Forms
{
    public partial class RTFTests : Form
    {
        public RTFTests()
        {
            InitializeComponent();
        }

        private void rtfCodeBox_TextChanged(object sender, EventArgs e)
        {
            rtfRenderBox.SelectAll();
            try
            {
                rtfRenderBox.Rtf = rtfCodeBox.Text;
            } catch
            {
                rtfRenderBox.Text = "N/A";
            }
        }

        private void RTFTests_SizeChanged(object sender, EventArgs e)
        {
            rtfCodeBox.Size = new Size(400, ClientSize.Height);
            rtfRenderBox.Size = new Size(ClientSize.Width - 400, ClientSize.Height);
        }
    }
}
