using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace formelemethoden
{
    class RegExpComparator: IComparer<String>
    {
        public int Compare(string x, string y)
        {
            if (x.Length == y.Length)
            {
                return x.CompareTo(y);
            }
            else
            {
                return x.Length - y.Length;
            }
        }
    }
}
