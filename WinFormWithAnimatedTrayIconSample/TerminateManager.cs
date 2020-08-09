/*

This file based on WinFormWithTrayIcon Copyright (c) 2010-2017 John J Schultz

All usage is subject only to the terms of The MIT License (MIT):

-----------------------------------------------------------------------

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

*/
using System;
using System.Windows.Forms;

namespace WinFormWithAnimatedTrayIconSample
{
    public class TerminateManager
    {
        private readonly Form1 _form;
        private readonly NotifyIcon _notifyIcon;
        internal bool Terminating;
        private readonly Timer _timer;

        public TerminateManager(Form1 form, NotifyIcon notifyIcon)
        {
            _form = form;
            _notifyIcon = notifyIcon;
            _timer = new Timer();
            _timer.Tick += TimerTick;
        }

        internal void FormClosing(object sender, FormClosingEventArgs e)
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
                _notifyIcon.Dispose();

                // Set the termination flag so that the next entry into this event will
                // not be cancelled.
                Terminating = true;

                // Activate the timer to fire
                _timer.Interval = 100;
                _timer.Enabled = true;
                _timer.Start();
            }
        }

        internal void TimerTick(object sender, EventArgs e)
        {
            // the idle state is past.. at this point, the tray notification is gone from
            // the system tray.  

            // Deactivate the timer.. it is no longer needed.
            _timer.Stop();
            _timer.Enabled = false;

            // close the form, which will start the shutdown of the application.
            _form.Close();
        }
    }
}
