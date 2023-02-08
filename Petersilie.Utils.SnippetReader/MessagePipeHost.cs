using Petersilie.Utils.SnippetReader.Keyboard;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Petersilie.Utils.SnippetReader
{
    public partial class MessagePipeHost : Form
    {
        private List<OverlayWindow> _overlayWindows = new List<OverlayWindow>();
        private static object _lock = new object();


        private void Overlay_SnippetReady(object sender, SnippetReadyEventArgs e)
        {
            var test = OCR.ImageToText.GetText(e.Snippet);
            if (test == null) {
                test = string.Empty;
            }
            Clipboard.SetText(test);
            System.Diagnostics.Debug.WriteLine(test);
        }


        private void ShowOverlays()
        {
            foreach (Screen screen in Screen.AllScreens)
            {
                var overlay = new OverlayWindow(screen, Color.FromArgb(0, 0, 0), 0.65);
                overlay.FormClosed += OnOverlayClosed;
                overlay.SnippetReady += Overlay_SnippetReady;
                _overlayWindows.Add(overlay);
                overlay.Show();
            }
        }


        private void OnOverlayClosed(object sender, EventArgs e)
        {
            lock (_lock) {
                foreach (var window in _overlayWindows.ToArray()) {
                    window.Close();
                    window.Dispose();
                }
                _overlayWindows.Clear();
            }            
        }


        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            ShowOverlays();
        }

        protected override void OnLoad(EventArgs e)
        {
            Visible = false;
            ShowInTaskbar = false;
            base.OnLoad(e);
        }


        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            ShowOverlays();
        }        

        
        public MessagePipeHost()
        {
            InitializeComponent();
        }
    }
}
