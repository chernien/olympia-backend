using Microsoft.AspNetCore.Mvc;
using Olympia.Interfaces;

namespace Olympia.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RemiseController : ControllerBase
    {
        private readonly IRemiseRepository _remiseRepository;

        public RemiseController(IRemiseRepository remiseRepository)
        {
            _remiseRepository = remiseRepository;
        }

        [HttpGet]
        public IActionResult GetRemises()
        {
            var remises = _remiseRepository.GetRemises();

            if (remises == null || remises.Count == 0)
            {
                return NotFound();
            }

            return Ok(remises);
        }

        [HttpGet("{reference}/{remcod}")]
        public IActionResult GetRemiseByRefByRemcod(string reference, string remcod)
        {
            var remise = _remiseRepository.GetRemiseByRefByRemcod(reference, remcod);

            if (remise == null)
            {
                return NotFound();
            }

            return Ok(remise);
        }
    }
}
