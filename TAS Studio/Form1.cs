using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace TAS_Studio
{
    public partial class Form1 : Form
    {
        private bool isRecording = false;
        private List<(Keys key, long time)> recordedKeys = new List<(Keys, long)>();
        private Stopwatch stopwatch = new Stopwatch();

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;
        }

        private void record_Click(object sender, EventArgs e)
        {
            if (!isRecording)
            {
                recordedKeys.Clear();
                stopwatch.Restart();
                isRecording = true;
                record.Text = "Stop Recording";
            }
            else
            {
                isRecording = false;
                stopwatch.Stop();
                record.Text = "Record";
                ShowRecordedData();
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (isRecording)
            {
                recordedKeys.Add((e.KeyCode, stopwatch.ElapsedMilliseconds));
                stopwatch.Restart();
            }
        }

        private void ShowRecordedData()
        {
            StringBuilder sb = new StringBuilder();
            int yOffset = 0;
            foreach (var entry in recordedKeys)
            {
                Label inpt = new Label
                {
                    Text = $"Key: {entry.key}, Time Since Last Key: {entry.time} ms",
                    AutoSize = true,
                    Location = new System.Drawing.Point(5, yOffset) // Position label
                };
                sb.AppendLine($"Key: {entry.key}, Time Since Last Key: {entry.time} ms");
                InputPanel.Controls.Add(inpt);
                yOffset += inpt.Height + 5; // Move down for the next label
            }
            MessageBox.Show(sb.ToString(), "Recorded Keys");
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void Playback_Click(object sender, EventArgs e)
        {
            if (recordedKeys.Count == 0)
                return;

            new Thread(() =>
            {
                foreach (var entry in recordedKeys)
                {
                    Thread.Sleep((int)entry.time);
                    keybd_event((byte)entry.key, 0, 0, 0);
                    keybd_event((byte)entry.key, 0, 2, 0);
                }
            }).Start();
        }
    }
}
