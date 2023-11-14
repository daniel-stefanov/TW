namespace TW.Model
{
    public abstract class NetworkElement
    {
        public string Id { get; init; }
        public double MaxCapacity { get; set; } = 0;

        public virtual double Demand
        {
            get
            {
                double result = (1 / GetLossRatio(this)) * RawDemand;
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

        protected abstract double RawDemand
        {
            get;
        }

        public virtual double LoadRatio
        {
            get
            {
                double result = RawDemand / MaxCapacity;
                return result;
            }
        }

        public Func<NetworkElement, double> GetLossRatio { get; set; } = (NetworkElement element) => 1;
        public Func<NetworkElement, double> CalculateCost { get; set; } = (NetworkElement element) => 1;

        public NetworkElement(string id)
        {
            Id = id;
        }
    }
}
