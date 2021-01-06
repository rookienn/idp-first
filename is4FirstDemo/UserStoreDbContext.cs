using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace is4FirstDemo
{
    public class UserStoreDbContext: IdentityDbContext
    {
        public DbSet<CustomIdentityUser> CustomUsers { get; set; }
        public UserStoreDbContext(DbContextOptions<UserStoreDbContext> opt) : base(opt) { }
    }
}
