using Calcular.CoreApi.Models;
using Calcular.CoreApi.Models.Business;
using Calcular.CoreApi.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Calcular.CoreApi.Controllers.Business
{
    [Route("api/[controller]")]
    public class AtividadeController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<User> userManager;

        public AtividadeController(ApplicationDbContext db, UserManager<User> userManager)
        {
            this.db = db;
            this.userManager = userManager;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] string filter)
        {
            var result = db.Atividades
                            .Include(x => x.Responsavel)
                            .Include(x => x.TipoAtividade)
                            .ToList();
            return Ok(result);
        }

        [HttpGet("currentuser")]
        public async Task<IActionResult> GetAtividadesByUser([FromQuery] string filter)
        {
            var user = userManager.GetUserAsync(HttpContext.User).Result;
            var isRevisor = await userManager.IsInRoleAsync(user, "Revisor");

            var result = db.Atividades
                            .Include(x => x.Responsavel)
                            .Include(x => x.Servico).ThenInclude(x => x.Processo).ThenInclude(x => x.Advogado)
                            .Include(x => x.TipoAtividade)
                            .Where(x => x.ResponsavelId == user.Id
                                        || (isRevisor && x.TipoAtividade.Nome == "Revisão"))
                            .ToList();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var result = db.Atividades
                            .Include(x => x.Responsavel)
                            .Include(x => x.Servico).ThenInclude(x => x.Processo).ThenInclude(x => x.Advogado)
                            .Include(x => x.TipoAtividade)
                            .Include(x => x.AtividadeOrigem).ThenInclude(x => x.Responsavel)
                            .Include(x => x.AtividadeOrigem).ThenInclude(x => x.TipoAtividade)
                            .SingleOrDefault(x => x.Id == id);

            return Ok(result);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Atividade atividade)
        {
            if (db.TipoAtividades.Single(x => x.Id == atividade.TipoAtividadeId).Nome == "Revisão")
                return BadRequest("Atividade do tipo Revisão não pode ser criada diretamente.");

            db.Atividades.Add(atividade);
            db.SaveChanges();

            return Ok(atividade);
        }

        [HttpPut]
        public IActionResult Put([FromBody] Atividade newItem)
        {
            var item = db.Atividades.Single(x => x.Id == newItem.Id);

            if (item.TipoAtividade.Nome == "Revisão" && newItem.TipoAtividade.Nome != "Revisão")
                return BadRequest("Atividade do tipo Revisão não pode ser alterada.");

            item.Nome = newItem.Nome;
            item.Entrega = newItem.Entrega;
            item.Tempo = newItem.Tempo;
            item.TipoImpressao = newItem.TipoImpressao;
            item.Observacao = newItem.Observacao;
            item.TipoAtividadeId = newItem.TipoAtividadeId;
            item.ResponsavelId = newItem.ResponsavelId;

            db.SaveChanges();
            return Ok(item);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var item = db.Atividades.Single(x => x.Id == id);

            if (item.Tempo != null || !string.IsNullOrEmpty(item.Observacao) || item.Entrega != null)
                return BadRequest("Atividade não pode ser excluída");

            db.Atividades.Remove(item);
            db.SaveChanges();
            return Ok(item);
        }

        [HttpGet("responsavel/{id}")]
        public IActionResult GetResponsavel(int id)
        {
            var item = db.Users.Select(x => new KeyValuePair<string, string>(x.Id, x.Name));
            return Ok(item);
        }

        [HttpGet("tipoatividade")]
        public IActionResult GetTipoAtividade()
        {
            return Ok(db.TipoAtividades.ToList());
        }

        [Route("tipoimpressao")]
        [HttpGet]
        public IActionResult GetParte()
        {
            var result = Enum.GetValues(typeof(TipoImpressaoEnum))
                            .Cast<TipoImpressaoEnum>()
                            .Select(x => new KeyValuePair<int, string>((int)x, EnumHelpers.GetEnumDescription(x)));
            return Ok(result);
        }
    }
}
