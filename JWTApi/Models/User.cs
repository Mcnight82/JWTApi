using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace JWTApi.Models;

public class User
{
    public int Iduser { get; set; }

    public string? Datos { get; set; } = "{}";
    
    [NotMapped]
    public JsonDocument? DataJson
    {
        get
        {
            return string.IsNullOrEmpty(Datos) ? JsonDocument.Parse("{}") : JsonDocument.Parse(Datos);
            
        }
        set
        {
            Datos = value !=null ? value.RootElement.GetRawText() : "{}";
        }
    }
}
