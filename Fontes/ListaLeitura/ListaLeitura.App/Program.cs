using Ler.ListaLeitura.App.Negocio;
using Ler.ListaLeitura.App.Repositorio;
using Microsoft.AspNetCore.Hosting;
using System;

namespace Ler.ListaLeitura.App
{
    class Program
    {
        static void Main(string[] args)
        {
            var _repo = new LivroRepositorioCSV();
            /*Uma das opções será o servidor web adotado pela sua aplicação, que no caso será o Kestrel, 
             * que é um servidor web já implementado pela galera do AspNet.Core.*/

            //Quando a construção de um objeto é complexo, isolamos essa complexidade em uma classe, seguindo um padrão famoso chamado Builder. Então usaremos um objeto da classe WebHostBuilder para criar esse objeto IWebHost pra gente. Então fazemos new WebHostBuilder.Em seguida podemos construir o host usando o método Build().Se você quiser economizar linhas de código pode fazer diretamente assim(mostrando o uso desnecessário da váriavel builder).
            IWebHost host = new WebHostBuilder()
                .UseKestrel() //Para dizer que usaremos o Kestrel, chamamos um método de extensão UseKestrel(). 
                .UseStartup<Startup>() //Basta que informemos qual é a classe durante a construção do objeto-host usando o método de extensão UseStartup<>(), passando por generics a classe a ser utilizada. Por convenção, o nome da classe é Startup, mas você pode escolher o nome que achar melhor.
                .Build();

            //Por fim, subimos o servidor usando o método Run().
            host.Run();

            //ImprimeLista(_repo.ParaLer);
            //ImprimeLista(_repo.Lendo);
            //ImprimeLista(_repo.Lidos);
            //Console.ReadLine();
        }

        static void ImprimeLista(ListaDeLeitura lista)
        {
            Console.WriteLine(lista);
        }
    }
}
