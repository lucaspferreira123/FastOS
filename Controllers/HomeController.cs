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


        public HomeController(ILogger<HomeController> logger, ClienteBusiness clienteBusiness, ProdutoBusiness produtoBusiness)
        {
            _logger = logger;
            _clienteBusiness = clienteBusiness;
            _produtoBusiness = produtoBusiness;
        }

        public async Task<IActionResult> Index()
        {
            var clientes = await _clienteBusiness.ObterTodosClientes();
            var produtos = await _produtoBusiness.ObterProdutos();

            ViewBag.Produtos = produtos;
            ViewBag.Clientes = clientes;

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
