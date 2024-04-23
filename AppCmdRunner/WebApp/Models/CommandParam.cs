using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class CommandParam
    {

        [Required]
        public string File { get; set; }

        public string Args { get; set; }
    }
}
