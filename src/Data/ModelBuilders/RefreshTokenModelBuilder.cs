using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.ModelBuilders;

public static class RefreshTokenModelBuilder
{
    public static void BuildRefreshTokenModel(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RefreshToken>()
            .HasOne(rt => rt.User)
            .WithMany()
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
