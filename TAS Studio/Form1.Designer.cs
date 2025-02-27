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
            vScrollBar1 = new VScrollBar();
            playback = new Button();
            save = new Button();
            load = new Button();
            InputPanel.SuspendLayout();
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
            InputPanel.Controls.Add(vScrollBar1);
            InputPanel.Location = new Point(12, 41);
            InputPanel.Name = "InputPanel";
            InputPanel.Size = new Size(371, 397);
            InputPanel.TabIndex = 1;
            // 
            // vScrollBar1
            // 
            vScrollBar1.Location = new Point(354, 0);
            vScrollBar1.Name = "vScrollBar1";
            vScrollBar1.Size = new Size(17, 426);
            vScrollBar1.TabIndex = 0;
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
            // save
            // 
            save.Location = new Point(12, 12);
            save.Name = "save";
            save.Size = new Size(75, 23);
            save.TabIndex = 3;
            save.Text = "Save to file";
            save.UseVisualStyleBackColor = true;
            save.Click += save_Click;
            // 
            // load
            // 
            load.Location = new Point(93, 12);
            load.Name = "load";
            load.Size = new Size(91, 23);
            load.TabIndex = 4;
            load.Text = "Load from file";
            load.UseVisualStyleBackColor = true;
            load.Click += load_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(541, 450);
            Controls.Add(load);
            Controls.Add(save);
            Controls.Add(playback);
            Controls.Add(InputPanel);
            Controls.Add(record);
            Name = "Form1";
            Text = "Form1";
            InputPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Button record;
        private Panel InputPanel;
        private Button playback;
        private VScrollBar vScrollBar1;
        private Button save;
        private Button load;
    }
}
