using Microsoft.AspNetCore.Mvc;
using Olympia.Interfaces;
using Olympia.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Olympia.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        public ERP210OLYMPIA_FContext _context;
        public IAuthRepository _authRepository;

        public AuthController(ERP210OLYMPIA_FContext context, IAuthRepository authRepository)
        {
            this._context = context;
            this._authRepository = authRepository;
        }

        // Get all users including clients, commercials, and T2 users
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ICollection<Authentication>))]
        public IActionResult GetAllClients()
        {
            var clients = _context.CLI.Where(c => !string.IsNullOrEmpty(c.EMAIL) && c.U_RDV == 2).ToList();
            var commercials = _context.VRP.Where(c => !string.IsNullOrEmpty(c.EMAIL)).ToList();
            var t2Users = _context.T2.Where(c => !string.IsNullOrEmpty(c.EMAIL)).ToList();

            // Add commercials to Authentication if not existing
            foreach (var client in commercials)
            {
                var existingAuthEntry = _context.Authentication.FirstOrDefault(a => a.Username == client.EMAIL.ToString().Trim());

                if (existingAuthEntry == null)
                {
                    var authEntry = new Authentication
                    {
                        Username = client.EMAIL.ToString().Trim(),
                        Password = "P@ssw0rd123",
                        Role = "ROLE_COMM"
                    };
                    _context.Authentication.Add(authEntry);
                }
            }
            _context.SaveChanges();

            // Add clients to Authentication if not existing
            foreach (var client in clients)
            {
                var existingAuthEntry = _context.Authentication.FirstOrDefault(a => a.Username == client.EMAIL.ToString().Trim());

                if (existingAuthEntry == null)
                {
                    var authEntry = new Authentication
                    {
                        Username = client.EMAIL.ToString().Trim(),
                        Password = "P@ssw0rd",
                        Role = "CLIENT"
                    };
                    _context.Authentication.Add(authEntry);
                }
            }
            _context.SaveChanges();

            // Add T2 users to Authentication if not existing
            foreach (var client in t2Users)
            {
                var existingAuthEntry = _context.Authentication.FirstOrDefault(a => a.Username == client.EMAIL.ToString().Trim());

                if (existingAuthEntry == null)
                {
                    var authEntry = new Authentication
                    {
                        Username = client.EMAIL.ToString().Trim(),
                        Password = "P@ssw0rdT22",
                        Role = "ROLE_CONTACT",
                        // 🔽 nouveaux champs
                        DOS = Convert.ToInt32(client.DOS),
                        ETB = client.U_ETB?.Trim(),
                        DEPOT = client.U_DEPOT?.Trim()
                    };
                    _context.Authentication.Add(authEntry);
                }
            }
            _context.SaveChanges();

            var contacts = _authRepository.GetAuthentications();
            return Ok(contacts);
        }
        [HttpPut("change-password")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Username))
                return BadRequest(new { message = "Invalid request data" });

            var user = _context.Authentication.FirstOrDefault(a => a.Username == request.Username);
            if (user == null)
                return BadRequest(new { message = "User not found" });

            if (user.Password != request.OldPassword.Trim())
                return BadRequest(new { message = "Incorrect old password" });

            user.Password = request.NewPassword.Trim();

            var changeRequest = new ChangePasswordRequest
            {
                Username = request.Username,
                OldPassword = request.OldPassword.Trim(),
                NewPassword = request.NewPassword.Trim(),
            };
            _context.ChangePasswordRequest.Add(changeRequest);

            _context.SaveChanges();
            return Ok(new { message = "Password changed successfully" });
        }

        [HttpGet("RoleContact")]
        public async Task<ActionResult<IEnumerable<Authentication>>> GetRoleContact()
        {
            try
            {
                var contacts = await _context.Set<Authentication>()
                    .Where(a => a.Role == "ROLE_CONTACT")
                    .ToListAsync();

                if (!contacts.Any())
                {
                    return NotFound("Aucun utilisateur avec le rôle ROLE_CONTACT trouvé.");
                }

                return Ok(contacts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur serveur : {ex.Message}");
            }
        }
        // Get one user by username
        [HttpGet("{username}")]
        [ProducesResponseType(200, Type = typeof(Authentication))]
        public IActionResult GetOneClient(string username)
        {
            var client = _authRepository.GetAuthenticationByUsername(username);
            return Ok(client);
        }

        // Create a new user
        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateClient([FromBody] Authentication createClient)
        {
            if (createClient == null)
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var clientAuth = new Authentication
            {
                Username = createClient.Username,
                Password = createClient.Password,
            };

            await _context.AddAsync(clientAuth);
            _context.SaveChanges();

            return Ok(clientAuth);
        }

        // Client login
        [HttpPost("login")]
        [ProducesResponseType(200, Type = typeof(Authentication))]
        public IActionResult onLogin(Authentication request)
        {
            var valid_request = _authRepository.LoginClient(request.Username, request.Password);
            var client = _context.CLI.Where(c => c.EMAIL.Trim() == request.Username && c.DOS == 2 ).FirstOrDefault();

            if (valid_request)
            {
                _context.SaveChanges();
                return Ok(client);
            }
            else
            {
                return BadRequest("wrong credentials");
            }
        }

        // Commercial login
        [HttpPost("login/comm")]
        [ProducesResponseType(200, Type = typeof(Authentication))]
        public IActionResult onLoginComm(Authentication request)
        {
            var valid_request = _authRepository.Login(request.Username, request.Password);
            var commercial = _context.VRP.Where(c => c.EMAIL.Trim() == request.Username).FirstOrDefault();

            if (valid_request)
            {
                _context.SaveChanges();
                return Ok(commercial);
            }
            else
            {
                return BadRequest("wrong credentials");
            }
        }

        [HttpPost("login/contact")]
        public IActionResult onLoginContact(Authentication request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("username and password are required");
            }

            var user = _authRepository.LoginContact(request.Username, request.Password);

            if (user == null)
            {
                return Unauthorized("wrong credentials");
            }

            var t2User = _context.T2
                .FirstOrDefault(c => c.EMAIL != null && c.EMAIL.Trim() == request.Username.Trim());

            // 🔽 mapping vers DTO
            var authDto = new AuthenticationResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role,
                DOS = user.DOS,
                ETB = user.ETB,
                DEPOT = user.DEPOT
            };

            var response = new
            {
                Authentication = authDto,
                T2 = t2User
            };

            return Ok(response);
        }
    }
}
