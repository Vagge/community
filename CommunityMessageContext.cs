using Community.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Community.Data
{
    public class CommunityMessageContext : DbContext
    {
        public CommunityMessageContext(DbContextOptions<CommunityMessageContext> options)
            : base(options)
        {
        }
        public DbSet<Message> Messages { get; set; }

    }
}
