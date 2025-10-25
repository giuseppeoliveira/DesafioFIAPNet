using SchoolAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace SchoolAPI.Infrastructure;

public interface IUsuarioAdminRepository
{
    Task<Usuarioadmin?> GetUsuarioAdminByEmailAsync(
        string emailQuery,
        CancellationToken cancellationToken = default
    );
}

public class UsuarioAdminRepository : IUsuarioAdminRepository
{
    private readonly SchoolAPIContext context;

    public UsuarioAdminRepository(SchoolAPIContext context)
    {
        this.context = context;
    }

    public async Task<Usuarioadmin?> GetUsuarioAdminByEmailAsync(
        string emailQuery,
        CancellationToken cancellationToken = default
    )
    {
        var usuarioAdmin = await context.Usuarioadmin
            .FirstOrDefaultAsync(x => x.Email == emailQuery, cancellationToken);
            
        return usuarioAdmin;
    }
}
