using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SchoolAPI.Application.UseCases;
using SchoolAPI.Domain;

namespace SchoolAPI.Controllers;

[ApiController]
[Route("api/v1/alunos")]
public class AlunoController : ControllerBase
{
    private readonly IAlunoService alunoService;
    public AlunoController(IAlunoService alunoService)
    {
        this.alunoService = alunoService;
    }

    [Authorize]
    [HttpGet(Name = "ListarAlunos")]
    [ProducesResponseType(200, Type = typeof(PaginacaoResponse<IAlunoService.AlunoListResponse>))]
    [ApiDoc("Pesquisa os alunos", "Retorna uma lista paginada de alunos considerando as consultas passadas.")]
    public async Task<IResult> ListarAlunos(
        [FromQuery] string? cpfQuery = null,
        [FromQuery] string? nomeQuery = null,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 10,
        CancellationToken cancellationToken = default
    )
    {

        if(pagina < 1)
            return Results.BadRequest(new { Message = "Página inválida"});

        if(tamanhoPagina < 1)
            return Results.BadRequest(new { Message = "Tamanho da Pagina inválida"});

        var paginatedResponse = await alunoService.GetAlunosAsync(
            new PaginacaoQuery(
                pagina,
                tamanhoPagina
            ),
            nomeQuery: nomeQuery,
            cpfQuery: cpfQuery,
            cancellationToken    
        );

        return Results.Ok(paginatedResponse);
    }

    [Authorize]
    [HttpPost(Name = "CadastrarAluno")]
    [ApiDoc("Cadastra um aluno", "Cadastra um aluno com o conteudo passado, apos verificacoes.")]
    public async Task<IResult> CadastrarAluno(
        [FromBody] IAlunoService.AlunoCadastroRequest alunoRequest,
        CancellationToken cancellationToken = default 
    )
    {
        var (
            nome,
            dataNascimento,
            cpf,
            email,
            senha
        ) = alunoRequest;


        var senhaInvalida = senha is {Length: < 8} || !(
            senha.Any(char.IsUpper) && senha.Any(char.IsLower) && senha.Any(c => !char.IsLetterOrDigit(c))
        );

        if(senhaInvalida)
            return Results.BadRequest(new { Message = "Senha Invalida"});
        
        if(!Validacao.IsValidCpf(cpf))
            return Results.BadRequest(new { Message = "Cpf invalido"});

        if(!Validacao.IsEmailValid(email))
            return Results.BadRequest(new { Message = "Email invalido"});

        var agora = DateTime.UtcNow;

        if((dataNascimento < agora.AddYears(-150)) || (dataNascimento > agora) )
            return Results.BadRequest(new { Message = "Data de Nascimento invalido"});

        if(nome is { Length: < 3 or > 100})
            return Results.BadRequest(new { Message = "Nome invalido"});

        int idCriado = 0;

        try
        {
            idCriado = await alunoService.CreateAlunoAsync(alunoRequest, cancellationToken);
        } catch (InvalidOperationException e)
        {
            return Results.Json(
                new { Message = "Aluno está em conflito com cpf ou email"}, 
                statusCode: StatusCodes.Status403Forbidden
            );
        }

        return  Results.Ok( new { Id = idCriado });
    }

    [Authorize]
    [HttpPut("{id}", Name = "EditarAluno")]
    [ApiDoc("Edita um aluno", "Edita um aluno com o conteudo passado, apos verificacoes.")]
    public async Task<IResult> EditarAluno(
        [FromRoute] int id,
        [FromBody] IAlunoService.AlunoCadastroRequest alunoRequest,
        CancellationToken cancellationToken = default
    )
    {
        var (
            nome,
            dataNascimento,
            cpf,
            email,
            senha
        ) = alunoRequest;

        var senhaInvalida = senha is { Length: < 8 } || !(
            senha.Any(char.IsUpper) && senha.Any(char.IsLower) && senha.Any(c => !char.IsLetterOrDigit(c))
        );

        if (senhaInvalida)
            return Results.BadRequest(new { Message = "Senha invalida" });

        if (!Validacao.IsValidCpf(cpf))
            return Results.BadRequest(new { Message = "Cpf invalido" });

        if (!Validacao.IsEmailValid(email))
            return Results.BadRequest(new { Message = "Email invalido" });

        var agora = DateTime.UtcNow;

        if ((dataNascimento < agora.AddYears(-150)) || (dataNascimento > agora))
            return Results.BadRequest(new { Message = "Data de Nascimento invalida" });

        if (nome is { Length: < 3 or > 100 })
            return Results.BadRequest(new { Message = "Nome invalido" });

        try
        {
            await alunoService.UpdateAlunoAsync(id, alunoRequest, cancellationToken);
        }
        catch (InvalidOperationException e)
        {
            return Results.Json(
                new { Message = "Aluno está em conflito por cpf ou email" },
                statusCode: StatusCodes.Status403Forbidden
            );
        }
        catch (KeyNotFoundException e)
        {
            return Results.Json(
                new { Message = "Aluno nao encontrado" },
                statusCode: StatusCodes.Status404NotFound
            );
        }

        return Results.Ok();
    }


    [Authorize]
    [HttpDelete("{id}", Name = "DeletarAluno")]
    [ApiDoc("Deleta um aluno", "Deleta o aluno e suas matriculas.")]
    public async Task<IResult> DeletaAluno(
        [FromRoute] int id,
        CancellationToken cancellationToken = default
    )
    {
        await alunoService.DeleteAlunoAsync(id, cancellationToken);
        return Results.Ok();
    }

    [Authorize]
    [HttpPost("{id}/matriculas",Name = "MatriculaAluno")]
    [ApiDoc("Matricula um aluno", "Matricula um aluno na turma especificada ou retorna id da matircula existente.")]
    public async Task<IResult> MatricularAluno(
        [FromRoute] int id,
        [FromQuery] int turmaId,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            int idCriado = await alunoService.MatricularAlunoAsync(id, turmaId, cancellationToken);
            return Results.Ok(new { Id = idCriado });

        }
        catch (InvalidOperationException e)
        {
            return Results.Json(
                new { Message = "Aluno já está matriculado nessa turma." },
                statusCode: StatusCodes.Status403Forbidden
            );
        }
    }
}
