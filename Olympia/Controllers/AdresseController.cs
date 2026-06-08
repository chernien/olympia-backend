using Microsoft.AspNetCore.Mvc;
using Olympia.Models;

namespace Olympia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdressesController : ControllerBase
    {
        private readonly ERP210OLYMPIA_FContext _context;

        public AdressesController(ERP210OLYMPIA_FContext context)
        {
            _context = context;
        }

        // POST: api/adresses
        [HttpPost]
        public IActionResult PostAdresse([FromBody] Adresse adresse)
        {
            if (adresse == null || adresse.Commande_Id <= 0)
                return BadRequest(new { message = "Données d'adresse invalides." });

            _context.Adresses.Add(adresse);
            _context.SaveChanges();

            return Ok(new { message = "Adresse enregistrée avec succès.", adresse_Id = adresse.Adresse_Id });
        }
    }
}
