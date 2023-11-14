namespace TW.Model
{
    //Sink
    public class Consumer : Node
    {
        public override double Demand => RawDemand;

        public override double MaxCapacity => RawDemand;

        public override double RawDemand { get; set; }

        public Consumer(string id) : base(id)
        {
        }
    }
}
