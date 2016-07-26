using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtheloBLL
{
    class HumanPlayer : Player 
    {
        public HumanPlayer(string i_Name, Board.eTokenMarks i_TokenMark)
            : base(i_Name, i_TokenMark)
        {
        }

        public override void GetPlayerMove(
            Board i_CurrentBoard,
            Board.TokenLocation i_SelectedLocation,
            out Board.eTokenMarks[,] o_NewBoardState,
            out int o_NumberOfTokensReplaced)
        {
            o_NewBoardState = null;
            o_NumberOfTokensReplaced = 0;
            i_CurrentBoard.PlaceToken(TokenMark, i_SelectedLocation.RowN, i_SelectedLocation.ColN, out o_NewBoardState, out o_NumberOfTokensReplaced);
            if (o_NumberOfTokensReplaced <= 0 || o_NewBoardState == null)
            {
                throw new ArgumentException("Method was supposed to be called with legal move but was called with invalid token placment");
            }
        }
    }
}
