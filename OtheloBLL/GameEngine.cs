using System;
using System.Collections.Generic;

namespace OtheloBLL
{
    /// <summary>
    ///   This is the game's brain. It has full control on the game logic and AI
    /// </summary>
    public sealed class GameEngine
    {
        /// <summary>
        ///   Definitions
        /// </summary>

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
            int numberOfTokensReplacedInBestMove = 0;
            o_BoardStateInBestMove = null;
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

                        if (MoveIsBetter(numberOfTokensReplaced, numberOfTokensReplacedInBestMove))
                        {
                            o_BoardStateInBestMove = boardStateAfterPlacingToken;
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

        // If option is equally better to previous, select one of them at random
        // (chances are not truely equal for each option, the last option has a better chance of being selected as it is only
        // evaluated once. AI and Random mechanisem might be updated in later versions)
        private bool MoveIsBetter(int numberOfTokensReplaced, int numberOfTokensReplacedInBestMove)
        {
            return  (
                        (numberOfTokensReplaced > numberOfTokensReplacedInBestMove) ||
                        (
                            (numberOfTokensReplaced == numberOfTokensReplacedInBestMove) &&
                            (numberOfTokensReplaced > 0) &&
                            (RandomDecision.GetRandomNumber(0, 1) == 0)
                        )
                    );
        }

        public void UpdateCurrentBoardState(Board.eTokenMarks[,] i_NewBoardState)
        {
            m_Board.CurrentTableState = i_NewBoardState;
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

            if (i_PlayerOne.TokenMark == Board.eTokenMarks.PlayerTwoToken)
            {
                i_PlayerOneScore = numberOfCircles;
                i_PlayerTwoScore = numberOfCrosses;
            }
            else
            {
                i_PlayerOneScore = numberOfCrosses;
                i_PlayerTwoScore = numberOfCircles;
            }
        }

        public Board.eTokenMarks[,] CurrentTableState
        {
            get { return m_Board.CurrentTableState; }
        }

        // Members
        private Board m_Board = null;
    }
}
