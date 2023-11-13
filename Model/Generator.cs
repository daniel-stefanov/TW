namespace TW.Model
{
    //Source
    public class Generator : Node
    {
        public double Production { get; set; } = 0;
        public override double Demand
        {
            get { return Links.Where(x => x.NodeIn == this).Sum(x => x.Demand) + (Production * -1); }
        }

        public Generator(string id) : base(id)
        {
        }
    }
}
