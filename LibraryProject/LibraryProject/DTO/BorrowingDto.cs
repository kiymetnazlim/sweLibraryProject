namespace LibraryProject.DTO
{
    public class BorrowingDto
    {
        public int BorrowId { get; set; }          // Ödünç ID'si
        public string UserName { get; set; }       // Kullanıcı Adı
        public string BookTitle { get; set; }      // Kitap Başlığı
        public DateTime BorrowDate { get; set; }   // Ödünç Alma Tarihi
        public DateTime DueDate { get; set; }      // Son Teslim Tarihi
        public DateTime? ReturnDate { get; set; }  // İade Tarihi (Nullable)
        public decimal? OverdueFine { get; set; }  // Gecikme Cezası (Nullable)
    }
}
