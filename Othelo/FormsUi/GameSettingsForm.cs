using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Othelo
{
    /// <summary>
    ///   The opening window, used for choosing table size and selecting the type of opponent.
    /// </summary>
    internal class GameSettingsForm : Form
    {
        private const string k_PlayComputerStr = "Play against the computer";
        private const string k_PlayHumanStr = "Play against your friend";

        /// <summary>
        /// This method will be called once, just before the first time the form is displayed
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.InitControls();
        }

        /// <summary>
        /// Uppon clicking on board size - increase board size that will be used when the game will start.
        /// </summary>
        private void m_ButtonBoardSize_Click(object sender, EventArgs e)
        {
            this.m_RequestedBoardSize += this.m_RequestedBoardSize < 14 ? 2 : 0;
            string boardSizeButtonText;
            this.createBoardSizeButtonText(out boardSizeButtonText);
            this.m_ButtonBoardSize.Text = boardSizeButtonText;
        }

        /// <summary>
        /// Uppon start game - check which button was clicked and according to it either start the game vs computer or human.
        /// </summary>
        private void m_ButtonStartGame_Click(object sender, EventArgs e)
        {
            Button buttonClicked = sender as Button;
            this.m_SelectedOpponent = buttonClicked.Text == k_PlayComputerStr ? Player.ePlayerType.Computer : Player.ePlayerType.Human;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        public GameSettingsForm()
        {
            this.Size = new Size(300, 135);
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Othello - Game Settings";
        }

        private void InitControls()
        {
            this.m_RequestedBoardSize = 6;
            string boardSizeButtonText;
            this.createBoardSizeButtonText(out boardSizeButtonText);
            this.m_ButtonBoardSize.Text = boardSizeButtonText;
            this.m_ButtonBoardSize.Location = new Point(8, 17);
            this.m_ButtonBoardSize.Size = new Size(this.Size.Width - (this.m_ButtonBoardSize.Location.X * 2), 30);

            this.m_ButtonPlayAgainstComputer.Text = k_PlayComputerStr;
            this.m_ButtonPlayAgainstComputer.Location = new Point(this.m_ButtonBoardSize.Location.X, this.m_ButtonBoardSize.Bottom + 16);
            this.m_ButtonPlayAgainstComputer.Size = new Size((this.m_ButtonBoardSize.Size.Width / 2) - 8, this.m_ButtonBoardSize.Size.Height);

            this.m_ButtonPlayAgainstHuman.Text = k_PlayHumanStr;
            this.m_ButtonPlayAgainstHuman.Location = new Point(
                this.m_ButtonBoardSize.Right - this.m_ButtonPlayAgainstComputer.Size.Width,
                this.m_ButtonPlayAgainstComputer.Top);
            this.m_ButtonPlayAgainstHuman.Size = this.m_ButtonPlayAgainstComputer.Size;

            this.Controls.AddRange(new Control[] { this.m_ButtonBoardSize, this.m_ButtonPlayAgainstComputer, this.m_ButtonPlayAgainstHuman });

            this.m_ButtonBoardSize.Click += new EventHandler(this.m_ButtonBoardSize_Click);
            this.m_ButtonPlayAgainstComputer.Click += new EventHandler(this.m_ButtonStartGame_Click);
            this.m_ButtonPlayAgainstHuman.Click += new EventHandler(this.m_ButtonStartGame_Click);
        }

        private void createBoardSizeButtonText(out string o_BoardSizeButtonText)
        {
            StringBuilder boardSizeText = new StringBuilder();
            boardSizeText.Append("Board Size: ");
            boardSizeText.Append(this.m_RequestedBoardSize).Append("x").Append(this.m_RequestedBoardSize);
            boardSizeText.Append(" (click to increase)");
            o_BoardSizeButtonText = boardSizeText.ToString();
        }

        public int RequestedBoardSize
        {
            get { return this.m_RequestedBoardSize; }
        }

        public Player.ePlayerType OpponentType
        {
            get { return this.m_SelectedOpponent; }
        }

        private Button m_ButtonBoardSize = new Button();
        private Button m_ButtonPlayAgainstComputer = new Button();
        private Button m_ButtonPlayAgainstHuman = new Button();
        private int m_RequestedBoardSize;
        private Player.ePlayerType m_SelectedOpponent;
    }
}
