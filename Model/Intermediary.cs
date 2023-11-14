namespace TW.Model
{
    public class Intermediary : Node
    {
        public override double RawDemand
        {
            get
            {
                double result = Links.Where(x => x.NodeIn == this).Sum(x => x.Demand);
                return result;
            }
            set { }
        }

        public Intermediary(string id) : base(id)
        {
        }
    }
}
