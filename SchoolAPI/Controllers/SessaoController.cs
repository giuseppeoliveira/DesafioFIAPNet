using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SchoolAPI.Application.UseCases;

namespace SchoolAPI.Controllers;

[ApiController]
[Route("api/v1/sessao")]
public class SessaoController : ControllerBase
{
    private readonly ISessaoService sessaoService;
    public SessaoController(ISessaoService sessaoService)
    {
        this.sessaoService = sessaoService;
    }

    [AllowAnonymous]
    [HttpPost(Name = "SignIn")]
    [ApiDoc("Cria sessao de autenticacao", "Cria a sessao de autenticacao para um administrador.")]
    public async Task<IResult> SignIn(
        [FromBody] ISessaoService.SignInRequest singInRequest,
        CancellationToken cancellationToken = default
    )
    {
        var tokenResponse = await sessaoService.GetToken(
            singInRequest,
            cancellationToken
        );

        if (tokenResponse is null)
            return Results.Json(
                new { Message = "Usuario ou senha incorretos" },
                statusCode: StatusCodes.Status403Forbidden
            );

        return Results.Ok(new { tokenResponse.Value.Token, tokenResponse.Value.ExpiresAt });
    }
}
