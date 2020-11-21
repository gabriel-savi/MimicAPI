using appMimicAPI.Helpers;
using appMimicAPI.Models;
using appMimicAPI.Models.DTO;
using appMimicAPI.Repositories.Contracts;
using AutoMapper;
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
        private readonly IPalavraRepository _repository;
        private readonly IMapper _mapper;
        public PalavrasController(IPalavraRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        //APP -- /api/palavras/data?2020-11-13
        [HttpGet("", Name = "ObterPalavras")]
        public ActionResult ObterPalavras([FromQuery] PalavraUrlQuery query)
        {
            var itens = _repository.ObterPalavras(query);

            if (itens.Results.Count == 0)
            {
                return NotFound();
            }

            PaginationList<PalavraDTO> lista = CriarLinksListPalavraDTO(query, itens);

            return Ok(lista);
        }

        private PaginationList<PalavraDTO> CriarLinksListPalavraDTO(PalavraUrlQuery query, PaginationList<Palavra> itens)
        {
            var lista = _mapper.Map<PaginationList<Palavra>, PaginationList<PalavraDTO>>(itens);

            foreach (var palavra in lista.Results)
            {
                palavra.Links.Add(
                    new LinkDTO("self", Url.Link("ObterPalavra", new { id = palavra.Id }), "GET")
                );
            }

            lista.Links.Add(
                    new LinkDTO("self", Url.Link("ObterPalavras", query), "GET")
            );

            if (itens.Paginacao != null)
            {
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(itens.Paginacao));

                if (query.PagNumero + 1 <= itens.Paginacao.TotalPaginas)
                {
                    var queryString = new PalavraUrlQuery() { PagNumero = query.PagNumero + 1, PagRegistro = query.PagRegistro, Data = query.Data };
                    lista.Links.Add(
                        new LinkDTO("next", Url.Link("ObterPalavras", queryString), "GET")
                    );
                }
                if (query.PagNumero - 1 > 0)
                {
                    var queryString = new PalavraUrlQuery() { PagNumero = query.PagNumero - 1, PagRegistro = query.PagRegistro, Data = query.Data };
                    lista.Links.Add(
                        new LinkDTO("prev", Url.Link("ObterPalavras", queryString), "GET")
                    );
                }
            }

            return lista;
        }

        //web
        [HttpGet("{id}", Name = "ObterPalavra")]
        public ActionResult ObterPalavra(int id)
        {
            var obj = _repository.ObterPalavra(id);

            if(obj == null)
                return NotFound();

            PalavraDTO palavraDTO = _mapper.Map<Palavra, PalavraDTO>(obj);

            palavraDTO.Links.Add(
                new LinkDTO("self", Url.Link("ObterPalavra", new { id = palavraDTO.Id }), "GET")
            );
            palavraDTO.Links.Add(
                new LinkDTO("update", Url.Link("AtualizarPalavra", new { id = palavraDTO.Id }), "PUT")
            );
            palavraDTO.Links.Add(
                new LinkDTO("delete", Url.Link("DeletarPalavra", new { id = palavraDTO.Id }), "DELETE")
            );

            return Ok(palavraDTO);
        }

        [HttpPost("", Name = "CadastrarPalavra")]
        public ActionResult Cadastrar([FromBody] Palavra palavra)
        {
            if (palavra == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);

            _repository.Cadastrar(palavra);

            PalavraDTO palavraDTO = _mapper.Map<Palavra, PalavraDTO>(palavra);

            palavraDTO.Links = new List<LinkDTO>();
            palavraDTO.Links.Add(
                    new LinkDTO("self", Url.Link("ObterPalavra", new { palavra.Id }), "GET")
            );

            return Created($"/api/palavras/{palavra.Id}", palavraDTO);
        }

        [HttpPut("{id}", Name = "AtualizarPalavra")]
        public ActionResult Atualizar(int id, [FromBody] Palavra palavra)
        {
            var obj = _repository.ObterPalavra(id);

            if (obj == null)
                return NotFound();

            if (palavra == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);

            palavra.Id = id;
            palavra.DtaAlteracao = DateTime.Today;
            _repository.Atualizar(palavra);

            PalavraDTO palavraDTO = _mapper.Map<Palavra, PalavraDTO>(palavra);

            palavraDTO.Links = new List<LinkDTO>();
            palavraDTO.Links.Add(
                    new LinkDTO("self", Url.Link("ObterPalavra", new { palavra.Id }), "GET")
            );

            return Ok();
        }

        [HttpDelete("{id}", Name = "DeletarPalavra")]
        public ActionResult Deletar(int id)
        {
            var palavra = _repository.ObterPalavra(id);

            if (palavra == null)
                return NotFound();

            _repository.Deletar(id);

            return NoContent();
        }
    }
}
