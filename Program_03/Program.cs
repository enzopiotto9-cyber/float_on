using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Program_03
{
    public enum ModoFerramenta
    {
        AtratorGravitacional,
        BuracoNegro,
        SemearVida,
        PulsoMutacao,
        EscudoEstase
    }

    public class Anomalia
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Massa { get; set; }
        public bool EhBuracoNegro { get; set; }
        public int VidaUtil { get; set; }
    }

    public class OrganismoQuantico
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Vx { get; set; }
        public double Vy { get; set; }
        public double Energia { get; set; }
        public double VelocidadeGenetica { get; set; }
        public double SentidoGenetico { get; set; }
        public int Geracao { get; set; }
        public ConsoleColor EspectroCor { get; set; }
        public bool Vivo { get { return Energia > 0; } }
    }

    public class ParticulaEter
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Vx { get; set; }
        public double Vy { get; set; }
        public double Carga { get; set; }
    }

    class Program
    {
        const int LARGURA = 76;
        const int ALTURA = 26;

        static char[,] bufferChars = new char[LARGURA, ALTURA];
        static ConsoleColor[,] bufferCores = new ConsoleColor[LARGURA, ALTURA];

        static List<OrganismoQuantico> organismos = new List<OrganismoQuantico>();
        static List<ParticulaEter> particulas = new List<ParticulaEter>();
        static List<Anomalia> anomalias = new List<Anomalia>();

        static Random rng = new Random();
        static ModoFerramenta ferramentaAtual = ModoFerramenta.AtratorGravitacional;

        static int cursorX = LARGURA / 2;
        static int cursorY = ALTURA / 2;
        static double tempoSimulacao = 0;
        static int totalNascimentos = 0;
        static int geracaoMaxima = 1;
        static bool pausado = false;
        static bool executando = true;

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.CursorVisible = false;
            Console.Title = "AETHER-IX // SIMULADOR DE BIOSFERA QUANTICA";

            SequenciaInicializacao();
            InicializarEcossistema();

            Stopwatch relogio = Stopwatch.StartNew();
            double ultimoFrame = 0;
            const double INTERVALO_FRAME = 1000.0 / 30.0;

            while (executando)
            {
                ProcessarEntrada();

                double agora = relogio.Elapsed.TotalMilliseconds;
                if (agora - ultimoFrame >= INTERVALO_FRAME)
                {
                    ultimoFrame = agora;
                    if (!pausado)
                    {
                        AtualizarFisicaEBio(0.05);
                        tempoSimulacao += 0.05;
                    }
                    RenderizarFrame();
                }
                Thread.Sleep(1);
            }
        }

        static double Limitar(double valor, double min, double max)
        {
            if (valor < min) return min;
            if (valor > max) return max;
            return valor;
        }

        static void SequenciaInicializacao()
        {
            Console.Clear();
            string[] boot = {
                "◈ CONECTANDO A MATRIZ DE DADOS XENO-ORBITAL...",
                "◈ CALIBRANDO CONSTANTES GRAVITACIONAIS...",
                "◈ SINTETIZANDO DNA QUANTICO E PARTICULAS DE ETER...",
                "◈ INTERFACE NEURAL SINCRONIZADA. BEM-VINDO, ARQUITETO."
            };

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n\n    ╔══════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("    ║          ◈ AETHER-IX // SIMULADOR DE BIOSFERA ALIENIGENA         ║");
            Console.WriteLine("    ╚══════════════════════════════════════════════════════════════════╝\n");

            foreach (var linha in boot)
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write("      ");
                foreach (char c in linha)
                {
                    Console.Write(c);
                    Thread.Sleep(10);
                }
                Console.WriteLine();
                Thread.Sleep(150);
            }
            Thread.Sleep(400);
            Console.Clear();
        }

        static void InicializarEcossistema()
        {
            organismos.Clear();
            particulas.Clear();
            anomalias.Clear();
            totalNascimentos = 0;
            geracaoMaxima = 1;

            for (int i = 0; i < 180; i++)
            {
                particulas.Add(new ParticulaEter
                {
                    X = rng.NextDouble() * (LARGURA - 2) + 1,
                    Y = rng.NextDouble() * (ALTURA - 2) + 1,
                    Vx = (rng.NextDouble() - 0.5) * 1.5,
                    Vy = (rng.NextDouble() - 0.5) * 1.5,
                    Carga = rng.NextDouble() * 10 + 5
                });
            }

            for (int i = 0; i < 22; i++)
            {
                CriarOrganismo(rng.Next(5, LARGURA - 5), rng.Next(4, ALTURA - 4), 1, 1.0, ConsoleColor.Green);
            }

            anomalias.Add(new Anomalia
            {
                X = LARGURA / 2.0,
                Y = ALTURA / 2.0,
                Massa = 45,
                EhBuracoNegro = false,
                VidaUtil = 999999
            });
        }

        static void CriarOrganismo(double x, double y, int geracao, double velGenetica, ConsoleColor cor)
        {
            totalNascimentos++;
            if (geracao > geracaoMaxima) geracaoMaxima = geracao;

            organismos.Add(new OrganismoQuantico
            {
                X = x,
                Y = y,
                Vx = (rng.NextDouble() - 0.5) * 2,
                Vy = (rng.NextDouble() - 0.5) * 2,
                Energia = 65,
                VelocidadeGenetica = velGenetica,
                SentidoGenetico = 8.0 + rng.NextDouble() * 6.0,
                Geracao = geracao,
                EspectroCor = cor
            });
        }

        static void ProcessarEntrada()
        {
            while (Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.W:
                    case ConsoleKey.UpArrow:
                        cursorY = Math.Max(1, cursorY - 1);
                        break;
                    case ConsoleKey.S:
                    case ConsoleKey.DownArrow:
                        cursorY = Math.Min(ALTURA - 2, cursorY + 1);
                        break;
                    case ConsoleKey.A:
                    case ConsoleKey.LeftArrow:
                        cursorX = Math.Max(1, cursorX - 2);
                        break;
                    case ConsoleKey.D:
                    case ConsoleKey.RightArrow:
                        cursorX = Math.Min(LARGURA - 2, cursorX + 2);
                        break;

                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        ferramentaAtual = ModoFerramenta.AtratorGravitacional;
                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        ferramentaAtual = ModoFerramenta.BuracoNegro;
                        break;
                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        ferramentaAtual = ModoFerramenta.SemearVida;
                        break;
                    case ConsoleKey.D4:
                    case ConsoleKey.NumPad4:
                        ferramentaAtual = ModoFerramenta.PulsoMutacao;
                        break;
                    case ConsoleKey.D5:
                    case ConsoleKey.NumPad5:
                        ferramentaAtual = ModoFerramenta.EscudoEstase;
                        break;

                    case ConsoleKey.Spacebar:
                        AplicarFerramentaNoCursor();
                        break;

                    case ConsoleKey.P:
                        pausado = !pausado;
                        break;

                    case ConsoleKey.R:
                        InicializarEcossistema();
                        break;

                    case ConsoleKey.Escape:
                        executando = false;
                        break;
                }
            }
        }

        static void AplicarFerramentaNoCursor()
        {
            switch (ferramentaAtual)
            {
                case ModoFerramenta.AtratorGravitacional:
                    anomalias.Add(new Anomalia
                    {
                        X = cursorX,
                        Y = cursorY,
                        Massa = 65,
                        EhBuracoNegro = false,
                        VidaUtil = 450
                    });
                    break;

                case ModoFerramenta.BuracoNegro:
                    anomalias.Add(new Anomalia
                    {
                        X = cursorX,
                        Y = cursorY,
                        Massa = 140,
                        EhBuracoNegro = true,
                        VidaUtil = 350
                    });
                    break;

                case ModoFerramenta.SemearVida:
                    for (int i = 0; i < 5; i++)
                    {
                        double ox = Limitar(cursorX + rng.Next(-3, 4), 2, LARGURA - 3);
                        double oy = Limitar(cursorY + rng.Next(-2, 3), 2, ALTURA - 3);
CriarOrganismo(ox, oy, 1, 1.2, ConsoleColor.Cyan);
}
break;
case ModoFerramenta.PulsoMutacao:
ConsoleColor[] espectros = { ConsoleColor.Magenta, ConsoleColor.Yellow, ConsoleColor.Cyan, ConsoleColor.Green };
foreach (var org in organismos)
{
double dx = org.X - cursorX;
double dy = org.Y - cursorY;
double dist = Math.Sqrt(dx * dx + dy * dy);
if (dist < 14)
{
org.VelocidadeGenetica = Math.Min(3.2, org.VelocidadeGenetica * 1.35);
org.Energia += 35;
org.EspectroCor = espectros[rng.Next(espectros.Length)];
}
}
break;
case ModoFerramenta.EscudoEstase:
anomalias.Clear();
break;
}
}
static void AtualizarFisicaEBio(double dt)
{
for (int i = anomalias.Count - 1; i >= 0; i--)
{
anomalias[i].VidaUtil--;
if (anomalias[i].VidaUtil <= 0) anomalias.RemoveAt(i);
}
foreach (var p in particulas)
{
foreach (var a in anomalias)
{
double dx = a.X - p.X;
double dy = (a.Y - p.Y) * 2.0;
double distSq = dx * dx + dy * dy + 4.0;
double forca = a.Massa / distSq;
p.Vx += (dx / Math.Sqrt(distSq)) * forca * dt * 8.0;
p.Vy += (dy / Math.Sqrt(distSq)) * forca * dt * 4.0;
}
p.Vx *= 0.985;
p.Vy *= 0.985;
p.X += p.Vx * dt * 10;
p.Y += p.Vy * dt * 10;
if (p.X < 1) { p.X = 1; p.Vx *= -0.8; }
if (p.X >= LARGURA - 1) { p.X = LARGURA - 2; p.Vx *= -0.8; }
if (p.Y < 1) { p.Y = 1; p.Vy *= -0.8; }
if (p.Y >= ALTURA - 1) { p.Y = ALTURA - 2; p.Vy *= -0.8; }
}
var novosFilhos = new System.Collections.Generic.List<OrganismoQuantico>();

for (int i = organismos.Count - 1; i >= 0; i--)
{
var org = organismos[i];
org.Energia -= 0.18 * org.VelocidadeGenetica;
ParticulaEter? alvoMaisProximo = null;
double menorDist = org.SentidoGenetico;
foreach (var p in particulas)
{
double dx = p.X - org.X;
double dy = (p.Y - org.Y) * 2.0;
double d = Math.Sqrt(dx * dx + dy * dy);
if (d < menorDist)
{
menorDist = d;
alvoMaisProximo = p;
}
}
if (alvoMaisProximo != null)
{
double angulo = Math.Atan2(alvoMaisProximo.Y - org.Y, alvoMaisProximo.X - org.X);
org.Vx += Math.Cos(angulo) * 0.45 * org.VelocidadeGenetica;
org.Vy += Math.Sin(angulo) * 0.25 * org.VelocidadeGenetica;
if (menorDist < 1.5)
{
org.Energia += alvoMaisProximo.Carga * 1.8;
alvoMaisProximo.X = rng.Next(2, LARGURA - 2);
alvoMaisProximo.Y = rng.Next(2, ALTURA - 2);
}
}
else
{
org.Vx += (rng.NextDouble() - 0.5) * 0.4;
org.Vy += (rng.NextDouble() - 0.5) * 0.2;
}
foreach (var a in anomalias)
{
if (a.EhBuracoNegro)
{
double dx = a.X - org.X;
double dy = (a.Y - org.Y) * 2.0;
double d = Math.Sqrt(dx * dx + dy * dy);
if (d < 2.2) org.Energia = 0;
}
}
double velAtual = Math.Sqrt(org.Vx * org.Vx + org.Vy * org.Vy);
double velMax = 1.6 * org.VelocidadeGenetica;
if (velAtual > velMax && velAtual > 0)
{
org.Vx = (org.Vx / velAtual) * velMax;
org.Vy = (org.Vy / velAtual) * velMax;
}
org.X += org.Vx * dt * 8;
org.Y += org.Vy * dt * 8;
if (org.X < 1) { org.X = 1; org.Vx *= -1; }
if (org.X >= LARGURA - 1) { org.X = LARGURA - 2; org.Vx *= -1; }
if (org.Y < 1) { org.Y = 1; org.Vy *= -1; }
if (org.Y >= ALTURA - 1) { org.Y = ALTURA - 2; org.Vy *= -1; }
if (org.Energia >= 130 && organismos.Count + novosFilhos.Count < 65)
{
org.Energia *= 0.5;
double mutacaoVel = Limitar(org.VelocidadeGenetica + (rng.NextDouble() - 0.45) * 0.25, 0.5, 3.5);
ConsoleColor novaCor = mutacaoVel > 2.2 ? ConsoleColor.Magenta : (mutacaoVel > 1.5 ? ConsoleColor.Yellow : ConsoleColor.Green);
novosFilhos.Add(new OrganismoQuantico
{
X = org.X,
Y = org.Y,
Vx = -org.Vx,
Vy = -org.Vy,
Energia = 60,
VelocidadeGenetica = mutacaoVel,
SentidoGenetico = org.SentidoGenetico + (rng.NextDouble() - 0.5),
Geracao = org.Geracao + 1,
EspectroCor = novaCor
});
totalNascimentos++;
if (org.Geracao + 1 > geracaoMaxima) geracaoMaxima = org.Geracao + 1;
}
if (!org.Vivo)
{
particulas[rng.Next(particulas.Count)].X = org.X;
particulas[rng.Next(particulas.Count)].Y = org.Y;
organismos.RemoveAt(i);
}
}
organismos.AddRange(novosFilhos);
if (organismos.Count == 0)
{
CriarOrganismo(LARGURA / 2.0, ALTURA / 2.0, 1, 1.0, ConsoleColor.Cyan);
}
}
static void RenderizarFrame()
{
for (int x = 0; x < LARGURA; x++)
{
for (int y = 0; y < ALTURA; y++)
{
bufferChars[x, y] = ' ';
bufferCores[x, y] = ConsoleColor.Black;
}
}
for (int x = 4; x < LARGURA - 1; x += 8)
{
for (int y = 2; y < ALTURA - 1; y += 4)
{
bufferChars[x, y] = '·';
bufferCores[x, y] = ConsoleColor.DarkGray;
}
}
foreach (var p in particulas)
{
int px = (int)Math.Round(p.X);
int py = (int)Math.Round(p.Y);
if (px > 0 && px < LARGURA - 1 && py > 0 && py < ALTURA - 1)
{
double vel = Math.Sqrt(p.Vx * p.Vx + p.Vy * p.Vy);
bufferChars[px, py] = vel > 1.2 ? '✦' : '∙';
bufferCores[px, py] = vel > 1.2 ? ConsoleColor.Cyan : ConsoleColor.DarkCyan;
}
}
foreach (var a in anomalias)
{
int ax = (int)Math.Round(a.X);
int ay = (int)Math.Round(a.Y);
if (ax > 0 && ax < LARGURA - 1 && ay > 0 && ay < ALTURA - 1)
{
bufferChars[ax, ay] = a.EhBuracoNegro ? '◉' : '✺';
bufferCores[ax, ay] = a.EhBuracoNegro ? ConsoleColor.Red : ConsoleColor.Yellow;
}
}
foreach (var org in organismos)
{
int ox = (int)Math.Round(org.X);
int oy = (int)Math.Round(org.Y);
if (ox > 0 && ox < LARGURA - 1 && oy > 0 && oy < ALTURA - 1)
{
bufferChars[ox, oy] = org.Geracao >= 4 ? '◈' : (org.Energia > 90 ? '◆' : '◊');
bufferCores[ox, oy] = org.EspectroCor;
}
}
if (cursorX > 0 && cursorX < LARGURA - 1 && cursorY > 0 && cursorY < ALTURA - 1)
{
bufferChars[cursorX, cursorY] = '╬';
bufferCores[cursorX, cursorY] = ConsoleColor.White;
}
Console.SetCursorPosition(0, 0);
Console.ForegroundColor = ConsoleColor.DarkCyan;
Console.WriteLine("╔════════════════════════════════════════════════════════════════════════════╦══════════════════════════════════╗");
Console.Write("║ ");
Console.ForegroundColor = ConsoleColor.Cyan;
string status = pausado ? "[ESTASE ATIVA]" : "[SIMULACAO ATIVA]";
string cabecalho = $"◈ AETHER-IX // CAMARA QUANTICA   [CICLO: {tempoSimulacao:F1}s]   {status}";
Console.Write(cabecalho.PadRight(74));
Console.ForegroundColor = ConsoleColor.DarkCyan;
Console.WriteLine(" ║     TELEMETRIA XENO-BIOLOGICA    ║");
Console.WriteLine("╠════════════════════════════════════════════════════════════════════════════╬══════════════════════════════════╣");
for (int y = 0; y < ALTURA; y++)
{
Console.ForegroundColor = ConsoleColor.DarkCyan;
Console.Write("║");
ConsoleColor corAtual = ConsoleColor.Black;
for (int x = 0; x < LARGURA; x++)
{
if (bufferCores[x, y] != corAtual)
{
corAtual = bufferCores[x, y];
Console.ForegroundColor = corAtual;
}
Console.Write(bufferChars[x, y]);
}
Console.ForegroundColor = ConsoleColor.DarkCyan;
Console.Write("║ ");
DesenharTelemetria(y);
Console.ForegroundColor = ConsoleColor.DarkCyan;
Console.WriteLine(" ║");
}
Console.WriteLine("╚════════════════════════════════════════════════════════════════════════════╩══════════════════════════════════╝");
}
static void DesenharTelemetria(int linha)
{
string texto = "";
ConsoleColor cor = ConsoleColor.Gray;
switch (linha)
{
case 0: texto = $"BIOMASSA VIVA   : {organismos.Count:D3} especimes"; cor = ConsoleColor.Green; break;
case 1: texto = $"GERACAO MAXIMA  : GEN-{geracaoMaxima:D2}"; cor = ConsoleColor.Magenta; break;
case 2: texto = $"NASCIMENTOS     : {totalNascimentos:D4}"; cor = ConsoleColor.Cyan; break;
case 3: texto = $"ANOMALIAS       : {anomalias.Count:D2} ativas"; cor = ConsoleColor.Yellow; break;
case 4: texto = "────────────────────────────────"; cor = ConsoleColor.DarkCyan; break;
case 5: texto = "ESPECTRO EVOLUTIVO:"; cor = ConsoleColor.White; break;
case 6: texto = " ◊ Primordial   (Vel. Base)"; cor = ConsoleColor.Green; break;
case 7: texto = " ◆ Adaptado     (Vel. Alta)"; cor = ConsoleColor.Yellow; break;
case 8: texto = " ◈ Apice Quant. (Mutacao Max)"; cor = ConsoleColor.Magenta; break;
case 9: texto = " ✦ Particula de Eter Livre"; cor = ConsoleColor.Cyan; break;
case 10: texto = "────────────────────────────────"; cor = ConsoleColor.DarkCyan; break;
case 11: texto = "FERRAMENTA DO ARQUITETO:"; cor = ConsoleColor.White; break;
case 12: texto = $" ➤ {ferramentaAtual}"; cor = ConsoleColor.Cyan; break;
case 13: texto = "────────────────────────────────"; cor = ConsoleColor.DarkCyan; break;
case 14: texto = "MATRIZ DE COMANDOS:"; cor = ConsoleColor.DarkGray; break;
case 15: texto = "  Estrela Atratora"; cor = ferramentaAtual == ModoFerramenta.AtratorGravitacional ? ConsoleColor.Yellow : ConsoleColor.Gray; break;
case 16: texto = "  Singularidade (Buraco Negro)"; cor = ferramentaAtual == ModoFerramenta.BuracoNegro ? ConsoleColor.Red : ConsoleColor.Gray; break;
case 17: texto = "  Semear Vida Quantica"; cor = ferramentaAtual == ModoFerramenta.SemearVida ? ConsoleColor.Green : ConsoleColor.Gray; break;
case 18: texto = "  Pulso de Radiacao Mutante"; cor = ferramentaAtual == ModoFerramenta.PulsoMutacao ? ConsoleColor.Magenta : ConsoleColor.Gray; break;
case 19: texto = "  Colapsar Todas Anomalias"; cor = ferramentaAtual == ModoFerramenta.EscudoEstase ? ConsoleColor.Cyan : ConsoleColor.Gray; break;
case 20: texto = "────────────────────────────────"; cor = ConsoleColor.DarkCyan; break;
case 21: texto = " [WASD/Setas] Mover Mira ╬"; cor = ConsoleColor.White; break;
case 22: texto = " [ESPACO]     Ativar Ferramenta"; cor = ConsoleColor.Yellow; break;
case 23: texto = "     Pausar / Estase"; cor = ConsoleColor.Gray; break;
case 24: texto = "     Resetar Biosfera"; cor = ConsoleColor.Gray; break;
case 25: texto = " [ESC]        Desconectar"; cor = ConsoleColor.DarkRed; break;
}
if (texto.Length > 32) texto = texto.Substring(0, 32);
Console.ForegroundColor = cor;
Console.Write(texto.PadRight(32));
}
}
}