﻿using System;
using System.Collections.Generic;
using System.Text;

namespace FifaResults.Entities
{
    public class PlayerWithClub : Player
    {
        #region Fields

        private readonly Club _club;

        #endregion

        #region Properties

        public Club Club => _club;

        #endregion

        #region Constructors

        public PlayerWithClub(
            string name, 
            string nationality, 
            int age, 
            int value, 
            int wage,
            Club club) : base(name, nationality, age, value, wage)
        {
            _club = club;
        }

        #endregion

        #region Methods

        protected override string GetClubAsMarkdown()
        {
            return $"{Club.GetMarkdown()}";
        }

        #endregion
    }
}
