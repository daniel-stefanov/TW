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
                return 0;
                //double result = Links.Where(x => x.NodeIn == this).Sum(x => x.Demand) + (Production * -1);
                //return result;
            }
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
