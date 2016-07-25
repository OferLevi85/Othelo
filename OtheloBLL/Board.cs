using System;
using System.Collections.Generic;
using System.Text;

namespace OtheloBLL
{
    /// <summary>
    ///   This class controls the game board. GameEngine decideds on moves and board makes them.
    /// </summary>
    public sealed class Board
    {
        /// <summary>
        ///   Definitions
        /// </summary>
        public enum eTokenMarks
        {
            Blank,
            PlayerOneToken,
            PlayerTwoToken
        }

        private enum eDirectionFromToken
        {
            Up,
            UpRight,
            Right,
            DownRight,
            Down,
            DownLeft,
            Left,
            UpLeft
        }

        public struct TokenLocation
        {
            public TokenLocation(int i_RowN, int i_ColN)
            {
                ColN = i_ColN;
                RowN = i_RowN;
            }

            public readonly int ColN;
            public readonly int RowN;
        }

        /// <summary>
        ///   Methods
        /// </summary>
   
        /// <remarks>
        ///   Note - if o_NumberOfTokensReplaced == 0 and o_NewBoardState is null than placing a token will fail and a new token selection will be initiated (if existing)
        /// </remarks>
        public void TryPlacingATokenOnBoard(eTokenMarks i_Token, int i_Row, int i_Col, out eTokenMarks[,] o_NewBoardState, out int o_NumberOfTokensReplaced)
        {
            if (i_Row >= BoardSize || i_Row >= BoardSize || CurrentTableState[i_Row, i_Col] != eTokenMarks.Blank)
            {
                throw new System.InvalidOperationException("Ileagal token location");
            }

            o_NumberOfTokensReplaced = 0;
            o_NewBoardState = new eTokenMarks[BoardSize, BoardSize];
            o_NewBoardState = CurrentTableState;
            foreach (eDirectionFromToken direction in Enum.GetValues(typeof(eDirectionFromToken)))
            {
                int numberOfTokenReplacedForDirection = 0;
                eTokenMarks[,] boardWithDirectionUpdated = null;

                performTokensReplacementForDirection(i_Token, direction, i_Row, i_Col, o_NewBoardState, out boardWithDirectionUpdated, out numberOfTokenReplacedForDirection);
                if (numberOfTokenReplacedForDirection > 0 && boardWithDirectionUpdated != null)
                {
                    o_NewBoardState = boardWithDirectionUpdated;
                    o_NumberOfTokensReplaced += numberOfTokenReplacedForDirection;
                }
            }
        }

        /// <summary>
        ///   Switch token for a specific direction. Note that a direction might not perform any switches and yet the move will succeed 
        ///   (other directions will have tokens switched)
        /// </summary>
        private void performTokensReplacementForDirection(
            eTokenMarks i_Token,
            eDirectionFromToken i_Direction,
            int i_Row,
            int i_Col,
            eTokenMarks[,] i_PreviousBoardState,
            out eTokenMarks[,] o_NewBoardState,
            out int o_NumberOfTokenReplaced)
        {
            o_NewBoardState = null; // This will not be changed unless switch in direction is legal. If it is, the temp board state will be copied here. 
            o_NumberOfTokenReplaced = 0; // Same as above. Don't want to touch output parameters until I know that the move is legal.

            eTokenMarks[,] boardStateWithUpdatedDirection = new eTokenMarks[BoardSize, BoardSize];
            Array.Copy(i_PreviousBoardState, boardStateWithUpdatedDirection, i_PreviousBoardState.Length);
            boardStateWithUpdatedDirection[i_Row, i_Col] = i_Token;

            int possibleNumberOfTokensReplaced = 0;
            bool reachedBorder;
            eTokenMarks previousTokenValue = eTokenMarks.Blank;
            int row = i_Row;
            int col = i_Col;
            do
            {
                advanceInMatrix(i_Direction, ref row, ref col, out reachedBorder);
                if (!reachedBorder)
                {
                    previousTokenValue = boardStateWithUpdatedDirection[row, col];
                    boardStateWithUpdatedDirection[row, col] = i_Token;
                }
                ++possibleNumberOfTokensReplaced;
            }
            while (previousTokenValue != i_Token && previousTokenValue != eTokenMarks.Blank && !reachedBorder);

            if (!reachedBorder && previousTokenValue != eTokenMarks.Blank && possibleNumberOfTokensReplaced > 1)
            {
                o_NumberOfTokenReplaced = possibleNumberOfTokensReplaced;
                o_NewBoardState = boardStateWithUpdatedDirection;
            }
        }

