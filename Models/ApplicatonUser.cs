using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Models;
public class ApplicationUser :IdentityUser{
    [Required, MaxLength(30)]
    public string FirstName{get;set;}
    [Required, MaxLength(30)]
    public string LastName{get;set;}
}