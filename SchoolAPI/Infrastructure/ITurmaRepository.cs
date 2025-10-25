using Microsoft.EntityFrameworkCore;
using SchoolAPI.Domain;
using SchoolAPI.Domain.Entities;

namespace SchoolAPI.Infrastructure;

public interface ITurmaRepository
{
    Task<PaginacaoResponse<Turma>> GetTurmaAsync(
        PaginacaoQuery query,
        string nomeQuery = "",
        CancellationToken cancellationToken = default
    );

    Task<Turma?> GetTurmaByIdAsync(
        int id,
        CancellationToken cancellationToken = default
    );

    Task<int> CreateTurmaAsync(
        Turma request,
        CancellationToken cancellationToken = default
    );

    Task<bool> UpdateTurmaAsync(
        int id,
        Turma request,
        CancellationToken cancellationToken = default
    );

    Task DeleteTurmaAsync(
        int id,
        CancellationToken cancellationToken = default
    );
}

public class TurmaRepository : ITurmaRepository
{
    private readonly SchoolAPIContext context;

    public TurmaRepository(SchoolAPIContext context)
    {
        this.context = context;
    }

    public async Task<int> CreateTurmaAsync(Turma request, CancellationToken cancellationToken = default)
    {
        await context.Turma.AddAsync(request, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return request.Id;
    }

    public async Task DeleteTurmaAsync(int id, CancellationToken cancellationToken = default)
    {
        await context.Matricula.Where(x => x.Turmaid == id).ExecuteDeleteAsync(cancellationToken);
        await context.Turma.Where(x => x.Id == id).ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<PaginacaoResponse<Turma>> GetTurmaAsync(
        PaginacaoQuery query, 
        string nomeQuery = "", 
        CancellationToken cancellationToken = default
    )
    {
        var dbQuery = context.Turma
            .Include(x => x.Matricula)
            .Where(t =>
                (nomeQuery.Length > 0 && t.Nome.Contains(nomeQuery)) || nomeQuery.Length == 0
            ).OrderBy(a => a.Nome); 

        var totalCount = await dbQuery.CountAsync(cancellationToken);
        var totalPages = totalCount / query.TamanhoPagina;

        var skipAmount = (query.Pagina - 1) * query.TamanhoPagina;

        var allItems = await dbQuery.Skip(skipAmount).Take(query.TamanhoPagina).ToArrayAsync(cancellationToken);

        return new PaginacaoResponse<Turma>(totalCount, totalPages, allItems);
    }

    public async Task<Turma?> GetTurmaByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await context.Turma
            .Include(x => x.Matricula)
            .ThenInclude(m => m.Aluno)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<bool> UpdateTurmaAsync(int id, Turma request, CancellationToken cancellationToken = default)
    {
        var turma = await context.Turma.FindAsync([id], cancellationToken: cancellationToken);
        if (turma == null)
        {
            return false;
        }

        turma.Nome = request.Nome;
        turma.Descricao= request.Descricao;
        context.Turma.Update(turma);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
