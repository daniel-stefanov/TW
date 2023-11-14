using System.Text;
using TW.Model;

namespace TW.Logic
{
    public static class Controller
    {
        public static List<List<Node>> FindAllPaths(Network net)
        {
            IEnumerable<Node> startNodes = net.Elements.Where(x => x is Intermediary node && !node.Links.Any(y => y.NodeOut == x));
            List<List<Node>> paths = new List<List<Node>>();
            foreach (Node node in startNodes)
            {
                paths = GetPaths(paths, new List<Node> { node }, node);
            }
            return paths;
        }

        private static List<List<Node>> GetPaths(List<List<Node>> paths, List<Node> currentPath, Node currentElement)
        {
            List<List<Node>> overlappingPaths = paths.Where(x => x.Contains(currentElement)).ToList();
            if (overlappingPaths.Any())
            {
                foreach (List<Node> overlappingPath in overlappingPaths)
                {
                    List<Node> tmp = new List<Node>(currentPath);
                    int i = overlappingPath.IndexOf(currentElement) + 1;
                    tmp.AddRange(overlappingPath.GetRange(i, overlappingPath.Count - i));
                    paths.Add(tmp);
                }
            }
            else
            {
                List<Node> children = new List<Node>();
                if (currentElement is Link link)
                {
                    children.Add(link.NodeOut);
                }
                else if (currentElement is Intermediary node)
                {
                    children.AddRange(node.Links.Where(x => x.NodeIn == node));
                }
                if (children.Count == 0)
                {
                    paths.Add(currentPath);
                }
                else
                {
                    foreach (Node child in children)
                    {
                        List<Node> branch = new List<Node>(currentPath);
                        branch.Add(child);
                        paths = GetPaths(paths, branch, child);
                    }
                }
            }
            return paths;
        }

        public static double GetAverageOptimalityDelta(List<Node> path, double granularity)
        {
            List<double> optimality = new List<double>();
            foreach (Node node in path)
            {
                double optimalLoad = FindOptimalFlow(node, granularity);
                double delta = Math.Abs(optimalLoad - node.RawDemand);
                optimality.Add(delta);
            }
            return optimality.Average();
        }

        public static double FindOptimalFlow(Node element, double granularity)
        {
            if (element is Generator || element is Consumer)
            {
                return element.MaxCapacity;
            }
            Node test = new Node($"test_{element.Id}");
            test.MaxCapacity = element.MaxCapacity;
            test.GetLoss = element.GetLoss;

            double optimalRawDemand = 0;
            double minimalLossPerUnit = double.MaxValue;
            double flow = granularity;
            while (flow < test.MaxCapacity)
            {
                test.RawDemand = flow;
                double result = test.GetLoss(test);
                double lossPerUnit = test.GetLoss(test) / flow;
                if (lossPerUnit < minimalLossPerUnit)
                {
                    optimalRawDemand = flow;
                    minimalLossPerUnit = lossPerUnit;
                }
                flow += granularity;
            }
            return optimalRawDemand;
        }

        public static Report GenerateReport(Network net)
        {

        }

        public static Report CompareReports(Report r1, Report r2)
        {

        }

        public static Network SuggestLoadBalancing(Network net)
        {
            throw new NotImplementedException();
        }

        public static Network SuggestPathOptimization(Network net)
        {
            throw new NotImplementedException();
        }

        public static string ToString(this Node element, Func<Node, string>[] selectors, string separator)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Func<Node, string> selector in selectors)
            {
                sb.Append(selector(element));
                sb.Append(separator);
            }
            sb.Length -= separator.Length;
            return sb.ToString();
        }
    }
}
