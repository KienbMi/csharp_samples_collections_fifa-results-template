using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using FifaResults.Entities;
using FifaResults.Entities.Comparer;

namespace FifaResults.Logic
{
    public class Controller
    {
        #region Constants

        private const string FileName = "data.csv";
        private const char SplitSeparator = ';';

        #endregion

        #region Fields

        private List<Club> _clubs;
        private List<Player> _players;
        private IDictionary<string, Club> _clubsCache;
        private string[] _lines;

        #endregion

        #region Propertise

        public IEnumerable<Club> Clubs => _clubs;
        public IEnumerable<Player> Players => _players;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialisiert den Controller:
        ///  - Liest die Datei per Hilfsmethode "ReadLinesFromFile()" in die interne "_lines"-Struktur
        ///  - Instanziert den "_clubsCache"
        /// </summary>
        public Controller()
        {

        }

        #endregion

        /// <summary>
        /// Lade alle Clubs ohne die Player!
        ///
        /// Die Clubs sind in der List "_clubs" sortiert nach dem Name aufsteigend zu sortieren!
        /// </summary>
        public void LoadClubs()
        {
            ReadLinesFromFile();

            if (_lines == null)
            {
                throw new Exception("File does not exists or is empty");
            }

            _clubs = new List<Club>();
            _clubsCache = new Dictionary<string, Club>();

            bool ignoreFirstLine = true;

            foreach (string line in _lines)
            {
                //0;158023;L. Messi;31;Argentina;FC Barcelona; https://cdn.sofifa.org/teams/2/light/241.png; €110.5M;€565K;Left;RF;5'7;159lbs
                if (ignoreFirstLine == false)
                {
                    string[] data = line.Split(SplitSeparator);
                    if (data.Length >= 7)
                    {
                        string clubname = data[5];
                        string logo = data[6];

                        if (string.IsNullOrEmpty(clubname) == false &&
                            _clubsCache.ContainsKey(clubname) == false)
                        {
                            Club club = new Club(
                                clubname,
                                logo);

                            _clubs.Add(club);
                            _clubsCache.Add(clubname, club);
                        }
                    }
                }
                ignoreFirstLine = false;
            }
        }

        /// <summary>
        /// Lädt die Player zu den zuvor geladenen Clubs.
        /// </summary>
        public void LoadPlayersForClubs()
        {
            if (_clubsCache  == null)
            {
                throw new InvalidOperationException("Call to LoadPlayersForClubs() has to be performed after LoadClubs()");
            }
            
            ReadLinesFromFile();

            if (_lines == null)
            {
                throw new Exception("File does not exists or is empty");
            }

            _players = new List<Player>();
            bool ignoreFirstLine = true;

            foreach (string line in _lines)
            {
                //Nr;ID;Name;Age;Nationality;Club;Club Logo;Value;Wage;Preferred Foot;Position;Height;Weight
                //0;158023;L. Messi;31;Argentina;FC Barcelona; https://cdn.sofifa.org/teams/2/light/241.png; €110.5M;€565K;Left;RF;5'7;159lbs
                if (ignoreFirstLine == false)
                {
                    string[] data = line.Split(SplitSeparator);
                    if (data.Length >= 12)
                    {
                        string playerName = data[2];
                        int age = int.Parse(data[3]);
                        string nationality = data[4];
                        string clubName = data[5];
                        string clubLogo = data[6];
                        int value = 0;
                        int wage = 0;
                        string preferedFoot = data[9];
                        string position = data[10];
                        double height = 0;
                        double weight = 0;

                        try
                        {
                            value = ParseCurrency(data[7]);
                            wage = ParseCurrency(data[8]);
                            height = ParseHeightToMetric(data[11]);
                            weight = ParseWeightToMetric(data[12]);
                        }
                        catch
                        {
                            // skip line if not valid
                            //break;
                        }

                        Club club;

                        if (_clubsCache.TryGetValue(clubName, out club))
                        {
                            PlayerWithClub player = new PlayerWithClub(
                                playerName,
                                nationality,
                                age,
                                value,
                                wage,
                                club);

                            _players.Add(player);
                            club.AddPlayer(player);
                        }
                        else
                        {
                            Player player = new Player(
                                playerName,
                                nationality,
                                age,
                                value,
                                wage);

                            _players.Add(player);
                        }
                    }
                }
                ignoreFirstLine = false;
            }
        }

        public string GetTop10ClubsAsMarkdown()
        {
            if (_clubs == null)
            {
                throw new InvalidOperationException("Clubliste ist leer");
            }

            _clubs.Sort(new SortByOverallValueDesc());

            StringBuilder sb = new StringBuilder(GenerateClubMarkdownHeader());

            for (int i = 0; i < 10 && i < _clubs.Count; i++)
            {
                sb.AppendLine(_clubs[i].GetMarkdown());
            }
            return sb.ToString();
        }

