using System.ComponentModel.DataAnnotations;

namespace Monshi.Web.Models;

public class OtpCodePost
{
    [Required]
    [MaxLength(6)]
    public string Code { get; set; }
}