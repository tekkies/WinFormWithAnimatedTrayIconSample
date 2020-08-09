using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WinFormWithAnimatedTrayIconSample
{
    public partial class Form1 : Form
    {
        private const int IconWidth = 64;
        private readonly TerminateManager _terminateManager;

        public Form1()
        {
            InitializeComponent();
            _terminateManager = new TerminateManager(this, notifyIcon1);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            contextMenuStrip1.Items.Clear();
            contextMenuStrip1.Items.Add("&Restore");
            contextMenuStrip1.Items.Add("-");
            contextMenuStrip1.Items.Add("E&xit");
            timerAnimation.Start();
            DrawIcon();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _terminateManager.FormClosing(sender, e);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == WindowState)
                Hide();
        }

        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == "&Restore")
            {
                Show();
                WindowState = FormWindowState.Normal;
            }

            else if (e.ClickedItem.Text == "E&xit")
            {
                Close();
            }
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void minimizeButton_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void timerAnimation_Tick(object sender, EventArgs e)
        {
            DrawIcon();
        }

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = CharSet.Auto)]
        extern static bool DestroyIcon(IntPtr handle);

        private void DrawIcon()
        {
            if (!_terminateManager.Terminating)
            {
                using (var bitmap = new Bitmap(IconWidth, IconWidth, PixelFormat.Format32bppPArgb))
                {
                    var dateTime = DateTime.Now;
                    var angle = ((dateTime.Second * 1000 + dateTime.Millisecond) % 10000) * 360 / 10000;
                    using (var graphics = Graphics.FromImage(bitmap))
                    {
                        var rectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                        graphics.FillRectangle(Brushes.Black, rectangle);
                        graphics.FillPie(Brushes.White, rectangle, angle, 360);
                        graphics.FillPie(Brushes.Red, rectangle, 0, angle);
                    }

                    var hIcon = bitmap.GetHicon();
                    notifyIcon1.Icon = Icon.FromHandle(hIcon);
                    DestroyIcon(notifyIcon1.Icon.Handle);
                }
            }
        }
    }
}
