using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

public class IndexViewModel
{
    public IndexViewModel()
    {
        TtlValues = new Dictionary<int, string>()
        {
            { 1, "Hour" },
            { 24, "Day" }
        };
    }
    public string Message { get; set; }
    public int Ttl {get;set;}
    public Dictionary<int, string> TtlValues { get; set; }
    
    
}