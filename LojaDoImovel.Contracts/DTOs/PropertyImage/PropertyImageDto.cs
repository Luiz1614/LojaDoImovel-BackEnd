namespace LojaDoImovel.Contracts.DTOs.PropertyImageDtos;

public class PropertyImageDto
{
    public int PropertyId { get; set; }

    public string ImagePath { get; set; }

    public bool IsMain { get; set; }
    public int DisplayOrder { get; set; }
}
