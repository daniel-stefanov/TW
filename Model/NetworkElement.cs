namespace TW.Model
{
    public class NetworkElement
    {
        public string Id { get; init; }
        public List<Link> Links { get; set; } = new List<Link>();
        public double MaxCapacity { get; set; } = 0;

        public virtual double Demand
        {
            get
            {
                return (1 / CalculateLoss(this)) * RawDemand;
            }
        }

        protected virtual double RawDemand
        {
            get
            {
                return Links.Where(x => x.NodeIn == this).Sum(x => x.Demand);
            }
        }

        public double LoadRatio
        {
            get
            {
                return RawDemand / MaxCapacity;
            }
        }

        public Func<NetworkElement, double> CalculateLoss { get; set; } = (NetworkElement element) => 0;
        public Func<NetworkElement, double> CalculateCost { get; set; } = (NetworkElement element) => 0;

        public NetworkElement(string id)
        {
            Id = id;
        }
    }
}
