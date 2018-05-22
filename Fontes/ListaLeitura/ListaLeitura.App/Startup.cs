using Ler.ListaLeitura.App.Negocio;
using Ler.ListaLeitura.App.Repositorio;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ler.ListaLeitura.App
{
    public class Startup
    {
        //Essa mensagem está dizendo que é para adicionarmos o serviço de roteamento na aplicação. E isso é feito no método ConfigureServices() da própria classe Startup. Então vamos fazer isso. É nesse método que configuramos todos os serviços necessários para a aplicação rodar.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();
        }

        /*
         O tipo que é utilizado para definir o fluxo requisição-resposta da aplicação é a interface IApplicationBuilder.
         Então para eu configurar um request pipeline na minha aplicação eu tenho que receber como argumento de 
         entrada um objeto do tipo IApplicationBuilder.
        */
        //Segunda parte
        //A verificação de adequação de uma string específica em determinado padrão é tarefa das expressões regulares.
        //O AspNet.Core chama os caminhos de rotas.
        //Cada requisição que precisar ser tratada deve ser encapsulada numa rota. Para montar as rotas vou precisar de um construtor de rotas, um RouteBuilder. E depois vou criar 3 rotas, um para cada tipo de requisição, livros para ler, livros lendo e livros lidos, usando o método MapRoute() do builder. Lembre-se que isso precisará ser feito no método Configure().
        

        public void Configure(IApplicationBuilder app) {            
            //Perceba que como argumento do método MapRoute() vou passar duas informações. Justamente o caminho que desejo atender e o um objeto do tipo RequestDelegate
            var builder = new RouteBuilder(app);
            builder.MapRoute("Livros/ParaLer", LivrosParaLer);
            builder.MapRoute("Livros/Lendo", LivrosLendo);
            builder.MapRoute("Livros/Lidos", LivrosLidos);

            //O AspNet chama esse tipo de rotas de rotas com templates. Funciona assim: você cria o template delimitando segmentos distintos com chaves. E sempre que houver uma requisição que se adeque àquele padrão de rota, o AspNet Core vai capturar o valor dentro daquele segmento e armazená-lo no nome do segmento da rota
            builder.MapRoute("Cadastro/NovoLivro/{nome}/{autor}", NovoLivroParaLer);

            //Na documentação sobre o roteamento no Asp.NET também temos a possibilidade de adicionar restrições às rotas mapeadas. São as chamadas Route Constraints. As restrições limitam o mapeamento e fazem com que o ASP.NET só execute determinado request delegate se a restrição for atendida.
            builder.MapRoute("Livros/Detalhes/{id:int}", ExibeDetalhes);

            builder.MapRoute("Cadastro/NovoLivro", ExibeFormulario);

            builder.MapRoute("Cadastro/Incluir", ProcessarFormularo);

            //Para finalizar eu crio a coleção de rotas usando o builder, que era o nosso dicionário anterior, através do método Build(), e uso essa coleção na minha aplicação. Também não preciso mais usar o método Run() para passar o delegate AtendeRequisicao, porque isso tudo está na coleção de rotas.
            var rotas = builder.Build();

            app.UseRouter(rotas);

            /*Esse tipo identifica todos os métodos que possuam como retorno um objeto 
            do tipo Task(usado para trabalhar com paralelismo) e aceita como argumento de entrada um objeto 
            do tipo HttpContext. */

            //app.Run(Roteamento);
        }

        public Task ProcessarFormularo(HttpContext context)
        {
            var livro = new Livro()
            {
                //Como a query string faz parte da requisição, é lá que vamos procurar uma propriedade chamada Query, que é do tipo IQueryCollection. Essa coleção permite que você a indexe pela chave usando os colchetes
                Titulo = context.Request.Form["titulo"].First(),
                Autor = context.Request.Form["autor"].First()
            };

            var repo = new LivroRepositorioCSV();
            repo.Incluir(livro);

            return context.Response.WriteAsync("O livro foi adicionado com sucesso.");
        }

        public Task ExibeFormulario(HttpContext context)
        {
            var html = CarregaArquivoHTML("formulario");
            return context.Response.WriteAsync(html);            
        }

        private string CarregaArquivoHTML(string nomeArquivo)
        {
            var nomeCompletoArquivo = $"HTML/{nomeArquivo}.html";
            using(var arquivo = File.OpenText(nomeCompletoArquivo))
            {
                return arquivo.ReadToEnd();
            }
        }

        public Task ExibeDetalhes(HttpContext context)
        {
            int id = Convert.ToInt32(context.GetRouteValue("id"));
            var _repo = new LivroRepositorioCSV();
            var livro = _repo.Todos.First(l => l.Id == id);
            return context.Response.WriteAsync(livro.Detalhes());
        }

        public Task NovoLivroParaLer(HttpContext context)
        {
            var livro = new Livro()
            {
                //um método chamado GetRouteValue(), que recebe como argumento a chave que é justamente o nome do segmento que você definiu no template da rota.
                Titulo = context.GetRouteValue("nome").ToString(),
                Autor = context.GetRouteValue("autor").ToString()
            };

            var repo = new LivroRepositorioCSV();
            repo.Incluir(livro);

            return context.Response.WriteAsync("O livro foi adicionado com sucesso.");
        }

        public Task Roteamento(HttpContext context)
        {
            var _repo = new LivroRepositorioCSV();

            var caminhosAtendidos = new Dictionary<string, RequestDelegate> {
                { "/Livros/ParaLer", LivrosParaLer},
                { "/Livros/Lendo", LivrosLendo },
                {"/Livros/Lidos", LivrosLidos }
            };

            if (caminhosAtendidos.ContainsKey(context.Request.Path)) {
                var metodo = caminhosAtendidos[context.Request.Path];

                return metodo.Invoke(context);
            }

            context.Response.StatusCode = 404;
            return context.Response.WriteAsync("Caminho inexistente.");            
        }

        /*
         No ASP.NET Core toda informação referente às requisições está representada na classe HttpContext
         */
        public Task LivrosParaLer(HttpContext context)
        {            
            var _repo = new LivroRepositorioCSV();
            var conteudoArquivo = CarregaArquivoHTML("para-ler");

            foreach(var livro in _repo.ParaLer.Livros)
            {
                conteudoArquivo = conteudoArquivo.Replace("#NOVO-ITEM#", $"<li>{livro.Titulo} - {livro.Autor}</li>#NOVO-ITEM#");
            }
            conteudoArquivo = conteudoArquivo.Replace("#NOVO-ITEM#", "");

            /* para escrever uma resposta para a requisição que chegar ao servidor, 
             * usamos o método WriteAsync() na propriedade Response             
              */
            return context.Response.WriteAsync(conteudoArquivo);            
        }

        public Task LivrosLendo(HttpContext context)
        {
            var _repo = new LivroRepositorioCSV();            
            return context.Response.WriteAsync(_repo.Lendo.ToString());
        }

        public Task LivrosLidos(HttpContext context)
        {
            var _repo = new LivroRepositorioCSV();            
            return context.Response.WriteAsync(_repo.Lidos.ToString());
        }
    }
}