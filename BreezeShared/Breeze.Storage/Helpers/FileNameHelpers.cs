using System;
using System.Collections.Generic;
using System.Text;

namespace Breeze.Storage.Helpers
{
    public static class FileNameHelpers
    {
        public static string EnsureEndsWith(this string input, string endsWith)
        {
            if (!input.EndsWith(endsWith))
            {
                input = input + endsWith;
            }

            return input;
        }


    }
}
