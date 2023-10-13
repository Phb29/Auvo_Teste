using AuvoTeste.Models;
using AuvoTeste.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace AuvoTeste.Controllers
{

    public class HomeController : Controller
    {

        private readonly IRecursoHumanoRepository _recursoHumanoRepository;

        public HomeController(IRecursoHumanoRepository recursoHumanoRepository)
        {
            _recursoHumanoRepository = recursoHumanoRepository;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UploadFolder(UploadArquivo model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index");
            }

            await _recursoHumanoRepository.ProcessarArquivos(model);
            return View("Index");
        }
        [HttpGet]
        public IActionResult Download()
        {
            var departamentoTemp = DadosTempDepartamento.Departamento?.OrderBy(d => d.Departamentos).ToList();
            var jsonstr = JsonSerializer.Serialize(departamentoTemp);
            var byteArray = System.Text.Encoding.UTF8.GetBytes(jsonstr);

            return File(byteArray, "application/json", DadosTempDepartamento.NomeArquivo + ".json");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}