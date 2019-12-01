using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroBatya
{
    interface IActivation
    {
        double Process(double value);
        double Derivative(double value);
    }

    class Linear : IActivation
    {
        public double Process(double value)
        {
            return value;
        }
        public double Derivative(double value)
        {
            return 1.0;
        }
    }

    class Sigmoid : IActivation
    {
        public double Process(double value)
        {
            return 1.0 / (1.0 + Math.Exp(-value));
        }
        public double Derivative(double value)
        {
            return Process(value) * (1.0 - Process(value));
        }
    }

    class ReLu : IActivation
    {
        public double Process(double value)
        {
            return Math.Max(0, value);
        }
        public double Derivative(double value)
        {
            return value >= 0.0 ? 1.0 : 0.0;
        }
    }
}
