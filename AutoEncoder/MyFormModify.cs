using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoEncoder
{
    class MyFormModify : EventArgs
    {
        public Form1 form1;

        public MyFormModify(Form1 f1)
        {
            form1 = f1;
        }
    }
}
