using appMimicAPI.Database;
using appMimicAPI.Helpers;
using appMimicAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace appMimicAPI.Controllers
{
    [Route("/api/palavras")]
    public class PalavrasController : ControllerBase
    {
        private readonly MimicContext _banco;
        public PalavrasController(MimicContext banco)
        {
            _banco = banco;
        }

        //APP -- /api/palavras/data?2020-11-13
        [Route("")]
        [HttpGet]
        public ActionResult ObterPalavras([FromQuery] PalavraUrlQuery query)
        {
            var item = _banco.Palavras.AsQueryable();
            if (query.Data.HasValue)
            {
                item = item.Where(a => a.DtaCriacao > query.Data.Value || a.DtaAlteracao > query.Data.Value);
            }

            if (query.PagNumero.HasValue)
            {
                var qtdeTotRegistro = item.Count();
                item = item.Skip((query.PagNumero.Value - 1) * query.PagRegistro.Value).Take(query.PagRegistro.Value);

                var paginacao = new Paginacao();
                
                paginacao.NumeroPagina = query.PagNumero.Value;
                paginacao.RegistroPorPagina = query.PagRegistro.Value;
                paginacao.TotalRegistros = qtdeTotRegistro;
                paginacao.TotalPaginas = (int) Math.Ceiling( (double)qtdeTotRegistro / query.PagRegistro.Value);

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginacao));

                if (query.PagNumero > paginacao.TotalPaginas)
                {
                    return NotFound();
                }
            }

            return Ok(item);
        }

        //web
        [Route("{id}")]
        [HttpGet]
        public ActionResult ObterPalavra(int id)
        {
            var obj = _banco.Palavras.Find(id);

            if(obj == null)
                return NotFound();

            return Ok(obj);
        }

        [Route("")]
        [HttpPost]
        public ActionResult Cadastrar([FromBody] Palavra palavra)
        {
            palavra.DtaCriacao = DateTime.Today;
            palavra.Ativo = true;

            _banco.Palavras.Add(palavra);
            _banco.SaveChanges();
            
            return Created($"/api/palavras/{palavra.Id}", palavra);
        }

        [Route("{id}")]
        [HttpPut]
        public ActionResult Atualizar(int id, [FromBody] Palavra palavra)
        {
            var obj = _banco.Palavras.AsNoTracking().FirstOrDefault(a=> a.Id == id);

            if (obj == null)
                return NotFound();

            palavra.Id = id;
            palavra.DtaAlteracao = DateTime.Today;

            _banco.Palavras.Update(palavra);
            _banco.SaveChanges();

            return Ok();
        }

        [Route("{id}")]
        [HttpDelete]
        public ActionResult Deletar(int id)
        {
            var palavra = _banco.Palavras.Find(id);

            if (palavra == null)
                return NotFound();

            palavra.Ativo = false;

            _banco.Palavras.Update(palavra);
            _banco.SaveChanges();

            return NoContent();
        }
    }
}
