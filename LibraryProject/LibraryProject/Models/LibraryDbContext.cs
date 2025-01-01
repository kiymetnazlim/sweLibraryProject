using Microsoft.EntityFrameworkCore;
using LibraryProject.Models;

namespace LibraryProject
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
        {
        }

        // DbSets for each table
        public DbSet<Book> Books { get; set; }
        
        public DbSet<Category> Categories { get; set; }
        public DbSet<Favority> Favorities { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure the primary keys
            modelBuilder.Entity<Book>().HasKey(b => b.BookId);
           
            modelBuilder.Entity<Category>().HasKey(c => c.CategoryId);
            modelBuilder.Entity<Favority>().HasKey(f => f.FavorityId);
            modelBuilder.Entity<Reservation>().HasKey(r => r.ReservationId);
            modelBuilder.Entity<User>().HasKey(u => u.UserId);

            // Configure relationships with navigation properties and foreign keys
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Category)
                .WithMany(c => c.Books) // Category has many Books
                .HasForeignKey(b => b.CategoryId);

            

           

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
    

   


      

   
   

    
}
