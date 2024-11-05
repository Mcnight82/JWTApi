
using JWTApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace JWTApi.Controllers.Admin
{
    
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AdminController : ControllerBase
    {

        private readonly ModelContext conexto;
        private readonly IHttpClientFactory cliente;

        public AdminController(ModelContext _conexto, IHttpClientFactory _cliente)
        {
            
            conexto = _conexto;
            cliente = _cliente;
            
        }



        [HttpGet("listarUnid")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ListarUnid([FromHeader(Name = "Authorization")] string token)
        {
            
            //1. validamos que efectivamente venga el token en la cabecera
            if (string.IsNullOrEmpty(token))
            {
                
                return StatusCode(StatusCodes.Status400BadRequest, new { message = "Acesso Denegado (credenciales inválidos!)" });
                
            }
            //2. si efectivamente el token viene en la cabecera, se procede a ejecutar la petición
            else
            {

                try
                {

                    var data = cliente.CreateClient();
                    data.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    return Ok("Hola Admin, Ahora SI");



                }
                catch (Exception ex)
                {
                    
                    return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
                    
                }



            }
            
            
            
            
            
        }




    }
    
    
    
    
}

