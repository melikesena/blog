public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }

    // Kategorinin blog yazıları
    public ICollection<Blog> Blogs { get; set; }
}