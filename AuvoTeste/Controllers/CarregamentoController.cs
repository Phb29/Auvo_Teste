using AuvoTeste.Models;
using Microsoft.AspNetCore.Mvc;

namespace AuvoTeste.Controllers
{
    public class CarregamentoController : Controller
    {
        //Método para verificar o progresso do carregamento do JSON
        [HttpGet]
        public JsonResult Carregar()
        {
            return Json(DadosTempDepartamento.PorcentagemAtual);
        }
    }
}
