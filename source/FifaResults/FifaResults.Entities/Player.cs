using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using FifaResults.Entities.Contracts;

namespace FifaResults.Entities
{
    public class Player : IMarkdownProvider, IComparable<Player>
    {
        #region Fields

        private readonly string _name;
        private readonly string _nationality;
        private readonly int _age;
        private readonly int _value;
        private readonly int _wage;

        #endregion

        #region Properties

        public string Name => _name;
        public string Nationality => _nationality;
        public int Age => _age;
        public int Value => _value;
        public int Wage => _wage;

        #endregion

        #region Constructors

        public Player(string name, string nationality, int age, int value, int wage)
        {
            _name = name;
            _nationality = nationality;
            _age = age;
            _value = value;
            _wage = wage;
        }

        #endregion

        #region Methods

        protected virtual string GetClubAsMarkdown()
        {
            return "-";
        }

        public string GetMarkdown()
        {
            //sb.AppendLine("|Name|Age|Wage|Value|Nationality|Club|");
            //sb.AppendLine("|----|--:|---:|----:|:---------:|:---|");
            StringBuilder sb = new StringBuilder();
            sb.Append($"|{Name}|{Age}|{Wage.ToString("c0", CultureInfo.CreateSpecificCulture("de-DE"))}|{Value.ToString("c0", CultureInfo.CreateSpecificCulture("de-DE"))}|{Nationality}|{GetClubAsMarkdown()}|");

            return sb.ToString();
        }

        public override string ToString()
        {
            return $"{Name}, {Age}, {Wage}, {Value}";
        }

        public int CompareTo(Player other)
        {
            if (other == null)
            {
                return -1;
            }

            int result = 0;

            result = Nationality.CompareTo(other.Nationality);
            if (result == 0)
            {
                result = other.Value.CompareTo(Value);
            }

            return result;
        }

        #endregion
    }
}
