using appMimicAPI.Database;
using appMimicAPI.Helpers;
using appMimicAPI.Models;
using appMimicAPI.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace appMimicAPI.Repositories
{
    public class PalavraRepository : Contracts.IPalavraRepository
    {
        private readonly Database.IPalavraRepository _banco;
        public PalavraRepository(Database.IPalavraRepository banco)
        {
            _banco = banco; 
        }

        public PaginationList<Palavra> ObterPalavras(PalavraUrlQuery query)
        {
            var lista = new PaginationList<Palavra>();
            var item = _banco.Palavras.AsNoTracking().AsQueryable();
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
                paginacao.TotalPaginas = (int)Math.Ceiling((double)qtdeTotRegistro / query.PagRegistro.Value);

                lista.Paginacao = paginacao;
            }

            lista.Results.AddRange(item.ToList());
            
            return lista;
        }

        public Palavra ObterPalavra(int id)
        {
            return _banco.Palavras.AsNoTracking().FirstOrDefault(a => a.Id == id);
        }

        public void Cadastrar(Palavra palavra)
        {
            palavra.DtaCriacao = DateTime.Today;
            palavra.Ativo = true;

            _banco.Palavras.Add(palavra);
            _banco.SaveChanges();
        }

        public void Atualizar(Palavra palavra)
        {
            _banco.Palavras.Update(palavra);
            _banco.SaveChanges();
        }

        public void Deletar(int id)
        {
            var palavra = ObterPalavra(id);
            palavra.Ativo = false;
            _banco.Palavras.Update(palavra);
            _banco.SaveChanges();
        }
    }
}
