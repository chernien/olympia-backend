using lang_server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Olympia.Interfaces;
using Olympia.Models;
using Olympia.Services;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Net;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Olympia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtController : ControllerBase
    {
        private readonly ERP210OLYMPIA_FContext _context;
        private readonly IArtRepository _artRepository;
        private readonly SmsService _smsService;

        public ArtController(
            ERP210OLYMPIA_FContext context,
            IArtRepository artRepository,
            SmsService smsService)
        {
            _context = context;
            _artRepository = artRepository;
            _smsService = smsService;
        }


        //[HttpGet]
        //public IActionResult GetAllArt()
        //{
        //    var articles = _artRepository.GetArts().Select(a => new
        //    {
        //        artId = a.ArtId,
        //        Dos = a.Dos,
        //        Des = a.Des.Trim(),
        //        Media = a.Media.Trim(),
        //        Ref = a.Ref.Trim(),
        //        Fam0001 = a.Fam0001.Trim()
        //    })
        //        .ToList();
        //    return Ok(articles);
        //}


        [HttpGet]
        public IActionResult GetAllArt([FromQuery] int dos)
        {
            var articles = _artRepository.GetArts(dos)
                .Select(a => new
                {
                    artId = a.ArtId,
                    Dos = a.Dos,
                    Des = a.Des.Trim(),
                    Media = a.Media.Trim(),
                    Ref = a.Ref.Trim(),
                    Fam0001 = a.Fam0001.Trim()
                })
                .ToList();

            return Ok(articles);
        }

        [HttpGet("{id}")]
        public IActionResult GetArtById(int id)
        {
            var article = _context.Art.Where(a => a.ArtId == id).Select(a => new
            {
                artId = a.ArtId,
                Dos = a.Dos,
                Des = a.Des.Trim(),
                Media = a.Media.Trim(),
                Ref = a.Ref.Trim(),
                Fam0001 = a.Fam0001.Trim()
            }).FirstOrDefault();
            return Ok(article);
        }

        /*[HttpGet("image")]
        public IActionResult GetImage(int artPhotoId)
        {
            var article = _context.Art.FirstOrDefault(a => a.ArtId == artPhotoId);

            if (article == null || article.Media == null || article.Media.Trim() == "")
            {
                var path1 = "C:\\Users\\sami\\Desktop\\Olympia\\Olympia\\Olympia\\wwwroot\\404.png";
                return File(System.IO.File.OpenRead(path1), "application/octet-stream", Path.GetFileName(path1));
            }
            try
            {
                var path = "C:\\divalto\\OLYMPIA\\Fichiers\\images\\" + article.Media.Trim();
                return File(System.IO.File.OpenRead(path), "application/octet-stream", Path.GetFileName(path));
            }
            catch (Exception ex)
            {
                var path1 = "C:\\Users\\sami\\Desktop\\Olympia\\Olympia\\Olympia\\wwwroot\\404.png";
                return File(System.IO.File.OpenRead(path1), "application/octet-stream", Path.GetFileName(path1));
            }
        }*/

        //[HttpGet("famille")]
        //public IActionResult GetNatures()
        //{
        //    var articles = _context.Art;
        //    var familles = articles
        //        .Where(a => !string.IsNullOrEmpty(a.Fam0001) && a.Dos == 2 && a.U_ACTIF == 2)
        //        .Join(
        //            _context.T012.Where(t => t.Famno == 1 && t.Dos == 2),
        //            a => a.Fam0001.Trim(),
        //            t => t.Fam.Trim(),
        //            (a, t) => new { Fam0001 = t.Fam, Lib = t.Lib, Count = 1 }
        //        )
        //        .GroupBy(a => a.Fam0001)
        //        .Select(g => new { Fam0001 = g.Key.Trim(), Lib = g.First().Lib, Count = g.Sum(x => x.Count) })
        //        .ToList();
        //    return Ok(familles);
        //}


        [HttpGet("famille/{dos}")]
        public IActionResult GetNatures(int dos)
        {
            var articles = _context.Art;

            var familles = articles
                .Where(a =>
                    !string.IsNullOrEmpty(a.Fam0001)
                    && a.Dos == dos
                    && a.U_ACTIF == 2
                )
                .Join(
                    _context.T012.Where(t => t.Famno == 1 && t.Dos == dos),
                    a => a.Fam0001.Trim(),
                    t => t.Fam.Trim(),
                    (a, t) => new
                    {
                        Fam0001 = t.Fam,
                        Lib = t.Lib,
                        Count = 1
                    }
                )
                .GroupBy(a => a.Fam0001)
                .Select(g => new
                {
                    Fam0001 = g.Key.Trim(),
                    Lib = g.First().Lib,
                    Count = g.Sum(x => x.Count)
                })
                .ToList();

            return Ok(familles);
        }


        [HttpGet("fam")]
        public IActionResult GetNature()
        {
            var articles = _context.Art;
            var familles = articles
                .Where(a => !string.IsNullOrEmpty(a.Fam0001) && a.Dos == 2)
                .GroupBy(a => a.Fam0001)
                .Select(g => new { Fam0001 = g.Key.Trim(), Count = g.Count() })
                .ToList();
            return Ok(familles);
        }

        [HttpGet("sref1")]
        public IActionResult GetSref1s(string reference)
        {
            var tarifs = _context.Tar.Where(t => t.Ref.Trim() == reference.Trim());
            var sref1Counts = tarifs
                .Where(a => !string.IsNullOrEmpty(a.Sref1))
                .GroupBy(a => a.Sref1)
                .Select(g => new { Sref1 = g.Key.Trim(), Count = g.Count() })
                .ToList();

            var sref1Libs = new List<string>();
            foreach (var sref1Count in sref1Counts)
            {
                var lib = _context.T019
                    .Where(t => t.Sref1.Trim() == sref1Count.Sref1)
                    .Select(t => t.Lib)
                    .FirstOrDefault();

                sref1Libs.Add(lib);
            }

            var result = sref1Counts.Zip(sref1Libs, (s, lib) => new { Sref1 = s.Sref1, Lib = lib });
            var sortedResult = result.OrderBy(r => r.Sref1);

            return Ok(sortedResult);
        }

        [HttpGet("sref2/{reference}")]
        public IActionResult GetSref2s(string reference)
        {
            var tarifs = _context.Tar
     .Where(t => t.Ref.Trim() == reference.Trim()); 
         var sref2Counts = tarifs
                .Where(a => !string.IsNullOrEmpty(a.Sref2) )
                .GroupBy(a => a.Sref2)
                .Select(g => new { Sref2 = g.Key.Trim(), Count = g.Count() })
                .ToList();

            var sref2Libs = new List<string>();
            foreach (var sref2Count in sref2Counts)
            {
                var lib = _context.T019
                    .Where(t => t.Sref1.Trim() == sref2Count.Sref2 && t.Dos == 2)
                    .Select(t => t.Lib)
                    .FirstOrDefault();

                sref2Libs.Add(lib);
            }

            var result = sref2Counts.Zip(sref2Libs, (s, lib) => new { Sref2 = s.Sref2, Lib = lib });

            return Ok(result);
        }


        //[HttpGet("tarif")]

        //public IActionResult GetSref2s(string? sref1, string? sref2, string reference)

        //{
        //    var tarifsQuery = _context.Tar.Where(t => t.Ref.Trim() == reference.Trim() && t.Dos == 2);

        //    if (!string.IsNullOrEmpty(sref1))
        //    {
        //        tarifsQuery = tarifsQuery.Where(t => t.Sref1.Trim() == sref1.Trim());
        //    }
        //    if (!string.IsNullOrEmpty(sref2))
        //    {
        //        tarifsQuery = tarifsQuery.Where(t => t.Sref2.Trim() == sref2.Trim());
        //    }


        //    var tarifs = tarifsQuery.ToList();

        //    return Ok(tarifs);
        //}


        [HttpGet("tarif")]
        public IActionResult GetSref2s(
    string? sref1,
    string? sref2,
    string reference,
    int dos
)
        {
            var tarifsQuery = _context.Tar
                .Where(t => t.Ref.Trim() == reference.Trim()
                         && t.Dos == dos);

            if (!string.IsNullOrEmpty(sref1))
            {
                tarifsQuery = tarifsQuery
                    .Where(t => t.Sref1.Trim() == sref1.Trim());
            }

            if (!string.IsNullOrEmpty(sref2))
            {
                tarifsQuery = tarifsQuery
                    .Where(t => t.Sref2.Trim() == sref2.Trim());
            }

            var tarifs = tarifsQuery.ToList();

            return Ok(tarifs);
        }


        /*        [HttpPost("commande")]
        public IActionResult PasserCommande([FromBody] CommandeDto commandeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (commandeDto.ContactId == null || commandeDto.ContactId == 0)
            {
                commandeDto.ContactId = '1';
            }

            var contact = _context.T2.FirstOrDefault(t => t.T2_ID == commandeDto.ContactId);

            var commande = new Commande
            {
                ClientId = commandeDto.ClientId,
                GuestId = commandeDto.GuestId,
                Type = commandeDto.Type,
                ContactId = commandeDto.ContactId,
                CE4 = commandeDto.CE4,
                OrderDate = DateTime.Now,
                Dos = 2,
                Tiers = commandeDto.Tiers,
                Reglement = contact?.CONTACT,
                Status = "N'est pas traité",
                Total = 0
            };

            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };

            string jsonString = JsonSerializer.Serialize(commande, options);

            _context.Commande.Add(commande);
            _context.SaveChanges();

            foreach (var articleDto in commandeDto.Articles)
            {
                var stockCE4_1 = _context.CommandeArticle
                    .Where(ca => ca.ArticleId == articleDto.ArtId && ca.CE4 == '1')
                    .Sum(ca => ca.Stock);

                var fquantite = Math.Min(articleDto.Quantite, stockCE4_1);

                var uPts = _context.Art
                    .Where(a => a.Ref == articleDto.Ref && a.Dos == 2)
                    .Select(a => a.U_PTS)
                    .FirstOrDefault();

                // Nouveau calcul de fidelity selon les règles spécifiées
                var fidelity = 0; // Par défaut 0 pour CE4=1
                var stock = 0;

                // Insertion de l'article dans la commande
                var sql = "INSERT INTO CommandeArticle (CommandeId, ArticleId, Quantite, Fquantite, Ref, Sref1, Sref2, Dos, PRIX, CE4, U_PTS, Stock, Fidelity) VALUES (@CommandeId, @ArticleId, @Quantite, @Fquantite, @Ref, @Sref1, @Sref2, @Dos, @Prix, @Ce4, @UPts, @Stock, @Fidelity)";
                _context.Database.ExecuteSqlRaw(sql,
                    new SqlParameter("@CommandeId", commande.Commande_Id),
                    new SqlParameter("@ArticleId", articleDto.ArtId),
                    new SqlParameter("@Quantite", articleDto.Quantite),
                    new SqlParameter("@Fquantite", fquantite),
                    new SqlParameter("@Ref", articleDto.Ref),
                    new SqlParameter("@Sref1", articleDto.Sref1),
                    new SqlParameter("@Sref2", articleDto.Sref2),
                    new SqlParameter("@Dos", 2),
                    new SqlParameter("@Ce4", commande.Type),
                    new SqlParameter("@Prix", articleDto.prix),
                    new SqlParameter("@UPts", uPts),
                    new SqlParameter("@Stock", stock),
                    new SqlParameter("@Fidelity", fidelity));

                // 1. Mise à jour du stock pour les articles avec mêmes Ref, Sref1, Sref2 et CE4=1
                var updateStockSql1 = @"
            WITH GroupedArticles AS (
                SELECT 
                    CommandeArticle_Id,
                    ROW_NUMBER() OVER (PARTITION BY Ref, Sref1, Sref2 ORDER BY CommandeArticle_Id) as RowNum,
                    SUM(Stock) OVER (PARTITION BY Ref, Sref1, Sref2) as TotalStock
                FROM CommandeArticle
                WHERE CE4 = '1' AND Ref = @Ref AND Sref1 = @Sref1 AND Sref2 = @Sref2
            )
            UPDATE ca
            SET ca.Stock = CASE 
                WHEN ga.RowNum = 1 THEN ga.TotalStock
                ELSE 0
            END
            FROM CommandeArticle ca
            INNER JOIN GroupedArticles ga ON ca.CommandeArticle_Id = ga.CommandeArticle_Id";

                _context.Database.ExecuteSqlRaw(updateStockSql1,
                    new SqlParameter("@Ref", articleDto.Ref),
                    new SqlParameter("@Sref1", articleDto.Sref1),
                    new SqlParameter("@Sref2", articleDto.Sref2));

                // 2. Mise à jour du stock CE4=2 avec la somme des stocks CE4=1 pour mêmes Ref, Sref1, Sref2
                // Modification ici pour mettre à jour TOUTES les lignes CE4=2, pas seulement la première
                var updateStockSql2 = @"
            UPDATE ca2
            SET ca2.Stock = (
                SELECT COALESCE(SUM(ca1.Stock), 0)
                FROM CommandeArticle ca1
                WHERE ca1.CE4 = '1' 
                AND ca1.Ref = ca2.Ref 
                AND ca1.Sref1 = ca2.Sref1 
                AND ca1.Sref2 = ca2.Sref2
            )
            FROM CommandeArticle ca2
            WHERE ca2.CE4 = '2'
            AND ca2.Ref = @Ref
            AND ca2.Sref1 = @Sref1
            AND ca2.Sref2 = @Sref2";

                _context.Database.ExecuteSqlRaw(updateStockSql2,
                    new SqlParameter("@Ref", articleDto.Ref),
                    new SqlParameter("@Sref1", articleDto.Sref1),
                    new SqlParameter("@Sref2", articleDto.Sref2));

                // 3. Mise à jour de Fquantite pour CE4=2 (Stock - Quantite si Stock > Quantite, sinon 0)
                var updateFquantiteSql = @"
            UPDATE CommandeArticle
            SET Fquantite = CASE 
                WHEN Stock > Quantite THEN Stock - Quantite
                ELSE 0
            END
            WHERE CE4 = '2'
            AND Ref = @Ref
            AND Sref1 = @Sref1
            AND Sref2 = @Sref2";

                _context.Database.ExecuteSqlRaw(updateFquantiteSql,
                    new SqlParameter("@Ref", articleDto.Ref),
                    new SqlParameter("@Sref1", articleDto.Sref1),
                    new SqlParameter("@Sref2", articleDto.Sref2));

                // 4. Mise à jour de Fidelity pour CE4=2 selon les nouvelles règles
                var updateFidelitySql = @"
            UPDATE CommandeArticle
            SET Fidelity = CASE 
                WHEN CE4 = '1' THEN 0
                WHEN CE4 = '2' THEN 
                    CASE 
                        WHEN Stock < Quantite THEN Stock * U_PTS
                        ELSE Quantite * U_PTS
                    END
            END
            WHERE Ref = @Ref
            AND Sref1 = @Sref1
            AND Sref2 = @Sref2";

                _context.Database.ExecuteSqlRaw(updateFidelitySql,
                    new SqlParameter("@Ref", articleDto.Ref),
                    new SqlParameter("@Sref1", articleDto.Sref1),
                    new SqlParameter("@Sref2", articleDto.Sref2));

                var updateStockWithFquantiteSql = @"
            UPDATE CommandeArticle
            SET Stock = Fquantite
            WHERE CE4 = '2'
            AND Ref = @Ref
            AND Sref1 = @Sref1
            AND Sref2 = @Sref2";

                _context.Database.ExecuteSqlRaw(updateStockWithFquantiteSql,
                    new SqlParameter("@Ref", articleDto.Ref),
                    new SqlParameter("@Sref1", articleDto.Sref1),
                    new SqlParameter("@Sref2", articleDto.Sref2));

                var updateSql = "UPDATE Commande SET Total = Total + (@prix * @Quantite) WHERE Commande_Id = @CommandeId";
                _context.Database.ExecuteSqlRaw(updateSql,
                    new SqlParameter("@prix", articleDto.prix),
                    new SqlParameter("@Quantite", articleDto.Quantite),
                    new SqlParameter("@CommandeId", commande.Commande_Id));
            }

            var updateFideliteSql = @"
        UPDATE Guest
        SET Fidelite = (
            SELECT COALESCE(SUM(ca.Fidelity), 0)
            FROM CommandeArticle ca
            INNER JOIN Commande c ON ca.CommandeId = c.Commande_Id
            WHERE c.GuestId = Guest.Id
        )
        WHERE Id = @GuestId";

            _context.Database.ExecuteSqlRaw(updateFideliteSql, new SqlParameter("@GuestId", commande.GuestId));

            return Ok(commande);
        }*/

        [HttpPost("commande")]
        public async Task<IActionResult> PasserCommande([FromBody] CommandeDto commandeDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (commandeDto.ContactId == null || commandeDto.ContactId == 0)
                commandeDto.ContactId = 1;

            var contact = _context.T2.FirstOrDefault(t => t.T2_ID == commandeDto.ContactId);

            var commande = new Commande
            {
                ClientId = commandeDto.ClientId,
                GuestId = commandeDto.GuestId,
                Type = commandeDto.Type,
                ContactId = commandeDto.ContactId,
                CE4 = commandeDto.CE4,
                OrderDate = DateTime.Now,
                // 🔥 DYNAMIC
                Dos = commandeDto.Dos,
                Etb = commandeDto.Etb,   // 🔥 AJOUT
                DEPOT = commandeDto.DEPOT,

                Tiers = commandeDto.Tiers,
                Reglement = contact?.CONTACT,
                Status = "N'est pas traité",
                Total = 0
            };

            _context.Commande.Add(commande);
            _context.SaveChanges();

            foreach (var articleDto in commandeDto.Articles)
            {
                var stockCE4_1 = _context.CommandeArticle
                    .Where(ca => ca.ArticleId == articleDto.ArtId && ca.CE4 == "1")
                    .Sum(ca => ca.Stock);

                var fquantite = Math.Min(articleDto.Quantite, stockCE4_1);

                var uPts = _context.Art
                    .Where(a => a.Ref == articleDto.Ref && a.Dos == commandeDto.Dos)
                    .Select(a => a.U_PTS)
                    .FirstOrDefault();

                int stock = 0;
                int fidelity = 0;

                var tarif = _context.Tar
                    .Where(t => t.Ref.Trim() == articleDto.Ref.Trim() &&
                                t.Sref1.Trim() == articleDto.Sref1.Trim() &&
                                t.Sref2.Trim() == articleDto.Sref2.Trim() &&
                                Math.Abs(t.Pub - ((articleDto.prix ?? 0) / 1.19m)) < 0.001m)
                    .Select(t => t.Tacod.TrimEnd())
                    .FirstOrDefault();


                var sql = @"
            INSERT INTO CommandeArticle 
            (CommandeId, ArticleId, Quantite, Fquantite, Ref, Sref1, Sref2, Dos, ETB , DEPOT, PRIX, CE4, U_PTS, Stock, Fidelity, Eche, Tarif, ContactId) 
            VALUES 
            (@CommandeId, @ArticleId, @Quantite, @Fquantite, @Ref, @Sref1, @Sref2, @Dos, @ETB, @DEPOT, @Prix, @Ce4, @UPts, @Stock, @Fidelity, @Eche, @Tarif, @ContactId)";

                _context.Database.ExecuteSqlRaw(sql,
                    new SqlParameter("@CommandeId", commande.Commande_Id),
                    new SqlParameter("@ArticleId", articleDto.ArtId),
                    new SqlParameter("@Quantite", articleDto.Quantite),
                    new SqlParameter("@Fquantite", fquantite),
                    new SqlParameter("@Ref", articleDto.Ref),
                    new SqlParameter("@Sref1", articleDto.Sref1),
                    new SqlParameter("@Sref2", articleDto.Sref2),
                    new SqlParameter("@Dos", commandeDto.Dos),
                    new SqlParameter("@ETB", commandeDto.Etb ?? (object)DBNull.Value),
                    new SqlParameter("@DEPOT", commandeDto.DEPOT),
                    new SqlParameter("@Ce4", commande.Type),
                    new SqlParameter("@Prix", articleDto.prix),
                    new SqlParameter("@UPts", uPts),
                    new SqlParameter("@Stock", stock),
                    new SqlParameter("@Fidelity", fidelity),
                    new SqlParameter("@Eche", articleDto.Eche),
                    new SqlParameter("@Tarif", tarif ?? (object)DBNull.Value),
                    new SqlParameter("@ContactId", commande.ContactId)
                );

                /* if (commande.Type == 2)
                {
                    int quantiteRestante = articleDto.Quantite;

                    var lignesStock = _context.CommandeArticle
                        .Where(ca => ca.Ref == articleDto.Ref &&
                                     ca.Sref1 == articleDto.Sref1 &&
                                     ca.Sref2 == articleDto.Sref2 &&
                                     ca.CE4 == "1" &&
                                     ca.Stock > 0 &&
                                     ca.ContactId == commande.ContactId)
                        .OrderBy(ca => ca.CommandeArticle_Id)
                        .ToList();

                    foreach (var ligne in lignesStock)
                    {
                        if (quantiteRestante <= 0)
                            break;

                        // Recharger la ligne depuis la base pour éviter les données périmées
                        _context.Entry(ligne).Reload();

                        var aSoustraire = Math.Min(ligne.Stock, quantiteRestante);
                        ligne.Stock -= aSoustraire;
                        quantiteRestante -= aSoustraire;
                    }

                    try
                    {
                        _context.SaveChanges();
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        // Si quelqu’un a modifié la même ligne entre-temps,
                        
                        // on recharge les valeurs réelles depuis la BD
                        foreach (var entry in ex.Entries)
                        {
                            entry.Reload();                           
                        }

                        // Retenter la sauvegarde
                        _context.SaveChanges();
                    }
                }*/



            var updateFquantiteSql = @"
            UPDATE CommandeArticle
            SET Fquantite = Quantite
            WHERE CE4 IN ('2','9') AND Ref = @Ref AND Sref1 = @Sref1 AND Sref2 = @Sref2";

                _context.Database.ExecuteSqlRaw(updateFquantiteSql,
                    new SqlParameter("@Ref", articleDto.Ref),
                    new SqlParameter("@Sref1", articleDto.Sref1),
                    new SqlParameter("@Sref2", articleDto.Sref2)
                );

                var updateFidelitySql = @"
UPDATE CommandeArticle
SET Fidelity = CASE
    WHEN CE4 = '1' THEN 0
    WHEN CE4 IN ('2','9') THEN 
        CASE
            WHEN Quantite < (
                SELECT COALESCE(SUM(Stock), 0)
                FROM CommandeArticle ca
                WHERE ca.Ref = CommandeArticle.Ref
                AND ca.Sref1 = CommandeArticle.Sref1
                AND ca.Sref2 = CommandeArticle.Sref2
                AND ca.ContactId = CommandeArticle.ContactId
                AND ca.CE4 = '1'
                GROUP BY ca.Ref, ca.Sref1, ca.Sref2, ca.ContactId
            ) THEN Quantite * U_PTS 
            ELSE COALESCE((
                SELECT SUM(Stock)
                FROM CommandeArticle ca
                WHERE ca.Ref = CommandeArticle.Ref
                AND ca.Sref1 = CommandeArticle.Sref1
                AND ca.Sref2 = CommandeArticle.Sref2
                AND ca.ContactId = CommandeArticle.ContactId
                AND ca.CE4 = '1'
                GROUP BY ca.Ref, ca.Sref1, ca.Sref2, ca.ContactId
            ), 0) * U_PTS
        END
    ELSE Fidelity
END
WHERE Ref = @Ref AND Sref1 = @Sref1 AND Sref2 = @Sref2";




                _context.Database.ExecuteSqlRaw(updateFidelitySql,
                    new SqlParameter("@Ref", articleDto.Ref),
                    new SqlParameter("@Sref1", articleDto.Sref1),
                    new SqlParameter("@Sref2", articleDto.Sref2)
                );

                var updateTotalSql = "UPDATE Commande SET Total = Total + (@Prix * @Quantite) WHERE Commande_Id = @CommandeId";
                _context.Database.ExecuteSqlRaw(updateTotalSql,
                    new SqlParameter("@Prix", articleDto.prix),
                    new SqlParameter("@Quantite", articleDto.Quantite),
                    new SqlParameter("@CommandeId", commande.Commande_Id)
                );
            }


            var updateFideliteSql = @"
UPDATE Guest
SET Fidelite = Fidelite + (
    SELECT COALESCE(SUM(ca.Fidelity), 0)
    FROM CommandeArticle ca
    INNER JOIN Commande c ON ca.CommandeId = c.Commande_Id
    WHERE c.GuestId = Guest.Id
      AND ca.CommandeId = @CommandeId
    GROUP BY c.GuestId
)
WHERE Id = @GuestId";

            _context.Database.ExecuteSqlRaw(updateFideliteSql,
                new SqlParameter("@CommandeId", commande.Commande_Id),
                new SqlParameter("@GuestId", commande.GuestId));



            try
            {
                var guest = _context.Guest.FirstOrDefault(g => g.Id == commande.GuestId);
                if (guest != null && !string.IsNullOrWhiteSpace(guest.Tel))
                {
                    if (commande.Type == 2) 
                    {
                        var totalFidelite = _context.Guest
                            .Where(g => g.Num == guest.Num)
                            .Sum(g => g.Fidelite);
                        var sb = new System.Text.StringBuilder();
                        sb.AppendLine($"Bonjour {guest.Prenom} {guest.Nom},");
                        sb.AppendLine($"Total de points : {totalFidelite}");

                        string message = sb.ToString();
                        string smsResponse = await _smsService.SendSmsAsync(guest.Tel, message);
                        Console.WriteLine($"✅ SMS envoyé à {guest.Tel} : {smsResponse}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur lors de l'envoi du SMS : {ex.Message}");
            }


            return Ok(commande);
        }


        [HttpGet("comd/{cmdId}")]
        public IActionResult GetCommandeById(int cmdId)
        {
            var commande = _context.Commande.Where(c => (int)c.Commande_Id == (int)cmdId).FirstOrDefault();
            return Ok(commande);
        }
        [HttpGet("commande/{commandeId}")]
        public IActionResult GetCommande(int commandeId)
        {
            var commande = _context.Commande
                .Include(c => c.Client) 
                .Include(c => c.CONTACT)
                .Include(c => c.CommandeArticles)
                .FirstOrDefault(c => c.Commande_Id == commandeId);
            if (commande == null)
            {
                return NotFound();
            }
            // Créer un objet DTO pour le résultat
            var commandeDto = new
            {
                CommandeId = commande.Commande_Id,
                OrderDate = commande.OrderDate,
                Total = commande.Total,
                Client = new CLI
                {
                    CLI_ID = commande.Client.CLI_ID,
                    NOM = commande.Client.NOM,
                    EMAIL = commande.Client.EMAIL,
                    // Autres propriétés du client
                },
                Contact = new T2
                {
                    T2_ID = commande.CONTACT.T2_ID,
                    NOM = commande.CONTACT.NOM,
                    EMAIL = commande.CONTACT.EMAIL,
                    // Autres propriétés du CONTACT
                },
                Articles = commande.CommandeArticles.Select(ca => new ArticleDto
                {
                    ArtId = ca.Article.ArtId,
                    Des = ca.Article.Des,

                })
            };
            return Ok(commandeDto);
        }

        [HttpGet("commande/{commandeId}/articles")]
        public IActionResult GetCommandeArticles(int commandeId)
        {   var commande = _context.Commande.Where(c => c.Commande_Id == commandeId).FirstOrDefault();
            var commandeArticles = _context.CommandeArticle.ToList();
            var sql = @"
            SELECT *
            FROM CommandeArticle ca
            WHERE ca.CommandeId = @commandeId";
            var articles = _context.CommandeArticle
                .FromSqlRaw(sql, new SqlParameter("@commandeId", commandeId))
                 .Select(a => new 
                 {
                     ArtId = a.ArticleId,
                     Dos = a.Dos,
                     Ref = a.Ref,
                     Sref1 = a.Sref1,
                     Sref2 = a.Sref2,
                     Stock = a.Stock,
                     Fidelity=a.Fidelity,
                     Des=_artRepository.GetArtById(a.ArticleId).Des,
                     Fam0001 = _artRepository.GetArtById(a.ArticleId).Fam0001,
                    prix= a.PRIX*a.Quantite,
                    Quantite = a.Quantite

                 })
                 .ToList();

            return Ok(articles);
        }

        [HttpGet("client/{tiers}/commandes")]
        public IActionResult GetCommandesClient(string tiers)
        {
            var commandesClient = _context.Commande
                .Where(c => _context.CLI.Any(cli => cli.TIERS.Trim() == tiers.Trim() && cli.CLI_ID == c.ClientId))
                .ToList();

            if (!commandesClient.Any())
            {
                return NotFound(); // Aucun client ou commande trouvé
            }

            // Configuration pour JSON
            var jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                MaxDepth = 64
            };

            return new JsonResult(commandesClient, jsonOptions);
        }


        [HttpGet("contact/{CONTACTId}/commandes")]
        public IActionResult GetCommandesContact(int contactId)
        {
            var commandesContact = _context.Commande
                .Where(c => c.ContactId == contactId)

                .ToList();

            return Ok(commandesContact);
        }
        [HttpGet("commercial/{reference}/commandes")]
        public IActionResult GetCommandesCommercial(string reference)
        {
            var clients = _context.CLI.Where(a => a.REPR_0001.Trim() == reference.Trim()).ToList();

            if (clients.Count == 0)
            {
                return NotFound(); // No clients found with the given reference
            }

            var commandesClient = new List<Commande>(); // Create a list to hold orders

            foreach (var client in clients)
            {
                var clientId = client.CLI_ID; // Assuming there's an Id property for clients

                var commandes = _context.Commande
                    .Where(c => c.ClientId == clientId)
                    .ToList();

                commandesClient.AddRange(commandes); // Add orders to the list
            }

            // Configure JsonSerializerOptions to handle object cycles
            var jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                MaxDepth = 64 
            };

            return new JsonResult(commandesClient, jsonOptions); 
        }
        [HttpGet("contact/{tiers}/contact")]
        public IActionResult GetCommandesContact(string tiers)
        {
            var contacts = _context.T2.Where(a => a.TIERS.Trim() == tiers.Trim()).ToList();

            if (contacts.Count == 0)
            {
                return NotFound(); 
            }

            var commandesContact = new List<Commande>(); 

            foreach (var contact in contacts)
            {
                var contactId = contact.T2_ID; 

                var commandes = _context.Commande
                    .Where(c => c.ContactId == contactId)
                    .ToList();

                commandesContact.AddRange(commandes); 
            }

            var jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                MaxDepth = 64 
            };

            return new JsonResult(commandesContact, jsonOptions); 
        }
        [HttpGet("list/commandes")]
        public IActionResult GetCommandes()
        {
            var commandesClient = _context.Commande

                .ToList();

            return Ok(commandesClient);
        }
        [HttpGet("list/comdart")]

        public IActionResult GetCommandesArtss()
        {
            var commandesClient = _context.CommandeArticle
                .ToList();

            return Ok(commandesClient);
        }

        [HttpGet("list/reglement")]

        public IActionResult GetModeReglement()
        {
            var reglements = _context.T006.Where(a => a.DOS == 2)
                .ToList();

            return Ok(reglements);
        }

    }
}