using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TAS_Studio
{
    public partial class Form1 : Form
    {
        private bool isRecording = false;
        private List<(Keys key, long time)> recordedKeys = new List<(Keys, long)>();
        private Stopwatch stopwatch = new Stopwatch();

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
            foreach (var entry in recordedKeys)
            {
                sb.AppendLine($"Key: {entry.key}, Time Since Last Key: {entry.time} ms");
            }
            MessageBox.Show(sb.ToString(), "Recorded Keys");
        }
    }
}
