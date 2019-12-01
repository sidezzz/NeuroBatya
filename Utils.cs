using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroBatya
{
    static class Utils
    {
        public static List<double> NumToBits(double num)
        {
            var result = new List<double>();

            int n = (int)num;
            for (int i = 0; i < 32; i++)
            {
                result.Add((n >> i) & 1);
            }

            return result;
        }

        public static double BitsToNum(List<double> bits)
        {
            int n = 0;
            int i = 0;
            foreach (var bit in bits)
            {
                n |= (bit > 0.5 ? 1 : 0) << i;
                i++;
            }

            return (double)n;
        }
    }
}
