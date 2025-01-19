using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.UI
{
    public static class FormHelpers
    {
        public static Label CreateLabel(float textSize)
        {
            var label = new Label();
            label.AutoSize = true;
            label.Font = new Font("Segoe UI Light", textSize);
            return label;
        }

        public static TextBox CreateTextBox()
        {
            var textBox = new TextBox();
            textBox.Font = new Font("Segoe UI Light", 12f);
            textBox.ForeColor = Color.Black;
            return textBox;
        }
    }
}
