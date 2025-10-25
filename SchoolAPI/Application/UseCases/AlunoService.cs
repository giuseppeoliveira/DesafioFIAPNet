using SchoolAPI.Domain;
using SchoolAPI.Domain.Entities;
using SchoolAPI.Infrastructure;

namespace SchoolAPI.Application.UseCases;

public interface IAlunoService
{
    Task<PaginacaoResponse<AlunoListResponse>> GetAlunosAsync(
        PaginacaoQuery query,
        string? nomeQuery = null,
        string? cpfQuery = null,
        CancellationToken cancellationToken = default
    );

    public record AlunoListResponse(
        string Nome,
        DateTime DataNascimento,
        string Email,
        int Id,
        string Cpf 
    );

    Task<int> CreateAlunoAsync(
        AlunoCadastroRequest request,
        CancellationToken cancellationToken = default
    );

    public record AlunoCadastroRequest(
        string Nome,
        DateTime DataNascimento,
        string Cpf,
        string Email,
        string Senha
    );

    Task UpdateAlunoAsync(
        int id,
        AlunoCadastroRequest request,
        CancellationToken cancellationToken = default
    );

    Task DeleteAlunoAsync(
        int id,
        CancellationToken cancellationToken = default
    );


    Task<int> MatricularAlunoAsync(
        int alunoId,
        int turmaId,
        CancellationToken cancellationToken = default
    );
}

public class AlunoService : IAlunoService
{
    private readonly IAlunoRepository alunoRepository;
    private readonly ICryptographyService cryptographyService;
    private readonly IMatriculaRepository matriculaRepository;

    public AlunoService(
        IAlunoRepository alunoRepository,
        ICryptographyService cryptographyService,
        IMatriculaRepository matriculaRepository
    )
    {
        this.alunoRepository = alunoRepository;
        this.cryptographyService = cryptographyService;
        this.matriculaRepository = matriculaRepository;
    }

    public async Task<int> CreateAlunoAsync(IAlunoService.AlunoCadastroRequest request, CancellationToken cancellationToken = default)
    {
        var alunosEmConflito = await alunoRepository.GetAlunosSameCpfOrEmailAsync(
            request.Email,
            request.Cpf,
            cancellationToken
        );

        if (alunosEmConflito.Any())
            throw new InvalidOperationException("Aluno ja estava presente");

        var (salt, cipherPassword) = cryptographyService.PasswordEncypt(request.Senha);

        var aluno = new Aluno
        {
            Nome = request.Nome,
            Datanascimento = DateOnly.FromDateTime(request.DataNascimento),
            Cpf = new string(request.Cpf.Where(char.IsDigit).ToArray()),
            Email = request.Email,
            Datacriado = DateTime.UtcNow,
            Senha = cipherPassword,
            Salt = salt
        };

        return await alunoRepository.CreateAlunoAsync(aluno, cancellationToken);
    }

    public async Task<PaginacaoResponse<IAlunoService.AlunoListResponse>> GetAlunosAsync(
        PaginacaoQuery query,
        string? nomeQuery = null,
        string? cpfQuery = null,
        CancellationToken cancellationToken = default
    )
    {
        var alunosResponse = await alunoRepository.GetAlunosAsync(
            query,
            nomeQuery ?? "",
            cpfQuery is { Length: > 1 } validCpf ? new string(validCpf.Where(char.IsDigit).ToArray()) : "",
            cancellationToken
        );

        return new PaginacaoResponse<IAlunoService.AlunoListResponse>(
            alunosResponse.QntdItens,
            alunosResponse.Paginas,
            alunosResponse.Items
                .Select(x => new IAlunoService.AlunoListResponse(
                    x.Nome,
                    x.Datanascimento.ToDateTime(TimeOnly.MinValue),
                    x.Email,
                    x.Id,
                    x.Cpf
                ))
                .ToArray()
        );
    }

    public async Task UpdateAlunoAsync(
        int id,
        IAlunoService.AlunoCadastroRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var alunosEmConflito = await alunoRepository.GetAlunosSameCpfOrEmailAsync(
            request.Email,
            request.Cpf,
            cancellationToken
        );

        if (alunosEmConflito.Any(a => a.Id != id))
            throw new InvalidOperationException("Aluno ja estava presente");

        var (salt, cipherPassword) = cryptographyService.PasswordEncypt(request.Senha);

        var aluno = new Aluno
        {
            Nome = request.Nome,
            Datanascimento = DateOnly.FromDateTime(request.DataNascimento),
            Cpf = new string(request.Cpf.Where(char.IsDigit).ToArray()),
            Email = request.Email,
            Datacriado = DateTime.UtcNow,
            Senha = cipherPassword,
            Salt = salt
        };

        var atualizou = await alunoRepository.UpdateAlunoAsync(id, aluno, cancellationToken);
        if (!atualizou)
            throw new KeyNotFoundException("Aluno nao encontrado");
    }

    public async Task DeleteAlunoAsync(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        await alunoRepository.DeleteAlunoAsync(id, cancellationToken);
    }

    public async Task<int> MatricularAlunoAsync(
        int alunoId,
        int turmaId,
        CancellationToken cancellationToken = default
    )
    {
        var matriculaExistente = await matriculaRepository.MatriculaMesmoAlunoTurma(
            alunoId,
            turmaId,
            cancellationToken
        );

        if (matriculaExistente is not null)
            throw new InvalidOperationException("Aluno ja está matriculado nessa turma");

        var matricula = new Matricula
        {
            Turmaid = turmaId,
            Alunoid = alunoId
        };

        await matriculaRepository.AdicionarMatricula(matricula, cancellationToken);

        return matricula.Id;
    }
}
