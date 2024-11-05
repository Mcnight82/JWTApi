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

//agregamos boton de autorizacion
builder.Services.AddSwaggerGen(c =>
{   
    //1. creamos la documentacion swagger con su titulo y version
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "JWTApi", Version = "v1" });
    
    //2. definimos la seguridad de la api, IMPORTANTE el BEARER que es lo que se antepone al token mismo 
    //antes de su envio
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Cabecera de autorizacion usando esquema Bearer.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
        
        
    });
    
    //añadimos requerimientos de seguridad
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] { }
        }
        
        
    });
    
} );

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