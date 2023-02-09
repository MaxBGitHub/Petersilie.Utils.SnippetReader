using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;

namespace Petersilie.Utils.SnippetReader
{
    public partial class OverlayWindow : Form
    {
        private Screen _screen;
        public Screen Screen
        {
            get { 
                return _screen; 
            }
        }

        // Start X coordinate of screen capture rectangle.
        // Set by MouseClick event.
        private float _xc;
        // Start Y coordinate of screen capture rectangle.
        // Set by MouseClick event.
        private float _yc;
        // Destination X set by MouseMove event.
        private float _x;
        // Destination Y set by MouseMove event.
        private float _y;

        // TRUE when left mouse button has been clicked.
        private bool _activated = false;
        // TRUE when mouse is moved and left mouse button is clicked.
        private bool _invokedPainting = false;
        // The priviously drawing rectangle which needs 
        // to be cleared. Graphics.Clear(Color) has a large
        // performance hit since the whole graphics area is
        // being redrawn / cleared.
        private RectangleF _previousCapture = RectangleF.Empty;
        private Rectangle _previoudBorder = Rectangle.Empty;


        private Bitmap CaptureScreen()
        {
            int x = (int)Math.Abs(_previousCapture.X);
            int y = (int)Math.Abs(_previousCapture.Y);
            int w = (int)Math.Abs(_previousCapture.Width);
            int h = (int)Math.Abs(_previousCapture.Height);

            try
            {
                Bitmap screenMap = new Bitmap(
                    w, h, PixelFormat.Format32bppArgb
                );

                Point ptTopLeft = this.PointToScreen(new Point(x, y));

                using (var destGr = Graphics.FromImage(screenMap))
                {
                    Size sz = new Size(w, h);
                    destGr.CopyFromScreen(ptTopLeft.X, ptTopLeft.Y, 0, 0, sz);
                }
                return screenMap;
            }
            catch {
                return null;
            }
            
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            if (!(_invokedPainting)) {
                base.OnPaint(e);
                return;
            }
            else {
                UpdateCaptureRect(e.Graphics);
            }
        }


        private void UpdateCaptureRect(Graphics g)
        {
            RectangleF rcCapture = new RectangleF(
                Math.Min(_xc, _x), Math.Min(_yc, _y), 
                Math.Abs(_xc - _x), Math.Abs(_yc - _y));

            Rectangle rcBorder = new Rectangle(
                (int)rcCapture.X, (int)rcCapture.Y,
                (int)rcCapture.Width, (int)rcCapture.Height
            );
            rcBorder.Inflate(1, 1);

            using (var brush = new SolidBrush(this.BackColor)) {
                g.FillRectangle(brush, _previousCapture);
                using (var pen = new Pen(brush, this.BorderWidth)) {
                    g.DrawRectangle(pen, _previoudBorder);
                }
            }

            using (var brush = new SolidBrush(this.TransparencyKey)) {
                g.FillRectangle(brush, rcCapture);
                _previousCapture = rcCapture;
            }

            using (var pen = new Pen(CaptureBorderColor, this.BorderWidth)) {
                g.DrawRectangle(pen, rcBorder);
                _previoudBorder = rcBorder;
            }
        }


        protected override void OnMouseMove(MouseEventArgs e)
        {
            //base.OnMouseMove(e);
            if (!(_activated)) {
                return;
            }
            _x = e.X;
            _y = e.Y;
            _invokedPainting = true;
            this.Refresh();
            _invokedPainting = false;
        }


        // Delegate for closing form. 
        private delegate void CloseDelegate();
        private void Shutdown()
        {
            CloseDelegate close = this.Close;
            if (InvokeRequired) {
                Invoke(close);
            }
            else {
                close();
            }
        }


        private event EventHandler<SnippetReadyEventArgs> onSnippetReady;
        public event EventHandler<SnippetReadyEventArgs> SnippetReady
        {
            add { 
                onSnippetReady += value; 
            }
            remove { 
                onSnippetReady -= value;
            }
        }

        private void OnSnippetReadyInternal(Bitmap snippet)
        {
            onSnippetReady?.Invoke(this, new SnippetReadyEventArgs(snippet));
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            // Do not close if screen capture is activated
            if (e.Button != MouseButtons.Left && _activated) {            
                return;
            }
            // Close if no screen capture is activated.
            else if (e.Button != MouseButtons.Left && !_activated) {
                Shutdown();
            }

            _activated = false;
            Bitmap result = CaptureScreen();
            if (result != null) {
                OnSnippetReadyInternal(result);
            }
            Shutdown();            
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left && !_activated) {
                Shutdown();
            }

            _activated = true;
            _xc = e.X;
            _yc = e.Y;
        }


        //
        // Closes the overlay window if the user pressed
        // the escape key.
        //
        protected override void OnKeyDown(KeyEventArgs e)
        {           
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Escape) {
                Shutdown();
            }
        }


        private Color _defaultTransparencyKey = Color.FromArgb(255, 88, 56, 174);

        [DefaultValue(typeof(Color), "0x5838AE")]
        public new Color TransparencyKey
        {
            get {
                return base.TransparencyKey;
            }
            set {
                base.TransparencyKey = value;
            }
        }


        public static readonly Color DefaultBorderColor = Color.FromArgb(255, 241, 241, 241);
        private Color _captureBorderColor = DefaultBorderColor;
        [DefaultValue(typeof(Color), "0xF1F1F1")]
        public Color CaptureBorderColor
        {
            get {
                return _captureBorderColor;
            }
            set {
                _captureBorderColor = value;
            }
        }


        private float _borderWidth = 2f;
        [DefaultValue(2f)]
        public float BorderWidth
        {
            get {
                return _borderWidth;
            }
            set {
                _borderWidth = value;
            }
        }


        public OverlayWindow(Screen screen, Color backColor, double opacity = 0.85)
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            this.TransparencyKey = _defaultTransparencyKey;

            this.BackColor  = backColor;
            this.ForeColor  = backColor;
            this.Opacity    = opacity;

            _screen = screen;

            this.Cursor = Cursors.Cross;

            this.SetBounds(
                screen.Bounds.X, 
                screen.Bounds.Y, 
                screen.Bounds.Width, 
                screen.Bounds.Height
            );

            this.Location = new Point(
                screen.Bounds.X, 
                screen.Bounds.Y
            );
        }
    }
}
