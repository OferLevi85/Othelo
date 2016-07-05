using System;
using System.Collections.Generic;

namespace Othelo
{
    /// <summary>
    ///   This is the game's brain. It has full control on the game logic and AI
    /// </summary>
    internal sealed class GameEngine
    {
        /// <summary>
        ///   Definitions
        /// </summary>
        public static Random s_RandMechanisem = new Random(); // In order for true randomness to occur, don't create instances of this class. Use it as is through the program life.

        public enum eGetInputFromUser
        {
            Row,
            Column
        }

        public GameEngine(int i_TableSize)
        {
            m_Board = new Board(i_TableSize); 
        }

        /// <summary>
        ///   Methods for managing moves in board
        /// </summary>
        /// <remarks>
        ///   Used for receiving the best possible move but also returns all legal moves a player can make.
        /// </remarks>
        /// <param name="o_AvailableMoves"> Used for receiving all available moves for player (so UI can later display his options)</param>
        public void GetBestPossibleMove(
            Board.eTokenMarks i_Token,
            out Board.eTokenMarks[,] o_BoardStateInBestMove,
            out List<Board.TokenLocation> o_AvailableMoves)
        {
            o_BoardStateInBestMove = null;
            int numberOfTokensReplacedInBestMove = 0;
            o_AvailableMoves = new List<Board.TokenLocation>();
            for (int rowN = 0; rowN < m_Board.BoardSize; ++rowN)
            {
                for (int colN = 0; colN < m_Board.BoardSize; ++colN)
                {
                    if (m_Board[rowN, colN] == Board.eTokenMarks.Blank)
                    {
                        int numberOfTokensReplaced;
                        Board.eTokenMarks[,] boardStateAfterPlacingToken;
                        m_Board.TryPlacingATokenOnBoard(i_Token, rowN, colN, out boardStateAfterPlacingToken, out numberOfTokensReplaced);
                        if (numberOfTokensReplaced > 0)
                        {
                            o_AvailableMoves.Add(new Board.TokenLocation(rowN, colN));
                        }

                        // If option is better, update to current. If it is equally better to previous, select one of them at random
                        // (chances are not truely equal for each option, the last option has a better chance of being selected as it is only
                        // evaluated once. AI and Random mechanisem might be updated in later versions)
                        if (numberOfTokensReplaced > numberOfTokensReplacedInBestMove ||
                            (numberOfTokensReplaced == numberOfTokensReplacedInBestMove && numberOfTokensReplaced > 0 && s_RandMechanisem.Next() % 2 == 0))
                        {
                            o_BoardStateInBestMove = new Board.eTokenMarks[m_Board.BoardSize, m_Board.BoardSize];
                            Array.Copy(boardStateAfterPlacingToken, o_BoardStateInBestMove, boardStateAfterPlacingToken.Length);
                            numberOfTokensReplacedInBestMove = numberOfTokensReplaced;
                        }
                    }
                }
            }

            if (numberOfTokensReplacedInBestMove == 0)
            {
                o_AvailableMoves = null;
            }
        }

        public void UpdateCurrentBoardState(Board.eTokenMarks[,] i_NewBoardState)
        {
            if (i_NewBoardState == null || i_NewBoardState.GetLength(0) != m_Board.BoardSize)
            {
                throw new ArgumentException("Input board state is null or in incorrect size");
            }

            m_Board.updateCurrentBoardState(i_NewBoardState);
        }

        // Methods for interaction with human player
        public void GetHumanPlayerMoveSelection(
            Player i_HumanPlayer,
            Board.TokenLocation i_SelectedLocation,
            out Board.eTokenMarks[,] o_NewBoardState,
            out int o_NumberOfTokensReplaced)
        {
            if (i_HumanPlayer.PlayerType != Player.ePlayerType.Human)
            {
                throw new System.InvalidOperationException("This method can only be called when refering to a human player");
            }

            o_NewBoardState = null;
            o_NumberOfTokensReplaced = 0;
            m_Board.TryPlacingATokenOnBoard(i_HumanPlayer.TokenMark, i_SelectedLocation.RowN, i_SelectedLocation.ColN, out o_NewBoardState, out o_NumberOfTokensReplaced);
            if (o_NumberOfTokensReplaced <= 0 || o_NewBoardState == null)
            {
                throw new ArgumentException("Method was supposed to be called with legal move but was called with invalid token placment");
            }
        }

        public void GetCurrentScore(Player i_PlayerOne, Player i_PlayerTwo, out int i_PlayerOneScore, out int i_PlayerTwoScore)
        {
            int numberOfCircles;
            int numberOfCrosses;
            m_Board.GetTokensBalance(out numberOfCircles, out numberOfCrosses);

            i_PlayerOneScore = i_PlayerOne.TokenMark == Board.eTokenMarks.PlayerTwoToken ? numberOfCircles : numberOfCrosses;
            i_PlayerTwoScore = i_PlayerTwo.TokenMark == Board.eTokenMarks.PlayerTwoToken ? numberOfCircles : numberOfCrosses;
        }

        public Board.eTokenMarks[,] CurrentTableState
        {
            get { return m_Board.CurrentTableState; }
        }

        // Members
        private Board m_Board = null;
    }
}
