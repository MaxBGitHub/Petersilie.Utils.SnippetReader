using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petersilie.Utils.SnippetReader
{
    public class SnippetReadyEventArgs
    {
        public Bitmap Snippet;
        public SnippetReadyEventArgs(Bitmap snippet)
        {
            this.Snippet = snippet;
        }
    }
}
