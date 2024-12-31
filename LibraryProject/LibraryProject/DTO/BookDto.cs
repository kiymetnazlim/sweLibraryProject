using System.ComponentModel.DataAnnotations;

namespace LibraryProject.DTO
{
    public class BookDto
    {
        [Required(ErrorMessage = "Kitap adı gereklidir.")]
        [StringLength(200, ErrorMessage = "Kitap adı en fazla 200 karakter uzunluğunda olabilir.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Yazar adı gereklidir.")]
        [StringLength(100, ErrorMessage = "Yazar adı en fazla 100 karakter uzunluğunda olabilir.")]
        public string Author { get; set; }

        [Required(ErrorMessage = "Tür gereklidir.")]
        [StringLength(50, ErrorMessage = "Tür en fazla 50 karakter uzunluğunda olabilir.")]
        public string Genre { get; set; }

        [Required(ErrorMessage = "ISBN gereklidir.")]
        [StringLength(13, MinimumLength = 10, ErrorMessage = "ISBN numarası 10 ile 13 karakter arasında olmalıdır.")]
        public string ISBN { get; set; }

        [Required(ErrorMessage = "Yayın yılı gereklidir.")]
        [Range(1000, 9999, ErrorMessage = "Yayın yılı geçerli bir yıl olmalıdır.")]
        public int PublicationYear { get; set; }

        [Required(ErrorMessage = "Raf konumu gereklidir.")]
        [StringLength(100, ErrorMessage = "Raf konumu en fazla 100 karakter uzunluğunda olabilir.")]
        public string ShelfLocation { get; set; }
    }
}
