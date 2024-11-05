using System.IdentityModel.Tokens.Jwt;
using JWTApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


namespace JWTApi.Services;

public class ServiciosJWT
{
    private readonly IConfiguration config;

    public ServiciosJWT(IConfiguration _config)
    {
        
        config = _config;
        
    }
    
    //creamos el metodo que genera la encriptacion de la password en SHA256
    public string GenerarSha256(string text)
    {
        
        //llamamos al using para usar este metodo solo una vez por llamado y eliminarlo de la memoria
        using (SHA256 Sha256Hash = SHA256.Create())
        {
            
            //creamos el array de bytes para la cadena y le pasamos el texto
            byte[] bytes = Sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(text));
            
            //creamos una nueva instancia de Builder
            StringBuilder Builder = new StringBuilder();
            
            //recorremos el array lo formateamos y anexamos al builder
            for (int a = 0; a < bytes.Length; a++)
            {
                
                Builder.Append(bytes[a].ToString("x2"));
                
            }
            
            return Builder.ToString();
            
            
        }
        
        
        
        
    }
    
    
    //creamos el metodo de JsonWebToken para el uso de las instancias de usuarios
    public string GenerarJWT(User usuario)
    {
        
        //creamos los claims que serviran para contrastar estas credenciales con la base de datos
        var userClaims = new[]
        {

            new Claim(ClaimTypes.NameIdentifier, usuario.Iduser.ToString()),
            new Claim(ClaimTypes.Email, usuario.Datos.ToString()),
            new Claim(ClaimTypes.Role, usuario.Datos.ToString())


        };
        
        //buscamos el secreto o llave en appsettings.json y lo asignamos a esta variable
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Key"]));
        
        //para la credencial, le entregamos esta key en algoritmo de 256
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        
        //ahora creamos el token conformado de 3 partes
        var token = new JwtSecurityToken(
            
            
            claims: userClaims, //agregamos el array de claims
            expires: DateTime.Now.AddMinutes(30), //indicamos el tiempo de expiracion
            signingCredentials: credentials //firmamos la credencial

        );
        
        //por supuesto ahora devolvemos el token
        return new JwtSecurityTokenHandler().WriteToken(token);




    }
    
    
    
    
    
    
    
    
}