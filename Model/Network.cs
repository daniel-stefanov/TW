namespace TW.Model
{
    public class Network
    {
        const bool ThrowOnFailedConnection = true;

        public List<NetworkElement> Elements { get; set; } = new List<NetworkElement>();

        double PowerLossF(NetworkElement net)
        {
            return (net.MaxCapacity / 3) + ((net.MaxCapacity / 3) * 2 * net.LoadRatio);
        }

        public void Initialize()
        {
            Generator g1 = new Generator("g1");
            g1.Production = 12;
            g1.MaxCapacity = 15;
            AddElement(g1);

            Generator g2 = new Generator("g2");
            g2.Production = 20;
            g2.MaxCapacity = 20;
            AddElement(g2);

            Node i1 = new Node("i1");
            i1.MaxCapacity = 10;
            i1.GetLossRatio = PowerLossF;
            AddElement(i1);

            Node i2 = new Node("i2");
            i2.MaxCapacity = 9;
            i2.GetLossRatio = PowerLossF;
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

        public void AddElement(NetworkElement node)
        {
            if (Elements.Contains(node))
            {
                throw new ArgumentException("Node already exists.");
            }
            Elements.Add(node);
        }

        public (IEnumerable<Link> successfulLinks, IEnumerable<(Link, string)> failedLinks) ConnectElement(string nodeId, IEnumerable<string> inputIds, IEnumerable<Node> outputIds)
        {
            Node? node = (Node?)Elements.Find(x => x is Node && x.Id == nodeId);
            IEnumerable<Node?> inputs = inputIds.Select(x => (Node?)Elements.Find(x => x is Node && x.Id == nodeId));
            IEnumerable<Node?> outputs = inputIds.Select(x => (Node?)Elements.Find(x => x is Node && x.Id == nodeId));
            if (node == null || inputs.Any(x => x == null) || outputs.Any(x => x == null))
            {
                throw new ArgumentException("Unknown node.");
            }
            return ConnectElement(node, inputs, outputs);
        }

        public (IEnumerable<Link> successfulLinks, IEnumerable<(Link, string)> failedLinks) ConnectElement(Node node, IEnumerable<Node?> inputs, IEnumerable<Node?> outputs)
        {
            if (!Elements.Contains(node))
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
                if (!Elements.Contains(input))
                {
                    failedLinks.Add((link, "Unknown node."));
                    continue;
                }

                link.MaxCapacity = 10;
                link.GetLossRatio = PowerLossF;

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
                    if (ThrowOnFailedConnection)
                    {
                        throw new Exception(msg);
                    }
                    continue;
                }
                if (!CircularDependencyOk(node, output))
                {
                    string msg = "Circular dependency check failed.";
                    failedLinks.Add((link, msg));
                    if (ThrowOnFailedConnection)
                    {
                        throw new Exception(msg);
                    }
                    continue;
                }
                if (!Elements.Contains(output))
                {
                    string msg = "Unknown node.";
                    failedLinks.Add((link, msg));
                    if (ThrowOnFailedConnection)
                    {
                        throw new Exception(msg);
                    }
                    continue;
                }

                link.MaxCapacity = 10;
                link.GetLossRatio = PowerLossF;

                node.Links.Add(link);
                output.Links.Add(link);

                successfulLinks.Add(link);
            }
            return (successfulLinks, failedLinks);
        }

        public bool CircularDependencyOk(Node source, Node target)
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
