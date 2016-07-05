using System;
using System.Collections.Generic;
using System.Text;

namespace Othelo
{
    /// <summary>
    ///   This struct is used to keep player records. No logic is used in these structs because GameEngine controlls AI and Human decides on moves by himself.
    /// </summary>
    internal struct Player
    {
        public enum ePlayerType
        {
            Human = 1,
            Computer
        }

        /// <summary>
        ///   Methods
        /// </summary>
        public Player(string i_Name, ePlayerType i_Type, Board.eTokenMarks i_TokenMark)
        {
            m_Name = i_Name;
            m_Type = i_Type;
            m_TokenMark = i_TokenMark;
            m_Score = 0;
        }

        public string PlayerName
        {
            get { return m_Name; }
        }

        public ePlayerType PlayerType
        {
            get { return m_Type; }
        }

        public Board.eTokenMarks TokenMark
        {
            get { return m_TokenMark; }
        }

        public int Score
        {
            get { return m_Score; }
            set { m_Score = value; }
        }

        /// <summary>
        ///   Members
        /// </summary>
        private readonly string m_Name;
        private readonly ePlayerType m_Type;
        private readonly Board.eTokenMarks m_TokenMark;
        private int m_Score;
    }
}
