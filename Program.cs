using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    option =>
    {
        option.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Azure AD Prueba", Version = "v1"});
        option.AddSecurityDefinition("oauth2", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Description = "Oauth 2.0 para la autorizacion",
            Name = "oauth2.0",
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.OAuth2,
            Flows = new Microsoft.OpenApi.Models.OpenApiOAuthFlows
            {
                AuthorizationCode = new Microsoft.OpenApi.Models.OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri(builder.Configuration["SwaggerAzureAD:AuthorizationUrl"]),
                    TokenUrl = new Uri(builder.Configuration["SwaggerAzureAD:TokenUrl"]),
                    Scopes = new Dictionary<string, string>
                    {
                        {builder.Configuration["SwaggerAzureAD:Scope"], "Access Api as User" }
                    }
                }
            }
        });
        option.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference{Type=ReferenceType.SecurityScheme,Id="oauth2"}
                },
                new[]{ builder.Configuration["SwaggerAzureAD:Scope"] }
            }
        });
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(
        options =>{
            options.OAuthClientId(builder.Configuration["SwaggerAzureAD:ClientId"]);
            options.OAuthUsePkce();
            options.OAuthScopeSeparator(" ");

    });
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
