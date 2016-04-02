using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace UCS.Core
{
    public class ConsoleStreamer : TextWriter
    {

        TextBox TB = null;

        public ConsoleStreamer(TextBox output)
        {
            TB = output;
        }

        public override void Write(char value)
        {
            base.Write(value);
            TB.Dispatcher.BeginInvoke(new Action(() =>
            {
                TB.AppendText(value.ToString());
            }), DispatcherPriority.Render);
        }

        public override Encoding Encoding
        {
            get
            {
                return Encoding.UTF8;
            }
        }
    }
}
