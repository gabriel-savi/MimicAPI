using appMimicAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace appMimicAPI.Database
{
    public class IPalavraRepository : DbContext
    {
        public IPalavraRepository(DbContextOptions<IPalavraRepository> options) : base(options)
        {

        }

        public DbSet<Palavra> Palavras { get; set; }
    }
}
