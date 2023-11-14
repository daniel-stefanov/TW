namespace TW.Model
{
    //Source
    public class Generator : Node
    {
        public double Production { get; set; } = 0;
        public override double Demand
        {
            get
            {
                return RawDemand;
            }
        }

        public override double RawDemand
        {
            get
            {
                return Production * -1;
            }
            set { }
        }

        public override double LoadRatio
        {
            get
            {
                return Production / MaxCapacity;
            }
        }

        public Generator(string id) : base(id)
        {
        }
    }
}
