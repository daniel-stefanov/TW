namespace TW.Model
{
    public abstract class NetworkElement
    {
        public string Id { get; init; }
        public virtual double MaxCapacity { get; set; } = 0;

        public virtual double Demand
        {
            get
            {
                double result = GetLoss(this) + RawDemand;
                return result;
            }
        }

        public virtual double FactualLoss
        {
            get
            {
                double result = Demand - RawDemand;
                return result;
            }
        }

        public virtual double RawDemand { get; set; } = 0;

        public virtual double LoadRatio
        {
            get
            {
                double result = RawDemand / MaxCapacity;
                return result;
            }
        }

        public Func<NetworkElement, double> GetLoss { get; set; } = (NetworkElement element) => 1;
        public Func<NetworkElement, double> CalculateCost { get; set; } = (NetworkElement element) => 1;

        public NetworkElement(string id)
        {
            Id = id;
        }
    }
}
