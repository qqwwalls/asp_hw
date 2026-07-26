using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductsApi.Models;

[Table("refresh_tokens")]
public class RefreshToken
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("token")]
    public string Token { get; set; } = string.Empty;

    [Column("expires")]
    public DateTime Expires { get; set; }

    [Column("created")]
    public DateTime Created { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }
}
