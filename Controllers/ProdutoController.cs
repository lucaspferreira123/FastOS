using MeuProjeto.Business;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using TesteMVC.Models;

namespace TesteProjeto.Controllers
{
    public class ProdutoController : Controller
    {
        private readonly ProdutoBusiness _produtoBusiness;

        public ProdutoController(ProdutoBusiness produtoBusiness)
        {
            _produtoBusiness = produtoBusiness;
        }
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        [Route("Produto/CadastrarProduto")]
        public async Task<IActionResult> CadastrarProduto([FromBody] ProdutoViewModel produto)
        {
            try
            {
                var produtoCadastrado = await _produtoBusiness.CadastrarProduto(produto);
                return Ok(produtoCadastrado);
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocorreu um erro interno.");
            }
        }

        [HttpPut]
        [Route("Produto/AlterarProduto")]
        public async Task<IActionResult> AlterarProduto([FromBody] ProdutoViewModel produto)
        {
            try
            {
                var produtoAlterado = await _produtoBusiness.AlterarProduto(produto);
                return Ok(produtoAlterado);
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocorreu um erro interno.");
            }
        }

        [HttpDelete]
        [Route("Produto/ExcluirProduto/{idProduto}")]
        public async Task<IActionResult> ExcluirProduto(int idProduto)
        {
            try
            {
                var produtoExcluido = await _produtoBusiness.ExcluirProduto(idProduto);
                return Ok(produtoExcluido);
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocorreu um erro interno.");
            }
        }

        [HttpGet]
        [Route("Produto/ObterProdutos")]
        public async Task<IActionResult> ObterProdutos()
        {
            try
            {
                var produtos = await _produtoBusiness.ObterProdutos();
                return Ok(produtos);
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocorreu um erro interno.");
            }
        }

        [HttpGet]
        [Route("Produto/ObterProduto/{idProduto}")]
        public async Task<IActionResult> ObterProduto(int idProduto)
        {
            try
            {
                var produto = await _produtoBusiness.ObterProdutoPeloId(idProduto);
                return Ok(produto);
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocorreu um erro interno.");
            }
        }
    }
}

