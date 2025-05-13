namespace poe.Models
{
    public class EventSearchViewModel
    {
        public List<Event> Events { get; set; }
        public string SearchString { get; set; }
        public string DateFilter { get; set; }
    }
}
