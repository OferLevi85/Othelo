using System;
using System.Collections.Generic;
using System.Text;

namespace OtheloBLL
{
    /// <summary>
    ///   This struct is used to keep player records. No logic is used in these structs because GameEngine controlls AI and Human decides on moves by himself.
    /// </summary>
    public abstract class Player
    {
        /// <summary>
        ///   Methods
        /// </summary>
        public Player(string i_Name, Board.eTokenMarks i_TokenMark)
        {
            PlayerName = i_Name;
            TokenMark = i_TokenMark;
            TotalGamesScore = 0;
        }

        public readonly string PlayerName;
        public readonly Board.eTokenMarks TokenMark;
        public int TotalGamesScore { get; set; }

        public abstract void GetPlayerMove(
            Board i_CurrentBoard,
            Board.TokenLocation i_SelectedLocation,
            out Board.eTokenMarks[,] o_NewBoardState,
            out int o_NumberOfTokensReplaced);
    }
}
