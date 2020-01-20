using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FifaResults.Entities.Contracts;

namespace FifaResults.Entities
{
    public class Club : IMarkdownProvider
    {
        #region Fields

        private readonly IList<Player> _players;
        private readonly string _name;
        private readonly string _logo;

        #endregion

        #region Properties

        public string Name => _name;
        public string Logo => _logo;
        public IEnumerable<Player> Players => _players;

        public double AverageAge
        {
            get
            {
                double result = 0;

                if (_players.Count > 0)
                {
                    double sum = 0;
                    foreach (Player player in _players)
                    {
                        sum += player.Age;
                    }
                    result = sum / _players.Count;
                }
                return result;
            }
        }

        public double AverageWage
        {
            get
            {
                double result = 0;

                if (_players.Count > 0)
                {
                    double sum = 0;
                    foreach (Player player in _players)
                    {
                        sum += player.Wage;
                    }
                    result = sum / _players.Count;
                }
                return result;
            }
        }

        public long OverallValue
        {
            get
            {
                long result = 0;

                if (_players.Count > 0)
                {
                    foreach (Player player in _players)
                    {
                        result += player.Value;
                    }
                }
                return result;
            }
        }

        #endregion

        #region Constructors

        public Club(string name, string logo)
        {
            _name = name;
            _logo = logo;
            _players = new List<Player>();
        }

        #endregion

        #region Methods

        public void AddPlayer(Player player)
        {
            if (player != null)
            { 
                if (_players.Contains(player) == false)
                {
                    _players.Add(player);
                }
            }
        }


        public string GetMarkdown()
        {
            StringBuilder sb = new StringBuilder();
            //sb.AppendLine("|Name|Players|AverageAge|AverageWage|OverallValue|");
            //sb.AppendLine("|----|------:|---------:|----------:|-----------:|");
            sb.AppendLine($"|{Name}|{Players}|{AverageAge}|{AverageWage}|{OverallValue}");
            return sb.ToString();
        }

        #endregion

    }
}
