using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using VoucherHub.Modules.Vouchers.Infrastructure.Persistence;

namespace VoucherHub.Tests;

public class VoucherHubWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<VoucherHubDbContext>>();
            services.RemoveAll<VoucherHubDbContext>();
            services.RemoveAll<IDbContextOptionsConfiguration<VoucherHubDbContext>>();

            services.AddDbContext<VoucherHubDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName));
        });
    }
}
