using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroBatya
{
    class Neuron
    {
        public class Link
        {
            static Random WeightGenerator = new Random();
            public Neuron From { get; }
            public Neuron To { get; }
            public double Weight { get; set; }
            public Link(Neuron from, Neuron to)
            {
                From = from;
                To = to;
                Weight = WeightGenerator.NextDouble() - 0.5;
            }
        }

        public IActivation Function { get; set; }
        public List<Link> InputLinks { get; }
        public List<Link> OutputLinks { get; }
        public double Input { get; set; }
        public double Output { get; protected set; }
        public double Derivative { get => Function.Derivative(Input); }
        public double ErrorSum { get; set; }
        public Neuron()
        {
            Function = new Linear();
            InputLinks = new List<Link>();
            OutputLinks = new List<Link>();
        }
        public Link Connect(Neuron neuron)
        {
            return neuron.ConnectTo(this);
        }
        public virtual Link ConnectTo(Neuron neuron)
        {
            var link = new Link(neuron, this);
            InputLinks.Add(link);
            neuron.OutputLinks.Add(link);
            return link;
        }
        public virtual void Activate()
        {
            if (InputLinks.Count != 0)
            {
                Input = 0.0;
                foreach (var link in InputLinks)
                {
                    Input += link.Weight * link.From.Output;
                }
            }
            Output = Function.Process(Input);
            ErrorSum = 0.0;
        }
    }

    class BiasNeuron : Neuron
    {
        public BiasNeuron() : base()
        {
            Input = 1.0;
            Output = 1.0;
        }
        public override Link ConnectTo(Neuron neuron)
        {
            return null;
        }
        public override void Activate()
        {
            ErrorSum = 0.0;
        }
    }
}
