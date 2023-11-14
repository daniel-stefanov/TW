using TW.Model;

namespace TW.Logic
{
    public class NodeAnalysis
    {
        public Node Node { get; set; }
        public double Load { get; set; }
        public double MinimumLossPerUnitFlow { get; set; }
        public double MinimumCostPerUnitFlow { get; set; }

    }
}
