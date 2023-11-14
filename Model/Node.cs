namespace TW.Model
{
    public class Node
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

        public Func<Node, double> GetLoss { get; set; } = (Node element) => 1;
        public Func<Node, double> CalculateCost { get; set; } = (Node element) => 1;

        public Node(string id)
        {
            Id = id;
        }
    }
}
