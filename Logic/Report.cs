namespace TW.Logic
{
    public class Report
    {
        public DateTime Timestamp { get; private set; }
        public List<PathAnalysis> Paths { get; set; }
        public List<string> Warnings { get; set; }
        public List<string> Recommendations { get; set; }
    }
}
