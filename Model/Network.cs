using System.ComponentModel;

namespace TW.Model
{
    public class Network
    {
        public event EventHandler NetworkChanged;
        protected virtual void OnNetworkChanged(Object sender, CollectionChangeEventArgs e)
        {
            NetworkChanged?.Invoke(sender, e);
        }

        Random r = new Random();
        const bool throwOnFailedConnection = false;

        public List<Node> Nodes { get; set; } = new List<Node>();

        double PowerLossF(NetworkElement element)
        {
            double ratio = Math.Pow(element.LoadRatio, 2);
            double fixedLoss = element.MaxCapacity / 100;
            double variableLoss = (element.MaxCapacity / 10) * ratio;
            return fixedLoss + variableLoss;
        }

        public void InitializeRandom()
        {
            Nodes.Clear();
            int numberOfGenerators = r.Next(2, 5);
            int numberOfIntermediaries = r.Next(5, 20);
            int numberOfConsumers = r.Next(numberOfGenerators, numberOfGenerators * 2);

            int i;
            i = 0;
            while (i < numberOfGenerators)
            {
                Generator node = new Generator($"g{i}");
                node.Production = r.Next(10, 50);
                node.MaxCapacity = node.Production + r.Next(0, 10);
                Nodes.Add(node);
                i++;
            }

            i = 0;
            while (i < numberOfIntermediaries)
            {
                Intermediary node = new Intermediary($"i{i}");
                node.MaxCapacity = r.Next(25, 100);
                node.LossFn = PowerLossF;
                Nodes.Add(node);
                i++;
            }

            i = 0;
            while (i < numberOfConsumers)
            {
                Consumer node = new Consumer($"c{i}");
                node.RawDemand = r.Next(5, 25);
                Nodes.Add(node);
                i++;
            }

            List<Node> intermediaries = Nodes.Where(x => x is Intermediary).ToList();
            foreach (Intermediary intermediary in intermediaries)
            {
                IEnumerable<Node> inputs = intermediaries.OrderBy(x => r.Next()).Take(r.Next(1, 2));
                IEnumerable<Node> outputs = intermediaries.OrderBy(x => r.Next()).Take(r.Next(1, 2));
                ConnectElement(intermediary, inputs, outputs);
            }

            List<Node> generators = Nodes.Where(x => x is Generator).ToList();
            foreach (Generator generator in generators)
            {
                IEnumerable<Node> outputs = intermediaries.OrderBy(x => r.Next()).Take(r.Next(1, 2));
                foreach (Intermediary intermediary in outputs)
                {
                    ConnectElement(intermediary, new List<Node>() { generator }, Enumerable.Empty<Node>());
                }
            }

            List<Node> consumers = Nodes.Where(x => x is Consumer).ToList();
            foreach (Consumer consumer in consumers)
            {
                IEnumerable<Node> inputs = intermediaries.OrderBy(x => r.Next()).Take(r.Next(1, 2));
                foreach (Intermediary intermediary in inputs)
                {
                    ConnectElement(intermediary, Enumerable.Empty<Node>(), new List<Node>() { consumer });
                }
            }
        }

        public void InitializeDemo()
        {
            Nodes.Clear();
            Generator g1 = new Generator("g1");
            g1.Production = 12;
            g1.MaxCapacity = 15;
            AddElement(g1);

            Generator g2 = new Generator("g2");
            g2.Production = 20;
            g2.MaxCapacity = 20;
            AddElement(g2);

            Intermediary i1 = new Intermediary("i1");
            i1.MaxCapacity = r.Next(25, 100);
            i1.LossFn = PowerLossF;
            AddElement(i1);

            Intermediary i2 = new Intermediary("i2");
            i2.MaxCapacity = r.Next(25, 100);
            i2.LossFn = PowerLossF;
            AddElement(i2);

            Consumer c1 = new Consumer("c1");
            c1.RawDemand = 8;
            AddElement(c1);
            Consumer c2 = new Consumer("c2");
            c2.RawDemand = 11;
            AddElement(c2);

            ConnectElement(i1, new Node[] { g1, g2 }, new Node[] { i2 });
            ConnectElement(i2, Enumerable.Empty<Node>(), new Node[] { c1, c2 });
        }

