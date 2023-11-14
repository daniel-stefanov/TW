using TW.Model;

namespace TW.Control
{
    public class NodeAnalysis
    {
        public Node Node { get; set; }
        public double Load { get; set; }
        public double MinimumLossPerUnitFlow { get; set; }
        public double MinimumCostPerUnitFlow { get; set; }

    }
}
