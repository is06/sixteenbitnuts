namespace SixteenBitNuts
{
    public class EventTrigger : Entity
    {
        public bool IsEnabled { get; set; }

        public EventTrigger(Map map, string name) : base(map, name)
        {

        }
    }
}
