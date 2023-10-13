using System.ComponentModel.DataAnnotations;

namespace AuvoTeste.Models
{
    public class UploadArquivo
    {
        [Required(ErrorMessage = "Escolha uma pasta com arquivos CSV")]
        public List<IFormFile>? Arquivos { get; set; }
    }
}
