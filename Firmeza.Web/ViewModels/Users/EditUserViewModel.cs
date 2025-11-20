using System.ComponentModel.DataAnnotations;

namespace Firmeza.Web.ViewModels.Users
{
    public class EditUserViewModel : CreateUserViewModel 
    {
        [Required]    
        public string Id { get; set; }
    }
}
