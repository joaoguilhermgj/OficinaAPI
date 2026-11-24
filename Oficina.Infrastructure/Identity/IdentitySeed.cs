using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Oficina.Infrastructure.Identity
{
    public static class IdentitySeed
    {
        public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration)
        {
            using var scope = services.CreateScope();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            foreach (var perfil in Perfis.Todos)
            {
                if (!await roleManager.RoleExistsAsync(perfil))
                    await roleManager.CreateAsync(new IdentityRole(perfil));
            }

            var nome = configuration["AdminInicial:Nome"];
            var email = configuration["AdminInicial:Email"];
            var senha = configuration["AdminInicial:Senha"];

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
                throw new InvalidOperationException("Configure AdminInicial:Email e AdminInicial:Senha no User Secrets.");

            if (await userManager.FindByEmailAsync(email) is not null)
                return;

            var admin = new ApplicationUser
            {
                UserName = email,
                Email = email,
                Nome = string.IsNullOrWhiteSpace(nome) ? "Administrador" : nome
            };

            var resultado = await userManager.CreateAsync(admin, senha);

            if (!resultado.Succeeded)
                throw new InvalidOperationException("Erro ao criar o Admin inicial: " + string.Join(", ", resultado.Errors.Select(e => e.Description)));

            await userManager.AddToRoleAsync(admin, Perfis.Admin);
        }
    }
}