public class PropertyImage
{
    public int Id { get; set; }
    public int PropertyId { get; set; }

    public string ImagePath { get; set; }

    public bool IsMain { get; set; }
    public int DisplayOrder { get; set; }

    public Property Property { get; set; }
}
