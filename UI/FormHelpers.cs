using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.UI
{
    public static class FormHelpers
    {
        public static int GetFontHeight(float textSize)
        {
            var font = CreateFont(textSize);
            return TextRenderer.MeasureText("j", font).Height;
        }

        public static Label CreateLabel(float textSize)
        {
            var label = new Label();
            label.AutoSize = true;
            label.Font = CreateFont(textSize);
            return label;
        }

        public static TextBox CreateTextBox()
        {
            var textBox = new TextBox();
            textBox.Font = CreateFont(12f);
            textBox.ForeColor = Color.Black;
            return textBox;
        }

        private static Font CreateFont(float textSize) => new Font("Segoe UI Light", textSize);
    }
}
