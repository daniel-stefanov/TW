using TW.Logic;
using TW.Model;

namespace TW
{
    internal class Program
    {
        static Network DemoNetwork = new Network();
        static bool exit = false;
        static void Main(string[] args)
        {
            InitializeDemoNetwork();
            do
            {
                AnalyzeNetwork();
                ProcessInput();
            }
            while (!exit);
        }

        static void InitializeDemoNetwork()
        {
            Console.WriteLine("Initializing demo network...");
            DemoNetwork.Initialize();
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
            Console.WriteLine("Finding paths...");
            Console.WriteLine();
            List<List<NetworkElement>> paths = Logic.Logic.FindAllPaths(DemoNetwork);
            foreach (List<NetworkElement> path in paths)
            {
                Console.Write("Path: ");
                foreach (NetworkElement node in path)
                {
                    Console.Write($"[{node.Id}] ");
                }
                Console.WriteLine();
                Console.WriteLine($"Average load: {(path.Sum(x => x.LoadRatio) / path.Count) * 100}%");
                Console.WriteLine($"Peak load: {path.MaxBy(x => x.LoadRatio).ToString(new Func<NetworkElement, string>[] { x => x.Id, x => (x.LoadRatio * 100).ToString() }, ", ")}%");
                Console.WriteLine($"Average loss: {path.Sum(x => x.FactualLoss) / path.Count} MW");
                Console.WriteLine($"Total loss: {path.Sum(x => x.FactualLoss)} MW");
                Console.WriteLine($"Peak loss: {path.MaxBy(x => x.FactualLoss).ToString(new Func<NetworkElement, string>[] { x => x.Id, x => x.FactualLoss.ToString() }, ", ")} MW");

                Console.WriteLine();
            }
        }

        static void OptimizeNetwork()
        {

        }
    }
}