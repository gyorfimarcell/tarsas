using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;

namespace Tarsas
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<string> osvenyek = File.ReadAllLines("osvenyek.txt").ToList();
            List<int> dobasok = File.ReadAllLines("dobasok.txt")[0].Split(' ').Select(x => Convert.ToInt32(x)).ToList();

            Console.WriteLine($"2. feladat\nA dobások száma: {dobasok.Count}\nAz ösvények száma: {osvenyek.Count}");

            string leghoszabb = osvenyek.MaxBy(x => x.Length);
            Console.WriteLine($"\n3. feladat\nAz egyik leghosszabb a(z) {osvenyek.IndexOf(leghoszabb) + 1}.ösvény, hossza: {leghoszabb.Length}");

            Console.Write("\n4. feladat\nAdja meg az ösvény sorszámát! ");
            int osvenyIndex = Convert.ToInt32(Console.ReadLine()) - 1;
            Console.Write("Adja meg a játékosok számát! ");
            int jatekosok = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("\n5. feladat");
            osvenyek[osvenyIndex].GroupBy(x => x).ToList().ForEach(x => Console.WriteLine($"{x.Key}: {x.Count()} darab"));

            File.WriteAllLines("kulonleges.txt", osvenyek[osvenyIndex].Select((x, index) => $"{index + 1}\t{x}").Where(x => x.Last() != 'M'));

            Console.WriteLine("\n7. feladat");
            int kor;
            int[] poziciok = Jatek(osvenyek[osvenyIndex], dobasok, jatekosok, false, out kor);
            Console.WriteLine($"A játék a(z) {kor + 1}. körben fejeződött be. A legtávolabb jutó(k) sorszáma: {poziciok.ToList().IndexOf(poziciok.Max()) + 1}");

            Console.WriteLine("\n8.feladat");
            poziciok = Jatek(osvenyek[osvenyIndex], dobasok, jatekosok, true, out kor);
            var eredmenyek = poziciok.Select((x, index) => new { x, index }).ToList();
            Console.WriteLine($"Nyertes(ek): {string.Join(' ', eredmenyek.Where(x => x.x >= osvenyek[osvenyIndex].Length - 1).Select(x => x.index + 1))}");
            eredmenyek.Where(x => x.x < osvenyek[osvenyIndex].Length - 1).ToList().ForEach(x => Console.WriteLine($"{x.index + 1}. játékos, {x.x + 1}. mező"));
        }

        public static int[] Jatek(string osveny, List<int> dobasok, int jatekosok, bool specialisMezok, out int kor)
        {
            int korIndex = 0;
            int jatekosIndex = 0;
            int[] poziciok = new int[jatekosok];
            Array.Fill(poziciok, -1);
            do
            {
                int dobas = dobasok[korIndex * jatekosok + jatekosIndex];

                poziciok[jatekosIndex] += dobas;
                if (specialisMezok && poziciok[jatekosIndex] <= osveny.Length - 1)
                {
                    if (osveny[poziciok[jatekosIndex]] == 'E')
                    {
                        poziciok[jatekosIndex] += dobas;
                    }
                    else if (osveny[poziciok[jatekosIndex]] == 'V')
                    {
                        poziciok[jatekosIndex] -= dobas;
                    }
                }

                if (jatekosIndex == jatekosok - 1)
                {
                    if (poziciok.Any(x => x >= osveny.Length - 1))
                    {
                        kor = korIndex;
                        return poziciok;
                    }

                    jatekosIndex = 0;
                    korIndex++;
                }
                else
                {
                    jatekosIndex++;
                }
            } while (true);
        }
    }
}