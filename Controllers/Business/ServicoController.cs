﻿using Calcular.CoreApi.Models;
using Calcular.CoreApi.Models.Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
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
        public IActionResult GetAll([FromQuery] string filter)
        {
            var result = db.Servicos
                            .Include(x => x.Processo).ThenInclude(x => x.Advogado)
                            .Include(x => x.Atividades).ThenInclude(x => x.Responsavel)
                            .Where(x => string.IsNullOrEmpty(filter)
                                        || x.Processo.Numero.Contains(filter)
                                        || x.Processo.Advogado.Nome.Contains(filter)
                                        || x.Processo.Advogado.Telefone.Contains(filter)
                                        || x.Processo.Advogado.Celular.Contains(filter))
                            .ToList()
                            .Select(x => new Servico
                            {
                                Id = x.Id,
                                Entrada = x.Entrada,
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
                                    TipoAtividade = a.TipoAtividade,
                                    Responsavel = a.Responsavel,
                                    Tempo = a.Tempo,
                                    EtapaAtividade = a.EtapaAtividade,
                                    TipoImpressao = a.TipoImpressao,
                                    TipoExecucao = a.TipoExecucao,
                                }).ToList(),
                            })
                            .ToList();

            return Ok(result);
        }

        [HttpGet]
        [Route("numero")]
        public IActionResult GetByNumber([FromQuery] string filter)
        {
            var result = db.Servicos
                            .Include(x => x.Processo).ThenInclude(x => x.Advogado)
                            .Include(x => x.Atividades).ThenInclude(x => x.Responsavel)
                            .Include(x => x.Atividades).ThenInclude(x => x.TipoAtividade)
                            .Where(x => string.IsNullOrEmpty(filter)
                                             || x.Processo.Numero.Contains(filter))
                            .Select(x => new Servico
                            {
                                Id = x.Id,
                                Entrada = x.Entrada,
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
                                    TipoAtividade = a.TipoAtividade,
                                    Responsavel = a.Responsavel,
                                    Tempo = a.Tempo,
                                    EtapaAtividade = a.EtapaAtividade,
                                    TipoImpressao = a.TipoImpressao,
                                    TipoExecucao = a.TipoExecucao,
                                }).ToList(),
                            })
                            .ToList();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var result = db.Servicos
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
                            .Include(x => x.Processo).ThenInclude(x => x.Advogado)
                            .Include(x => x.Atividades).ThenInclude(x => x.Responsavel)
                            .Include(x => x.Atividades).ThenInclude(x => x.TipoAtividade)
                            .Single(x => x.Id == model.Id);
            return Ok(result);
        }

        [HttpPut]
        public IActionResult Put([FromBody] Servico model)
        {
            var item = db.Servicos.Single(x => x.Id == model.Id);

            item.Volumes = model.Volumes;
            item.Entrada = model.Entrada;
            item.Prazo = model.Prazo;
            item.Saida = model.Saida;

            db.SaveChanges();

            var result = db.Servicos
                           .Include(x => x.Processo).ThenInclude(x => x.Advogado)
                           .Include(x => x.Atividades).ThenInclude(x => x.Responsavel)
                           .Include(x => x.Atividades).ThenInclude(x => x.TipoAtividade)
                           .Single(x => x.Id == model.Id);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var item = db.Servicos.Single(x => x.Id == id);
            db.Servicos.Remove(item);
            db.SaveChanges();
            return Ok(item);
        }
    }
}
