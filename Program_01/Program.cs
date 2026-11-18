using System;
using System.Collections.Generic;
using System.Globalization;

namespace Program_01 //o desing foi feito pelo claudecode
{
    public class CalculadoraAvancada
    {
        public double UltimoResultado { get; private set; } = 0;
        public List<string> Historico { get; private set; } = new List<string>();

        public double Somar(double a, double b) => Registrar($"{a} + {b}", a + b);
        public double Subtrair(double a, double b) => Registrar($"{a} - {b}", a - b);
        public double Multiplicar(double a, double b) => Registrar($"{a} × {b}", a * b);
        
        public double Dividir(double a, double b)
        {
            if (b == 0) throw new DivideByZeroException("Divisão por zero não permitida.");
            return Registrar($"{a} ÷ {b}", a / b);
        }

        public double Potencia(double baseNum, double expoente) => Registrar($"{baseNum} ^ {expoente}", Math.Pow(baseNum, expoente));
        public double RestoDivisao(double a, double b) => Registrar($"{a} % {b}", a % b);
        
        public double RaizQuadrada(double a)
        {
            if (a < 0) throw new ArgumentException("Não existe raiz real de número negativo.");
            return Registrar($"√({a})", Math.Sqrt(a));
        }

        public double Porcentagem(double valor, double percentual) => Registrar($"{percentual}% de {valor}", (valor * percentual) / 100.0);
        public double Seno(double graus) => Registrar($"sen({graus}°)", Math.Sin(graus * Math.PI / 180.0));
        public double Cosseno(double graus) => Registrar($"cos({graus}°)", Math.Cos(graus * Math.PI / 180.0));
        public double Tangente(double graus) => Registrar($"tan({graus}°)", Math.Tan(graus * Math.PI / 180.0));
        
        public double Logaritmo(double a)
        {
            if (a <= 0) throw new ArgumentException("O logaritmo exige um número maior que zero.");
            return Registrar($"log10({a})", Math.Log10(a));
        }

        public long Fatorial(int n)
        {
            if (n < 0 || n > 20) throw new ArgumentException("Digite um inteiro entre 0 e 20.");
            long resultado = 1;
            for (int i = 2; i <= n; i++) resultado *= i;
            Registrar($"{n}!", resultado);
            return resultado;
        }

        private double Registrar(string expressao, double resultado)
        {
            UltimoResultado = Math.Round(resultado, 8);
            Historico.Add($"{expressao} = {UltimoResultado}");
            return UltimoResultado;
        }

        public void LimparHistorico()
        {
            Historico.Clear();
            UltimoResultado = 0;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "⚡ ULTRA CALC PRO v2.0 - C#";

            CalculadoraAvancada calc = new CalculadoraAvancada();
            string mensagemStatus = "Pronto para calcular. Dica: digite 'ans' para usar o último resultado.";
            ConsoleColor corStatus = ConsoleColor.Cyan;

            while (true)
            {
                DesenharInterface(calc, mensagemStatus, corStatus);

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("\n  ➤ Escolha uma opção: ");
                Console.ResetColor();
                string opcao = Console.ReadLine()?.Trim().ToLower() ?? "";

                if (opcao == "0" || opcao == "sair")
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\n  Até logo! Obrigado por usar a Ultra Calc Pro.\n");
                    Console.ResetColor();
                    break;
                }

                try
                {
                    switch (opcao)
                    {
                        case "1": // Soma
                            ExecutarOperacaoBinaria(calc, "+", (a, b) => calc.Somar(a, b));
                            break;
                        case "2": // Subtração
                            ExecutarOperacaoBinaria(calc, "-", (a, b) => calc.Subtrair(a, b));
                            break;
                        case "3": // Multiplicação
                            ExecutarOperacaoBinaria(calc, "×", (a, b) => calc.Multiplicar(a, b));
                            break;
                        case "4": // Divisão
                            ExecutarOperacaoBinaria(calc, "÷", (a, b) => calc.Dividir(a, b));
                            break;
                        case "5": // Potência
                            ExecutarOperacaoBinaria(calc, "^", (a, b) => calc.Potencia(a, b));
                            break;
                        case "6": // Módulo
                            ExecutarOperacaoBinaria(calc, "%", (a, b) => calc.RestoDivisao(a, b));
                            break;
                        case "7": // Raiz Quadrada
                            double r = LerNumero("Digite o número: ", calc.UltimoResultado);
                            calc.RaizQuadrada(r);
                            break;
                        case "8": // Porcentagem
                            double v = LerNumero("Digite o valor base: ", calc.UltimoResultado);
                            double p = LerNumero("Digite a porcentagem (%): ", calc.UltimoResultado);
                            calc.Porcentagem(v, p);
                            break;
                        case "9": // Seno
                            double angS = LerNumero("Digite o ângulo em graus: ", calc.UltimoResultado);
                            calc.Seno(angS);
                            break;
                        case "10": // Cosseno
                            double angC = LerNumero("Digite o ângulo em graus: ", calc.UltimoResultado);
                            calc.Cosseno(angC);
                            break;
                        case "11": // Tangente
                            double angT = LerNumero("Digite o ângulo em graus: ", calc.UltimoResultado);
                            calc.Tangente(angT);
                            break;
                        case "12": // Logaritmo
                            double logVal = LerNumero("Digite o número: ", calc.UltimoResultado);
                            calc.Logaritmo(logVal);
                            break;
                        case "13": // Fatorial
                            int fatVal = (int)LerNumero("Digite um número inteiro (0 a 20): ", calc.UltimoResultado);
                            calc.Fatorial(fatVal);
                            break;
                        case "c": // Limpar
                            calc.LimparHistorico();
                            mensagemStatus = "Memória e histórico limpos!";
                            corStatus = ConsoleColor.DarkYellow;
                            continue;
                        default:
                            mensagemStatus = "Opção inválida! Escolha um número do menu.";
                            corStatus = ConsoleColor.Red;
                            continue;
                    }

                    mensagemStatus = $"Cálculo realizado com sucesso! Resultado salvo em 'ans'.";
                    corStatus = ConsoleColor.Green;
                }
                catch (Exception ex)
                {
                    mensagemStatus = $"ERRO: {ex.Message}";
                    corStatus = ConsoleColor.Red;
                }
            }
        }

