using MeuProjeto.Business;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TesteMVC.Models;

namespace TesteMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ClienteBusiness _clienteBusiness;
        private readonly ProdutoBusiness _produtoBusiness;
        private readonly StatusBusiness _statusBusiness;


        public HomeController(ILogger<HomeController> logger, ClienteBusiness clienteBusiness, ProdutoBusiness produtoBusiness, StatusBusiness statusBusiness)
        {
            _logger = logger;
            _clienteBusiness = clienteBusiness;
            _produtoBusiness = produtoBusiness;
            _statusBusiness = statusBusiness;
        }

        public async Task<IActionResult> Index()
        {
            var clientes = await _clienteBusiness.ObterTodosClientes();
            var produtos = await _produtoBusiness.ObterProdutos();
            var status = await _statusBusiness.ObterTodosStatus();

            ViewBag.Produtos = produtos;
            ViewBag.Clientes = clientes;
            ViewBag.Status = status;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
