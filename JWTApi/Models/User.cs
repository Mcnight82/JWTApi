using System;
using System.Collections.Generic;
using System.Text.Json;

namespace JWTApi.Models;

public partial class User
{
    public int Iduser { get; set; }

    public string? Datos { get; set; }

    public JsonDocument DataJson
    {
        get{return JsonDocument.Parse(Datos);} 
        set{ Datos = value.RootElement.GetRawText();}
    }
}
