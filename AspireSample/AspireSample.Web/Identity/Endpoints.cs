using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace AspireSample.Web.Identity;

internal static class Endpoints
{
    internal static IEndpointConventionBuilder MapIdentityEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("identity");

        group.MapGet(pattern: "/login", OnLogin).AllowAnonymous();
        group.MapPost(pattern: "/logout", OnLogout);

        return group;
    }

    internal static IEndpointConventionBuilder MapIdentityApiEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("api/identity");

        group.MapGet(pattern: "/claims", async (HttpContext context) =>
        {
            var claims = context.User.Claims.ToDictionary(c => c.Type, c => c.Value);
            var accessToken = await context.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            claims.Add("access_token", accessToken ?? string.Empty);

            return TypedResults.Ok(claims);
        }).RequireAuthorization();

        return group;
    }

    internal static ChallengeHttpResult OnLogin() =>
        TypedResults.Challenge(properties: new AuthenticationProperties
        {
            RedirectUri = "/"
        });

    internal static SignOutHttpResult OnLogout() =>
        TypedResults.SignOut(properties: new AuthenticationProperties
        {
            RedirectUri = "/"
        },
        [
            CookieAuthenticationDefaults.AuthenticationScheme,
            OpenIdConnectDefaults.AuthenticationScheme
        ]);
}