        public void AddElement(Node node)
        {
            if (Nodes.Contains(node))
            {
                throw new ArgumentException("Node already exists.");
            }
            Nodes.Add(node);
            node.ElementChanged += ElementChanged;
            OnNetworkChanged(this, new CollectionChangeEventArgs(CollectionChangeAction.Add, node));
        }

        public (IEnumerable<Link> successfulLinks, IEnumerable<(Link, string)> failedLinks) ConnectElement(string nodeId, IEnumerable<string> inputIds, IEnumerable<string> outputIds)
        {
            Intermediary node = (Intermediary)Nodes.Find(x => x is Intermediary && x.Id == nodeId);
            IEnumerable<Node> inputs = inputIds.Select(x => Nodes.Find(x => x is Intermediary && x.Id == nodeId));
            IEnumerable<Node> outputs = inputIds.Select(x => Nodes.Find(x => x is Intermediary && x.Id == nodeId));
            if (node == null || inputs.Any(x => x == null) || outputs.Any(x => x == null))
            {
                throw new ArgumentException("Unknown node.");
            }
            return ConnectElement(node, inputs, outputs);
        }

        public (IEnumerable<Link> successfulLinks, IEnumerable<(Link, string)> failedLinks) ConnectElement(Intermediary node, IEnumerable<Node> inputs, IEnumerable<Node> outputs)
        {
            if (!Nodes.Contains(node))
            {
                throw new ArgumentException("Unknown node.");
            }
            List<Link> successfulLinks = new List<Link>();
            List<(Link, string)> failedLinks = new List<(Link, string)>();
            foreach (Node input in inputs)
            {
                if (input == null)
                {
                    continue;
                }

                Link link = new Link($"{input.Id} > {node.Id}");
                link.NodeIn = input;
                link.NodeOut = node;


                if (node.Links.Any(x => x.NodeIn.Id == input.Id))
                {
                    failedLinks.Add((link, "Already connected."));
                    continue;
                }
                if (!CircularDependencyOk(input, node))
                {
                    failedLinks.Add((link, "Circular dependency check failed."));
                    continue;
                }
                if (!Nodes.Contains(input))
                {
                    failedLinks.Add((link, "Unknown node."));
                    continue;
                }

                link.MaxCapacity = r.Next(25, 100);
                link.LossFn = PowerLossF;

                link.ElementChanged += ElementChanged;

                input.Links.Add(link);
                node.Links.Add(link);

                successfulLinks.Add(link);
            }
            foreach (Node output in outputs)
            {
                if (output == null)
                {
                    continue;
                }
                Link link = new Link($"{node.Id} > {output.Id}");
                link.NodeIn = node;
                link.NodeOut = output;


                if (node.Links.Any(x => x.NodeOut.Id == output.Id))
                {
                    string msg = "Already connected.";
                    failedLinks.Add((link, msg));
                    if (throwOnFailedConnection)
                    {
                        throw new Exception(msg);
                    }
                    continue;
                }
                if (!CircularDependencyOk(node, output))
                {
                    string msg = "Circular dependency check failed.";
                    failedLinks.Add((link, msg));
                    if (throwOnFailedConnection)
                    {
                        throw new Exception(msg);
                    }
                    continue;
                }
                if (!Nodes.Contains(output))
                {
                    string msg = "Unknown node.";
                    failedLinks.Add((link, msg));
                    if (throwOnFailedConnection)
                    {
                        throw new Exception(msg);
                    }
                    continue;
                }

                link.MaxCapacity = r.Next(25, 100);
                link.LossFn = PowerLossF;

                node.Links.Add(link);
                output.Links.Add(link);

                successfulLinks.Add(link);
            }
            return (successfulLinks, failedLinks);
        }

        public bool CircularDependencyOk(Node source, Node target)
        {
            if (target == source)
            {
                return false;
            }
            foreach (Link link in target.Links.Where(x => x.NodeIn == target))
            {
                if (link.NodeOut == source)
                {
                    return false;
                }
                else
                {
                    return CircularDependencyOk(source, link.NodeOut);
                }
            }
            return true;
        }

        private void ElementChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnNetworkChanged(this, new CollectionChangeEventArgs(CollectionChangeAction.Refresh, sender));
        }
    }
}
