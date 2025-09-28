public class Offer
{
    public int Id { get; set; }
    public string Title { get; set; }       // e.g., "20% Off Pizza"
    public string Description { get; set; } // e.g., "Valid until Oct 31"
    public decimal Discount { get; set; }   // Optional numeric discount
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string ImageUrl { get; set; }    // Optional image for the offer
}
