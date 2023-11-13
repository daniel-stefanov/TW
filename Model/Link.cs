namespace TW.Model
{
    //Edge
    public class Link : NetworkElement
    {
        public Node NodeIn { get; set; }
        public Node NodeOut { get; set; }

        protected override double RawDemand => NodeOut.Demand;

        public Link(string id) : base(id)
        {
        }
    }
}
