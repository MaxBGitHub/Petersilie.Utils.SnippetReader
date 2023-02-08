using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Petersilie.Utils.SnippetReader.Keyboard
{
    internal sealed class KeyboardHook : IDisposable
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);


        private class InternalMessageHook : NativeWindow, IDisposable
        {
            private static int WM_HOTKEY = 0x0312;


            public event EventHandler<KeyPressedEventArgs> KeyPressed;

            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);

                if (m.Msg == WM_HOTKEY)
                {
                    Keys key = (Keys)((m.LParam.ToInt32() >> 16) & 0xffff);
                    ModifierKeys modifier = (ModifierKeys)((m.LParam.ToInt32() & 0xffff));

                    KeyPressed?.Invoke(this, new KeyPressedEventArgs(modifier, key));
                }
            }


            public InternalMessageHook()
            {
                this.CreateHandle(new CreateParams());
            }

            ~InternalMessageHook() { Dispose(false); }
            public void Dispose() { Dispose(true); }
            private void Dispose(bool disposing)
            {
                if (disposing) {
                    GC.SuppressFinalize(this);
                }
                this.DestroyHandle();
            }
        }

        private InternalMessageHook _hookWindow = new InternalMessageHook();
        private int _currentId;


        public void RegisterHotKey(ModifierKeys modifier, Keys key)
        {
            _currentId = _currentId + 1;
            if (!(RegisterHotKey(_hookWindow.Handle, _currentId, (uint)modifier, (uint)key))) {
                int err = Marshal.GetLastWin32Error();
                System.Diagnostics.Debug.WriteLine(err);
                throw new InvalidOperationException("Could not register hot key.");
            }
        }


        public event EventHandler<KeyPressedEventArgs> KeyPressed;

        public KeyboardHook()
        {
            _hookWindow.KeyPressed += delegate (object sender, KeyPressedEventArgs args) {
                KeyPressed?.Invoke(this, args);
            };
        }


        ~KeyboardHook() { Dispose(false); }
        public void Dispose() { Dispose(true); }
        private void Dispose(bool disposing)
        {
            if (disposing) {
                GC.SuppressFinalize(this);
            }

            for (int i=_currentId; i>0; i--) {
                UnregisterHotKey(_hookWindow.Handle, i);
            }
            _hookWindow.Dispose();
        }
    }
}
