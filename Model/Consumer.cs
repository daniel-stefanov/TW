namespace TW.Model
{
    //Sink
    public class Consumer : Node
    {
        public override double Demand => RawDemand;

        public new double RawDemand { get; set; } = 0;

        public Consumer(string id) : base(id)
        {
        }
    }
}
