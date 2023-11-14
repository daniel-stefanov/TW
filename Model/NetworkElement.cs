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
                double result = (1 / CalculateLoss(this)) * RawDemand;
                return result;
            }
        }

        protected abstract double RawDemand
        {
            get;
        }

        public double LoadRatio
        {
            get
            {
                double result = RawDemand / MaxCapacity;
                return result;
            }
        }

        public Func<NetworkElement, double> CalculateLoss { get; set; } = (NetworkElement element) => 1;
        public Func<NetworkElement, double> CalculateCost { get; set; } = (NetworkElement element) => 1;

        public NetworkElement(string id)
        {
            Id = id;
        }
    }
}
