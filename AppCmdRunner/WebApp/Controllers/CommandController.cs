using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class CommandController : ControllerBase
    {
        private readonly ILogger<CommandController> _logger;
        private readonly RabbitService _service;

        public CommandController(ILogger<CommandController> logger, RabbitService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpPost]
        public IActionResult Run([FromBody] CommandParam model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _service.Send(model);

            return Ok();
        }
    }
}
