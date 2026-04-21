using FastOS.Application.Services;
using Microsoft.AspNetCore.Mvc;
using FastOS.Domain.Entities;
using FastOS.Domain.ValueObjects;

namespace FastOS.API.Controllers
{
    public class DiagnosticoController : Controller
    {
        //private readonly ClienteBusiness _clienteBusiness;

        public DiagnosticoController(/*ClienteBusiness clienteBusiness*/)
        {
            //_clienteBusiness = clienteBusiness;
        }

        [HttpPost]
        [Route("Diagnostico/ReceberDiagnostico")]
        public IActionResult ReceberDiagnostico([FromBody] DiagnosticoDto diagnostico)
        {

            

            return Ok(new { status = "os_criada" });
        }
    }
}

