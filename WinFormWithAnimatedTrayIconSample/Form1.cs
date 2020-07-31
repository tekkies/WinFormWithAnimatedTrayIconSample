using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace WinFormWithTrayIcon
{
    public partial class Form1 : Form
    {
        private const int IconWidth = 64;
        bool Terminating;

        public Form1()
        {
            InitializeComponent();
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            contextMenuStrip1.Items.Clear();
            contextMenuStrip1.Items.Add("&Restore");
            contextMenuStrip1.Items.Add("-");
            contextMenuStrip1.Items.Add("E&xit");
            animationTimer.Start();
            DrawIcon();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Terminating)
            {
                // the idle state has occurred, and the tray notification should be gone.
                // ok to shutdown now
                return;
            }

            if (e.CloseReason == CloseReason.UserClosing && MessageBox.Show("Are you sure you want to close this form?", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.Cancel)
            {
                // only the user, selecting Cancel in a MessageBox, can do this.
                e.Cancel = true;
            }
            if (!e.Cancel)
            {
                // The application will shut down.

                // We cancel the shutdown, because the timer will do the shutdown when it fires.
                // This will return to the app and allow the idle state to occur.
                e.Cancel = true;

                // Dispose of the tray icon this way.
                notifyIcon1.Dispose();
                
                // Set the termination flag so that the next entry into this event will
                // not be cancelled.
                Terminating = true;
                
                // Activate the timer to fire
                timerCloseApp.Interval = 100;
                timerCloseApp.Enabled = true;
                timerCloseApp.Start();
            }
        }

        private void closeTimer_Tick(object sender, EventArgs e)
        {
            // the idle state is past.. at this point, the tray notification is gone from
            // the system tray.  

            // Deactivate the timer.. it is no longer needed.
            timerCloseApp.Stop();
            timerCloseApp.Enabled = false;

            // close the form, which will start the shutdown of the application.
            Close();
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

        private void animationTimer_Tick(object sender, EventArgs e)
        {
            DrawIcon();
        }

        private void DrawIcon()
        {
            using (var bitmap = new Bitmap(IconWidth, IconWidth, PixelFormat.Format32bppPArgb))
            {
                var dateTime = DateTime.Now;
                var angle = ((dateTime.Second*1000+dateTime.Millisecond) % 10000) * 360 / 10000;
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    var rectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                    graphics.FillRectangle(Brushes.Black, rectangle);
                    graphics.FillPie(Brushes.White, rectangle, angle, 360);
                    graphics.FillPie(Brushes.Red, rectangle, 0, angle);
                }
                notifyIcon1.Icon = Icon.FromHandle(bitmap.GetHicon());
            }
        }
    }
}
