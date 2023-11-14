namespace TW.Model
{
    //Vertex
    public class Node : NetworkElement
    {
        public List<Link> Links { get; set; } = new List<Link>();

        protected override double RawDemand
        {
            get
            {
                double result = Links.Where(x => x.NodeIn == this).Sum(x => x.Demand);
                return result;
            }
        }

        public Node(string id) : base(id)
        {
        }
    }
}
