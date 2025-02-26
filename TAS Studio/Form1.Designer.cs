namespace TAS_Studio
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            record = new Button();
            InputPanel = new Panel();
            label1 = new Label();
            Playback = new Button();
            SuspendLayout();
            // 
            // record
            // 
            record.Location = new Point(436, 12);
            record.Name = "record";
            record.Size = new Size(93, 23);
            record.TabIndex = 0;
            record.Text = "Start recording";
            record.UseVisualStyleBackColor = true;
            record.Click += record_Click;
            // 
            // InputPanel
            // 
            InputPanel.BackColor = SystemColors.ControlLight;
            InputPanel.Location = new Point(12, 12);
            InputPanel.Name = "InputPanel";
            InputPanel.Size = new Size(371, 426);
            InputPanel.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(436, 174);
            label1.Name = "label1";
            label1.Size = new Size(38, 15);
            label1.TabIndex = 0;
            label1.Text = "label1";
            label1.Click += label1_Click;
            // 
            // Playback
            // 
            Playback.Location = new Point(436, 41);
            Playback.Name = "Playback";
            Playback.Size = new Size(93, 23);
            Playback.TabIndex = 2;
            Playback.Text = "Playback";
            Playback.UseVisualStyleBackColor = true;
            Playback.Click += Playback_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(541, 450);
            Controls.Add(Playback);
            Controls.Add(label1);
            Controls.Add(InputPanel);
            Controls.Add(record);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button record;
        private Panel InputPanel;
        private Label label1;
        private Button Playback;
    }
}