        public string GetLast10ClubsAsMarkdown()
        {
            if (_clubs == null)
            {
                throw new InvalidOperationException("Clubliste ist leer");
            }

            _clubs.Sort(new SortByOverallValueAsc());
                       
            StringBuilder sb = new StringBuilder(GenerateClubMarkdownHeader());

            for (int i = 0; i < 10 && i < _clubs.Count; i++)
            {
                sb.AppendLine(_clubs[i].GetMarkdown());
            }
            return sb.ToString();
        }


        /// <summary>
        /// Liefert alle Fifa Player mit einem Wert unter € 100.000 sortiert nach der Nationalität (aufsteigend)
        /// und dem Value (absteigend).
        /// </summary>
        /// <returns></returns>
        public string GetPlayersUnder100KValueByNationAndValueAsMarkdown()
        {

            StringBuilder sb = new StringBuilder(GeneratePlayerMarkdownHeader());

            _players.Sort();
            foreach (Player player in _players)
            {
                if (player.Value < 100000)
                {
                    sb.AppendLine(player.GetMarkdown());
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Liefert den Markdown Header für die "Player"-Ausgabe.
        /// </summary>
        private string GeneratePlayerMarkdownHeader()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("|Name|Age|Wage|Value|Nationality|Club|");
            sb.AppendLine("|----|--:|---:|----:|:---------:|:---|");

            return sb.ToString();
        }

        /// <summary>
        /// Liefert den Markdown Header für die "Club"-Ausgabe.
        /// </summary>
        private string GenerateClubMarkdownHeader()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("|Name|Players|AverageAge|AverageWage|OverallValue|");
            sb.AppendLine("|----|------:|---------:|----------:|-----------:|");

            return sb.ToString();
        }

        /// <summary>
        /// Dies Hilfsmethode liest die Zeilen aus der Input-Datei
        /// in den internen Cache "_lines" ein.
        /// </summary>
        private void ReadLinesFromFile()
        {
            _lines = File.ReadAllLines(
                Utils.GetFullNameInApplicationTree(FileName),
                Encoding.UTF8);
        }

        /// <summary>
        /// Wandelt eine Währung als Zeichenkette in einen Integer um.
        /// </summary>
        public static int ParseCurrency(string currencyString)
        {
            if (currencyString == null)
            {
                throw new ArgumentNullException(nameof(currencyString));
            }

            int result = 0;

            //€110.5M
            int PosAfterPoint = -1;
            int factor = 1;

            foreach (char character in currencyString)
            {
                if (char.IsDigit(character))
                {
                    result *= 10;
                    result += character - '0';

                    if (PosAfterPoint >= 0)
                    {
                        PosAfterPoint++;
                    }

                }
                else if (character == '.')
                {
                    PosAfterPoint = 0;
                }
                else if (character == 'K')
                {
                    factor = 1000;
                }
                else if (character == 'M')
                {
                    factor = 1000000;
                }
            }

            result *= factor;
            if (PosAfterPoint >= 1)
            {
                result /= (PosAfterPoint * 10);
            }

            return result;
        }

        /// <summary>
        /// Wandelt ein Gewicht als Zeichenkette in einen Double (metrisch) um.
        /// Das Ergebnis wird auf 2 Nachkommastellen gerundet!
        /// Die notwendige Formel finden Sie in den Hinweisen.
        /// </summary>
        public static double ParseWeightToMetric(string weightString)
        {
            // 159lbs
            if (string.IsNullOrEmpty(weightString))
            {
                throw new ArgumentNullException(nameof(weightString));
            }

            double result = 0;

            foreach (char character in weightString)
            {
                if (char.IsDigit(character))
                {
                    result *= 10;
                    result += character - '0';
                }
            }

            return Math.Round(result *= 0.45359237, 2);
        }

        /// <summary>
        /// Wandelt eine Körpergröße als Zeichenkette in einen Double (metrisch) um.
        /// Das Ergebnis wird auf 2 Nachkommastellen gerundet!
        /// Die notwendige Formel finden Sie in den Hinweisen.
        /// </summary>
        public static double ParseHeightToMetric(string heightString)
        {
            // 5'7
            if (string.IsNullOrEmpty(heightString))
            {
                throw new ArgumentNullException(nameof(heightString));
            }

            int zoll = 0;
            int inches = 0;
            bool zollSignFound = false;

            foreach (char character in heightString)
            {
                if (char.IsDigit(character) && !zollSignFound)
                {
                    zoll *= 10;
                    zoll += character - '0';
                }
                else if (character == '\'')
                {
                    zollSignFound = true;
                }
                else if(char.IsDigit(character))
                {
                    inches *= 10;
                    inches += character - '0';
                }
            }

            return Math.Round((zoll * 12 + inches) * 2.54, 2);
        }
    }

}
