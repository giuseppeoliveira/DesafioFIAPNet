using SchoolAPI.Domain;
using SchoolAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace SchoolAPI.Infrastructure;


public interface IAlunoRepository
{
    Task<PaginacaoResponse<Aluno>> GetAlunosAsync(
        PaginacaoQuery query,
        string nomeQuery = "",
        string cpfQuery = "",
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<Aluno>> GetAlunosSameCpfOrEmailAsync(
        string emailQuery, 
        string cpfQuery, 
        CancellationToken cancellationToken = default
    );

    Task<int> CreateAlunoAsync(Aluno request,
                           CancellationToken cancellationToken = default);

    Task<bool> UpdateAlunoAsync(
        int id,
        Aluno request,
        CancellationToken cancellationToken = default
    );

    Task DeleteAlunoAsync(
        int id,
        CancellationToken cancellationToken = default
    );
}


public class AlunoRepository : IAlunoRepository
{
    private readonly SchoolAPIContext context;

    public AlunoRepository(SchoolAPIContext context)
    {
        this.context = context;
    }

    public async Task<int> CreateAlunoAsync(Aluno request, CancellationToken cancellationToken = default)
    {
        request.Datanascimento = DateOnly.FromDateTime(request.Datanascimento.ToDateTime(TimeOnly.MinValue).ToUniversalTime());
        request.Datacriado = DateTime.UtcNow;
        await context.Aluno.AddAsync(request, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return request.Id;
    }

    public async Task DeleteAlunoAsync(int id, CancellationToken cancellationToken = default)
    {
        await context.Matricula.Where(x => x.Alunoid == id).ExecuteDeleteAsync(cancellationToken);
        await context.Aluno.Where(x => x.Id == id).ExecuteDeleteAsync(cancellationToken); 
    }

    public async Task<PaginacaoResponse<Aluno>> GetAlunosAsync(
        PaginacaoQuery query, 
        string nomeQuery = "", 
        string cpfQuery = "", 
        CancellationToken cancellationToken = default
    )
    {
        var dbQuery = context.Aluno.Where(a => 
        (
            (nomeQuery.Length > 0 && a.Nome.Contains(nomeQuery)) || nomeQuery.Length == 0 
        ) && (
            (cpfQuery.Length > 0 && a.Cpf.StartsWith(cpfQuery) ) || cpfQuery.Length == 0
        )).OrderBy(a => a.Nome); 

    var totalCount = await dbQuery.CountAsync(cancellationToken);
    var totalPages = (int)Math.Ceiling((double)totalCount / query.TamanhoPagina);
    if (totalPages < 1) totalPages = 1;

        var skipAmount = (query.Pagina - 1) *  query.TamanhoPagina;

        var allItems = await dbQuery.Skip(skipAmount).Take(query.TamanhoPagina).ToArrayAsync(cancellationToken);

        return new PaginacaoResponse<Aluno>(totalCount, totalPages, allItems);
    }

    public async Task<IEnumerable<Aluno>> GetAlunosSameCpfOrEmailAsync(
        string emailQuery, 
        string cpfQuery, 
        CancellationToken cancellationToken = default
    )
    {
        var dbQuery = context.Aluno.Where(a => a.Email == emailQuery || a.Cpf == cpfQuery);

        return await dbQuery.ToArrayAsync(cancellationToken);
    }

    public async Task<bool> UpdateAlunoAsync(int id, Aluno request, CancellationToken cancellationToken = default)
    {
        var aluno = await context.Aluno.FindAsync([id], cancellationToken: cancellationToken);
        if (aluno == null)
        {
            return false;
        }
        aluno.Nome = request.Nome;
        aluno.Datanascimento = DateOnly.FromDateTime(request.Datanascimento.ToDateTime(TimeOnly.MinValue).ToUniversalTime());
        aluno.Cpf = request.Cpf;
        aluno.Email = request.Email;
        aluno.Senha = request.Senha;
        aluno.Salt = request.Salt;
        context.Aluno.Update(aluno);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}