using System.Text;
using DesafioProjetoHospedagem;
using DesafioProjetoHospedagem.Models;

Console.OutputEncoding = Encoding.UTF8;

Service service = new Service();
bool exibirMenu = true;

while (exibirMenu)
{
    Console.Clear();
    Console.WriteLine(@"O que deseja fazer?");

    Console.WriteLine(@"
    1 - Adicionar suíte
    2 - Adicionar hospede
    3 - Adicionar reserva
    4 - Relatório reserva
    5 - Encerrar aplicação");

    switch (Console.ReadLine())
    {
        case "1":
            service.NovaSuite();
            break;

        case "2":
            service.NovaPessoa();
            break;

        case "3":
            service.NovaReserva();
            break;

        case "4":
            service.RelatorioReservas();
            break;

        case "5":
            Console.WriteLine("\nAplicação finalizando!");
            service.AguaradarInputContinuar();
            exibirMenu = false;
            break;

        default:
            Console.WriteLine("Opção inválida\n");
            break;
    }
}