        static void ExecutarOperacaoBinaria(CalculadoraAvancada calc, string simbolo, Func<double, double, double> operacao)
        {
            double n1 = LerNumero("  1º Valor (ou 'ans'): ", calc.UltimoResultado);
            double n2 = LerNumero($"  2º Valor ({n1} {simbolo} ?): ", calc.UltimoResultado);
            operacao(n1, n2);
        }

        static double LerNumero(string prompt, double ans)
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(prompt);
                Console.ForegroundColor = ConsoleColor.Cyan;
                string entrada = Console.ReadLine()?.Trim().ToLower() ?? "";
                Console.ResetColor();

                if (entrada == "ans") return ans;

                entrada = entrada.Replace(',', '.');
                if (double.TryParse(entrada, NumberStyles.Any, CultureInfo.InvariantCulture, out double numero))
                {
                    return numero;
                }

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("  [!] Valor inválido. Digite um número ou 'ans'.");
                Console.ResetColor();
            }
        }

        static void DesenharInterface(CalculadoraAvancada calc, string status, ConsoleColor corStatus)
        {
            Console.Clear();
            string linhaHorizontal = new string('═', 58);

            // Cabeçalho
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"╔{linhaHorizontal}╗");
            Console.WriteLine("║             ⚡ ULTRA CALC PRO - C# EDITION ⚡            ║");
            Console.WriteLine($"╠{linhaHorizontal}╣");

            // Visor LCD
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("║  VISOR (ANS): ");
            Console.ForegroundColor = ConsoleColor.Green;
            string visorTexto = calc.UltimoResultado.ToString("G", CultureInfo.InvariantCulture);
            Console.Write(visorTexto.PadLeft(41));
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("  ║");

            Console.WriteLine($"╠{linhaHorizontal}╣");

            // Menu em 2 Colunas
            DesenharLinhaMenu("[1] Soma (+)", "[8]  Porcentagem (%)");
            DesenharLinhaMenu("[2] Subtração (-)", "[9]  Seno (graus)");
DesenharLinhaMenu("[3] Multiplicação (×)", "[10] Cosseno (graus)");
DesenharLinhaMenu("[4] Divisão (÷)", "[11] Tangente (graus)");
DesenharLinhaMenu("[5] Potência (x^y)", "[12] Logaritmo (base 10)");
DesenharLinhaMenu("[6] Resto Divisão (%)", "[13] Fatorial (n!)");
DesenharLinhaMenu("[7] Raiz Quadrada (√)", "[C]  Limpar Memória");
DesenharLinhaMenu("[0] Sair do Sistema", "");
Console.ForegroundColor = ConsoleColor.Magenta;
Console.WriteLine($"╠{linhaHorizontal}╣");
// Histórico Recente (Últimas 4 operações)
Console.ForegroundColor = ConsoleColor.DarkCyan;
Console.WriteLine("║  HISTÓRICO RECENTE:                                      ║");
if (calc.Historico.Count == 0)
{
Console.ForegroundColor = ConsoleColor.DarkGray;
Console.WriteLine("║    Nenhuma operação registrada ainda.                    ║");
}
else
{
int inicio = Math.Max(0, calc.Historico.Count - 4);
for (int i = inicio; i < calc.Historico.Count; i++)
{
string item = "  • " + calc.Historico[i];
if (item.Length > 54) item = item.Substring(0, 51) + "...";
Console.ForegroundColor = ConsoleColor.Gray;
Console.Write("║  ");
Console.Write(item.PadRight(54));
Console.ForegroundColor = ConsoleColor.Magenta;
Console.WriteLine("  ║");
}
}
// Barra de Status
Console.ForegroundColor = ConsoleColor.Magenta;
Console.WriteLine($"╠{linhaHorizontal}╣");
Console.ForegroundColor = corStatus;
string statusFormatado = status.Length > 54 ? status.Substring(0, 51) + "..." : status;
Console.Write("║  ");
Console.Write(statusFormatado.PadRight(54));
Console.ForegroundColor = ConsoleColor.Magenta;
Console.WriteLine("  ║");
Console.WriteLine($"╚{linhaHorizontal}╝");
Console.ResetColor();
}
static void DesenharLinhaMenu(string col1, string col2)
{
Console.ForegroundColor = ConsoleColor.Magenta;
Console.Write("║  ");
Console.ForegroundColor = ConsoleColor.White;
Console.Write(col1.PadRight(26));
Console.ForegroundColor = ConsoleColor.Cyan;
Console.Write(col2.PadRight(28));
Console.ForegroundColor = ConsoleColor.Magenta;
Console.WriteLine("  ║");
}
}
}