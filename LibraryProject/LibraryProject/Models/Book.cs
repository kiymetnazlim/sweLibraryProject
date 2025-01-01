namespace LibraryProject.Models
{
    public class Book
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public int? PublicationYear { get; set; }
        public bool AvailabilityStatus { get; set; }
        public bool IsReserved { get; set; }
        public bool IsBorrowed { get; set; }
        public string ShelfLocation { get; set; }
        public int? CategoryId { get; set; }
        public string ISBN { get; set; }

        // Navigation properties
        public Category Category { get; set; }
        
        public ICollection<Favority> Favorities { get; set; }
        public ICollection<Reservation> Reservations { get; set; }
    }
}
