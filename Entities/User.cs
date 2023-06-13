using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Entities;

[Table("users")]
public class User
{
    public int Id { get; set; }
    
    [Column("userName")]
    [Required]
    public string? UserName { get; set; }
    
    
    [Column("passwordHash")]
    [Required]
    public byte[]? PasswordHash { get; set; }
    
    [Column("passwordSalt")]
    [Required]
    public byte[]? PasswordSalt { get; set; }
}