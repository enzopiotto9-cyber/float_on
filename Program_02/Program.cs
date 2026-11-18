using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Program_01 //feito 25% pelo claude code e 10% google gemini IA.
{
    public enum TipoTile { Parede, Chao, Porta, Escada, Bau, Armadilha }
    public enum TipoInimigo { Drone, Sentinela, Executor, Boss }

    public class Entidade
    {
        public string Nome { get; set; }
        public char Simbolo { get; set; }
        public ConsoleColor Cor { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int HpMax { get; set; }
        public int Hp { get; set; }
        public int Ataque { get; set; }
        public int Defesa { get; set; }
        public int XpRecompensa { get; set; }
        public TipoInimigo Tipo { get; set; }
        public bool Vivo => Hp > 0;
    }

    public class Jogador : Entidade
    {
        public int Nivel { get; set; } = 1;
        public int Xp { get; set; } = 0;
        public int XpProximoNivel { get; set; } = 50;
        public int EnergiaMax { get; set; } = 100;
        public int Energia { get; set; } = 100;
        public int Pocoes { get; set; } = 2;
        public int Granadas { get; set; } = 1;
        public int Ouro { get; set; } = 0;
        public string ArmaAtual { get; set; } = "Lâmina de Plasma";
        public string ArmaduraAtual { get; set; } = "Traje Tático Mk.I";
    }

    class Program
    {
        // Configurações da Arena
        const int LARGURA_MAPA = 46;
        const int ALTURA_MAPA = 22;
        const int RAIO_VISAO = 6;

        static TipoTile[,] mapa = new TipoTile[LARGURA_MAPA, ALTURA_MAPA];
        static bool[,] explorado = new bool[LARGURA_MAPA, ALTURA_MAPA];
        static bool[,] visivel = new bool[LARGURA_MAPA, ALTURA_MAPA];

        static Jogador player;
        static List<Entidade> inimigos = new List<Entidade>();
        static List<string> logCombate = new List<string>();
        static Random rng = new Random();

        static int andarAtual = 1;
        static int turnos = 0;
        static bool jogoRodando = true;

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.CursorVisible = false;
            Console.Title = "⚔️ CYBER DUNGEON: PROTOCOLO ÔMEGA";

            TelaTitulo();
            IniciarNovoJogo();

            while (jogoRodando)
            {
                AtualizarCampoDeVisao();
                RenderizarTela();

                if (!player.Vivo)
                {
                    TelaGameOver();
                    break;
                }

                ConsoleKeyInfo tecla = Console.ReadKey(true);
                ProcessarTurno(tecla.Key);
            }
        }

        static void TelaTitulo()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"
    ╔══════════════════════════════════════════════════════════════════════════╗
    ║                                                                          ║
    ║     ██████╗██╗   ██╗██████╗ ███████╗██████╗     ██████╗ ██╗   ██╗███╗    ║
    ║    ██╔════╝╚██╗ ██╔╝██╔══██╗██╔════╝██╔══██╗    ██╔══██╗██║   ██║████╗   ║
    ║    ██║      ╚████╔╝ ██████╔╝█████╗  ██████╔╝    ██║  ██║██║   ██║██╔██╗  ║
    ║    ██║       ╚██╔╝  ██╔══██╗██╔══╝  ██╔══██╗    ██║  ██║██║   ██║██║╚██╗ ║
    ║    ╚██████╗   ██║   ██████╔╝███████╗██║  ██║    ██████╔╝╚██████╔╝██║ ╚██╗║
    ║     ╚═════╝   ╚═╝   ╚═════╝ ╚══════╝╚═╝  ╚═╝    ╚═════╝  ╚═════╝ ╚═╝  ╚═╝║
    ║                        [ PROTOCOLO ÔMEGA v3.0 ]                          ║
    ╚══════════════════════════════════════════════════════════════════════════╝");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\n      Explore o complexo subterrâneo, evolua seu equipamento e sobreviva.");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("\n      [CONTROLES]");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("      • W A S D / Setas : Mover e Atacar inimigos ao colidir");
            Console.WriteLine("      • [1]             : Usar Nano-Poção (Restaura 45% HP)");
            Console.WriteLine("      • [2]             : Granada PEM (Dano em área ao redor - 30 Energia)");
            Console.WriteLine("      • [3]             : Sobrecarga (Cura 20 Energia em troca de 1 turno)");
            Console.WriteLine("      • [ESC]           : Sair do Jogo");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("\n      Pressione [ENTER] para iniciar a infiltração...");
            while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }
        }

        static void IniciarNovoJogo()
        {
            player = new Jogador
            {
                Nome = "Agente K-7",
                Simbolo = '@',
                Cor = ConsoleColor.Cyan,
                HpMax = 100,
                Hp = 100,
                Ataque = 14,
                Defesa = 3
            };

            andarAtual = 1;
            turnos = 0;
            logCombate.Clear();
            AdicionarLog("Sistema online. Infiltração iniciada no Setor 1.", ConsoleColor.Cyan);
            GerarNivel();
        }

        static void GerarNivel()
        {
            inimigos.Clear();
            for (int x = 0; x < LARGURA_MAPA; x++)
            {
                for (int y = 0; y < ALTURA_MAPA; y++)
                {
                    mapa[x, y] = TipoTile.Parede;
                    explorado[x, y] = false;
                    visivel[x, y] = false;
                }
            }

            List<(int cx, int cy, int w, int h)> salas = new List<(int, int, int, int)>();
            int tentativas = 35;

            for (int i = 0; i < tentativas; i++)
            {
                int w = rng.Next(5, 10);
                int h = rng.Next(4, 7);
                int x = rng.Next(1, LARGURA_MAPA - w - 1);
                int y = rng.Next(1, ALTURA_MAPA - h - 1);

                bool sobrepoe = false;
                foreach (var s in salas)
                {
                    if (x < s.cx + s.w + 1 && x + w + 1 > s.cx && y < s.cy + s.h + 1 && y + h + 1 > s.cy)
                    {
                        sobrepoe = true;
                        break;
                    }
                }

                if (!sobrepoe)
                {
                    for (int rx = x; rx < x + w; rx++)
                        for (int ry = y; ry < y + h; ry++)
                            mapa[rx, ry] = TipoTile.Chao;

                    if (salas.Count > 0)
                    {
                        var anterior = salas[salas.Count - 1];
                        CriarCorredor(anterior.cx + anterior.w / 2, anterior.cy + anterior.h / 2, x + w / 2, y + h / 2);
                    }
                    salas.Add((x, y, w, h));
                }
            }

            // Posiciona Jogador na primeira sala
            player.X = salas[0].cx + salas[0].w / 2;
            player.Y = salas[0].cy + salas[0].h / 2;

            // Posiciona Escada na última sala
            var ultimaSala = salas[salas.Count - 1];
            mapa[ultimaSala.cx + ultimaSala.w / 2, ultimaSala.cy + ultimaSala.h / 2] = TipoTile.Escada;

            // Popula salas intermediárias com inimigos, baús e armadilhas
            for (int i = 1; i < salas.Count; i++)
            {
                var sala = salas[i];
                int centroX = sala.cx + sala.w / 2;
                int centroY = sala.cy + sala.h / 2;

                // Chance de Baú
                if (rng.Next(100) < 45 && i != salas.Count - 1)
                {
                    mapa[centroX, centroY] = TipoTile.Bau;
                }
                else if (rng.Next(100) < 25 && i != salas.Count - 1)
                {
                    mapa[centroX, centroY] = TipoTile.Armadilha;
                }

                // Spawna Inimigos
                int qtdInimigos = rng.Next(1, 3);
                for (int e = 0; e < qtdInimigos; e++)
                {
                    int ex = rng.Next(sala.cx, sala.cx + sala.w);
                    int ey = rng.Next(sala.cy, sala.cy + sala.h);
                    if ((ex != player.X || ey != player.Y) && mapa[ex, ey] == TipoTile.Chao && ObterInimigoEm(ex, ey) == null)
                    {
                        inimigos.Add(CriarInimigo(ex, ey, i == salas.Count - 1 && andarAtual % 3 == 0 && e == 0));
                    }
                }
            }
        }

        static void CriarCorredor(int x1, int y1, int x2, int y2)
        {
            int x = x1, y = y1;
            while (x != x2)
            {
                mapa[x, y] = TipoTile.Chao;
                x += x < x2 ? 1 : -1;
            }
            while (y != y2)
            {
                mapa[x, y] = TipoTile.Chao;
                y += y < y2 ? 1 : -1;
            }
        }

        static Entidade CriarInimigo(int x, int y, bool isBoss)
        {
            double escala = 1 + (andarAtual - 1) * 0.25;
            if (isBoss)
            {
                return new Entidade
                {
                    Nome = "TITÃ CIBERNÉTICO",
                    Simbolo = 'Ω',
                    Cor = ConsoleColor.Magenta,
                    X = x,
                    Y = y,
                    HpMax = (int)(90 * escala),
                    Hp = (int)(90 * escala),
                    Ataque = (int)(18 * escala),
                    Defesa = (int)(5 * escala),
                    XpRecompensa = 120 * andarAtual,
                    Tipo = TipoInimigo.Boss
                };
            }

            int roll = rng.Next(100);
            if (roll < 50)
            {
                return new Entidade
                {
                    Nome = "Drone Vigia",
                    Simbolo = 'd',
                    Cor = ConsoleColor.Yellow,
                    X = x,
                    Y = y,
HpMax = (int)(22 * escala),
Hp = (int)(22 * escala),
Ataque = (int)(7 * escala),
Defesa = 1,
XpRecompensa = 15 * andarAtual,
Tipo = TipoInimigo.Drone
};
}
else if (roll < 85)
{
return new Entidade
{
Nome = "Sentinela Armado",
Simbolo = 'S',
Cor = ConsoleColor.DarkYellow,
X = x,
Y = y,
HpMax = (int)(38 * escala),
Hp = (int)(38 * escala),
Ataque = (int)(11 * escala),
Defesa = (int)(3 * escala),
XpRecompensa = 28 * andarAtual,
Tipo = TipoInimigo.Sentinela
};
}
else
{
return new Entidade
{
Nome = "Executor Alpha",
Simbolo = 'E',
Cor = ConsoleColor.Red,
X = x,
Y = y,
HpMax = (int)(55 * escala),
Hp = (int)(55 * escala),
Ataque = (int)(15 * escala),
Defesa = (int)(4 * escala),
XpRecompensa = 45 * andarAtual,
Tipo = TipoInimigo.Executor
};
}
}
static void ProcessarTurno(ConsoleKey tecla)
{
int dx = 0, dy = 0;
bool acaoRealizada = false;
switch (tecla)
{
case ConsoleKey.W:
case ConsoleKey.UpArrow:
dy = -1; break;
case ConsoleKey.S:
case ConsoleKey.DownArrow:
dy = 1; break;
case ConsoleKey.A:
case ConsoleKey.LeftArrow:
dx = -1; break;
case ConsoleKey.D:
case ConsoleKey.RightArrow:
dx = 1; break;
case ConsoleKey.D1:
case ConsoleKey.NumPad1:
UsarPocao();
acaoRealizada = true;
break;
case ConsoleKey.D2:
case ConsoleKey.NumPad2:
UsarGranadaPEM();
acaoRealizada = true;
break;
case ConsoleKey.D3:
case ConsoleKey.NumPad3:
player.Energia = Math.Min(player.EnergiaMax, player.Energia + 25);
AdicionarLog("Você canalizou energia (+25 EN).", ConsoleColor.Cyan);
acaoRealizada = true;
break;
case ConsoleKey.Escape:
jogoRodando = false;
return;
}
if (dx != 0 || dy != 0)
{
int novoX = player.X + dx;
int novoY = player.Y + dy;
if (novoX >= 0 && novoX < LARGURA_MAPA && novoY >= 0 && novoY < ALTURA_MAPA)
{
Entidade alvo = ObterInimigoEm(novoX, novoY);
if (alvo != null)
{
Atacar(player, alvo);
acaoRealizada = true;
}
else if (mapa[novoX, novoY] != TipoTile.Parede)
{
player.X = novoX;
player.Y = novoY;
acaoRealizada = true;
VerificarInteracaoTile();
}
}
}
if (acaoRealizada)
{
turnos++;
if (turnos % 2 == 0 && player.Energia < player.EnergiaMax) player.Energia += 2;
TurnoDosInimigos();
}
}
static void VerificarInteracaoTile()
{
TipoTile tile = mapa[player.X, player.Y];
if (tile == TipoTile.Bau)
{
mapa[player.X, player.Y] = TipoTile.Chao;
int evento = rng.Next(100);
if (evento < 35)
{
player.Pocoes++;
AdicionarLog("Baú aberto! Encontrou +1 Nano-Poção.", ConsoleColor.Green);
}
else if (evento < 60)
{
player.Granadas++;
AdicionarLog("Baú aberto! Encontrou +1 Granada PEM.", ConsoleColor.Yellow);
}
else if (evento < 80)
{
int bonusAtk = rng.Next(2, 5);
player.Ataque += bonusAtk;
player.ArmaAtual = $"Katana Térmica +{andarAtual}";
AdicionarLog($"Upgrade de Arma! Ataque aumentou em +{bonusAtk}!", ConsoleColor.Magenta);
}
else
{
int bonusDef = rng.Next(1, 4);
player.Defesa += bonusDef;
player.ArmaduraAtual = $"Blindagem Titan +{andarAtual}";
AdicionarLog($"Upgrade de Armadura! Defesa aumentou em +{bonusDef}!", ConsoleColor.Cyan);
}
}
else if (tile == TipoTile.Armadilha)
{
mapa[player.X, player.Y] = TipoTile.Chao;
int dano = rng.Next(8, 16);
player.Hp -= dano;
AdicionarLog($"ALERTA! Mina laser ativada! Você sofreu {dano} de dano!", ConsoleColor.Red);
}
else if (tile == TipoTile.Escada)
{
andarAtual++;
player.Hp = Math.Min(player.HpMax, player.Hp + 15);
AdicionarLog($"Descendo para o Setor {andarAtual}... (+15 HP recuperado)", ConsoleColor.Green);
GerarNivel();
}
}
static void UsarPocao()
{
if (player.Pocoes <= 0)
{
AdicionarLog("Sem Nano-Poções no inventário!", ConsoleColor.DarkGray);
return;
}
player.Pocoes--;
int cura = (int)(player.HpMax * 0.45);
player.Hp = Math.Min(player.HpMax, player.Hp + cura);
AdicionarLog($"Nano-Poção injetada! +{cura} HP restaurado.", ConsoleColor.Green);
}
static void UsarGranadaPEM()
{
if (player.Granadas <= 0)
{
AdicionarLog("Você não possui Granadas PEM!", ConsoleColor.DarkGray);
return;
}
if (player.Energia < 30)
{
AdicionarLog("Energia insuficiente (requer 30 EN)!", ConsoleColor.Red);
return;
}
player.Granadas--;
player.Energia -= 30;
int atingidos = 0;
for (int i = inimigos.Count - 1; i >= 0; i--)
{
var ini = inimigos[i];
int dist = Math.Abs(ini.X - player.X) + Math.Abs(ini.Y - player.Y);
if (dist <= 4)
{
int dano = player.Ataque * 2;
ini.Hp -= dano;
atingidos++;
if (!ini.Vivo) FinalizarInimigo(ini);
}
}
AdicionarLog($"EXPLOSÃO PEM! {atingidos} inimigos eletrocutados na área!", ConsoleColor.Yellow);
}
static void Atacar(Entidade atacante, Entidade defensor)
{
bool critico = rng.Next(100) < 18;
int danoBase = Math.Max(1, atacante.Ataque - defensor.Defesa + rng.Next(-2, 4));
if (critico) danoBase = (int)(danoBase * 1.6);
defensor.Hp -= danoBase;
if (atacante == player)
{
string tagCrit = critico ? " [CRÍTICO!]" : "";
AdicionarLog($"Você atingiu {defensor.Nome} por {danoBase} de dano{tagCrit}.", critico ? ConsoleColor.Yellow : ConsoleColor.White);
if (!defensor.Vivo) FinalizarInimigo(defensor);
}
else
{
AdicionarLog($"{atacante.Nome} atacou você causando {danoBase} de dano!", ConsoleColor.Red);
}
}
static void FinalizarInimigo(Entidade ini)
{
inimigos.Remove(ini);
player.Xp += ini.XpRecompensa;
player.Ouro += rng.Next(5, 20) * andarAtual;
AdicionarLog($"{ini.Nome} destruído! (+{ini.XpRecompensa} XP)", ConsoleColor.Green);
if (player.Xp >= player.XpProximoNivel)
{
player.Nivel++;
player.Xp -= player.XpProximoNivel;
player.XpProximoNivel = (int)(player.XpProximoNivel * 1.45);
player.HpMax += 20;
player.Hp = player.HpMax;
player.Ataque += 4;
player.Defesa += 2;
AdicionarLog($"LEVEL UP! Você alcançou o Nível {player.Nivel}! Status restaurados.", ConsoleColor.Magenta);
}
}
static void TurnoDosInimigos()
{
foreach (var ini in inimigos)
{
int dist = Math.Abs(ini.X - player.X) + Math.Abs(ini.Y - player.Y);
if (dist == 1)
{
Atacar(ini, player);
}
else if (dist <= 7) // Raio de detecção
{
int stepX = Math.Sign(player.X - ini.X);
int stepY = Math.Sign(player.Y - ini.Y);
if (stepX != 0 && PodeMover(ini.X + stepX, ini.Y))
ini.X += stepX;
else if (stepY != 0 && PodeMover(ini.X, ini.Y + stepY))
ini.Y += stepY;
}
}
}
static bool PodeMover(int x, int y)
{
if (x < 0 || x >= LARGURA_MAPA || y < 0 || y >= ALTURA_MAPA) return false;
if (mapa[x, y] == TipoTile.Parede) return false;
if (x == player.X && y == player.Y) return false;
if (ObterInimigoEm(x, y) != null) return false;
return true;
}
static Entidade ObterInimigoEm(int x, int y)
{
foreach (var ini in inimigos)
if (ini.X == x && ini.Y == y) return ini;
return null;
}
static void AtualizarCampoDeVisao()
{
for (int x = 0; x < LARGURA_MAPA; x++)
{
for (int y = 0; y < ALTURA_MAPA; y++)
{
double dist = Math.Sqrt(Math.Pow(x - player.X, 2) + Math.Pow(y - player.Y, 2));
if (dist <= RAIO_VISAO)
{
visivel[x, y] = true;
explorado[x, y] = true;
}
else
{
visivel[x, y] = false;
}
}
}
}
static void AdicionarLog(string msg, ConsoleColor cor)
{
logCombate.Add(msg);
if (logCombate.Count > 5) logCombate.RemoveAt(0);
}
static void RenderizarTela()
{
Console.SetCursorPosition(0, 0);
// Topo HUD
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("╔══════════════════════════════════════════════╦════════════════════════════════════╗");
Console.Write("║ ");
Console.ForegroundColor = ConsoleColor.White;
Console.Write($"SETOR SUBTERRÂNEO: {andarAtual:D2}   |   TURNO: {turnos:D4}    ");
Console.ForegroundColor = ConsoleColor.Cyan;
Console.Write("║ ");
Console.ForegroundColor = ConsoleColor.Magenta;
Console.Write($"STATUS: {player.Nome} (NV.{player.Nivel})".PadRight(35));
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("║");
Console.WriteLine("╠══════════════════════════════════════════════╬════════════════════════════════════╣");
// Renderização Linha por Linha (Mapa à esquerda + Painel RPG à direita)
for (int y = 0; y < ALTURA_MAPA; y++)
{
Console.ForegroundColor = ConsoleColor.Cyan;
Console.Write("║");
for (int x = 0; x < LARGURA_MAPA; x++)
{
if (x == player.X && y == player.Y)
{
Console.ForegroundColor = player.Cor;
Console.Write(player.Simbolo);
}
else if (visivel[x, y])
{
Entidade ini = ObterInimigoEm(x, y);
if (ini != null)
{
Console.ForegroundColor = ini.Cor;
Console.Write(ini.Simbolo);
}
else
{
DesenharTile(mapa[x, y], true);
}
}
else if (explorado[x, y])
{
DesenharTile(mapa[x, y], false);
}
else
{
Console.Write(' ');
}
}
Console.ForegroundColor = ConsoleColor.Cyan;
Console.Write("║ ");
DesenharLinhaPainelLateral(y);
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("║");
}
// Rodapé e Log de Combate
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("╠══════════════════════════════════════════════╩════════════════════════════════════╣");
for (int i = 0; i < 5; i++)
{
Console.ForegroundColor = ConsoleColor.Cyan;
Console.Write("║ ");
if (i < logCombate.Count)
{
Console.ForegroundColor = i == logCombate.Count - 1 ? ConsoleColor.White : ConsoleColor.Gray;
string texto = "> " + logCombate[i];
Console.Write(texto.PadRight(81));
}
else
{
Console.Write(new string(' ', 81));
}
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("║");
}
Console.WriteLine("╚═══════════════════════════════════════════════════════════════════════════════════╝");
}
static void DesenharTile(TipoTile tile, bool iluminado)
{
switch (tile)
{
case TipoTile.Parede:
Console.ForegroundColor = iluminado ? ConsoleColor.DarkCyan : ConsoleColor.DarkGray;
Console.Write('█');
break;
case TipoTile.Chao:
Console.ForegroundColor = iluminado ? ConsoleColor.Gray : ConsoleColor.DarkGray;
Console.Write(iluminado ? '·' : ' ');
break;
case TipoTile.Escada:
Console.ForegroundColor = ConsoleColor.Green;
Console.Write('▼');
break;
case TipoTile.Bau:
Console.ForegroundColor = iluminado ? ConsoleColor.Yellow : ConsoleColor.DarkYellow;
Console.Write('■');
break;
case TipoTile.Armadilha:
Console.ForegroundColor = iluminado ? ConsoleColor.DarkRed : ConsoleColor.DarkGray;
Console.Write(iluminado ? '^' : '·');
break;
}
}
static void DesenharLinhaPainelLateral(int linha)
{
string conteudo = "";
ConsoleColor cor = ConsoleColor.White;
switch (linha)
{
case 0:
conteudo = $"HP : {GerarBarra(player.Hp, player.HpMax, 12)} {player.Hp}/{player.HpMax}";
cor = player.Hp < player.HpMax * 0.3 ? ConsoleColor.Red : ConsoleColor.Green;
break;
case 1:
conteudo = $"EN : {GerarBarra(player.Energia, player.EnergiaMax, 12)} {player.Energia}/{player.EnergiaMax}";
cor = ConsoleColor.Cyan;
break;
case 2:
conteudo = $"XP : {GerarBarra(player.Xp, player.XpProximoNivel, 12)} {player.Xp}/{player.XpProximoNivel}";
cor = ConsoleColor.Magenta;
break;
case 3:
conteudo = "──────────────────────────────────";
cor = ConsoleColor.DarkCyan;
break;
case 4:
conteudo = $"ATAQUE : {player.Ataque}   |   DEFESA : {player.Defesa}";
cor = ConsoleColor.Yellow;
break;
case 5:
conteudo = $"CRÉDITOS : ${player.Ouro}";
cor = ConsoleColor.DarkYellow;
break;
case 6:
conteudo = "──────────────────────────────────";
cor = ConsoleColor.DarkCyan;
break;
case 7:
conteudo = $"ARMA: {player.ArmaAtual}";
cor = ConsoleColor.Gray;
break;
case 8:
conteudo = $"TRAJE: {player.ArmaduraAtual}";
cor = ConsoleColor.Gray;
break;
case 9:
conteudo = "──────────────────────────────────";
cor = ConsoleColor.DarkCyan;
break;
case 10:
conteudo = "INVENTÁRIO & HABILIDADES:";
cor = ConsoleColor.Cyan;
break;
case 11:
conteudo = $"[1] Nano-Poção    (x{player.Pocoes})";
cor = ConsoleColor.Green;
break;
case 12:
conteudo = $"[2] Granada PEM   (x{player.Granadas}) [30 EN]";
cor = ConsoleColor.Yellow;
break;
case 13:
conteudo = "[3] Canalizar Energia (+25 EN)";
cor = ConsoleColor.Cyan;
break;
case 14:
conteudo = "──────────────────────────────────";
cor = ConsoleColor.DarkCyan;
break;
case 15:
conteudo = "LEGENDA DO RADAR:";
cor = ConsoleColor.DarkGray;
break;
case 16:
conteudo = "@ Você   ▼ Próximo Setor";
cor = ConsoleColor.White;
break;
case 17:
conteudo = "■ Baú    ^ Mina Laser";
cor = ConsoleColor.Yellow;
break;
case 18:
conteudo = "d Drone  S Sentinela  E Executor";
cor = ConsoleColor.Red;
break;
case 19:
conteudo = "Ω Titã Cibernético (Boss)";
cor = ConsoleColor.Magenta;
break;
default:
conteudo = "";
break;
}
if (conteudo.Length > 34) conteudo = conteudo.Substring(0, 34);
Console.ForegroundColor = cor;
Console.Write(conteudo.PadRight(35));
}
static string GerarBarra(int atual, int max, int tamanho)
{
atual = Math.Max(0, Math.Min(atual, max));
int preenchido = max > 0 ? (int)Math.Round((double)atual / max * tamanho) : 0;
return "[" + new string('█', preenchido) + new string('░', tamanho - preenchido) + "]";
}
static void TelaGameOver()
{
Console.Clear();
Console.ForegroundColor = ConsoleColor.Red;
Console.WriteLine(@"
╔══════════════════════════════════════════════════════════════════════════╗
║                                                                          ║
║        ██████╗  █████╗ ███╗   ███╗███████╗     ██████╗ ██╗   ██╗         ║
║       ██╔════╝ ██╔══██╗████╗ ████║██╔════╝    ██╔═══██╗██║   ██║         ║
║       ██║  ███╗███████║██╔████╔██║█████╗      ██║   ██║██║   ██║         ║
║       ██║   ██║██╔══██║██║╚██╔╝██║██╔══╝      ██║   ██║╚██╗ ██╔╝         ║
║       ╚██████╔╝██║  ██║██║ ╚═╝ ██║███████╗    ╚██████╔╝ ╚████╔╝          ║
║        ╚═════╝ ╚═╝  ╚═╝╚═╝     ╚═╝╚══════╝     ╚═════╝   ╚═══╝           ║
║                      [ SINAL DO AGENTE PERDIDO ]                         ║
╚══════════════════════════════════════════════════════════════════════════╝");
Console.ForegroundColor = ConsoleColor.White;
Console.WriteLine($"\n      • Setor Alcançado : {andarAtual}");
Console.WriteLine($"      • Nível Final     : {player.Nivel}");
Console.WriteLine($"      • Turnos Sobrev.  : {turnos}");
Console.WriteLine($"      • Créditos        : ${player.Ouro}");
Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine("\n      Pressione [ENTER] para encerrar...");
while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }
}
}
}