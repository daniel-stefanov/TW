using TW.Model;

namespace TW.Control
{
    public class PathAnalysis
    {
        public List<ElementAnalysis> Path { get; set; } = new List<ElementAnalysis>();
        public List<ElementAnalysis> PassiveElements { get => Path.Where(x => x.Element is not Generator && x.Element is not Consumer).ToList(); }
        public double AverageLoadPercent { get; set; }
        public ElementAnalysis PeakLoadPercent { get; set; }
        public double AverageLoss { get; set; }
        public ElementAnalysis PeakLoss { get; set; }
        public double TotalLoss { get; set; }
        public double AverageOptimalityPercentDelta { get; set; }
        public ElementAnalysis PeakOptimalityPercentDelta { get; set; }
    }
}
