using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Breeze.Helpers
{
    public static class StringHelpers
    {
        public static List<string> ToList(this string input)
        {
            return input.Split('\n').Select(y => y.Replace("\r", "")).ToList();
        }


    }
}
