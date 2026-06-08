using Microsoft.AspNetCore.Mvc;
using Olympia.Interfaces;

namespace Olympia.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TarRemiseController : ControllerBase
    {
        private readonly ITarRemiseRepository _tarRemiseRepository;

        public TarRemiseController(ITarRemiseRepository tarRemiseRepository)
        {
            _tarRemiseRepository = tarRemiseRepository;
        }

        [HttpGet]
        public IActionResult GetTarRemise()
        {
            var tarRemise = _tarRemiseRepository.GetTarRemise();

            if (tarRemise == null || tarRemise.Count == 0)
            {
                return NotFound();
            }

            return Ok(tarRemise);
        }

        [HttpGet("{reference}")]
        public IActionResult GetTarRemiseByRef(string reference)
        {
            var tarRemise = _tarRemiseRepository.GetTarRemiseByRef(reference);

            if (tarRemise == null)
            {
                return NotFound();
            }

            return Ok(tarRemise);
        }
        [HttpGet("{reference}/{remcod}")]
        public IActionResult GetTarRemiseByRefByRemcod(string reference, string remcod)
        {
            var tarRemise = _tarRemiseRepository.GetTarRemiseByRefByRemcod(reference, remcod);

            if (tarRemise == null)
            {
                return NotFound();
            }

            return Ok(tarRemise);
        }
    }
}
