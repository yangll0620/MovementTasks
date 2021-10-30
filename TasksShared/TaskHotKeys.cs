using System.Windows.Input;
using System.Runtime.InteropServices;
using System;

namespace TasksShared
{
    public class TaskHotKeys
    {
        // Variables for hot Keys
        public static readonly int WM_HOTKEY = 0x312;
        public static readonly int HotKeyId_Start = 0x3000;
        public static readonly int HotKeyId_Stop = 0x3001;
        public static readonly int HotKeyId_Pause = 0x4000;
        public static readonly int HotKeyId_Resume = 0x4001;
        public static Key key_Start = Key.S;
        public static Key key_Stop = Key.Space;
        public static Key key_Pause = Key.P;
        public static Key key_Resume = Key.R;


        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr windowHandle, int hotkeyId, uint modifierKeys, uint virtualKey);

        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr windowHandle, int hotkeyId);
    }
}
