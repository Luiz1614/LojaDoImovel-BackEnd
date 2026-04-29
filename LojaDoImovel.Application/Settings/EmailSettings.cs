using System.ComponentModel.DataAnnotations;

namespace LojaDoImovel.Application.Settings;

public class EmailSettings
{
    [Required] 
    public string Host { get; set; } = string.Empty;
    [Required] 
    public int Port { get; set; }
    [Required] 
    public bool EnableSsl { get; set; }
    [Required] 
    public string Username { get; set; } = string.Empty;
    [Required] 
    public string Password { get; set; } = string.Empty;
}