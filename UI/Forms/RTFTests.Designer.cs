namespace poetrain.UI.Forms
{
    partial class RTFTests
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.rtfCodeBox = new System.Windows.Forms.TextBox();
            this.rtfRenderBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // rtfCodeBox
            // 
            this.rtfCodeBox.Font = new System.Drawing.Font("Consolas", 10.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.rtfCodeBox.Location = new System.Drawing.Point(0, 0);
            this.rtfCodeBox.Multiline = true;
            this.rtfCodeBox.Name = "rtfCodeBox";
            this.rtfCodeBox.Size = new System.Drawing.Size(400, 500);
            this.rtfCodeBox.TabIndex = 0;
            this.rtfCodeBox.TextChanged += new System.EventHandler(this.rtfCodeBox_TextChanged);
            // 
            // rtfRenderBox
            // 
            this.rtfRenderBox.Location = new System.Drawing.Point(400, 0);
            this.rtfRenderBox.Name = "rtfRenderBox";
            this.rtfRenderBox.ReadOnly = true;
            this.rtfRenderBox.Size = new System.Drawing.Size(654, 500);
            this.rtfRenderBox.TabIndex = 1;
            this.rtfRenderBox.Text = "";
            // 
            // RTFTests
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1052, 496);
            this.Controls.Add(this.rtfRenderBox);
            this.Controls.Add(this.rtfCodeBox);
            this.Name = "RTFTests";
            this.Text = "RTFTests";
            this.SizeChanged += new System.EventHandler(this.RTFTests_SizeChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox rtfCodeBox;
        private RichTextBox rtfRenderBox;
    }
}