using System.Text;
using Blog.Data;
using BlogApi;
using BlogApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var key = Encoding.ASCII.GetBytes(Configuration.JwtKey);

// Especificando como o token vai ser criptografado e descriptografado.
builder.Services.AddAuthentication(x => 
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x => 
{
    x.TokenValidationParameters = new TokenValidationParameters 
    {
        // Validar a chave de assinatura:
        ValidateIssuerSigningKey = true,
        // Como validar a chave de assinatura: 
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(options => {
        options.SuppressModelStateInvalidFilter = true;
    });
builder.Services.AddDbContext<BlogDataContext>();

//3 configurações diferentes para o life time dos serviços.
builder.Services.AddTransient<TokenService>(); // Sempre cria uma nova instância quando usar o [FromServices]
//builder.Services.AddScoped(); // Cria uma nova instância a cada transação
//builder.Services.AddSingleton(); // Singleton -> 1 por App!

var app = builder.Build();

//Configurando aplicação para usar autenticação e autorização.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();