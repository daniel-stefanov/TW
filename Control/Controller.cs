using System.ComponentModel;
using System.Text;
using TW.Model;

namespace TW.Control
{
    public static class Controller
    {
        static IRecommendationModule recommendationModule = new RecommendationModule();

        public async static Task<List<List<NetworkElement>>> FindAllPaths(Network net, ProgressChangedEventHandler? statusHandler)
        {
            statusHandler?.Invoke(net, new ProgressChangedEventArgs(0, "Finding root nodes..."));

            List<Node> startNodes = net.Nodes.Where(x => !x.Links.Any(y => y.NodeOut == x)).ToList();
            List<List<NetworkElement>> paths = new List<List<NetworkElement>>();
            int i = 0;
            while (i < startNodes.Count)
            {
                Node node = startNodes[i];
                statusHandler?.Invoke(net, new ProgressChangedEventArgs((int)(((double)i / (double)startNodes.Count) * 100), $"Gettings paths from {node.Id}"));
                paths = await GetPathsInternal(paths, new List<NetworkElement> { node }, node);
                i++;
            }

            statusHandler?.Invoke(net, new ProgressChangedEventArgs(100, $"Done. {paths.Count} unique paths found."));
            return paths;
        }

        private async static Task<List<List<NetworkElement>>> GetPathsInternal(List<List<NetworkElement>> paths, List<NetworkElement> currentPath, NetworkElement currentElement)
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
                        paths = await GetPathsInternal(paths, branch, child);
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

        public static async Task<Report> GenerateReport(Network net, ProgressChangedEventHandler? statusHandler)
        {
            Report report = new Report();
            List<List<NetworkElement>> paths = await FindAllPaths(net, statusHandler);
            int elementCounter = 0;
            int i = 0;
            while (i < paths.Count)
            {
                List<NetworkElement> path = paths[i];
                PathAnalysis analyzedPath = new PathAnalysis();

                int q = 0;
                while (q < path.Count)
                {
                    statusHandler?.Invoke(net, new ProgressChangedEventArgs((int)(((double)elementCounter / (double)paths.Sum(x => x.Count)) * 100), $"Analyzing paths..."));

                    NetworkElement element = path[q];
                    ElementAnalysis analyzedElement = new ElementAnalysis();
                    analyzedElement.Element = element;
                    analyzedElement.LoadPercent = element.LoadRatio * 100;
                    analyzedElement.Loss = element.LossFn(element);
                    if (element is Generator || element is Consumer)
                    {
                        analyzedElement.OptimalLoadPercent = 100;
                    }
                    else
                    {
                        analyzedElement.OptimalLoadPercent = (FindOptimalFlow(element, 0.01) / element.MaxCapacity) * 100;
                    }

                    analyzedPath.Path.Add(analyzedElement);

                    q++;
                    elementCounter++;
                }

                List<ElementAnalysis> pathIntermediaries = analyzedPath.PassiveElements.ToList();

                analyzedPath.AverageLoadPercent = pathIntermediaries.Average(x => x.LoadPercent);
                analyzedPath.PeakLoadPercent = pathIntermediaries.MaxBy(x => x.LoadPercent);
                analyzedPath.AverageLoss = pathIntermediaries.Average(x => x.Loss);
                analyzedPath.PeakLoss = pathIntermediaries.MaxBy(x => x.Loss);
                analyzedPath.TotalLoss = pathIntermediaries.Sum(x => x.Loss);
                analyzedPath.AverageOptimalityPercentDelta = pathIntermediaries.Average(x => x.OptimalityPercentDelta);

                report.Paths.Add(analyzedPath);

                i++;
            }

            statusHandler?.Invoke(net, new ProgressChangedEventArgs(100, $"Summarizing..."));

            List<List<NetworkElement>> normalPaths = paths.Where(x => x.First() is Generator && x.Last() is Consumer).ToList();
            report.SupplyDemandRatio = normalPaths.Sum(x => (x.First() as Generator).Production) / normalPaths.Sum(x => (x.Last() as Consumer).Demand);
            report.TotalLossess = report.Paths.SelectMany(x => x.Path).Where(x => x.Element is not Generator && x.Element is not Consumer).DistinctBy(x => x.Element.Id).Sum(x => x.Loss);
            report.AverageOptimalityDelta = report.Paths.Average(x => x.AverageOptimalityPercentDelta);

            statusHandler?.Invoke(net, new ProgressChangedEventArgs(100, $"Generating recommendations..."));

            recommendationModule.ApplyRecommendations(report);

            statusHandler?.Invoke(net, new ProgressChangedEventArgs(100, $"Done."));

            return report;
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

        public static string Articulate(this List<ElementAnalysis> path)
        {
            return string.Join(' ', path.Select(x => $"[{x.Element.Id}]").ToArray());
        }
    }
}
