using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using OtheloBLL;

namespace OtheloUI
{
    /// <summary>
    ///   This class is the graphic representation of the game board
    /// </summary>
    internal class BoardForm : Form
    {
        public event TokenClickEventHandler PlayerSelected;

        /// <summary>
        ///   If a token button was clicked, nothing to do but notify it. Othelo logic will listen and handle it from there.
        /// </summary>
        private void token_Click_EventHandler(object sender, TokenButton.TokenClickedEventArgs e)
        {
            if (this.PlayerSelected != null)
            {
                this.PlayerSelected(this, e);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.InitControls();
        }

        private void InitControls()
        {
            foreach (TokenButton button in this.m_TokenButtons)
            {
                this.Controls.Add(button);
            }
        }

        public BoardForm(int i_BoardSize)
        {
            int gameBoardXAxis = 0;
            int gameBoardYAxis = 0;
            int totalSize = 0; // The board is a cube so no need to diffrentiate between height and width
            this.m_TokenButtons = new TokenButton[i_BoardSize, i_BoardSize];

            for (int rowN = 0; rowN < i_BoardSize; ++rowN)
            {
                for (int colN = 0; colN < i_BoardSize; ++colN)
                {
                    this.m_TokenButtons[rowN, colN] = new TokenButton(new Board.TokenLocation(rowN, colN));
                    this.m_TokenButtons[rowN, colN].ClickOccured += new TokenClickEventHandler(this.token_Click_EventHandler);
                    this.m_TokenButtons[rowN, colN].Location = new Point(gameBoardXAxis, gameBoardYAxis);

                    if (colN + 1 == i_BoardSize)
                    {
                        gameBoardXAxis = 0;
                        gameBoardYAxis += this.m_TokenButtons[rowN, colN].Size.Height + 1;
                    }
                    else
                    {
                        gameBoardXAxis += this.m_TokenButtons[rowN, colN].Size.Width + 1;
                    }
                }

                totalSize += this.m_TokenButtons[rowN, 0].Size.Width + 1;
            }

            this.ClientSize = new Size(totalSize, totalSize);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Othello";
        }

        /// <summary>
        ///   Board will be updated (without available moves which will be added later if needed. They are not part of the
        ///   table state as they differ according to the current player).
        /// </summary>
        public void RedrawBoard(Board.eTokenMarks[,] i_BoardState)
        {
            if (this.m_TokenButtons.LongLength != i_BoardState.LongLength)
            {
                throw new ArgumentOutOfRangeException("i_BoardState", "i_BoardState matrix size is different than UI's m_TokenButtons");
            }

            for (int rowN = 0; rowN < i_BoardState.GetLength(0); ++rowN)
            {
                for (int colN = 0; colN < i_BoardState.GetLength(0); ++colN)
                {
                    this.m_TokenButtons[rowN, colN].SetButtonStatus(i_BoardState[rowN, colN]);
                }
            }
        }

        /// <summary>
        ///   Display available moves for the human player.
        /// </summary>
        /// <param name="i_SquareLocation"></param>
        public void SetAvailableMoveToken(Board.TokenLocation i_SquareLocation)
        {
            this.m_TokenButtons[i_SquareLocation.RowN, i_SquareLocation.ColN].SetAvailableMove();
        }

        private TokenButton[,] m_TokenButtons;
    }
}
