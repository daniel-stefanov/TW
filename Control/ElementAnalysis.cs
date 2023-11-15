using TW.Model;

namespace TW.Control
{
    public class ElementAnalysis
    {
        public NetworkElement Element { get; set; }
        public double LoadPercent { get; set; }
        public double Loss { get; set; }
        public double OptimalLoadPercent { get; set; }
        public double OptimalityPercentDelta { get => Math.Abs(LoadPercent - OptimalLoadPercent); }
    }
}
