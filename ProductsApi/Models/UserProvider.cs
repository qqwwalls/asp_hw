using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductsApi.Models;

[Table("users_providers")]
public class UserProvider
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("user_id")]
    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }

    [Required]
    [Column("provider_id")]
    public int ProviderId { get; set; }

    [ForeignKey(nameof(ProviderId))]
    public Provider? Provider { get; set; }

    [Required]
    [MaxLength(255)]
    [Column("number_provider")]
    public string NumberProvider { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
