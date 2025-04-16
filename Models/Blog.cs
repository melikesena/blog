namespace blog.Models{
    public class Blog
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime PublishedDate { get; set; }

        // User ilişkisi
        public string ApplicationUserId { get; set; }  // Foreign Key
        public ApplicationUser User { get; set; } // Navigation Property

        // Kategori ilişkisi
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        // Görsel (Opsiyonel)
        public string ImageUrl { get; set; }
    }
}