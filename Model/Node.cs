using System.Collections.ObjectModel;

namespace TW.Model
{
    //Vertex
    public class Node : NetworkElement
    {
        public ObservableCollection<Link> Links { get; set; } = new ObservableCollection<Link>();

        public Node(string id) : base(id)
        {
            Id = id;
            Links.CollectionChanged += Links_CollectionChanged;
        }

        private void Links_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnElementChanged(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(Links)));
        }
    }
}
