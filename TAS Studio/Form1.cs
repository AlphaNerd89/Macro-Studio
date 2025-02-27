using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace TAS_Studio
{
    public partial class Form1 : Form
    {
        private bool isRecording = false;
        private List<(Keys key, long time)> recordedKeys = new List<(Keys, long)>();
        private Stopwatch stopwatch = new Stopwatch();

        private List<(Keys key, long time)> savedState = new List<(Keys, long)>();
        private bool hasSavedState = false;

        private GlobalKeyboardHook _globalKeyboardHook;

        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;
            _globalKeyboardHook = new GlobalKeyboardHook();
            _globalKeyboardHook.KeyDown += GlobalKeyDown;
        }

        private void GlobalKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                SaveState();
            }
            else if (e.KeyCode == Keys.F2)
            {
                RestoreState();
            }
            else if (isRecording)
            {
                recordedKeys.Add((e.KeyCode, stopwatch.ElapsedMilliseconds));
                stopwatch.Restart();
            }
            else if (e.KeyCode == Keys.F6)
            {
                PlaybackKeys();
            }
            else if (e.KeyCode == Keys.F7)
            {
                recording();
            }
        }

        private void record_Click(object sender, EventArgs e)
        {
            recording();
        }

        private void recording()
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

        private void SaveState()
        {
            if (isRecording)
            {
                savedState = new List<(Keys, long)>(recordedKeys);
                hasSavedState = true;
                MessageBox.Show("State saved!", "Save State", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void RestoreState()
        {
            if (hasSavedState)
            {
                recordedKeys = new List<(Keys, long)>(savedState);
                ShowRecordedData();
                MessageBox.Show("State restored!", "Restore State", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ShowRecordedData()
        {
            InputPanel.Controls.Clear();
            int yOffset = 0;
            for (int i = 0; i < recordedKeys.Count; i++)
            {
                int index = i;
                Label inpt = new Label
                {
                    Text = $"Key: {recordedKeys[i].key}, Time Since Last Key: {recordedKeys[i].time} ms",
                    AutoSize = true,
                    Location = new System.Drawing.Point(5, yOffset)
                };
                inpt.DoubleClick += (s, e) => EditRecordedData(index, inpt);
                InputPanel.Controls.Add(inpt);
                yOffset += inpt.Height + 5;
            }
        }

        private void playback_Click(object sender, EventArgs e)
        {
            PlaybackKeys();
        }

        private void EditRecordedData(int index, Label label)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Enter new key and time (format: Key, Time)", "Edit Input", $"{recordedKeys[index].key}, {recordedKeys[index].time}");
            if (!string.IsNullOrWhiteSpace(input))
            {
                var parts = input.Split(',');
                if (parts.Length == 2 && Enum.TryParse(parts[0].Trim(), out Keys newKey) && long.TryParse(parts[1].Trim(), out long newTime))
                {
                    recordedKeys[index] = (newKey, newTime);
                    label.Text = $"Key: {newKey}, Time Since Last Key: {newTime} ms";
                }
                else
                {
                    MessageBox.Show("Invalid input format. Use: Key, Time", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void PlaybackKeys()
        {
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

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _globalKeyboardHook.Dispose();
        }

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);
    }

    // =============== Global Keyboard Hook Class ===============
    public class GlobalKeyboardHook : IDisposable
    {
        private LowLevelKeyboardProc _proc;
        private IntPtr _hookID = IntPtr.Zero;
        public event KeyEventHandler KeyDown;

        public GlobalKeyboardHook()
        {
            _proc = HookCallback;
            _hookID = SetHook(_proc);
        }

        public void Dispose()
        {
            UnhookWindowsHookEx(_hookID);
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
            {
                int vkCode = Marshal.ReadInt32(lParam);
                KeyDown?.Invoke(this, new KeyEventArgs((Keys)vkCode));
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
    }
}