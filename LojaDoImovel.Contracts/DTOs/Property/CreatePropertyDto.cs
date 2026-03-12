namespace LojaDoImovel.Contracts.DTOs.Property;

public class CreatePropertyDto
{
    public int EnterpriseId { get; set; }
    public string Code { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public decimal SalePrice { get; set; }
    public decimal? RentalPrice { get; set; }
    public decimal? CondoFee { get; set; }
    public decimal? PropertyTax { get; set; }
    public string Street { get; set; }
    public string Number { get; set; }
    public string? Complement { get; set; }
    public string Neighborhood { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string ZipCode { get; set; }
    public int Bedrooms { get; set; }
    public int Suites { get; set; }
    public int Bathrooms { get; set; }
    public int LivingRooms { get; set; }
    public int ParkingSpaces { get; set; }
    public decimal? PrivateArea { get; set; }
    public decimal? TotalArea { get; set; }
    public string PropertyType { get; set; }
    public string Purpose { get; set; }
    public bool IsPublished { get; set; }
    public bool IsFeatured { get; set; }
    public int CreatedByUserId { get; set; }
}