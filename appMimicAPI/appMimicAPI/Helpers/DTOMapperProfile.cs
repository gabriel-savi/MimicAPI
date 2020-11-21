using appMimicAPI.Models;
using appMimicAPI.Models.DTO;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace appMimicAPI.Helpers
{
    public class DTOMapperProfile : Profile
    {
        public DTOMapperProfile()
        {
            CreateMap<Palavra, PalavraDTO>();
            CreateMap<PaginationList<Palavra>, PaginationList<PalavraDTO>>();
        }
    }
}
