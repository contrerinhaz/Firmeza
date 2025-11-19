using System.ComponentModel.DataAnnotations;

namespace Firmeza.Web.ViewModels.Users
{
    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [RegularExpression(@"^[0-9]{6,12}$", ErrorMessage = "Número de documento inválido.")]
        public string DocumentNumber { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Correo inválido.")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Número de teléfono inválido.")]
        public string Phone { get; set; }

        public DateTime RegisterDate { get; set; }

        [Required]
        [StringLength(50)]
        public string UserName { get; set; }
    }
}
