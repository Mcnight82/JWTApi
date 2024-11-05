using System.Text.Json;
using JWTApi.Models;
using JWTApi.Models.DTOs;
using JWTApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace JWTApi.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AccessController : ControllerBase
    {


        private readonly ModelContext modelo;
        private readonly ServiciosJWT servicios;
        private readonly IHttpClientFactory cliente;

        public AccessController(ModelContext _context, ServiciosJWT _services, IHttpClientFactory _cliente)
        {
            
            modelo = _context;
            servicios = _services;
            cliente = _cliente;
            
            
        }


        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] SignInDTO registro)
        {
            
            //1. traemos los datos del formulario y usamos el generados SHA para encriptar la clave
            var userData = new
            {
                
                Nombre = registro.Nombre,
                Correo = registro.Correo,
                Rol = registro.Rol,
                Clave = servicios.GenerarSha256(registro.Clave)

            };
            //serializamos los datos para la insercion en la tabla
            var JsonDatos = JsonSerializer.Serialize(userData);
            
            
            //creamos la instancia de la entidad USER
            var user = new User
            {
                
                Datos = JsonDatos
            };
            
            //esperamos que se agregue el registro en la base de datos
            await modelo.Users.AddAsync(user);
            
            //esperamos la confirmacion de la insercion
            await modelo.SaveChangesAsync();
            
            //validamos su insercion
            if (user.Iduser > 0)
            {
                
                //retornamos el codigo de exito junto al objeto
                return StatusCode(StatusCodes.Status201Created, new { Message = "Usuario Agregado", user = user });
                
            }
            else
            {
                //retornamos el mensaje de error de creacion
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error en el proceso de registro, intentelo nuevamente" });
            }


        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            
            //1. extraemos la clave que viene en el formulario y aplicamos hash
            var passHash256 = servicios.GenerarSha256(login.Clave);
            
            //2. buscamos todos los registros que coincidan con la peticion a traves de sentencia SQL personalizada
            var userGetted = await modelo.Users.FromSqlRaw(@"SELECT * FROM USERS 
                                                            WHERE JSON_VALUE (DATOS, '$.Correo') = {0}
                                                            AND JSON_VALUE (DATOS, '$.Rol') = {1}
                                                            AND JSON_VALUE (DATOS, '$.Clave') = {2}", login.Correo, login.Rol, passHash256)
                                                .FirstOrDefaultAsync();


            //3. validamos los datos traidos
            if (userGetted != null)
            {
                
                //3.1 entregamos el token
                var token = servicios.GenerarJWT(userGetted);
                
                //3.2 retornamos el exito del login (solo si no tenemos listo los cases
                //return StatusCode(StatusCodes.Status200OK, new {message = "Usuario logueado correctamente", token});
                
                //3.3 ahora redirigiremos a los usuarios de acuerdo al rol asignado 

                switch (login.Rol)
                {
                    
                    case "Admin":

                        var adminUrl = "http://localhost:5046/api/Admin/listarUnid";
                        var adminRespuesta = await cliente.CreateClient("ClienteAdmin").GetAsync(adminUrl);
                        var adminRedirect = adminRespuesta.RequestMessage.RequestUri.AbsoluteUri;
                        return StatusCode(StatusCodes.Status200OK, new
                        {
                            Message = $"Bienvenido, admin{login.Correo}", 
                            token,
                            redirect = adminRedirect,
                        });
                    
                    
                    default: 
                        return StatusCode(StatusCodes.Status403Forbidden, new { message = "Rol no valido", token = token });
                    
                }
                
            }
            else
            {
                
                return StatusCode(StatusCodes.Status401Unauthorized, new { message = "No se pudo encontrar el usuario o es invalido" });
                
            }

        }
        
        
        
        
    }
    
    
    
}

