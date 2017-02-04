using Calcular.CoreApi.Models;
using Calcular.CoreApi.Models.Business;
using Calcular.CoreApi.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Calcular.CoreApi.Controllers.Business
{
    [Route("api/[controller]")]
    public class ServicoController : Controller
    {
        private readonly ApplicationDbContext db;

        public ServicoController(ApplicationDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] string filter, [FromQuery] bool all = false)
        {
            var pendente = filter.ContainsIgnoreNonSpacing("pendente");
            var entregue = filter.ContainsIgnoreNonSpacing("entregue");
            var calcelado = filter.ContainsIgnoreNonSpacing("cancelado");

            var result = db.Servicos
                           .Include(x => x.TipoServico)
                           .Include(x => x.Processo).ThenInclude(x => x.Advogado)
                           .Where(x => (string.IsNullOrEmpty(filter)
                                       || x.Processo.Numero.Contains(filter)
                                       || x.Processo.Advogado.Nome.Contains(filter)
                                       || x.Processo.Advogado.Telefone.Contains(filter)
                                       || x.Processo.Advogado.Celular.Contains(filter)
                                       || (pendente && x.Status == StatusEnum.Pendente)
                                       || (entregue && x.Status == StatusEnum.Entregue)
                                       || (calcelado && x.Status == StatusEnum.Cancelado))
                                       && (all || x.Status == StatusEnum.Pendente))
                            .OrderBy(x => x.Prazo)
                           .ToList()
                           .Select(x => new Servico
                           {
                               Id = x.Id,
                               Entrada = x.Entrada,
                               TipoServicoId = x.TipoServicoId,
                               TipoImpressao = x.TipoImpressao,
                               TipoServico = new TipoServico
                               {
                                   Id = x.TipoServico.Id,
                                   Nome = x.TipoServico.Nome,
                               },
                               Prazo = x.Prazo,
                               Saida = x.Saida,
                               Status = x.Status,
                               Volumes = x.Volumes,
                               Processo = new Processo
                               {
                                   Id = x.Processo.Id,
                                   Autor = x.Processo.Autor,
                                   Reu = x.Processo.Reu,
                                   Numero = x.Processo.Numero,
                                   Advogado = new Cliente
                                   {
                                       Id = x.Processo.Advogado.Id,
                                       Nome = x.Processo.Advogado.Nome
                                   }
                               }
                           });

            return Ok(result);
        }

        [HttpGet]
        [Route("numero")]
        public IActionResult GetByNumber([FromQuery] string filter)
        {
            var result = db.Servicos
                            .Include(x => x.TipoServico)
                            .Include(x => x.Processo).ThenInclude(x => x.Advogado)
                            .Include(x => x.Atividades).ThenInclude(x => x.Responsavel)
                            .Include(x => x.Atividades).ThenInclude(x => x.TipoAtividade)
                            .Where(x => string.IsNullOrEmpty(filter)
                                             || x.Processo.Numero.Contains(filter))
                            .Select(x => new Servico
                            {
                                Id = x.Id,
                                Entrada = x.Entrada,
                                TipoServicoId = x.TipoServicoId,
                                TipoImpressao = x.TipoImpressao,
                                TipoServico = new TipoServico
                                {
                                    Id = x.TipoServico.Id,
                                    Nome = x.TipoServico.Nome,
                                },
                                Prazo = x.Prazo,
                                Saida = x.Saida,
                                Status = x.Status,
                                Volumes = x.Volumes,
                                Processo = new Processo
                                {
                                    Id = x.Processo.Id,
                                    Autor = x.Processo.Autor,
                                    Reu = x.Processo.Reu,
                                    Numero = x.Processo.Numero,
                                    Local = x.Processo.Local,
                                    Parte = x.Processo.Parte,
                                    Advogado = new Cliente
                                    {
                                        Id = x.Processo.Advogado.Id,
                                        Nome = x.Processo.Advogado.Nome,
                                        Telefone = x.Processo.Advogado.Telefone,
                                        Celular = x.Processo.Advogado.Celular,
                                        Email = x.Processo.Advogado.Email,
                                    }
                                },
                                Atividades = x.Atividades.Select(a => new Atividade
                                {
                                    Id = a.Id,
                                    Entrega = a.Entrega,
                                    TipoAtividade = new TipoAtividade
                                    {
                                        Id = a.TipoAtividade.Id,
                                        Nome = a.TipoAtividade.Nome
                                    },
                                    Responsavel = a.Responsavel,
                                    Tempo = a.Tempo,
                                    EtapaAtividade = a.EtapaAtividade,
                                    TipoImpressao = a.TipoImpressao,
                                    TipoExecucao = a.TipoExecucao,
                                    Valor = a.Valor,
                                }).ToList(),
                            })
                            .ToList();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var result = db.Servicos
                            .Include(x => x.TipoServico)
                            .Include(x => x.Processo).ThenInclude(x => x.Advogado)
                            .Include(x => x.Atividades).ThenInclude(x => x.Responsavel)
                            .Include(x => x.Atividades).ThenInclude(x => x.TipoAtividade)
                            .SingleOrDefault(x => x.Id == id);

            return Ok(result);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Servico model)
        {
            try
            {
                model.Processo = null;
                db.Servicos.Add(model);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                return BadRequest(ex.InnerException.Message);
            }

            var result = db.Servicos
                            .Include(x => x.TipoServico)
                            .Include(x => x.Processo).ThenInclude(x => x.Advogado)
                            .Include(x => x.Atividades).ThenInclude(x => x.Responsavel)
                            .Include(x => x.Atividades).ThenInclude(x => x.TipoAtividade)
                            .Single(x => x.Id == model.Id);
            return Ok(result);
        }

        [HttpPut]
        public IActionResult Put([FromBody] Servico model)
        {
            var item = db.Servicos
                .Include(x => x.Atividades)
                .Single(x => x.Id == model.Id);

            item.Volumes = model.Volumes;
            item.Entrada = model.Entrada;
            item.Prazo = model.Prazo;

            if (item.Atividades.Count > 0 && model.Saida != null)
            {
                if (item.Atividades.Any(x => x.TipoExecucao != TipoExecucaoEnum.Finalizado && x.TipoExecucao != TipoExecucaoEnum.Revisar && x.TipoExecucao != TipoExecucaoEnum.Refazer))
                    return BadRequest("Existem atividades não finalizadas, impossível inserir data de saída.");

                item.Saida = model.Saida;
                item.Status = StatusEnum.Entregue;
            }

            db.SaveChanges();

            var result = db.Servicos
                           .Include(x => x.TipoServico)
                           .Include(x => x.Processo).ThenInclude(x => x.Advogado)
                           .Include(x => x.Atividades).ThenInclude(x => x.Responsavel)
                           .Include(x => x.Atividades).ThenInclude(x => x.TipoAtividade)
                           .Single(x => x.Id == model.Id);
            return Ok(result);
        }

        [HttpPost("cancel/{id}")]
        public IActionResult Cancel(int id)
        {
            var servico = db.Servicos
                        .Include(x => x.Atividades)
                        .Single(x => x.Id == id);

            servico.Status = StatusEnum.Cancelado;

            foreach (var item in servico.Atividades)
            {
                switch (item.TipoExecucao)
                {
                    case TipoExecucaoEnum.Pendente:
                        item.TipoExecucao = TipoExecucaoEnum.Cancelado;
                        break;
                    case TipoExecucaoEnum.Finalizado:
                        break;
                    case TipoExecucaoEnum.Revisar:
                        item.TipoExecucao = TipoExecucaoEnum.RevisarCancelado;
                        break;
                    case TipoExecucaoEnum.Refazer:
                        item.TipoExecucao = TipoExecucaoEnum.RefazerCancelado;
                        break;
                    case TipoExecucaoEnum.Cancelado:
                        break;
                    case TipoExecucaoEnum.RevisarCancelado:
                        break;
                    case TipoExecucaoEnum.RefazerCancelado:
                        break;
                    default:
                        break;
                }
            }

            db.SaveChanges();
            return Ok(servico);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var item = db.Servicos.Single(x => x.Id == id);
            db.Servicos.Remove(item);
            db.SaveChanges();
            return Ok(item);
        }

        [Route("status")]
        [HttpGet]
        public IActionResult GetStatus()
        {
            var result = Enum.GetValues(typeof(StatusEnum))
                            .Cast<StatusEnum>()
                            .Select(x => new KeyValuePair<int, string>((int)x, EnumHelpers.GetEnumDescription(x)));
            return Ok(result);
        }
    }
}
