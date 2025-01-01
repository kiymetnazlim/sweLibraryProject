namespace LibraryProject.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string? Email { get; set; }
        public string Password { get; set; }
        public string? Role { get; set; }
        public string? ContactDetails { get; set; }

        // Navigation properties
        
        public ICollection<Favority> Favorities { get; set; }
        public ICollection<Reservation> Reservations { get; set; }
    }
}
