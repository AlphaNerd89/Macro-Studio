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
            playback = new Button();
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
            // playback
            // 
            playback.Location = new Point(436, 41);
            playback.Name = "playback";
            playback.Size = new Size(93, 23);
            playback.TabIndex = 2;
            playback.Text = "Playback";
            playback.UseVisualStyleBackColor = true;
            playback.Click += playback_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(541, 450);
            Controls.Add(playback);
            Controls.Add(InputPanel);
            Controls.Add(record);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
        }

        #endregion

        private Button record;
        private Panel InputPanel;
        private Button playback;
    }
}
