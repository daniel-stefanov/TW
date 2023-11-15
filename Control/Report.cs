namespace TW.Control
{
    public class Report
    {
        public DateTime Timestamp { get; init; }
        public List<PathAnalysis> Paths { get; set; }
        public double SupplyDemandRatio { get; set; }
        public double TotalLossess { get; set; }
        public double AverageOptimalityDelta { get; set; }
        public List<string> Warnings { get; set; }
        public List<string> Recommendations { get; set; }

        public Report()
        {
            Timestamp = DateTime.Now;
        }
    }
}
