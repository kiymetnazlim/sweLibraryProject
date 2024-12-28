namespace LibraryProject.Models
{
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


        public decimal CalculateOverdueFine()
        {
            const decimal dailyFine = 0.50m; // Günlük ceza miktarı (50 kuruş)

            // Eğer iade edilmemişse bugünün tarihini kullanarak hesapla
            DateTime effectiveReturnDate = ReturnDate ?? DateTime.Now;

            // Eğer geçerlilik süresi aşılmışsa gün farkını hesapla
            if (effectiveReturnDate > DueDate)
            {
                int overdueDays = (effectiveReturnDate - DueDate).Days;
                return overdueDays * dailyFine;
            }

            return 0;
        }
    }


}
