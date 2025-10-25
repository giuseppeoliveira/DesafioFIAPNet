using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SchoolAPI.Application;
using SchoolAPI.Application.UseCases;
using SchoolAPI.Infrastructure;
using System.Reflection;
using Microsoft.OpenApi.Models;

namespace SchoolAPI;

public class DependencyInjection
{

    private static string[] allowedOrigins = ["localhost"];

    public static void AddServices(IServiceCollection services)
    {
        services.AddSingleton<ICryptographyService, CryptographyService>();
        services.AddScoped<IAlunoService, AlunoService>();
        services.AddScoped<ITurmaService, TurmaService>();
        services.AddScoped<ISessaoService, SessaoService>();
        services.AddScoped<IAlunoRepository, AlunoRepository>();
        services.AddScoped<ITurmaRepository, TurmaRepository>();
        services.AddScoped<IMatriculaRepository, MatriculaRepository>();
        services.AddScoped<IUsuarioAdminRepository, UsuarioAdminRepository>();
    }

    public static void CustomOpenApi(IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                (document.Components ??= new OpenApiComponents()).SecuritySchemes.Add("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Autenticacao JWT utilizando bearer token.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });

                return Task.CompletedTask;
            });

            // Adiciona transformer para summary, description e autorizacao
            options.AddOperationTransformer((operation, context, cancellationToken) =>
            {
                var descriptor = context.Description.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor;
                var method = descriptor?.MethodInfo;

                if (method is not null)
                {
                    var docAttr = method.GetCustomAttribute<ApiDocAttribute>();
                    if (docAttr != null)
                    {
                        operation.Summary = docAttr.Summary;
                        operation.Description = docAttr.Description;
                    }

                    // Adiciona requisito de autenticacao se atribute estiver presente
                    var hasAuthorize = method.DeclaringType?.GetCustomAttributes(true)
                                        .OfType<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>().Any() == true
                                    || method.GetCustomAttributes(true).OfType<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>().Any();
                    if (hasAuthorize)
                    {
                        (operation.Security ??= []).Add(new OpenApiSecurityRequirement
                        {
                            { 
                                new OpenApiSecurityScheme
                                {
                                    Reference = new OpenApiReference
                                    {
                                        Type = ReferenceType.SecurityScheme,
                                        Id = "Bearer"
                                    }
                                },
                                Array.Empty<string>() 
                            }
                        });
                    }
                }

                return Task.CompletedTask;
            });
        });
    }

    public static void CustomCors(IServiceCollection services)
    {
        services.AddOptions();

        services.TryAdd(ServiceDescriptor.Transient<ICorsService, CorsService>());
        services.TryAdd(ServiceDescriptor.Transient<ICorsPolicyProvider, DefaultCorsPolicyProvider>());
        services
            .AddOptions<CorsOptions>()
            .Configure<IServiceProvider>((corsOptions, sp) =>
            {

                corsOptions.AddDefaultPolicy(policyBuilder =>
                {
                    policyBuilder
                        .SetIsOriginAllowed((origin) =>
                        {
                            var originUri = new Uri(origin);
                            return allowedOrigins.Any(allowedOrigin => allowedOrigin == originUri.Host);
                        })
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .WithExposedHeaders("Content-Disposition");
                });
            });
    }

}

