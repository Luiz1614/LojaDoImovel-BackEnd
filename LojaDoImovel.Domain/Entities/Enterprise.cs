public class Enterprise
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public bool IsActive { get; set; }

    public ICollection<Property> Properties { get; set; }
}