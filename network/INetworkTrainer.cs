using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroBatya
{
    interface INetworkTrainer
    {
        void Train(Network network, IEnumerable<IEnumerable<double>> idealOutputSet, IEnumerable<IEnumerable<double>> trainSet);
    }
}
