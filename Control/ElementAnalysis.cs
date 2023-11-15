using TW.Model;

namespace TW.Control
{
    public class ElementAnalysis
    {
        public NetworkElement Element { get; set; }
        public double Load { get; set; }
        public double Loss { get; set; }
        public double OptimalLoad { get; set; }
    }
}
