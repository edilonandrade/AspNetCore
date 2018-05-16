using Ler.ListaLeitura.App.Repositorio;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Ler.ListaLeitura.App
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app) {
            //IApplicationBuilder: Interface para construir todo o pipeline
            //de requisição para aplicação
            app.Run(LivrosParaLer);
        }

        public Task LivrosParaLer(HttpContext context)
        {            
            var _repo = new LivroRepositorioCSV();
            return context.Response.WriteAsync(_repo.ParaLer.ToString());            
        }
    }
}