namespace LibraryProject.Models
{
    public class Favority
    {
        public int FavorityId { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        public DateTime? AddedDate { get; set; }

        // Navigation properties
        public User User { get; set; }
        public Book Book { get; set; }
    }

}
