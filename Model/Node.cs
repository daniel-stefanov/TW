namespace TW.Model
{
    //Vertex
    public class Node : NetworkElement
    {
        public List<Link> Links { get; set; } = new List<Link>();

        public Node(string id) : base(id)
        {
            Id = id;
        }
    }
}
