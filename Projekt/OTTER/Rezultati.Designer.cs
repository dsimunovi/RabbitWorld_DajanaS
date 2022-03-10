
namespace OTTER
{
    partial class Rezultati
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
            this.button1 = new System.Windows.Forms.Button();
            this.HighScore = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(272, 577);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(213, 36);
            this.button1.TabIndex = 0;
            this.button1.Text = "Povratak na izbornik";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // HighScore
            // 
            this.HighScore.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.HighScore.FormattingEnabled = true;
            this.HighScore.ItemHeight = 16;
            this.HighScore.Location = new System.Drawing.Point(12, 33);
            this.HighScore.Name = "HighScore";
            this.HighScore.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.HighScore.Size = new System.Drawing.Size(776, 514);
            this.HighScore.TabIndex = 1;
            this.HighScore.TabStop = false;
            this.HighScore.UseTabStops = false;
            // 
            // Rezultati
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(800, 625);
            this.Controls.Add(this.HighScore);
            this.Controls.Add(this.button1);
            this.Name = "Rezultati";
            this.Text = "Rezultati";
            this.Load += new System.EventHandler(this.Rezultati_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListBox HighScore;
    }
}