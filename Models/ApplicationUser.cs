using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    // Kullanıcıya özel alanlar eklemek istersen buraya ekleyebilirsin.
    // Örneğin, kullanıcının adını ve soyadını tutalım.
    public string FullName { get; set; }

    // Kullanıcıların blog yazılarına erişimi olacak.
    public ICollection<Blog> Blogs { get; set; }
}