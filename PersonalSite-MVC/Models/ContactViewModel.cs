using System.ComponentModel.DataAnnotations;

namespace PersonalSite_MVC.Models
{
    public class ContactViewModel
    {
        [Required(ErrorMessage = "* Required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "* Required")]
        [DataType(DataType.EmailAddress)] //looks for certian formatting  (@, .com, ect)
        public string Email { get; set; }

        [Required(ErrorMessage = "* Required")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "* Required")]
        [DataType(DataType.MultilineText)]// makes text box bigger for this field
        public string Message { get; set; }
    }
}
