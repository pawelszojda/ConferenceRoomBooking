using System.ComponentModel.DataAnnotations;

namespace ConferenceRoomBooking.ViewModels
{
    public class CreateReservationViewModel
    {
        [Required(ErrorMessage = "Wybierz salę")]
        [Display(Name = "Sala konferencyjna")]
        public int RoomId { get; set; }
        
        [Required(ErrorMessage = "Podaj datę i czas rozpoczęcia")]
        [Display(Name = "Data i godzina rozpoczęcia")]
        public DateTime StartTime { get; set; } = DateTime.Now.AddDays(1);
        
        [Required(ErrorMessage = "Podaj datę i czas zakończenia")]
        [Display(Name = "Data i godzina zakończenia")]
        public DateTime EndTime { get; set; } = DateTime.Now.AddDays(1).AddHours(2);
        
        [Required(ErrorMessage = "Opisz cel rezerwacji")]
        [StringLength(500, ErrorMessage = "Opis nie może przekraczać 500 znaków")]
        [Display(Name = "Cel spotkania")]
        public string Purpose { get; set; } = string.Empty;
    }
}