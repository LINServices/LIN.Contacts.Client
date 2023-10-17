﻿using LIN.Planner.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace LIN.Planner.Server.Controllers;


[Route("api")]
public class ToolsController : Controller
{


    /// <summary>
    /// Obtiene el estado del servidor
    /// </summary>
    [HttpGet("status")]
    public dynamic Status()
    {
        return StatusCode(200, new
        {
            Status = "Running"
        });
    }


    [HttpGet("myip")]
    public async Task<ActionResult> WhatsMyIP([FromServices] IHttpContextAccessor http)
    {

        // Cual es mi IP
        var myIP = Firewall.HttpIPv4(http);

        var isvalid = LIN.Modules.IP.ValidateIPv4(myIP);

        if (isvalid)
            return Ok(myIP);


        return StatusCode(503, myIP);

    }


}
