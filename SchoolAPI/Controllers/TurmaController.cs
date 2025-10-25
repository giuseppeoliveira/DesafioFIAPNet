using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SchoolAPI.Application.UseCases;
using SchoolAPI.Domain;

namespace SchoolAPI.Controllers;

[ApiController]
[Route("api/v1/turmas")]
public class TurmaController : ControllerBase
{
    private readonly ITurmaService turmaService;

    public TurmaController(ITurmaService turmaService)
    {
        this.turmaService = turmaService;
    }

    [Authorize]
    [HttpGet(Name = "ListarTurmas")]
    [ProducesResponseType(200, Type = typeof(PaginacaoResponse<ITurmaService.TurmaListResponse>))]
    [ApiDoc("Pesquisa as turmas", "Retorna uma lista paginada de turmas considerando as consultas passadas.")]
    public async Task<IResult> ListarTurmas(
        [FromQuery] string? nomeQuery = null,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 10,
        CancellationToken cancellationToken = default
    )
    {   
        if(pagina < 1)
            return Results.BadRequest(new { Message = "pagina was invalid"});

        if(tamanhoPagina < 1)
            return Results.BadRequest(new { Message = "tamanhoPagina was invalid"});

        var paginatedResponse = await turmaService.GetTurmaAsync(
            new PaginacaoQuery(
                pagina,
                tamanhoPagina
            ),
            nomeQuery: nomeQuery,
            cancellationToken    
        );

        return Results.Ok(paginatedResponse);
    }

    [Authorize]
    [HttpGet("{id}",Name = "DetalhesTurma")]
    [ProducesResponseType(200, Type = typeof(PaginacaoResponse<ITurmaService.TurmaDetalhesResponse>))]
    [ApiDoc("Consulta uma turma", "Retorna detalhes de uma turma e seus alunos matriculados.")]
    public async Task<IResult> ListarTurmas(
        [FromRoute] int id,
        CancellationToken cancellationToken = default
    )
    {
        var response = await turmaService.GetTurmaByIdAsync(
            id,
            cancellationToken
        );

        if(response is null)
            return Results.Json(
                new { Message = "Turma nao encontrada" },
                statusCode: StatusCodes.Status404NotFound
            );

        return Results.Ok(response);
    }

    [Authorize]
    [HttpPost(Name = "CadastrarTurma")]
    [ApiDoc("Cadastra uma Turma", "Cadastra uma turma com o conteudo passado.")]
    public async Task<IResult> CadastrarTurma(
        [FromBody] ITurmaService.TurmaCadastroRequest turmaRequest,
        CancellationToken cancellationToken = default 
    )
    {
        var (
            nome,
            _
        ) = turmaRequest;

        if(nome is { Length: < 3 or > 100})
            return Results.BadRequest(new { Message = "nome was invalid"});

        int idCriado = await turmaService.CreateTurmaAsync(turmaRequest, cancellationToken);

        return  Results.Ok( new { Id = idCriado });
    }

    [Authorize]
    [HttpPut("{id}", Name = "EditarTurma")]
    [ApiDoc("Edita uma turma", "Edita uma turma com o conteudo passado, apos verificacoes.")]
    public async Task<IResult> EditarTurma(
        [FromRoute] int id,
        [FromBody] ITurmaService.TurmaCadastroRequest turmaRequest,
        CancellationToken cancellationToken = default
    )
    {
        var (
            nome,
            _
        ) = turmaRequest;

        if (nome is { Length: < 3 or > 100 })
            return Results.BadRequest(new { Message = "nome was invalid" });

        try
        {
            await turmaService.UpdateTurmaAsync(id, turmaRequest, cancellationToken);
        }
        catch (KeyNotFoundException e)
        {
            return Results.Json(
                new { Message = "Turma nao encontrada" },
                statusCode: StatusCodes.Status404NotFound
            );
        }

        return Results.Ok();
    }


    [Authorize]
    [HttpDelete("{id}", Name = "DeletarTurma")]
    [ApiDoc("Deleta uma turma", "Deleta uma turma e suas matriculas.")]
    public async Task<IResult> DeletaAluno(
        [FromRoute] int id,
        CancellationToken cancellationToken = default
    )
    {
        await turmaService.DeleteTurmaAsync(id, cancellationToken);
        return Results.Ok();
    }
}
