using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord;

namespace NeuroBatya
{
    class Program
    {

        static Network BuildNetworkPerceptron(int inputCount, int[] hiddenLayers, int outputCount, IActivation function)
        {
            var network = new Network();
            var inputLayer = network.AddLayer<Layer>();
            for (int i = 0; i < inputCount; i++)
            {
                inputLayer.AddNeuron<Neuron>();
            }
            inputLayer.AddNeuron<BiasNeuron>();

            foreach (var n in hiddenLayers)
            {
                var hiddenLayer = network.AddLayer<Layer>();
                for (int i = 0; i < n; i++)
                {
                    hiddenLayer.AddNeuron<Neuron>().Function = function;
                }
                hiddenLayer.AddNeuron<BiasNeuron>();
            }

            var outputLayer = network.AddLayer<Layer>();
            for (int i = 0; i < outputCount; i++)
            {
                outputLayer.AddNeuron<Neuron>().Function = function;
            }

            return network;
        }

        class BackPropogationTrainer : INetworkTrainer
        {
            public double educationSpeed = 0.6;
            public double accuracy = 0.01;

            void FixWeightsRecursive(Layer layer)
            {
                if (layer.Previous != null)
                {
                    foreach (var n in layer.Neurons)
                    {
                        var derivative = n.Derivative;
                        var sigma = derivative * n.ErrorSum;
                        foreach (var l in n.InputLinks)
                        {
                            l.From.ErrorSum += l.Weight * sigma;
                            var delta = educationSpeed * sigma * l.From.Output;
                            l.Weight += delta;
                        }
                    }
                    FixWeightsRecursive(layer.Previous);
                }
            }

            public void Train(Network network, IEnumerable<IEnumerable<double>> idealOutputSet, IEnumerable<IEnumerable<double>> trainSet)
            {
                int idx = 0;
                foreach (var set in trainSet.Zip(idealOutputSet, (a, b) => new { input = a, output = b }))
                {
                    network.InputLayer.Store(set.input);

                    var percent = Math.Round((double)idx / (double)trainSet.Count() * 100.0, 3);
                    int fixCount = 0;
                    while (true)
                    {
                        Console.WriteLine($"Train index {idx - 1} {percent}% within {fixCount}");

                        Console.SetCursorPosition(0, Console.CursorTop - 1);

                        network.Activate();
                        //var networkOutput = network.OutputLayer.Output();

                        double cost = 0.0;
                        foreach (var tuple in network.OutputLayer.Neurons.Zip(set.output, (a, b) => new { neuron = a, expected = b }))
                        {
                            var error = (tuple.expected - tuple.neuron.Output);
                            tuple.neuron.ErrorSum = error;
                            cost += error * error;
                        }

                        var mse = cost / network.OutputLayer.Neurons.Count;

                        //Console.WriteLine($"Cost {cost} MSE {mse}");

                        if (mse > accuracy)
                        {
                            FixWeightsRecursive(network.OutputLayer);
                            fixCount++;
                        }
                        else
                        {
                            break;
                        }

                    }
                    //Console.SetCursorPosition(0, Console.CursorTop + 1);
                    idx++;
                    
                }
            }
        }

        static void MnistTest()
        {
            var dataSet = new Accord.DataSets.MNIST();

            var inputTraining = dataSet.Training.Item1.Select(x => x.Select(s => s / 255.0)).ToList();
            var inputTesting = dataSet.Testing.Item1.Select(x => x.Select(s => s / 255.0)).ToList();
            var idealOutputTrainSet = new List<List<double>>();
            foreach (var num in dataSet.Training.Item2)
            {
                var list = new List<double>();
                for (int a = 0; a < 10; a++)
                {
                    list.Add(0);
                }
                list[(int)num] = 1;
                idealOutputTrainSet.Add(list);
            }

            var network = BuildNetworkPerceptron(inputTraining[0].Count(), new int[] { 32 }, 10, new Sigmoid());

            var trainer = new BackPropogationTrainer();
            trainer.accuracy = 0.05;
            trainer.educationSpeed = 0.5;
            for (int epoch = 0; epoch < 15; epoch++)
            {
                var start = DateTime.Now;
                trainer.Train(network, idealOutputTrainSet, inputTraining);
                Console.WriteLine($"Train time {(DateTime.Now - start).Seconds}s");

                double accuracy = 0.0;
                foreach (var set in inputTesting.Zip(dataSet.Testing.Item2, (a, b) => new { input = a, output = b }))
                {
                    network.InputLayer.Store(set.input);
                    network.Activate();
                    var output = network.OutputLayer.Output();

                    var num = output.IndexOf(output.Max());

                    int idx = 0;
                    var cnt = set.input.Count();
                    foreach (var r in set.input)
                    {
                        Console.Write(r > 0.2 ? '.' : '@');
                        idx++;
                        if (idx % 28 == 0)
                        {
                            Console.WriteLine();
                        }
                    }
                    for (int i = idx; i < 28 * 28; i++)
                    {
                        Console.Write('@');
                        idx++;
                        if (idx % 28 == 0)
                        {
                            Console.WriteLine();
                        }
                    }
                    Console.WriteLine($"Expected {set.output} got {num}");
                    Console.SetCursorPosition(0, Console.CursorTop - 29);

                    if (set.output == num)
                    {
                        accuracy += 100.0 / inputTesting.Count;
                    }
                }

                Console.WriteLine($"Accuracy {accuracy}%");
            }

            Console.ReadLine();
        }

        static void Main(string[] args)
        {
            MnistTest();
            return;
        }
    }
}
