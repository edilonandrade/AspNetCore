﻿using Ler.ListaLeitura.App.Repositorio;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ler.ListaLeitura.App
{
    public class Startup
    {
        /*
         O tipo que é utilizado para definir o fluxo requisição-resposta da aplicação é a interface IApplicationBuilder.
         Então para eu configurar um request pipeline na minha aplicação eu tenho que receber como argumento de 
         entrada um objeto do tipo IApplicationBuilder.
        */
        public void Configure(IApplicationBuilder app) {
            /*Esse tipo identifica todos os métodos que possuam como retorno um objeto 
            do tipo Task(usado para trabalhar com paralelismo) e aceita como argumento de entrada um objeto 
            do tipo HttpContext. */

           app.Run(Roteamento);
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
            /* para escrever uma resposta para a requisição que chegar ao servidor, 
             * usamos o método WriteAsync() na propriedade Response             
              */
            return context.Response.WriteAsync(_repo.ParaLer.ToString());            
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