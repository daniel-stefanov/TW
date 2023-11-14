namespace TW.Model
{
    //Vertex
    public class Node : NetworkElement
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

        public Node(string id) : base(id)
        {
        }
    }
}
