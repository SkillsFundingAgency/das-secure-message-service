using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

public class AccessDeniedViewModel
{
    public string[] Organizations { get; set; }

    public AccessDeniedViewModel(string organizations)
    {
        Organizations = organizations.Split(",");
    }

}