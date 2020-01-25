using System;
using System.IO;
using System.Linq;
using System.Text;
using FifaResults.Logic;

namespace FifaResults.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Controller cntr = new Controller();
            cntr.LoadClubs();
            cntr.LoadPlayersForClubs();
            
            Console.WriteLine("Fifa 2019 Results");
            Console.WriteLine($"Es wurden {cntr.Clubs.Count()} Clubs geladen.");

            string markdown = cntr.GetTop10ClubsAsMarkdown();
            WriteContentToFile("top10clubs.md", markdown);
            markdown = cntr.GetLast10ClubsAsMarkdown();
            WriteContentToFile("last10clubs.md", markdown);
            markdown = cntr.GetPlayersUnder100KValueByNationAndValueAsMarkdown();
            WriteContentToFile("players.md", markdown);

        }

        private static void WriteContentToFile(string fileName, string content)
        {
            string fullName = Path.Combine(
                Utils.GetFullFolderNameInApplicationTree("output"), 
                fileName);

            File.WriteAllText(
                fullName,
                content,
                Encoding.UTF8);

            Console.WriteLine($">> Die Datei '{fullName}' wurde aktualisiert!");
        }
    }
}
