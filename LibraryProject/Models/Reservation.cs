namespace LibraryProject.Models
{
    public class Reservation
    {
        public int ReservationId { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        public DateTime ReservationDate { get; set; }
        public DateTime AvailableDate { get; set; }
        public string ReservationStatus { get; set; }


        // Navigation properties
        public User User { get; set; }
        public Book Book { get; set; }

    }

}
