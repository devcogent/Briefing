using Briefing.Models.Entites;
using Briefing.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Briefing.Data
{
    public class AppDbContext : DbContext   
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Briefings> briefings { get; set; } = default!;
        public DbSet<BriefAcknowledge> briefAcknowledges { get; set; } = default!;
        public DbSet<BriefingForBrief> briefingForBriefs { get; set; } = default!;
        public DbSet<Auth> auths { get; set; } = default!;

        public DbSet<BriefingFor> briefingFors { get; set; } = default!;
        public DbSet<ClientMaster> clientMasters { get; set; } = default!;
    }
}
