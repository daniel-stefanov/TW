using System.ComponentModel;

namespace TW.Model
{
    public class NetworkElement
    {
        public event PropertyChangedEventHandler ElementChanged;
        protected virtual void OnElementChanged(Object sender, PropertyChangedEventArgs e)
        {
            ElementChanged?.Invoke(sender, e);
        }

        public string Id { get; init; }
        private double _maxCapacity = 0;
        public virtual double MaxCapacity 
        { 
            get => _maxCapacity; 
            set
            {
                _maxCapacity = value;
                OnElementChanged(this, new PropertyChangedEventArgs(nameof(MaxCapacity)));
            }
        }
        public virtual double Demand
        {
            get
            {
                double result = LossFn(this) + RawDemand;
                return result;
            }
        }

        private double _rawDemand = 0;
        public virtual double RawDemand 
        { 
            get => _rawDemand;
            set
            {
                _rawDemand = value;
                OnElementChanged(this, new PropertyChangedEventArgs(nameof(RawDemand)));
            }
        }

        public virtual double LoadRatio
        {
            get
            {
                double result = RawDemand / MaxCapacity;
                return result;
            }
        }

        private Func<NetworkElement, double> _lossFn = (NetworkElement element) => 1;
        public Func<NetworkElement, double> LossFn 
        { 
            get => _lossFn;
            set
            {
                _lossFn = value;
                OnElementChanged(this, new PropertyChangedEventArgs(nameof(LossFn)));
            }
        } 
        private Func<NetworkElement, double> _costFn = (NetworkElement element) => 1;
        public Func<NetworkElement, double> CostFn
        { 
            get => _costFn;
            set
            {
                _costFn = value;
                OnElementChanged(this, new PropertyChangedEventArgs(nameof(CostFn)));
            }
        } 

        public NetworkElement(string id)
        {
            Id = id;
        }
    }
}
