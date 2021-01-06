using Microsoft.EntityFrameworkCore;

namespace is4FirstDemo.IdentityUserStore
{
    public class UserStoreDbContext1:DbContext
    {
        public UserStoreDbContext1(DbContextOptions opt) : base(opt)
        {

        }
        public DbSet<IdentityUser1> IdentityUser { get; set; }
        public DbSet<IdentityUserClaim1> IdentityUserClaim { get; set; }
    }
}
