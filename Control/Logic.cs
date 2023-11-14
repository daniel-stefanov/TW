using System.Text;
using TW.Model;

namespace TW.Control
{
    public static class Controller
    {
        public static List<List<NetworkElement>> FindAllPaths(Network net)
        {
            IEnumerable<Node> startNodes = net.Nodes.Where(x => !x.Links.Any(y => y.NodeOut == x));
            List<List<NetworkElement>> paths = new List<List<NetworkElement>>();
            foreach (Node node in startNodes)
            {
                paths = GetPaths(paths, new List<NetworkElement> { node }, node);
            }
            return paths;
        }

        private static List<List<NetworkElement>> GetPaths(List<List<NetworkElement>> paths, List<NetworkElement> currentPath, NetworkElement currentElement)
        {
            List<List<NetworkElement>> overlappingPaths = paths.Where(x => x.Contains(currentElement)).ToList();
            if (overlappingPaths.Any())
            {
                foreach (List<NetworkElement> overlappingPath in overlappingPaths)
                {
                    List<NetworkElement> tmp = new List<NetworkElement>(currentPath);
                    int i = overlappingPath.IndexOf(currentElement) + 1;
                    tmp.AddRange(overlappingPath.GetRange(i, overlappingPath.Count - i));
                    paths.Add(tmp);
                }
            }
            else
            {
                List<NetworkElement> children = new List<NetworkElement>();

                if (currentElement is Link link)
                {
                    children.Add(link.NodeOut);
                }
                else if (currentElement is Node node)
                {
                    children.AddRange(node.Links.Where(x => x.NodeIn == node));
                }

                if (children.Count == 0)
                {
                    if (currentElement is Consumer)
                    {
                        paths.Add(currentPath);
                    }
                }
                else
                {
                    foreach (NetworkElement child in children)
                    {
                        List<NetworkElement> branch = new List<NetworkElement>(currentPath);
                        branch.Add(child);
                        paths = GetPaths(paths, branch, child);
                    }
                }
            }
            return paths;
        }

        public static double GetAverageOptimalityDelta(List<NetworkElement> path, double granularity)
        {
            List<double> optimality = new List<double>();
            foreach (NetworkElement node in path)
            {
                double optimalLoad = FindOptimalFlow(node, granularity);
                double delta = Math.Abs(optimalLoad - node.RawDemand);
                optimality.Add(delta);
            }
            return optimality.Average();
        }

        public static double FindOptimalFlow(NetworkElement element, double granularity)
        {
            if (element is Generator || element is Consumer)
            {
                return element.MaxCapacity;
            }
            NetworkElement test = new NetworkElement($"test_{element.Id}");
            test.MaxCapacity = element.MaxCapacity;
            test.LossFn = element.LossFn;

            double optimalRawDemand = 0;
            double minimalLossPerUnit = double.MaxValue;
            double flow = granularity;
            while (flow < test.MaxCapacity)
            {
                test.RawDemand = flow;
                double result = test.LossFn(test);
                double lossPerUnit = test.LossFn(test) / flow;
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
            throw new NotImplementedException();
        }

        public static Report CompareReports(Report r1, Report r2)
        {
            throw new NotImplementedException();
        }

        public static Network SuggestLoadBalancing(Network net)
        {
            throw new NotImplementedException();
        }

        public static Network SuggestPathOptimization(Network net)
        {
            throw new NotImplementedException();
        }

        public static string ToString(this NetworkElement element, Func<NetworkElement, string>[] selectors, string separator)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Func<NetworkElement, string> selector in selectors)
            {
                sb.Append(selector(element));
                sb.Append(separator);
            }
            sb.Length -= separator.Length;
            return sb.ToString();
        }
    }
}
