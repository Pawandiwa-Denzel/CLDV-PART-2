namespace poe.Models
{
    public class VenueSearchViewModel
    {
        public List<Venue> Venues { get; set; }
        public List<string> Locations { get; set; }
        public string SearchString { get; set; }
        public string LocationFilter { get; set; }
        public int? CapacityFilter { get; set; }
    }
}
