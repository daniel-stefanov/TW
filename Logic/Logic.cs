using System.Text;
using TW.Model;

namespace TW.Logic
{
    public static class Logic
    {
        public static List<List<NetworkElement>> FindAllPaths(Network net)
        {
            IEnumerable<NetworkElement> startNodes = net.Elements.Where(x => !x.Links.Any(y => y.NodeOut == x));
            List<List<NetworkElement>> paths = new List<List<NetworkElement>>();
        }

        private static List<NetworkElement> GetPath(List<NetworkElement>

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
            return sb.ToString();
        }
    }
}
