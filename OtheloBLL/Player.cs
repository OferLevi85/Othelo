using System;
using System.Collections.Generic;
using System.Text;

namespace OtheloBLL
{
    /// <summary>
    ///   This struct is used to keep player records. No logic is used in these structs because GameEngine controlls AI and Human decides on moves by himself.
    /// </summary>
    public struct Player
    {
        public enum ePlayerType
        {
            Human = 1,
            Computer
        }

        /// <summary>
        ///   Methods
        /// </summary>
        public Player(string i_Name, ePlayerType i_Type, Board.eTokenMarks i_TokenMark) : this()
        {
            PlayerName = i_Name;
            PlayerType = i_Type;
            TokenMark = i_TokenMark;
            TotalGamesScore = 0;
        }

        public readonly string PlayerName;
        public readonly ePlayerType PlayerType;
        public readonly Board.eTokenMarks TokenMark;
        public int TotalGamesScore { get; set; }
    }
}
