namespace LojaDoImovel.Contracts.DTOs.Enterprise;

public class EnterpriseDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public required string Neighborhood { get; set; }
    public required string City { get; set; }
    public required string State { get; set; }
    public bool IsActive { get; set; }
}
