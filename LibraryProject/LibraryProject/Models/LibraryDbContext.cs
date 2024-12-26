using Microsoft.EntityFrameworkCore;

namespace LibraryProject
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
        {
        }

        // DbSets for each table
        public DbSet<Book> Books { get; set; }
        public DbSet<Borrowing> Borrowing { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Favority> Favorities { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure the primary keys
            modelBuilder.Entity<Book>().HasKey(b => b.BookId);
            modelBuilder.Entity<Borrowing>().HasKey(b => b.BorrowId);
            modelBuilder.Entity<Category>().HasKey(c => c.CategoryId);
            modelBuilder.Entity<Favority>().HasKey(f => f.FavorityId);
            modelBuilder.Entity<Reservation>().HasKey(r => r.ReservationId);
            modelBuilder.Entity<User>().HasKey(u => u.UserId);

            // Configure relationships with navigation properties and foreign keys
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Category)
                .WithMany(c => c.Books) // Category has many Books
                .HasForeignKey(b => b.CategoryId);

            modelBuilder.Entity<Borrowing>()
                .HasOne(b => b.User)
                .WithMany(u => u.Borrowings) // User has many Borrowings
                .HasForeignKey(b => b.UserId);

            modelBuilder.Entity<Borrowing>()
                .HasOne(b => b.Book)
                .WithMany(bk => bk.Borrowings) // Book has many Borrowings
                .HasForeignKey(b => b.BookId);

            modelBuilder.Entity<Favority>()
                .HasOne(f => f.User)
                .WithMany(u => u.Favorities) // User has many Favorities
                .HasForeignKey(f => f.UserId);

            modelBuilder.Entity<Favority>()
                .HasOne(f => f.Book)
                .WithMany(b => b.Favorities) // Book has many Favorities
                .HasForeignKey(f => f.BookId);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reservations) // User has many Reservations
                .HasForeignKey(r => r.UserId);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Book)
                .WithMany(b => b.Reservations) // Book has many Reservations
                .HasForeignKey(r => r.BookId);

            base.OnModelCreating(modelBuilder);
        }
    }

    // Entity models
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
        public ICollection<Borrowing> Borrowings { get; set; }
        public ICollection<Favority> Favorities { get; set; }
        public ICollection<Reservation> Reservations { get; set; }
    }

    public class Borrowing
    {
        public int BorrowId { get; set; }
        public string BookName { get; set; }
        public string Author { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        public string Status { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public decimal? OverdueFine { get; set; }



        // Navigation properties
        public User User { get; set; }
        public Book Book { get; set; }


        public static decimal CalculateOverdueFine(DateTime dueDate, DateTime? returnDate)
        {
            if (returnDate == null || returnDate <= dueDate)
            {
                return 0; // Ceza yok
            }

            TimeSpan overdueDays = returnDate.Value - dueDate;
            return (decimal)overdueDays.Days * 1; // Günlük ceza: 1 TL
        }

    }

    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // Navigation property
        public ICollection<Book> Books { get; set; }
    }

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

    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string? Email { get; set; }
        public string Password { get; set; }
        public string? Role { get; set; }
        public string? ContactDetails { get; set; }

        // Navigation properties
        public ICollection<Borrowing> Borrowings { get; set; }
        public ICollection<Favority> Favorities { get; set; }
        public ICollection<Reservation> Reservations { get; set; }
    }
}
