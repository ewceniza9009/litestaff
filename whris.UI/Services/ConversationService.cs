using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace whris.UI.Services
{
    public class LocalConfig
    {
        public static string ConnectionString => Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "localStorage", "conversations.db");
    }

    public class Conversation
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public string History { get; set; }
        public DateTime LastModified { get; set; }
        public bool Pin { get; set; }     
    }

    public class LocalAppDbContext : DbContext
    {
        public DbSet<Conversation> Conversations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var dbPath = LocalConfig.ConnectionString;
                Directory.CreateDirectory(Path.GetDirectoryName(dbPath));
                optionsBuilder.UseSqlite($"Data Source={dbPath}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Conversation>().HasKey(c => c.Id);
            modelBuilder.Entity<Conversation>().Property(c => c.UserId).IsRequired();
            modelBuilder.Entity<Conversation>().HasIndex(c => c.UserId);
            modelBuilder.Entity<Conversation>().Property(c => c.Pin).HasDefaultValue(false);      
        }
    }

    public interface IConversationService
    {
        Task<List<Conversation>> GetAllAsync(string userId);
        Task<Conversation> GetByIdAsync(int id, string userId);
        Task<Conversation> AddAsync(Conversation conversation);
        Task UpdateAsync(Conversation conversation);
        Task DeleteAsync(int id, string userId);
        Task<bool> TogglePinAsync(int id, string userId);
    }

    public class ConversationService : IConversationService
    {
        private LocalAppDbContext GetDbContext() => new LocalAppDbContext();

        public ConversationService()
        {
            EnsureDatabaseCreated();
        }

        private void EnsureDatabaseCreated()
        {
            using (var context = GetDbContext())
            {
                context.Database.EnsureCreated();
            }
        }

        public async Task<List<Conversation>> GetAllAsync(string userId)
        {
            using (var context = GetDbContext())
            {
                return await context.Conversations
                    .Where(c => c.UserId == userId)
                    .OrderByDescending(c => c.Pin)    
                    .ThenByDescending(c => c.LastModified)
                    .ToListAsync();
            }
        }

        public async Task<Conversation> GetByIdAsync(int id, string userId)
        {
            using (var context = GetDbContext())
            {
                return await context.Conversations
                    .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
            }
        }

        public async Task<Conversation> AddAsync(Conversation conversation)
        {
            using (var context = GetDbContext())
            {
                await context.Conversations.AddAsync(conversation);
                await context.SaveChangesAsync();
                return conversation;
            }
        }

        public async Task UpdateAsync(Conversation conversation)
        {
            using (var context = GetDbContext())
            {
                context.Conversations.Update(conversation);
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id, string userId)
        {
            using (var context = GetDbContext())
            {
                var record = await context.Conversations
                    .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
                if (record != null)
                {
                    context.Conversations.Remove(record);
                    await context.SaveChangesAsync();
                }
            }
        }

        public async Task<bool> TogglePinAsync(int id, string userId)
        {
            using (var context = GetDbContext())
            {
                var conversation = await context.Conversations
                    .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
                if (conversation != null)
                {
                    conversation.Pin = !conversation.Pin;
                    await context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
        }
    }
}