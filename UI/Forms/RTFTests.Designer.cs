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
            this._PhraseInputBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this._MatchSyllCheckbox = new System.Windows.Forms.CheckBox();
            this._MultiWordCheckbox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // _PhraseInputBox
            // 
            this._PhraseInputBox.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._PhraseInputBox.Location = new System.Drawing.Point(12, 44);
            this._PhraseInputBox.Name = "_PhraseInputBox";
            this._PhraseInputBox.Size = new System.Drawing.Size(1028, 50);
            this._PhraseInputBox.TabIndex = 0;
            this._PhraseInputBox.TextChanged += new System.EventHandler(this._PhraseInputBox_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(270, 32);
            this.label1.TabIndex = 1;
            this.label1.Text = "Enter a word or phrase...";
            // 
            // _MatchSyllCheckbox
            // 
            this._MatchSyllCheckbox.AutoSize = true;
            this._MatchSyllCheckbox.Checked = true;
            this._MatchSyllCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this._MatchSyllCheckbox.Location = new System.Drawing.Point(12, 100);
            this._MatchSyllCheckbox.Name = "_MatchSyllCheckbox";
            this._MatchSyllCheckbox.Size = new System.Drawing.Size(267, 36);
            this._MatchSyllCheckbox.TabIndex = 2;
            this._MatchSyllCheckbox.Text = "Match syllable count";
            this._MatchSyllCheckbox.UseVisualStyleBackColor = true;
            this._MatchSyllCheckbox.CheckedChanged += new System.EventHandler(this._MatchSyllCheckbox_CheckedChanged);
            // 
            // _MultiWordCheckbox
            // 
            this._MultiWordCheckbox.AutoSize = true;
            this._MultiWordCheckbox.Location = new System.Drawing.Point(285, 100);
            this._MultiWordCheckbox.Name = "_MultiWordCheckbox";
            this._MultiWordCheckbox.Size = new System.Drawing.Size(299, 36);
            this._MultiWordCheckbox.TabIndex = 3;
            this._MultiWordCheckbox.Text = "Multi-word suggestions";
            this._MultiWordCheckbox.UseVisualStyleBackColor = true;
            this._MultiWordCheckbox.CheckedChanged += new System.EventHandler(this._MultiWordCheckbox_CheckedChanged);
            // 
            // RTFTests
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(1052, 496);
            this.Controls.Add(this._MultiWordCheckbox);
            this.Controls.Add(this._MatchSyllCheckbox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._PhraseInputBox);
            this.Name = "RTFTests";
            this.Text = "RTFTests";
            this.SizeChanged += new System.EventHandler(this.RTFTests_SizeChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox _PhraseInputBox;
        private Label label1;
        private CheckBox _MatchSyllCheckbox;
        private CheckBox _MultiWordCheckbox;
    }
}