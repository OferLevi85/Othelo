using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using OtheloBLL;
using System.Threading;

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

            this.InitializeComponent();
            this.InitCustomLayout();
        }

        private void InitializeComponent()
        {
            this.PlayerTurn = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // PlayerTurn
            // 
            this.PlayerTurn.AutoSize = true;
            this.PlayerTurn.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.PlayerTurn.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.PlayerTurn.Location = new System.Drawing.Point(12, 233);
            this.PlayerTurn.Name = "PlayerTurn";
            this.PlayerTurn.Size = new System.Drawing.Size(95, 20);
            this.PlayerTurn.TabIndex = 0;
            this.PlayerTurn.Text = "Player\'s turn";
            // 
            // BoardForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.PlayerTurn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "BoardForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Othello";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void InitCustomLayout()
        {
            foreach (TokenButton button in this.m_TokenButtons)
            {
                this.Controls.Add(button);
            }

            this.ClientSize = new Size(m_TotalSize, m_TotalSize + 100);
            this.PlayerTurn.Location = new System.Drawing.Point(12, m_TotalSize + 40);
            this.PlayerTurn.Text = m_CurrentPlayerName + "'s turn!";
        }

        public BoardForm(int i_BoardSize)
        {
            int gameBoardXAxis = 0;
            int gameBoardYAxis = 0;
            m_TotalSize = 0; // The board is a cube so no need to diffrentiate between height and width
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

                m_TotalSize += this.m_TokenButtons[rowN, 0].Size.Width + 1;
            }

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Othello";
        }

        //public void DrawPlayerNameAnimation(object obj)
        //{
        //    Animation.Animate(this.PlayerTurn, Animation.Effect.Roll, 3000, 360);
        //}

        public void PlayerChanged(object sender, Othelo.PlayerChangedEventArgs e)
        {
            m_CurrentPlayerName = e.CurrentPlayer.PlayerName;
            if (this.PlayerTurn != null)
            {
                if (this.PlayerTurn.Visible)
                {
                    Animation.Animate(this.PlayerTurn, Animation.Effect.Roll, 150, 360);
                }

                if (e.HasMoves)
                {
                    this.PlayerTurn.Text = m_CurrentPlayerName + "'s turn!";
                }
                else
                {
                    this.PlayerTurn.Text = m_CurrentPlayerName + " dosen't have available moves! turn will pass!";
                }

                //Thread AnimationThread = new Thread(new ParameterizedThreadStart(DrawPlayerNameAnimation));
                //AnimationThread.Start();
                //AnimationThread.Join();
                Animation.Animate(this.PlayerTurn, Animation.Effect.Roll, 150, 360);
            }    
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

        private Label PlayerTurn;
        private int m_TotalSize = 0;
        private string m_CurrentPlayerName;
    }
}
