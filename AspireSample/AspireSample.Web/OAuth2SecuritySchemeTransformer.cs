
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace AspireSample.Web;

internal class OAuth2SecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider) : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();

        if (authenticationSchemes.Any(authScheme => authScheme.Name == OpenIdConnectDefaults.AuthenticationScheme))
        {
            var requirements = new Dictionary<string, OpenApiSecurityScheme>
            {
                ["OAuth2"] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri("http://localhost:8082/realms/AspireSample/protocol/openid-connect/auth"),
                            TokenUrl = new Uri("http://localhost:8082/realms/AspireSample/protocol/openid-connect/token"),
                            Scopes = new Dictionary<string, string>
                            {
                                { "catalog:read-write", "Read and write access to catalog" }
                            },
                        }
                    },
                    Description = "OAuth2 Authorization Code Flow (Keycloak)"
                }
            };

            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes = requirements;

            foreach (var operation in document.Paths.Values.SelectMany(path => path.Operations))
            {
                operation.Value.Security.Add(
                    new OpenApiSecurityRequirement
                    {
                        { new OpenApiSecurityScheme { Reference = new OpenApiReference { Id = "OAuth2", Type = ReferenceType.SecurityScheme } }, new[] { "catalog:read-write" } }
                    }
                );
            }
        }
    }
}
