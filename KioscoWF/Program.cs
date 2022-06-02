using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KioscoWF
{
    public static class Program
    {
        public static Kiosco kiosco;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            kiosco = new Kiosco();
            Application.Run(kiosco);
        }
    }
}
