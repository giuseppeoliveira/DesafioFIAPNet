using SchoolAPI.Infrastructure;
using SchoolAPI.Domain.Entities;
using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace SchoolAPI.Application.UseCases;

public interface ISessaoService
{
    public record SignInRequest(
        string Email,
        string Senha
    );

    Task<(string Token, DateTime ExpiresAt)?> GetToken(SignInRequest signInRequest, CancellationToken cancellationToken = default);
}

public class SessaoService : ISessaoService
{
    private readonly ICryptographyService cryptographyService;
    private readonly IUsuarioAdminRepository usuarioAdminRepository;
    private readonly byte[] jwtKey;

    public SessaoService(
        ICryptographyService cryptographyService,
        IUsuarioAdminRepository usuarioAdminRepository,
        IConfiguration configuration
    )
    {
        this.usuarioAdminRepository = usuarioAdminRepository;
        this.cryptographyService = cryptographyService;
        var jwtSettings = configuration.GetSection("JwtSettings");
        var rawKey = jwtSettings["Key"] ?? throw new Exception("Chave JWT não configurada!");
        jwtKey = Encoding.UTF8.GetBytes(rawKey);
    }

    public async Task<(string Token, DateTime ExpiresAt)?> GetToken(
        ISessaoService.SignInRequest signInRequest, 
        CancellationToken cancellationToken = default
    )
    {
        var usuario = await usuarioAdminRepository.GetUsuarioAdminByEmailAsync(signInRequest.Email, cancellationToken);
        
        if(usuario is null)
            return null;

        var passwordMatches = cryptographyService.ComparePassword(signInRequest.Senha, usuario.Senha, usuario.Salt);        
        if(!passwordMatches)
            return null;

        var securityKey = new SymmetricSecurityKey(jwtKey);
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var expiration = DateTime.UtcNow.AddHours(1);

        var claims = new Claim[] {
            new(nameof(Usuarioadmin.Id), usuario.Id.ToString()),
        };

        var tokenBuilder = new JwtSecurityToken(
            issuer: "SchoolAPI",
            expires: expiration,
            claims: claims,
            signingCredentials: credentials
        );

        var token = new JwtSecurityTokenHandler().WriteToken(tokenBuilder);

        return (token, expiration);
    }
}
