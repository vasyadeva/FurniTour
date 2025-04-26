namespace FurniTour.Server.Models.Item
{
    public class ItemFilterModel
    {
        public int colorID { get; set; }
        public int categoryID { get; set; }
        public int manufacturerID { get; set; }
        public string masterID { get; set; }
        public int minPrice { get; set; }
        public int maxPrice { get; set; }
        public string? searchString { get; set; }
    }
}