        /// <summary>
        ///   Returns current score
        /// </summary>
        public void GetTokensBalance(out int o_NumberOfCircles, out int o_NumberOfCrosses)
        {
            o_NumberOfCircles = 0;
            o_NumberOfCrosses = 0;

            foreach (eTokenMarks token in CurrentTableState)
            {
                o_NumberOfCircles += (token == eTokenMarks.PlayerTwoToken) ? 1 : 0;
                o_NumberOfCrosses += (token == eTokenMarks.PlayerOneToken) ? 1 : 0;
            }
        }

        private void advanceInMatrix(eDirectionFromToken i_Direction, ref int io_CurrentRow, ref int io_CurrentCol, out bool o_ReachedEnd)
        {
            o_ReachedEnd = false;
            int currentRow = io_CurrentRow;
            int currentCol = io_CurrentCol;

            switch (i_Direction)
            {
                case eDirectionFromToken.Up:
                    o_ReachedEnd = --currentRow < 0;
                    break;
                case eDirectionFromToken.UpRight:
                    o_ReachedEnd = --currentRow < 0 || ++currentCol >= BoardSize;
                    break;
                case eDirectionFromToken.Right:
                    o_ReachedEnd = ++currentCol >= BoardSize;
                    break;
                case eDirectionFromToken.DownRight:
                    o_ReachedEnd = ++currentRow >= BoardSize || ++currentCol >= BoardSize;
                    break;
                case eDirectionFromToken.Down:
                    o_ReachedEnd = ++currentRow >= BoardSize;
                    break;
                case eDirectionFromToken.DownLeft:
                    o_ReachedEnd = ++currentRow >= BoardSize || --currentCol < 0;
                    break;
                case eDirectionFromToken.Left:
                    o_ReachedEnd = --currentCol < 0;
                    break;
                case eDirectionFromToken.UpLeft:
                    o_ReachedEnd = --currentCol < 0 || --currentRow < 0;
                    break;
            }

            if (!o_ReachedEnd)
            {
                io_CurrentRow = currentRow;
                io_CurrentCol = currentCol;
            }
        }

        public int BoardSize
        {
            // Note: table is always a square so number of rows == number of columns
            get { return CurrentTableState.GetLength(0); }
        }

        public eTokenMarks this[int i_Row, int i_Col]
        {
            get { return CurrentTableState[i_Row, i_Col]; }
        }

        /// <remarks>
        ///   At board creation, puts 2 circles and 2 squares in the middle
        /// </remarks>
        public Board(int i_SizeOfTable)
        {
            CurrentTableState = new eTokenMarks[i_SizeOfTable, i_SizeOfTable];
            CurrentTableState[(i_SizeOfTable / 2) - 1, (i_SizeOfTable / 2) - 1] = eTokenMarks.PlayerTwoToken;
            CurrentTableState[(i_SizeOfTable / 2) - 1, (i_SizeOfTable / 2)] = eTokenMarks.PlayerOneToken;
            CurrentTableState[(i_SizeOfTable / 2), (i_SizeOfTable / 2) - 1] = eTokenMarks.PlayerOneToken;
            CurrentTableState[(i_SizeOfTable / 2), (i_SizeOfTable / 2)] = eTokenMarks.PlayerTwoToken;
        }

        public eTokenMarks[,] CurrentTableState { get; set; }
    }
}
