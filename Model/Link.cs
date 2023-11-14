namespace TW.Model
{
    //Edge
    public class Link : NetworkElement
    {
        public Node NodeIn { get; set; }
        public Node NodeOut { get; set; }

        public override double RawDemand { get => NodeOut.Demand; set { } }

        public Link(string id) : base(id)
        {
        }
    }
}
