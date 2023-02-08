using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Petersilie.Utils.SnippetReader
{
    internal static class Program
    {
        private static PipeHostContext _hostContext;


        private static void Application_Exit(object sender, EventArgs e)
        {
            _hostContext?.Dispose();
        }


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {                        
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            _hostContext = new PipeHostContext();
            Application.ApplicationExit += Application_Exit;
            Application.Run(_hostContext);
        }
    }
}
