using Microsoft.AspNetCore.Mvc;
using Olympia.Interfaces;
using Olympia.Models;

namespace Olympia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TarController : ControllerBase
    {

        public ERP210OLYMPIA_FContext _context;
        public ITarRepository _tarRepository;
        public TarController(ERP210OLYMPIA_FContext context, ITarRepository tarRepository)
        {
            this._context = context;
            this._tarRepository = tarRepository;
        }
        [HttpGet]
        public IActionResult GetAllArt()
        {
            var tars = _tarRepository.GetTars().Select(a => new
            {
                tarId = a.TarId,
                Dos = a.Dos,
                Pub = a.Pub,
                Qtye = a.Qte,
                Ref = a.Ref.Trim(),
                Sref1 = a.Sref1,
                Sref2 = a.Sref2
            })
                .ToList();
            return Ok(tars);
        }
        [HttpGet("list/{reference}")]
        public IActionResult GetAllArtByRef(string reference)
        {
            var tars = _tarRepository.GetTars(reference).Where(t=> !string.IsNullOrEmpty(t.Sref1)).Select(a => new
            {
                tarId = a.TarId,
                Dos = a.Dos,
                Pub = a.Pub,
                Qtye = a.Qte,
                Ref = a.Ref.Trim(),
                Sref1 = a.Sref1,
                Sref2 = a.Sref2
            })
                .ToList();
            return Ok(tars);
        }
        [HttpGet("{id}")]
        public IActionResult GetTarById(int id)
        {
            var tar = _tarRepository.GetArtById(id);
            return Ok(tar);
        }

        [HttpGet("image")]
        public IActionResult GetImage(int artPhotoId)
        {
            var article = _context.Art.FirstOrDefault(a => a.ArtId == artPhotoId);

            if (article == null || article.Media == null || article.Media.Trim() == "")
            {
                var path1 = "C:\\LANG\\lang-server\\lang-server\\wwwroot\\404.jpg";
                return File(System.IO.File.OpenRead(path1), "application/octet-stream", Path.GetFileName(path1));
            }
            try
            {
                var path = "C:\\divalto\\ERP210OLYMPIA_F\\Fichiers\\images\\" + article.Media.Trim();
                return File(System.IO.File.OpenRead(path), "application/octet-stream", Path.GetFileName(path));
            }
            catch (Exception ex)
            {
                var path1 = "C:\\LANG\\lang-server\\lang-server\\wwwroot\\404.jpg";
                return File(System.IO.File.OpenRead(path1), "application/octet-stream", Path.GetFileName(path1));
            }
        }

        [HttpGet("famille")]
        public IActionResult GetNatures()
        {
            var articles = _context.Art;
            var familles = articles
                .Where(a => !string.IsNullOrEmpty(a.Fam0001))
                .GroupBy(a => a.Fam0001)
                .Select(g => new { Fam0001 = g.Key.Trim(), Count = g.Count() })
                .ToList();
            return Ok(familles);
        }
        [HttpGet("tarifs/{reference}")]
        public IActionResult GetTarifs(string reference)
        {
            var tarifs = _context.Tar.Where(t => t.Ref.Trim() == reference.Trim() && t.Dos == 2).ToList();

            // Utiliser un dictionnaire pour stocker les valeurs uniques

            var tarifsDistinct = tarifs.GroupBy(tar => tar.Ref)
                .Select(group => new TarRefDto
                {
                    Ref = group.Key,
                    Sref1 = group.Select(tar => tar.Sref1).Distinct().ToList(),
                    Sref2 = group.Select(tar => tar.Sref2).Distinct().ToList()
                }).ToList();

            return Ok(tarifsDistinct);

        }

    }
}
