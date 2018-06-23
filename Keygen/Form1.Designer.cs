namespace Keygen
{
    partial class Form1
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
            this.label1 = new System.Windows.Forms.Label();
            this.serialNumberInputControl1 = new Setups.Common.Controls.SerialNumberInputControl();
            this.serialNumberInputControl2 = new Setups.Common.Controls.SerialNumberInputControl();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Request code";
            // 
            // serialNumberInputControl1
            // 
            this.serialNumberInputControl1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.serialNumberInputControl1.Location = new System.Drawing.Point(114, 12);
            this.serialNumberInputControl1.Name = "serialNumberInputControl1";
            this.serialNumberInputControl1.Size = new System.Drawing.Size(487, 30);
            this.serialNumberInputControl1.TabIndex = 1;
            // 
            // serialNumberInputControl2
            // 
            this.serialNumberInputControl2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.serialNumberInputControl2.Location = new System.Drawing.Point(114, 48);
            this.serialNumberInputControl2.Name = "serialNumberInputControl2";
            this.serialNumberInputControl2.Size = new System.Drawing.Size(487, 30);
            this.serialNumberInputControl2.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "Activation key";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(607, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(87, 27);
            this.button1.TabIndex = 4;
            this.button1.Text = "Paste";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.PasteFromClipboardHandler);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(607, 47);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(87, 27);
            this.button2.TabIndex = 5;
            this.button2.Text = "Copy";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.CopyToClipboardHandler);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(300, 95);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(119, 48);
            this.button3.TabIndex = 6;
            this.button3.Text = "Generate";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.GenerateActivationCodeHahdler);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(709, 159);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.serialNumberInputControl2);
            this.Controls.Add(this.serialNumberInputControl1);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "Key generator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private Setups.Common.Controls.SerialNumberInputControl serialNumberInputControl1;
        private Setups.Common.Controls.SerialNumberInputControl serialNumberInputControl2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
    }
}

