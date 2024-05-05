using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using DesafioProjetoHospedagem.Models;

namespace DesafioProjetoHospedagem
{
    public class Service
    {
        public List<Pessoa> Pessoas { get; set; }
        public List<Suite> Suites { get; set; }
        public List<Reserva> Reservas { get; set; }

        public Service() { 
            Pessoas = new List<Pessoa>();
            Suites = new List<Suite>();
            Reservas = new List<Reserva>();
        }

        public void NovaPessoa()
        {
            Console.WriteLine("\nQual o nome do hóspede?");
            string[] nomePessoa = Console.ReadLine().Split(new char[] { ' ' }, 2);
            Pessoa novaPessoa = new Pessoa(nome: nomePessoa[0], sobrenome: nomePessoa.Length > 1 ? nomePessoa[1] : string.Empty);
            Pessoas.Add(novaPessoa);
            Console.WriteLine($"{novaPessoa.NomeCompleto} cadastrado com sucesso!\n");
            AguaradarInputContinuar();
        }

        public void NovaSuite()
        {
            Console.WriteLine("\nQual o tipo da suite?");
            string tipo = Console.ReadLine();

            try
            {
                Console.WriteLine("\nQual a capacidade?");
                string strCapacidade = Console.ReadLine();

                int valorCapacidade;
                bool convert = int.TryParse(strCapacidade, out valorCapacidade);
                if (!convert)
                {
                    throw new Exception("O número informado para capacidade está invalido!");
                }

                Console.WriteLine("\nQual o valor da diária para essa suite?");
                string strDiaria = Console.ReadLine();

                decimal valorDiaria;
                convert = decimal.TryParse(strDiaria, out valorDiaria);
                if (!convert)
                {
                    throw new Exception("O valor informado para a diária está invalido!");
                }

                Suite novaSuite = new Suite(tipoSuite: tipo, capacidade: valorCapacidade, valorDiaria: valorDiaria);
                Suites.Add(novaSuite);
                Console.WriteLine($"{novaSuite.TipoSuite} cadastrada com sucesso!\n");
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}\nA nova suite não foi salva, por favor repetir a operação!");
            }
            finally
            {
                AguaradarInputContinuar();
            }
        }

        public void NovaReserva()
        {
            List<Pessoa> hospedes = new List<Pessoa>();
            Suite suite = new Suite();
            Reserva reserva = new Reserva();
            bool adicionarPessoa = true;
            bool suiteValida = false;
            bool diasValidos = false;
            int diasReserva = -1;

            while (!suiteValida) 
            { 
                Console.WriteLine("\nQual suite será utilizada nessa reserva?");
                foreach (Suite s in Suites)
                {
                    Console.WriteLine($"{Suites.FindIndex(su => su.TipoSuite == s.TipoSuite)} - {s.TipoSuite}");
                }

                string strId = Console.ReadLine();

                int idSuite = -1;
                suiteValida = int.TryParse(strId, out idSuite);

                if (!suiteValida || idSuite >= Suites.Count)
                {
                    Console.WriteLine("\nA suite selecionada está inválida, tente novamente!");
                    suiteValida = false;
                }
                else
                {
                    suite = Suites.ElementAt(idSuite);
                    Console.WriteLine($"\nSuite selecionada: {suite.TipoSuite}");
                }
            }

            while (adicionarPessoa)
            {
                Console.WriteLine("\nQuem será adicionado a essa reserva?");
                for (int i = 0; i < Pessoas.Count; i++)
                {
                    Console.WriteLine($"{i} - {Pessoas[i].NomeCompleto}");
                }

                AdicionarHospedeReserva(hospedes, Console.ReadLine());

                Console.WriteLine("\nHóspedes adicionados a reserva!");
                foreach (Pessoa pessoa in hospedes)
                {
                    Console.WriteLine($"{pessoa.NomeCompleto}");
                }

                Console.WriteLine("\nDeseja adicionar alguém mais a reserva? 1 - Sim ou 2 Não");

                if (Console.ReadLine() != "1") {
                    adicionarPessoa = false;
                } 

            }

            
            while (!diasValidos) { 
                Console.WriteLine("\nQuantas diárias serão necessárias?");
                string strId = Console.ReadLine();

                diasValidos = int.TryParse(strId, out diasReserva);

                if (!diasValidos || (diasReserva == -1)) {
                    Console.WriteLine("\nQuantidade de dias inválidos, tente novamente!");
                }
            }

            try
            {
                reserva.CadastrarSuite(suite);
                reserva.CadastrarHospedes(hospedes);
                reserva.DiasReservados = diasReserva;
                Reservas.Add(reserva);
                Console.WriteLine($"\nO total da reserva ficou em: {reserva.CalcularValorDiaria()}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"\n{e.Message} \nA reserva não foi realizada, tente nonvamente!");
            }
            finally
            {
                AguaradarInputContinuar();
            }

        }

        public List<Pessoa> AdicionarHospedeReserva(List<Pessoa> hospedes, string idPessoa)
        {
            try
            {
                int id = -1;
                var parse = int.TryParse(idPessoa, out id);

                if (!parse || (id > Pessoas.Count - 1))
                {
                    throw new Exception("Um ID inválido foi fornecido!");
                }

                Pessoa novaPessoa = Pessoas.ElementAt(id);

                if (hospedes.FindIndex(p => p.NomeCompleto == novaPessoa.NomeCompleto) >= 0) 
                {
                    throw new Exception("O ID informado já está nessa reserva!");
                }

                hospedes.Add(novaPessoa);
                return hospedes;
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}\nO novo hóspede não foi adicionado!");
                return hospedes;
            }
        }

        public void RelatorioReservas()
        {
            foreach (Reserva r in Reservas)
            {
                Console.WriteLine("----------------------------------------");
                Console.WriteLine("Hospedes:");
                foreach (Pessoa p in r.Hospedes)
                {
                    Console.WriteLine($"- {p.NomeCompleto}");
                }
                Console.WriteLine($"Acomodação: Suite {r.Suite.TipoSuite} - {r.Suite.ValorDiaria} reais por diária");
                Console.WriteLine($"Valor por {r.DiasReservados} diárias: {r.CalcularValorDiaria()} {(r.DiasReservados >= 10 ? " (desconto de 10%)" : "")}");
                Console.WriteLine("----------------------------------------");
            }

            AguaradarInputContinuar();
        }

        public void AguaradarInputContinuar() {
            Console.WriteLine($"Pressione uma tecla para continuar...\n");
            Console.ReadLine();
        }
    }
}
