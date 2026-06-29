using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace OrderManagement.API.Endpoints;

public static class AuthEndpoints
{
    // Fixed user credentials (as per spec)
    private const string FixedEmail = "dev@martech.com";
    private const string FixedPassword = "Senha@123";

    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/login", Handle)
            .WithName("Login")
            .WithTags("Auth")
            .AllowAnonymous();
    }

    private static IResult Handle(LoginRequest request, IConfiguration configuration)
    {
        if (request.Email != FixedEmail || request.Password != FixedPassword)
            return Results.Unauthorized();

        var jwtSettings = configuration.GetSection("Jwt");
        var secret = jwtSettings["Secret"]!;
        var issuer = jwtSettings["Issuer"]!;
        var audience = jwtSettings["Audience"]!;
        var expirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"] ?? "60");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Email, request.Email),
            new Claim(ClaimTypes.Name, "Developer"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Results.Ok(new { token = tokenString, expiresAt = token.ValidTo });
    }

    private sealed record LoginRequest(string Email, string Password);
}
