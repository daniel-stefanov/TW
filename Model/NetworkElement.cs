namespace TW.Model
{
    public class NetworkElement
    {
        public string Id { get; init; }
        public virtual double MaxCapacity { get; set; } = 0;
        public virtual double Demand
        {
            get
            {
                double result = LossFn(this) + RawDemand;
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

        public Func<NetworkElement, double> LossFn { get; set; } = (NetworkElement element) => 1;
        public Func<NetworkElement, double> CalculateCost { get; set; } = (NetworkElement element) => 1;

        public NetworkElement(string id)
        {
            Id = id;
        }
    }
}
