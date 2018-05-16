using Ler.ListaLeitura.App.Negocio;
using System.Collections.Generic;

namespace Ler.ListaLeitura.App.Repositorio
{
    interface ILivroRepositorio
    {
        ListaDeLeitura ParaLer { get; }
        ListaDeLeitura Lendo { get; }
        ListaDeLeitura Lidos { get; }
        IEnumerable<Livro> Todos { get; }
        void Incluir(Livro livro);
    }
}
