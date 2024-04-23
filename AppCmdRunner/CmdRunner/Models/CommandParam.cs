using System.ComponentModel.DataAnnotations;

namespace CmdRunner.Models
{
    public class CommandParam
    {

        public string File { get; set; }

        public string Args { get; set; }
    }
}
