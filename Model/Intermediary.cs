namespace TW.Model
{
    //Vertex
    public class Intermediary : Node
    {
        public List<Link> Links { get; set; } = new List<Link>();

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
