namespace TW.Model
{
    //Edge
    public class Link : Node
    {
        public Intermediary NodeIn { get; set; }
        public Intermediary NodeOut { get; set; }

        public override double RawDemand { get => NodeOut.Demand; set { } }

        public Link(string id) : base(id)
        {
        }
    }
}
