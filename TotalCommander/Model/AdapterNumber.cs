using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TotalCommander.Model
{
    static class AdapterNumber
    {
        public static string ToAdaptNumber(long num)
        {
            StringBuilder str = new StringBuilder(num.ToString());
            for (int i = str.Length - 3; i > 0; i-=3)
                str.Insert(i, ' ');
            return str.ToString();
        }
    }
}
