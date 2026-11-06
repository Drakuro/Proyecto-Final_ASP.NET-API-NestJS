using System.ComponentModel.DataAnnotations;

namespace PFNET.Users.Dto;

public class ChangeUserLanguageDto
{
    [Required]
    public string LanguageName { get; set; }
}