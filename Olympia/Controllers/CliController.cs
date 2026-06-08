using Microsoft.AspNetCore.Mvc;
using Olympia.Interfaces;
using Olympia.Models;

namespace Olympia.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CliController : ControllerBase
    {

        public ERP210OLYMPIA_FContext _context;
        public CliController(ERP210OLYMPIA_FContext context, IArtRepository artRepository)
        
        {
            this._context = context;
        }

        [HttpGet]
        public IActionResult GetAllClients()
        {
            var clients = _context.CLI.ToList(); 
            return Ok(clients);
        }
        [HttpGet("client/comm/{reference}")]
        public IActionResult GetClientByCommercial(string reference)
        {
            var clients = _context.CLI.Where(a=>a.REPR_0001.Trim()== reference.Trim()).ToList();
            return Ok(clients);
        }
        [HttpGet("client/contact/{tiers}")]
        public IActionResult GetClientByContact(string tiers)
        {
            var contacts = _context.T2.Where(a => a.TIERS.Trim() == tiers.Trim() && a.DOS == 2).ToList();
            return Ok(contacts);
        }
        [HttpGet("{id}")]
        public IActionResult GetClientById(int id)
        {
            var client = _context.CLI.Where(a=>a.CLI_ID==id).FirstOrDefault();
            return Ok(client);
        }
        [HttpGet("tiers/{tiers}")]
        public IActionResult GetClientByTiers(string tiers)
        {
            var clitiers = _context.CLI.Where(a => a.TIERS == tiers).FirstOrDefault();
            return Ok(clitiers);
        }
        [HttpGet("commercial")]
        public IActionResult GetAllComm()
        {
            var clients = _context.VRP.ToList();
            return Ok(clients);
        }

        [HttpGet("commercial/{id}")]
        public IActionResult GetComById(int id)
        {
            var client = _context.VRP.Where(a => a.VRP_ID == id);
            return Ok(client);
        }
        [HttpGet("contact")]
        public IActionResult GetAllContact()
        {
            var clients = _context.T2.ToList();
            return Ok(clients);
        }

        [HttpGet("contact/{id}")]
        public IActionResult GetContactById(int id)
        {
            var client = _context.T2.Where(a => a.T2_ID == id);
            return Ok(client);
        }
    }
}
