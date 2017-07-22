using System;
using System.Collections.Generic;

namespace HtmlTableToExecl
{
    public class E_Row
    {
        public int rowindex
        {
            get;
            set;
        }
        public List<E_Cell> cells
        {
            get;
            set;
        }
    }
}
