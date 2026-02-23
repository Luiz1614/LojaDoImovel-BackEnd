public class Property
{
    public int Id { get; set; }

    // Tenant
    public int EnterpriseId { get; set; }
    public Enterprise Enterprise { get; set; }

    // Identification
    public required string Code { get; set; }
    public required string Title { get; set; }
    public string Description { get; set; } = string.Empty;

    // Pricing
    public decimal SalePrice { get; set; }
    public decimal? RentalPrice { get; set; }
    public decimal? CondoFee { get; set; }
    public decimal? PropertyTax { get; set; }

    // Location
    public required string Street { get; set; }
    public required string Number { get; set; }
    public string Complement { get; set; } = string.Empty;
    public required string Neighborhood { get; set; }
    public required string City { get; set; }
    public required string State { get; set; }
    public required string ZipCode { get; set; }

    // Property Details
    public int Bedrooms { get; set; }
    public int Suites { get; set; }
    public int Bathrooms { get; set; }
    public int LivingRooms { get; set; }
    public int ParkingSpaces { get; set; }

    public decimal? PrivateArea { get; set; }
    public decimal? TotalArea { get; set; }

    public required string PropertyType { get; set; }
    public required string Purpose { get; set; }

    public bool IsPublished { get; set; }
    public bool IsFeatured { get; set; }

    public int CreatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public ICollection<PropertyImage>? Images { get; set; }
}
