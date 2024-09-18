using Azure.Core;
//using EF.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
//using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace KontrolarCloud.Middlewares
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public TokenValidationMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint != null)
            {
                var allowAnonymous = endpoint.Metadata.GetMetadata<IAllowAnonymous>() != null;

                if (!allowAnonymous)
                {
                    var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                    if (token != null)
                    {
                        var isValidToken = ValidateToken(token);

                        if (!isValidToken)
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            await context.Response.WriteAsync("Invalid Token");
                            return;
                        }
                    }
                    else
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("Token is missing");
                        return;
                    }
                }
            }

            await _next(context);
        }

        private bool ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token)) 
            { 
                return false;
            }

            var cleaned = token.Replace("\"", "");
           
            //var deserialized = JsonConvert.DeserializeObject(decryptedParam);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            try
            {
                tokenHandler.ValidateToken(cleaned.ToString(), new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(5),
                }, out SecurityToken validatedToken);

                return true;
            }
            catch (SecurityTokenExpiredException ex)
            {
                Console.WriteLine($"Token expired: {ex.Message}");
            }
            catch (SecurityTokenInvalidIssuerException ex)
            {
                Console.WriteLine($"Invalid issuer: {ex.Message}");
            }
            catch (SecurityTokenInvalidAudienceException ex)
            {
                Console.WriteLine($"Invalid audience: {ex.Message}");
            }
            catch (SecurityTokenInvalidSignatureException ex)
            {
                Console.WriteLine($"Invalid signature: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token validation failed: {ex.Message}");
            }

            return false;
        }
    }

}
