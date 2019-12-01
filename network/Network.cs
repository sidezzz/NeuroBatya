using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroBatya
{
    class Layer
    {
        public Layer Previous { get; set; }
        public Layer Next { get; set; }
        public List<Neuron> Neurons { get; }

        public Layer()
        {
            Neurons = new List<Neuron>();
        }
        public Neuron AddNeuron<T>() where T : Neuron, new()
        {
            var neuron = new T();

            var previous = Previous?.Neurons;
            if (previous != null)
            {
                foreach (var n in previous)
                {
                    n.Connect(neuron);
                }
            }

            var next = Next?.Neurons;
            if (next != null)
            {
                foreach (var n in next)
                {
                    n.ConnectTo(neuron);
                }
            }

            Neurons.Add(neuron);

            return neuron;
        }
        public void Activate()
        {
            foreach (var n in Neurons)
            {
                n.Activate();
            }
        }

        public void Store(IEnumerable<double> values)
        {
            int idx = 0;
            foreach (var tuple in Neurons.Zip(values, Tuple.Create))
            {
                tuple.Item1.Input = tuple.Item2;
                idx++;
            }
            for (int i = idx; i < Neurons.Count; i++)
            {
                Neurons[i].Input = 0.0;
            }
        }

        public List<double> Output() => Neurons.Select(x => x.Output).ToList();
    }

    class Network
    {
        public List<Layer> Layers { get; }
        public Layer InputLayer { get => Layers.First(); }
        public Layer OutputLayer { get => Layers.Last(); }

        public Network()
        {
            Layers = new List<Layer>();
        }
        public Layer AddLayer<T>() where T : Layer, new()
        {
            var layer = new T();

            try
            {
                var lastLayer = Layers.Last();
                layer.Previous = lastLayer;
                lastLayer.Next = layer;
            }
            catch (InvalidOperationException) { }

            Layers.Add(layer);

            return layer;
        }

        public void Activate()
        {
            foreach (var l in Layers)
            {
                l.Activate();
            }
        }
    }
}
