using SchoolAPI.Domain;
using SchoolAPI.Domain.Entities;
using SchoolAPI.Infrastructure;

namespace SchoolAPI.Application.UseCases;

public interface ITurmaService
{
    Task<PaginacaoResponse<TurmaListResponse>> GetTurmaAsync(
        PaginacaoQuery query,
        string? nomeQuery = null,
        CancellationToken cancellationToken = default
    );

    public record TurmaListResponse(
        string Nome,
        string Descricao,
        int Id,
        int QuantidadeAlunos
    );

    Task<TurmaDetalhesResponse?> GetTurmaByIdAsync(int id,
                                                  CancellationToken cancellationToken = default);
    public record TurmaDetalhesResponse(
        string Nome,
        string Descricao,
        int Id,
        int QuantidadeAlunos,
        IEnumerable<TurmaDetalhesResponse.AlunoSimplesResponse> Alunos
    )
    {
        public record AlunoSimplesResponse(
            string Nome,
            int Id
        );
    }

    Task<int> CreateTurmaAsync(TurmaCadastroRequest request,
                           CancellationToken cancellationToken = default);
    public record TurmaCadastroRequest(
    string Nome,
    string Descricao
);

    Task UpdateTurmaAsync(
        int id,
        TurmaCadastroRequest request,
        CancellationToken cancellationToken = default
    );

    Task DeleteTurmaAsync(
        int id,
        CancellationToken cancellationToken = default
    );
}

public class TurmaService : ITurmaService
{
    private readonly ITurmaRepository turmaRepository;

    public TurmaService(ITurmaRepository turmaRepository)
    {
        this.turmaRepository = turmaRepository;
    }

    public Task<int> CreateTurmaAsync(ITurmaService.TurmaCadastroRequest request, CancellationToken cancellationToken = default)
    {
        return turmaRepository.CreateTurmaAsync(
            new Turma
            {
                Nome = request.Nome,
                Descricao = request.Descricao
            },
            cancellationToken
        );
    }

    public Task DeleteTurmaAsync(int id, CancellationToken cancellationToken = default)
    {
        return turmaRepository.DeleteTurmaAsync(id, cancellationToken);
    }

    public async Task<PaginacaoResponse<ITurmaService.TurmaListResponse>> GetTurmaAsync(
        PaginacaoQuery query, 
        string? nomeQuery = null, 
        CancellationToken cancellationToken = default
    )
    {
        var turmaResponse = await turmaRepository.GetTurmaAsync(
           query,
           nomeQuery ?? "",
           cancellationToken
       );

        return new PaginacaoResponse<ITurmaService.TurmaListResponse>(
            turmaResponse.QntdItens,
            turmaResponse.Paginas,
            turmaResponse.Items
                .Select(x => new ITurmaService.TurmaListResponse(
                    x.Nome,
                    x.Descricao,
                    x.Id,
                    x.Matricula.Count
                ))
                .ToArray()
        );
    }

    public async Task<ITurmaService.TurmaDetalhesResponse?> GetTurmaByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var turma = await turmaRepository.GetTurmaByIdAsync(id, cancellationToken);
        return turma is null ? null : new ITurmaService.TurmaDetalhesResponse(
            turma.Nome,
            turma.Descricao,
            turma.Id,
            turma.Matricula.Count,
            turma.Matricula.Select(m => new ITurmaService.TurmaDetalhesResponse.AlunoSimplesResponse(
                m.Aluno.Nome,
                m.Aluno.Id
            ))
        );
    }

    public async Task UpdateTurmaAsync(int id, ITurmaService.TurmaCadastroRequest request, CancellationToken cancellationToken = default)
    {
        var turma = new Turma
        {
            Nome = request.Nome,
            Descricao = request.Descricao
        };

        var atualizou = await turmaRepository.UpdateTurmaAsync(id, turma, cancellationToken);
        if (!atualizou)
            throw new KeyNotFoundException("Turma nao encontrada");
    }
}

