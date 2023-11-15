using System.ComponentModel;
using TW.Control;
using TW.Model;

namespace TW
{
    internal class Program
    {
        static event ProgressChangedEventHandler ProgressChanged;

        static Network DemoNetwork = new Network();
        static bool exit = false;
        static void Main(string[] args)
        {
            ProgressChanged += Program_ProgressChanged;
            InitializeNetwork();
            do
            {
                AnalyzeNetwork();
                ProcessInput();
            }
            while (!exit);
        }

        private static void Program_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            Console.WriteLine($"{e.ProgressPercentage.ToString().PadLeft(3)}% : {e.UserState.ToString()}");
        }

        static void InitializeNetwork()
        {
            Console.WriteLine("Initializing demo network...");
            DemoNetwork.InitializeDemo();
            Console.WriteLine("Done.");
        }

        static void ProcessInput()
        {
            Console.WriteLine();
            bool valid = true;
            do
            {
                Console.Write('>');
                string? input = Console.ReadLine().Trim().ToLower();
                switch (input)
                {
                    case "add":
                        {
                            Console.WriteLine("Add syntax: generator <id> [outputId,...] / consumer <id> [inputId,...] / node id [inputId,...] [outputId,...]");
                            Console.Write('>');
                            string[] inputSplit = Console.ReadLine().Trim().ToLower().Split(' ');
                            switch (inputSplit[0])
                            {
                                case "generator":
                                    {
                                        if (inputSplit.Length != 3)
                                        {
                                            valid = false;
                                        }
                                        else
                                        {

                                        }
                                        break;
                                    }
                                case "consumer":
                                    {
                                        if (inputSplit.Length != 3)
                                        {
                                            valid = false;
                                        }
                                        else
                                        {

                                        }
                                        break;
                                    }
                                case "node":
                                    {
                                        if (inputSplit.Length != 4)
                                        {
                                            valid = false;
                                        }
                                        else
                                        {

                                        }
                                        break;
                                    }
                                default:
                                    {
                                        valid = false;
                                        break;
                                    }
                            }
                            break;
                        }
                    case "remove":
                        {
                            break;
                        }
                    case "connect":
                        {
                            break;
                        }
                    case "disconnect":
                        {
                            break;
                        }
                    case "optimize":
                        {
                            OptimizeNetwork();
                            break;
                        }
                    case "exit":
                        {
                            exit = true;
                            break;
                        }
                    case "":
                        {
                            break;
                        }
                    default:
                        {
                            Console.WriteLine("Invalid input.");
                            valid = false;
                            break;
                        }
                }
            } while (!valid);
        }

        static void AnalyzeNetwork()
        {
            Console.WriteLine("Analyzing network...");
            List<List<NetworkElement>> paths = Controller.FindAllPaths(DemoNetwork, ProgressChanged).Result;
            Console.WriteLine();
            foreach (List<NetworkElement> path in paths)
            {
                PrintPath(path);
            }
        }

        static void PrintReport(Report report)
        {

        }

        static void PrintPath(List<NetworkElement> path)
        {
            Console.Write("Path: ");
            foreach (NetworkElement node in path)
            {
                Console.Write($"[{node.Id}] ");
            }

            Console.WriteLine();

            foreach (NetworkElement node in path)
            {
                Console.WriteLine($" {node.Id} - {node.GetType().Name}");
                Console.WriteLine($"  Max Capacity: {node.MaxCapacity.ToString("0.00")} MW");
                Console.WriteLine($"  Demand: {node.Demand.ToString("0.00")} MW");
                Console.WriteLine($"  Optimal Flow: {Controller.FindOptimalFlow(node, 0.01).ToString("0.00")} MW");
                Console.WriteLine($"  Load: {(node.LoadRatio * 100).ToString("0.00")}%");
                Console.WriteLine($"  Loss: {node.LossFn(node).ToString("0.00")} MW");
            }

            List<NetworkElement> intermediaries = path.Where(x => x is Intermediary || x is Link).ToList();
            Console.WriteLine();
            Console.WriteLine($" Average load: {(intermediaries.Average(x => x.LoadRatio) * 100).ToString("0.00")}%");
            Console.WriteLine($" Peak load: {intermediaries.MaxBy(x => x.LoadRatio).ToString(new Func<NetworkElement, string>[] { x => x.Id, x => (x.LoadRatio * 100).ToString("0.00") }, ", ")}%");
            Console.WriteLine($" Average loss: {intermediaries.Average(x => x.LossFn(x)).ToString("0.00")} MW");
            Console.WriteLine($" Total loss: {intermediaries.Sum(x => x.LossFn(x)).ToString("0.00")} MW");
            Console.WriteLine($" Peak loss: {intermediaries.MaxBy(x => x.LossFn(x)).ToString(new Func<NetworkElement, string>[] { x => x.Id, x => x.LossFn(x).ToString("0.00") }, ", ")} MW");
            Console.WriteLine($" Avg Optimality Delta: {Controller.GetAverageOptimalityDelta(intermediaries, 0.01).ToString("0.00")} MW");


            Console.WriteLine();
            Console.WriteLine();
        }

        static void OptimizeNetwork()
        {

        }
    }
}