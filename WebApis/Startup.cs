using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.OpenApi.Models;
using System;
using WebApis.Authz;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace WebApis;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpClient();

        services.AddOptions();

        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        IdentityModelEventSource.ShowPII = true;

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.Authority = "http://localhost:8080/realms/myrealm";
            options.Audience = "oidc-code-pkce-angular"; // from keycloak setup

            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ClockSkew = new System.TimeSpan(0, 0, 30)
            };
            //options.Events = new JwtBearerEvents()
            //{
            //    // TODO return the www-authenticate header
            //    OnChallenge = context =>
            //    {
            //        context.HandleResponse();
            //        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            //        context.Response.ContentType = "application/json";

            //        // Ensure we always have an error and error description.
            //        if (string.IsNullOrEmpty(context.Error))
            //            context.Error = "invalid_token";
            //        if (string.IsNullOrEmpty(context.ErrorDescription))
            //            context.ErrorDescription = "This request requires a valid JWT access token to be provided";

            //        // Add some extra context for expired tokens.
            //        if (context.AuthenticateFailure != null && context.AuthenticateFailure.GetType() == typeof(SecurityTokenExpiredException))
            //        {
            //            var authenticationException = context.AuthenticateFailure as SecurityTokenExpiredException;
            //            context.Response.Headers.Add("x-token-expired", authenticationException?.Expires.ToString("o"));
            //            context.ErrorDescription = $"The token expired on {authenticationException?.Expires.ToString("o")}";
            //        }

            //        return context.Response.WriteAsync(JsonSerializer.Serialize(new
            //        {
            //            error = context.Error,
            //            error_description = context.ErrorDescription
            //        }));
            //    }
            //};
        });
    
        services.AddCors(options =>
        {
            options.AddPolicy("AllowMyOrigins",
                builder =>
                {
                    builder
                        .AllowCredentials()
                        .WithOrigins(
                            "https://localhost:4200")
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });

        services.AddSwaggerGen(c =>
        {
            // add JWT Authentication
            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "JWT Authentication",
                Description = "Enter JWT Bearer token **_only_**",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer", // must be lower case
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };
            c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {securityScheme, Array.Empty<string>()}
            });

            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Web APIs",
                Version = "v1",
                Description = "Web APIs",
                Contact = new OpenApiContact
                {
                    Name = "damienbod",
                    Email = string.Empty,
                    Url = new Uri("https://damienbod.com/"),
                },
            });
        });

        services.AddControllers(options =>
        {
            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                // .RequireClaim("email") // disabled this to test with users that have no email (no license added)
                .Build();
            options.Filters.Add(new AuthorizeFilter(policy));
        });

        services.AddSingleton<IAuthorizationHandler, IsAdminHandler>();

        services.AddAuthorization(options =>
        {
            options.AddPolicy("IsAdminRequirementPolicy", policyIsAdminRequirement =>
            {
                policyIsAdminRequirement.Requirements.Add(new IsAdminRequirement());
            });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "User API One");
            c.RoutePrefix = string.Empty;
        });

        app.UseCors("AllowMyOrigins");

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}