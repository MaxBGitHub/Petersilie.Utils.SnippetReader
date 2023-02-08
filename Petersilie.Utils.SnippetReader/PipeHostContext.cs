using Petersilie.Utils.SnippetReader.Keyboard;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Petersilie.Utils.SnippetReader
{
    internal class PipeHostContext : ApplicationContext
    {
        private void CleanUp()
        {
            if (_trayIcon!= null) {
                _trayIcon.Visible = false;
            }
            _trayIcon?.Dispose();
            _trayIcon = null;
        }


        private void Exit(object sender, EventArgs e)
        {
            CleanUp();
            Application.Exit();
        }


        private static MessagePipeHost _host = new MessagePipeHost();
        private void TakeScreenshot(object sender, EventArgs e)
        {
            if (_host.Visible) {
                _host.Activate();
            }
            else {
                _host.Show();
            }
        }


        AboutWindow _aboutWindow = new AboutWindow();
        private void ShowAbout(object sender, EventArgs e)
        {
            if (_aboutWindow.Visible) {
                _aboutWindow.Activate();
            }
            else {
                _aboutWindow.Show();
            }
        }


        private void OnHotkey_Pressed(object sender, KeyPressedEventArgs e)
        {
            TakeScreenshot(this, EventArgs.Empty);
        }


        KeyboardHook _hook = new KeyboardHook();

        private void EnableHotKey()
        {
            _hook.KeyPressed += OnHotkey_Pressed;
            _hook.RegisterHotKey(Keyboard.ModifierKeys.Alt | Keyboard.ModifierKeys.Control, Keys.D);
        }


        private NotifyIcon _trayIcon = new NotifyIcon();
        private MenuItem _screenShotItem;
        private MenuItem _configItem;
        private MenuItem _aboutItem;
        private MenuItem _exitItem;


        private void InitializeMenuItems()
        {
            _screenShotItem = new MenuItem("Take screenshot", TakeScreenshot);
            _configItem = new MenuItem("Configuration");
            _aboutItem = new MenuItem("About", ShowAbout);
            _exitItem = new MenuItem("Exit", Exit);
        }


        private void InitializeTrayIcon()
        {
            MenuItem[] items = new MenuItem[] {
                _screenShotItem,
                _configItem,
                _aboutItem,
                _exitItem
            };

            _trayIcon.Text = "SnippetReader - Screenshot to text";
            _trayIcon.Icon = Icon.FromHandle(Properties.Resources.app_icon_64px.GetHicon());
            _trayIcon.ContextMenu = new ContextMenu(items);
            _trayIcon.Visible = true;
        }

        public PipeHostContext() 
            : base()
        {
            InitializeMenuItems();
            InitializeTrayIcon();
            EnableHotKey();
        }


        protected override void Dispose(bool disposing)
        {            
            _hook?.Dispose();
            CleanUp();
            base.Dispose(disposing);
        }


        protected override void ExitThreadCore()
        {
            _hook?.Dispose();
            CleanUp();
            base.ExitThreadCore();            
        }


        protected override void OnMainFormClosed(object sender, EventArgs e)
        {
            _hook?.Dispose();
            CleanUp();
            base.OnMainFormClosed(sender, e);
        }
    }
}
