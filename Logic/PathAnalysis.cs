namespace TW.Logic
{
    public class PathAnalysis
    {
        public List<NodeAnalysis> Path { get; set; }
        public double AverageLoad { get; set; }
        public NodeAnalysis PeakLoad { get; set; }
        public double AverageLoss { get; set; }
        public NodeAnalysis PeakLoss { get; set; }
        public double AverageOptimalityDelta { get; set; }
    }
}
