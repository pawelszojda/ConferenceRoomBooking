using System.ComponentModel.DataAnnotations;

namespace ConferenceRoomBooking.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email jest wymagany")]
        [EmailAddress(ErrorMessage = "Nieprawidłowy format email")]
        public string Email { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Hasło jest wymagane")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        
        [Display(Name = "Zapamiętaj mnie")]
        public bool RememberMe { get; set; }
    }
}