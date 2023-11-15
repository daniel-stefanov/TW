namespace TW.Control
{
    public class PathAnalysis
    {
        public List<ElementAnalysis> Path { get; set; } = new List<ElementAnalysis>();
        public double AverageLoad { get; set; }
        public ElementAnalysis PeakLoad { get; set; }
        public double AverageLoss { get; set; }
        public ElementAnalysis PeakLoss { get; set; }
        public double AverageOptimalityDelta { get; set; }
        public ElementAnalysis PeakOptimalityDelta { get; set; }
    }
}
