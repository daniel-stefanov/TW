namespace TW.Model
{
    public class Network
    {
        Random r = new Random();
        const bool throwOnFailedConnection = false;

        public List<Node> Elements { get; set; } = new List<Node>();

        double PowerLossF(Node net)
        {
            double ratio = Math.Pow(net.LoadRatio, 2);
            double fixedLoss = net.MaxCapacity / 100;
            double variableLoss = (net.MaxCapacity / 10) * ratio;
            return fixedLoss + variableLoss;
        }

        public void InitializeRandom()
        {
            int numberOfGenerators = r.Next(2, 10);
            int numberOfIntermediaries = r.Next(10, 100);
            int numberOfConsumers = r.Next(numberOfGenerators, numberOfGenerators * 2);

            int i;
            i = 0;
            while (i < numberOfGenerators)
            {

                i++;
            }
        }

        public void InitializeDemo()
        {
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
            i1.GetLoss = PowerLossF;
            AddElement(i1);

            Intermediary i2 = new Intermediary("i2");
            i2.MaxCapacity = r.Next(25, 100);
            i2.GetLoss = PowerLossF;
            AddElement(i2);

            Consumer c1 = new Consumer("c1");
            c1.RawDemand = 8;
            AddElement(c1);
            Consumer c2 = new Consumer("c2");
            c2.RawDemand = 11;
            AddElement(c2);

            ConnectElement(i1, new Intermediary[] { g1, g2 }, new Intermediary[] { i2 });
            ConnectElement(i2, Enumerable.Empty<Intermediary>(), new Intermediary[] { c1, c2 });
        }

        public void AddElement(Node node)
        {
            if (Elements.Contains(node))
            {
                throw new ArgumentException("Node already exists.");
            }
            Elements.Add(node);
        }

        public (IEnumerable<Link> successfulLinks, IEnumerable<(Link, string)> failedLinks) ConnectElement(string nodeId, IEnumerable<string> inputIds, IEnumerable<Intermediary> outputIds)
        {
            Intermediary? node = (Intermediary?)Elements.Find(x => x is Intermediary && x.Id == nodeId);
            IEnumerable<Intermediary?> inputs = inputIds.Select(x => (Intermediary?)Elements.Find(x => x is Intermediary && x.Id == nodeId));
            IEnumerable<Intermediary?> outputs = inputIds.Select(x => (Intermediary?)Elements.Find(x => x is Intermediary && x.Id == nodeId));
            if (node == null || inputs.Any(x => x == null) || outputs.Any(x => x == null))
            {
                throw new ArgumentException("Unknown node.");
            }
            return ConnectElement(node, inputs, outputs);
        }

        public (IEnumerable<Link> successfulLinks, IEnumerable<(Link, string)> failedLinks) ConnectElement(Intermediary node, IEnumerable<Intermediary?> inputs, IEnumerable<Intermediary?> outputs)
        {
            if (!Elements.Contains(node))
            {
                throw new ArgumentException("Unknown node.");
            }
            List<Link> successfulLinks = new List<Link>();
            List<(Link, string)> failedLinks = new List<(Link, string)>();
            foreach (Intermediary input in inputs)
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
                if (!Elements.Contains(input))
                {
                    failedLinks.Add((link, "Unknown node."));
                    continue;
                }

                link.MaxCapacity = r.Next(25, 100);
                link.GetLoss = PowerLossF;

                input.Links.Add(link);
                node.Links.Add(link);

                successfulLinks.Add(link);
            }
            foreach (Intermediary output in outputs)
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
                if (!Elements.Contains(output))
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
                link.GetLoss = PowerLossF;

                node.Links.Add(link);
                output.Links.Add(link);

                successfulLinks.Add(link);
            }
            return (successfulLinks, failedLinks);
        }

        public bool CircularDependencyOk(Intermediary source, Intermediary target)
        {
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
    }
}
