using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtheloBLL
{
    class ComputerPlayer : Player
    {
        public ComputerPlayer(string i_Name, Board.eTokenMarks i_TokenMark)
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
        }
    }
}
