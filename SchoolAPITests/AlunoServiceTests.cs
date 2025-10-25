using NSubstitute;
using SchoolAPI.Application;
using System.Collections.Generic;
using SchoolAPI.Application.UseCases;
using SchoolAPI.Domain;
using SchoolAPI.Domain.Entities;
using SchoolAPI.Infrastructure;

namespace SchoolAPITests;

public class AlunoServiceTests
{
    private readonly IAlunoRepository alunoRepository = Substitute.For<IAlunoRepository>();
    private readonly IMatriculaRepository matriculaRepository = Substitute.For<IMatriculaRepository>();
    private readonly ICryptographyService cryptographyService = Substitute.For<ICryptographyService>();
    private readonly AlunoService service;

    public AlunoServiceTests()
    {
        service = new AlunoService(alunoRepository, cryptographyService, matriculaRepository);
    }

    [Fact]
    public async Task CreateAlunoAsync_Throws_WhenConflictExists()
    {
    // Preparação (Arrange)
        var request = new IAlunoService.AlunoCadastroRequest(
            Nome: "Maria",
            DataNascimento: new DateTime(2000, 1, 2, 13, 45, 0, DateTimeKind.Utc),
            Cpf: "123.456.789-00",
            Email: "maria@example.com",
            Senha: "Secret!"
        );
        alunoRepository
            .GetAlunosSameCpfOrEmailAsync(request.Email, request.Cpf, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IEnumerable<Aluno>>(new[] {
                new Aluno {
                    Id = 1,
                    Nome = request.Nome,
                    Datanascimento = DateOnly.FromDateTime(request.DataNascimento),
                    Cpf = request.Cpf,
                    Email = request.Email,
                    Senha = new byte[] { },
                    Salt = new byte[] { }
                }
            }));

    // Ação e Verificação (Act & Assert)
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateAlunoAsync(request));
        Assert.Equal("Aluno ja estava presente", ex.Message);
    }

    [Fact]
    public async Task CreateAlunoAsync_Success_CleansCpf_Encrypts_And_CallsRepo()
    {
    // Preparação (Arrange)
        var request = new IAlunoService.AlunoCadastroRequest(
            Nome: "João",
            DataNascimento: new DateTime(1995, 5, 10, 8, 30, 0, DateTimeKind.Utc),
            Cpf: "321.654.987-00",
            Email: "joao@example.com",
            Senha: "P@ssw0rd"
        );

        alunoRepository
            .GetAlunosSameCpfOrEmailAsync(request.Email, request.Cpf, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IEnumerable<Aluno>>(Array.Empty<Aluno>()));

        var expectedSalt = new byte[] { 1, 2, 3 };
        var expectedCipher = new byte[] { 4, 5, 6 };
        cryptographyService.PasswordEncypt(request.Senha).Returns((expectedSalt, expectedCipher));

        var cleanedCpf = "32165498700";
        alunoRepository
            .CreateAlunoAsync(
                Arg.Is<Aluno>(a =>
                    a.Nome == request.Nome &&
                    a.Datanascimento == DateOnly.FromDateTime(request.DataNascimento) &&
                    a.Cpf == cleanedCpf &&
                    a.Email == request.Email &&
                    a.Salt.SequenceEqual(expectedSalt) &&
                    a.Senha.SequenceEqual(expectedCipher) &&
                    a.Datacriado.HasValue
                ),
                Arg.Any<CancellationToken>()
            )
            .Returns(Task.FromResult(42));

    // Ação (Act)
        var id = await service.CreateAlunoAsync(request);

    // Verificação (Assert)
        Assert.Equal(42, id);
    alunoRepository.Received(1).CreateAlunoAsync(Arg.Any<Aluno>(), Arg.Any<CancellationToken>());
    cryptographyService.Received(1).PasswordEncypt(request.Senha);
    }

    [Fact]
    public async Task UpdateAlunoAsync_Throws_WhenConflictExists()
    {
    // Preparação (Arrange)
        var request = new IAlunoService.AlunoCadastroRequest(
            Nome: "Ana",
            DataNascimento: new DateTime(1990, 12, 1),
            Cpf: "111.222.333-44",
            Email: "ana@example.com",
            Senha: "abc"
        );

        alunoRepository
            .GetAlunosSameCpfOrEmailAsync(request.Email, request.Cpf, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IEnumerable<Aluno>>(new[] {
                new Aluno {
                    Id = 2,
                    Nome = request.Nome,
                    Datanascimento = DateOnly.FromDateTime(request.DataNascimento),
                    Cpf = request.Cpf,
                    Email = request.Email,
                    Senha = new byte[] { },
                    Salt = new byte[] { }
                }
            }));

    // Ação e Verificação (Act & Assert)
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateAlunoAsync(10, request));
        Assert.Equal("Aluno ja estava presente", ex.Message);
    }

    [Fact]
    public async Task UpdateAlunoAsync_Throws_WhenAlunoNotFound()
    {
    // Preparação (Arrange)
        var request = new IAlunoService.AlunoCadastroRequest(
            Nome: "Ana",
            DataNascimento: new DateTime(1990, 12, 1),
            Cpf: "111.222.333-44",
            Email: "ana@example.com",
            Senha: "abc"
        );

        alunoRepository
            .GetAlunosSameCpfOrEmailAsync(request.Email, request.Cpf, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IEnumerable<Aluno>>(Array.Empty<Aluno>()));

        cryptographyService.PasswordEncypt(request.Senha).Returns((new byte[] { 9 }, new byte[] { 8 }));

        alunoRepository
            .UpdateAlunoAsync(10, Arg.Any<Aluno>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(false));

    // Ação e Verificação (Act & Assert)
    var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => service.UpdateAlunoAsync(10, request));
        Assert.Equal("Aluno nao encontrado", ex.Message);
    }

    [Fact]
    public async Task GetAlunosAsync_WhenCpfLengthIs1_PassesEmptyCpfAndMapsFields()
    {
        // Arrange
        var query = new PaginacaoQuery(Pagina: 2, TamanhoPagina: 5);
        var aluno = new Aluno
        {
            Id = 7,
            Nome = "Bia",
            Email = "bia@example.com",
            Datanascimento = new DateOnly(2001, 7, 15),
            Cpf = "00000000000",
            Senha = new byte[] { 0 },
            Salt = new byte[] { 0 }
        };
        alunoRepository
            .GetAlunosAsync(
                query,
                "Bia",
                "",
                Arg.Any<CancellationToken>()
            )
            .Returns(Task.FromResult(new PaginacaoResponse<Aluno>(
                QntdItens: 1,
                Paginas: 3,
                Items: new[] { aluno }
            )));

    // Ação (Act)
        var result = await service.GetAlunosAsync(query, nomeQuery: "Bia", cpfQuery: "1");

    // Verificação: repositório foi chamado com CPF vazio
    alunoRepository.Received(1).GetAlunosAsync(query, "Bia", "", Arg.Any<CancellationToken>());

        Assert.Equal(1, result.QntdItens);
        Assert.Equal(3, result.Paginas);
        var item = Assert.Single(result.Items);
        Assert.Equal(aluno.Id, item.Id);
        Assert.Equal(aluno.Nome, item.Nome);
        Assert.Equal(aluno.Email, item.Email);
        Assert.Equal(aluno.Datanascimento.ToDateTime(TimeOnly.MinValue), item.DataNascimento);
    }

    [Fact]
    public async Task GetAlunosAsync_WhenCpfProvided_UsesDigitsOnly()
    {
        // Arrange
        var query = new PaginacaoQuery();
        var rawCpf = "12.345-678/90";
        var digitsOnly = "1234567890";

        alunoRepository
            .GetAlunosAsync(
                query,
                Arg.Is<string>(s => s == string.Empty),
                Arg.Is<string>(s => s == digitsOnly),
                Arg.Any<CancellationToken>()
            )
            .Returns(Task.FromResult(new PaginacaoResponse<Aluno>(0, 0, Array.Empty<Aluno>())));

    // Ação (Act)
        var result = await service.GetAlunosAsync(query, nomeQuery: null, cpfQuery: rawCpf);

    // Verificação (Assert)
        alunoRepository.Received(1).GetAlunosAsync(
            query,
            "",
            digitsOnly,
            Arg.Any<CancellationToken>()
        );
        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task MatricularAlunoAsync_ReturnsExistingId_WhenAlreadyEnrolled()
    {
    // Preparação (Arrange)
        var alunoId = 5;
        var turmaId = 10;
        var existing = new Matricula { Id = 99, Alunoid = alunoId, Turmaid = turmaId };

        matriculaRepository
            .MatriculaMesmoAlunoTurma(alunoId, turmaId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Matricula?>(existing));

    // Ação e Verificação - agora o serviço lança exceção quando já matriculado
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.MatricularAlunoAsync(alunoId, turmaId));
        Assert.Equal("Aluno ja está matriculado nessa turma", ex.Message);
        matriculaRepository.DidNotReceive().AdicionarMatricula(Arg.Any<Matricula>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task MatricularAlunoAsync_CreatesAndReturnsId_WhenNotEnrolled()
    {
    // Preparação (Arrange)
        var alunoId = 6;
        var turmaId = 11;

        matriculaRepository
            .MatriculaMesmoAlunoTurma(alunoId, turmaId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Matricula?>(null));

        matriculaRepository
            .When(x => x.AdicionarMatricula(Arg.Any<Matricula>(), Arg.Any<CancellationToken>()))
            .Do(ci =>
            {
                var m = ci.Arg<Matricula>();
                // simula o EF atribuindo o Id após o save
                m.Id = 123;
            });

    // Ação (Act)
        var id = await service.MatricularAlunoAsync(alunoId, turmaId);

    // Verificação (Assert)
        Assert.Equal(123, id);
    matriculaRepository.Received(1).AdicionarMatricula(Arg.Is<Matricula>(m => m.Alunoid == alunoId && m.Turmaid == turmaId), Arg.Any<CancellationToken>());
    }
}
