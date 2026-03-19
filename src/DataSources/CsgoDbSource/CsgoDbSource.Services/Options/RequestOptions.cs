using System;
using System.ComponentModel.DataAnnotations;

namespace CsgoDbSource.Services.Options;


public sealed class RequestOptions
{
    [Required]
    public required string BasePage { get; set; }
    [Required]
    public required string WeaponsPage { get; set; }
    [Required]
    public required string SkinsPage { get; set; }
    [Required]
    public required string AdditionalInfoPage { get; set; }
    [Required]
    public required string GlovesPage { get; set; }
    [Required]
    public required string AgentsPage { get; set; }
}