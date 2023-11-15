using System.ComponentModel;
using System.Text;
using TW.Model;

namespace TW.Control
{
    public static class Controller
    {
        public async static Task<List<List<NetworkElement>>> FindAllPaths(Network net, ProgressChangedEventHandler? statusHandler)
        {
            statusHandler?.Invoke(net, new ProgressChangedEventArgs(0, "Finding root nodes..."));

            List<Node> startNodes = net.Nodes.Where(x => !x.Links.Any(y => y.NodeOut == x)).ToList();
            List<List<NetworkElement>> paths = new List<List<NetworkElement>>();
            int i = 0;
            while(i<startNodes.Count) {
                Node node = startNodes[i];
                statusHandler?.Invoke(net, new ProgressChangedEventArgs((int)(((double)i/(double)startNodes.Count)*100), $"Gettings paths from {node.Id}"));
                paths = await GetPaths(paths, new List<NetworkElement> { node }, node);
                i++;
            }

            statusHandler?.Invoke(net, new ProgressChangedEventArgs(100, $"Done. {paths.Count} unique paths found."));
            return paths;
        }

        private async static Task<List<List<NetworkElement>>> GetPaths(List<List<NetworkElement>> paths, List<NetworkElement> currentPath, NetworkElement currentElement)
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
                        paths = await GetPaths(paths, branch, child);
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
