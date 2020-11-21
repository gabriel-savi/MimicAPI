using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace appMimicAPI.Models.DTO
{
    public class PalavraDTO : BaseDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public int Pontuacao { get; set; }
        public bool Ativo { get; set; }
        public DateTime DtaCriacao { get; set; }
        public DateTime? DtaAlteracao { get; set; }
    }
}
