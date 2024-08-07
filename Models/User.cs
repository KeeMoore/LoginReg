// #pragma warning disable CS8618
// using System;
// using System.ComponentModel.DataAnnotations;

// namespace LoginReg.Models
// {
//     public class User
//     {
//         public int Id { get; set; }

//         [Required, MinLength(2)]
//         public string FirstName { get; set; }

//         [Required, MinLength(2)]
//         public string LastName { get; set; }

//         [Required, EmailAddress]
//         public string Email { get; set; }

//         [Required, MinLength(8)]
//         [DataType(DataType.Password)]
//         [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
//         public string ConfirmPassword { get; set; }

//         public DateTime CreatedAt { get; set; } = DateTime.Now;
//         public DateTime UpdatedAt { get; set; } = DateTime.Now;

//         [Required]
//         [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
//         [DataType(DataType.Password)]
//         public string Password { get; internal set; }
//         public string PasswordHash { get; internal set; }
//     }
// }
#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;

namespace LoginReg.Models
{
    public class User
    {
        [Required(ErrorMessage = "First name is required.")]
        [MinLength(2, ErrorMessage = "First name must be at least 2 characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [MinLength(2, ErrorMessage = "Last name must be at least 2 characters.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Password confirm must match password.")]
        public string ConfirmPassword { get; set; }

        public string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int Id { get; internal set; }
    }
}
