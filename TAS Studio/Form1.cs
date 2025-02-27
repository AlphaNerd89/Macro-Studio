using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        private List<(int x, int y, MouseButtons button, long time)> recordedMouseClicks = new List<(int, int, MouseButtons, long)>();
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
            if (e.KeyCode == Keys.F2)
            {
                SaveState();
            }
            else if (e.KeyCode == Keys.F3)
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
                PlaybackInputs();
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
                stopwatch.Restart();  // Start the stopwatch from the current time
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
            }
        }

        private void RestoreState()
        {
            if (hasSavedState)
            {
                recordedKeys = new List<(Keys, long)>(savedState);
                ShowRecordedData();
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

            for (int i = 0; i < recordedMouseClicks.Count; i++)
            {
                int index = i;
                Label inpt = new Label
                {
                    Text = $"Mouse: ({recordedMouseClicks[i].x}, {recordedMouseClicks[i].y}), Button: {recordedMouseClicks[i].button}, Time Since Last Click: {recordedMouseClicks[i].time} ms",
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
            PlaybackInputs();
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

        private void PlaybackInputs()
        {
            new Thread(() =>
            {
                foreach (var entry in recordedKeys)
                {
                    Thread.Sleep((int)entry.time);  // Cast long to int
                    SimulateKeyPress((ushort)entry.key);
                }

                foreach (var mouseEntry in recordedMouseClicks)
                {
                    Thread.Sleep((int)mouseEntry.time);  // Cast long to int
                    SimulateMouseClick(mouseEntry.x, mouseEntry.y, mouseEntry.button);
                }
            }).Start();
        }


        private void SimulateKeyPress(ushort virtualKeyCode)
        {
            INPUT inputDown = new INPUT
            {
                type = INPUT_KEYBOARD,
                union = new InputUnion
                {
                    ki = new KEYBDINPUT
                    {
                        wVk = virtualKeyCode,
                        wScan = 0,
                        dwFlags = KEYEVENTF_KEYDOWN,
                        time = 0,
                        dwExtraInfo = IntPtr.Zero
                    }
                }
            };

            INPUT inputUp = new INPUT
            {
                type = INPUT_KEYBOARD,
                union = new InputUnion
                {
                    ki = new KEYBDINPUT
                    {
                        wVk = virtualKeyCode,
                        wScan = 0,
                        dwFlags = KEYEVENTF_KEYUP,
                        time = 0,
                        dwExtraInfo = IntPtr.Zero
                    }
                }
            };

            INPUT[] inputs = new INPUT[] { inputDown, inputUp };
            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));  // Cast int to uint
        }

        private void SimulateMouseClick(int x, int y, MouseButtons button)
        {
            SetCursorPos(x, y);

            INPUT inputDown = new INPUT
            {
                type = INPUT_MOUSE,
                union = new InputUnion
                {
                    mi = new MOUSEINPUT
                    {
                        dx = x,
                        dy = y,
                        dwFlags = (uint)(button == MouseButtons.Left ? MOUSEEVENTF_LEFTDOWN : MOUSEEVENTF_RIGHTDOWN),  // Cast to uint
                        time = 0,
                        dwExtraInfo = IntPtr.Zero
                    }
                }
            };

            INPUT inputUp = new INPUT
            {
                type = INPUT_MOUSE,
                union = new InputUnion
                {
                    mi = new MOUSEINPUT
                    {
                        dx = x,
                        dy = y,
                        dwFlags = (uint)(button == MouseButtons.Left ? MOUSEEVENTF_LEFTUP : MOUSEEVENTF_RIGHTUP),  // Cast to uint
                        time = 0,
                        dwExtraInfo = IntPtr.Zero
                    }
                }
            };

            INPUT[] inputs = new INPUT[] { inputDown, inputUp };
            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));  // Cast int to uint
        }


        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int x, int y);

        // SendInput P/Invoke and related structures
        const int INPUT_KEYBOARD = 1;
        const int INPUT_MOUSE = 0;
        const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        const int MOUSEEVENTF_LEFTUP = 0x0004;
        const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        const int MOUSEEVENTF_RIGHTUP = 0x0010;
        const int KEYEVENTF_KEYDOWN = 0x0000;
        const int KEYEVENTF_KEYUP = 0x0002;

        [StructLayout(LayoutKind.Sequential)]
        public struct INPUT
        {
            public uint type;
            public InputUnion union;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct InputUnion
        {
            [FieldOffset(0)] public MOUSEINPUT mi;
            [FieldOffset(0)] public KEYBDINPUT ki;
            [FieldOffset(0)] public HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HARDWAREINPUT
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint SendInput(uint nInputs, [In] INPUT[] pInputs, int cbSize);

        // Saving and Loading methods

        private void save_Click(object sender, EventArgs e)
        {
            SaveRecordedInputs();
        }

        private void load_Click(object sender, EventArgs e)
        {
            LoadRecordedInputs();
        }

        private void SaveRecordedInputs()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;
                StringBuilder sb = new StringBuilder();

                foreach (var entry in recordedKeys)
                {
                    sb.AppendLine($"{entry.key},{entry.time}");
                }

                foreach (var mouseEntry in recordedMouseClicks)
                {
                    sb.AppendLine($"{mouseEntry.x},{mouseEntry.y},{mouseEntry.button},{mouseEntry.time}");
                }

                File.WriteAllText(filePath, sb.ToString());
            }
        }

        private void LoadRecordedInputs()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                var lines = File.ReadAllLines(filePath);
                recordedKeys.Clear();
                recordedMouseClicks.Clear();

                foreach (var line in lines)
                {
                    var parts = line.Split(',');
                    if (parts.Length == 2 && Enum.TryParse(parts[0].Trim(), out Keys key) && long.TryParse(parts[1].Trim(), out long time))
                    {
                        recordedKeys.Add((key, time));
                    }
                    else if (parts.Length == 4 && int.TryParse(parts[0].Trim(), out int x) &&
                        int.TryParse(parts[1].Trim(), out int y) && Enum.TryParse(parts[2].Trim(), out MouseButtons button) &&
                        long.TryParse(parts[3].Trim(), out long mouseTime))
                    {
                        recordedMouseClicks.Add((x, y, button, mouseTime));
                    }
                }

                ShowRecordedData();
            }
        }

        private void reset_Click(object sender, EventArgs e)
        {
            
        }
    }

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
