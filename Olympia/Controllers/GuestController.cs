using Microsoft.AspNetCore.Mvc;
using Olympia.Interfaces;
using Olympia.Models;
using Olympia.Services; 
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Olympia.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GuestController : ControllerBase
    {
        private readonly IGuestRepository _guestRepository;
        private readonly SmsService _smsService;

        public GuestController(IGuestRepository guestRepository, SmsService smsService)
        {
            _guestRepository = guestRepository;
            _smsService = smsService;
        }

        [HttpGet]
        public IActionResult GetGuests()
        {
            var guests = _guestRepository.GetGuests();
            return Ok(guests);
        }

        [HttpGet("{id}")]
        public IActionResult GetGuestById(int id)
        {
            var guest = _guestRepository.GetGuestById(id);
            if (guest == null)
                return NotFound("Guest non trouvé.");

            return Ok(guest);
        }

        //[HttpPost]
        //public async Task<IActionResult> CreateGuest([FromBody] Guest guest)
        //{
        //    if (guest == null)
        //        return BadRequest("Données invalides.");

        //    if (_guestRepository.CreateGuest(guest))
        //    {
        //        if (!string.IsNullOrWhiteSpace(guest.Tel))
        //        {
        //            string message = $"Bienvenue {guest.Prenom} {guest.Nom} Numéro de fidélité {guest.Num} chez Olympia Club ";
        //            string smsResponse = await _smsService.SendSmsAsync(guest.Tel, message);
        //            return Ok(new { guest, smsResponse });
        //        }

        //        return Ok(new { guest, smsResponse = "Aucun numéro de téléphone fourni." });
        //    }

        //    return BadRequest("Impossible d'ajouter le guest.");
        //}

        //[HttpPost]
        //public async Task<IActionResult> CreateGuest([FromBody] Guest guest)
        //{
        //    if (guest == null)
        //        return BadRequest("Données invalides.");

        //    // Vérification doublon par CliId + CIN
        //    if (_guestRepository.GuestExistsByCliIdAndCin(guest.CliId, guest.Cin))
        //        return Conflict("Un client avec ce CIN existe déjà pour ce compte.");

        //    // Vérification doublon par CliId + Téléphone
        //    if (!string.IsNullOrWhiteSpace(guest.Tel) &&
        //        _guestRepository.GuestExistsByCliIdAndTel(guest.CliId, guest.Tel))
        //        return Conflict("Un client avec ce numéro de téléphone existe déjà pour ce compte.");

        //    if (_guestRepository.CreateGuest(guest))
        //    {

        //        // 🔥 Ne pas envoyer SMS pour DOS 1 et 6
        //        bool shouldSendSms = guest.Dos != 1 && guest.Dos != 6;


        //        if (shouldSendSms && !string.IsNullOrWhiteSpace(guest.Tel))
        //        {
        //            string message = $"Bienvenue {guest.Prenom} {guest.Nom} " +
        //                             $"Numéro de fidélité {guest.Num} chez Olympia Club";
        //            string smsResponse = await _smsService.SendSmsAsync(guest.Tel, message);
        //            return Ok(new { guest, smsResponse });
        //        }
        //        return Ok(new { guest, smsResponse = "Aucun numéro de téléphone fourni." });
        //    }

        //    return Ok(new
        //    {
        //        guest,
        //        smsResponse = "SMS non envoyé pour ce DOS."
        //    });
        //}



        //    return BadRequest("Impossible d'ajouter le guest.");
        //}


        [HttpPost]
        public async Task<IActionResult> CreateGuest([FromBody] Guest guest)
        {
            if (guest == null)
                return BadRequest("Données invalides.");

            // Vérification doublon par CliId + CIN
            if (_guestRepository.GuestExistsByCliIdAndCin(guest.CliId, guest.Cin))
                return Conflict("Un client avec ce CIN existe déjà pour ce compte.");

            // Vérification doublon par CliId + Téléphone
            if (!string.IsNullOrWhiteSpace(guest.Tel) &&
                _guestRepository.GuestExistsByCliIdAndTel(guest.CliId, guest.Tel))
                return Conflict("Un client avec ce numéro de téléphone existe déjà pour ce compte.");

            if (_guestRepository.CreateGuest(guest))
            {
                // 🔥 Ne pas envoyer SMS pour DOS 1 et 6
                bool shouldSendSms = guest.Dos != 1 && guest.Dos != 6;

                if (shouldSendSms && !string.IsNullOrWhiteSpace(guest.Tel))
                {
                    string message =
                        $"Bienvenue {guest.Prenom} {guest.Nom} " +
                        $"Numéro de fidélité {guest.Num} chez Olympia Club";

                    string smsResponse =
                        await _smsService.SendSmsAsync(guest.Tel, message);

                    return Ok(new
                    {
                        guest,
                        smsResponse
                    });
                }

                return Ok(new
                {
                    guest,
                    smsResponse = "SMS non envoyé pour ce DOS."
                });
            }

            return BadRequest("Impossible d'ajouter le guest.");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteGuest(int id)
        {
            var guest = _guestRepository.GetGuestById(id);
            if (guest == null)
                return NotFound("Guest non trouvé.");

            if (_guestRepository.DeleteGuest(guest))
                return Ok("Guest supprimé avec succès.");

            return BadRequest("Impossible de supprimer le guest.");
        }
    }
}
