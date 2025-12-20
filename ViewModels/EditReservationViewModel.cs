using System.ComponentModel.DataAnnotations;

namespace ConferenceRoomBooking.ViewModels
{
    public class EditReservationViewModel
    {
        public int Id { get; set; }
        
        public int RoomId { get; set; }
        
        public string RoomName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Podaj datę i czas rozpoczęcia")]
        [Display(Name = "Data i godzina rozpoczęcia")]
        public DateTime StartTime { get; set; }
        
        [Required(ErrorMessage = "Podaj datę i czas zakończenia")]
        [Display(Name = "Data i godzina zakończenia")]
        public DateTime EndTime { get; set; }
        
        [Required(ErrorMessage = "Opisz cel rezerwacji")]
        [StringLength(500, ErrorMessage = "Opis nie może przekraczać 500 znaków")]
        [Display(Name = "Cel spotkania")]
        public string Purpose { get; set; } = string.Empty;
    }
}