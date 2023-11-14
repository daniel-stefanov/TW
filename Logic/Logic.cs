using System.Text;
using TW.Model;

namespace TW.Logic
{
    public static class Logic
    {
        public static List<List<NetworkElement>> FindAllPaths(Network net)
        {
            IEnumerable<NetworkElement> startNodes = net.Elements.Where(x => x is Node node && !node.Links.Any(y => y.NodeOut == x));
            List<List<NetworkElement>> paths = new List<List<NetworkElement>>();
            foreach (NetworkElement node in startNodes)
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
                    paths.Add(currentPath);
                }
                else
                {
                    foreach (NetworkElement child in children)
                    {
                        currentPath.Add(child);
                        paths = GetPaths(paths, currentPath, child);
                    }
                }
            }
            return paths;
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
