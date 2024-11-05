using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using JWTApi.Models;
using JWTApi.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//mas adelante aqui implementar boton de autorizacion
builder.Services.AddSwaggerGen();

//agregamos los servicios Http
builder.Services.AddHttpClient();

//agregamos el contexto de conexion a datos
builder.Services.AddDbContext<ModelContext>(options =>
    {
        
        options.UseOracle(builder.Configuration.GetConnectionString("OracleConnection"));
        
    }
    
    
);

//agregamos la inyeccion del servicio de encriptacion de clave y generador de token
builder.Services.AddSingleton<ServiciosJWT>();

//agregamos el servicio de autenticación de jwt
builder.Services.AddAuthentication(

    config =>
    {

        config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

    }

).AddJwtBearer(options =>
{

    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {

        ValidateIssuerSigningKey = true,
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]!))

    };


});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//uso de autorizacion y autenticacion
app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

app.Run();