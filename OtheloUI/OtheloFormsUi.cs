﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using OtheloBLL;

namespace OtheloUI
{
    /// <summary>
    ///   This is the main class of the Windows Forms Othelo UI. It also the only class that interact directly with Othelo Logic.
    /// </summary>
    internal class OtheloFormsUi: IDisposable
    {
        private void player_Selected_EventHandler(object sender, TokenButton.TokenClickedEventArgs e)
        {
            m_OtheloGame.PlayHumanRound(e.TokenLocation);
        }

        private void m_OtheloGame_BoardChanged(object sender, Othelo.BoardChangedEventArgs e)
        {
            m_BoardUiForm.RedrawBoard(e.BoardState);
            if (e.AvailableMoves != null)
            {
                foreach (Board.TokenLocation squareLocation in e.AvailableMoves)
                {
                    m_BoardUiForm.SetAvailableMoveToken(squareLocation);
                }
            }
        }

        private void m_OtheloGame_GameFinished(object sender, Othelo.GameFinishedEventArgs e)
        {
            string gameFinishedMessage;
            getGameFinishedWinnerMessage(out gameFinishedMessage, e);
            DialogResult newGameResult = MessageBox.Show(gameFinishedMessage, "Othello", MessageBoxButtons.YesNo);
            if (newGameResult == DialogResult.Yes)
            {
                m_OtheloGame.StartNewGame();
            }
            else if (newGameResult == DialogResult.No)
            {
                m_BoardUiForm.Close();
            }
        }

        [STAThread]
        public static void Main()
        {
            // Catch errors and display them as message instead of debug window.
            try
            {
                OtheloFormsUi otheloFormsUi = new OtheloFormsUi();
                otheloFormsUi.StartOtheloFormsGame();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void StartOtheloFormsGame()
        {
            GameSettingsForm gameSettingsForm = new GameSettingsForm();
            DialogResult gameSettingsWindowResult = gameSettingsForm.ShowDialog();
            if (gameSettingsWindowResult != DialogResult.Cancel)
            {
                m_BoardUiForm = new BoardForm(gameSettingsForm.RequestedBoardSize);

                m_OtheloGame = new Othelo();
                m_OtheloGame.m_BoardChangedDelegate += m_OtheloGame_BoardChanged;
                m_OtheloGame.m_CurrentPlayerChangedDelegate += m_BoardUiForm.PlayerChanged;
                m_OtheloGame.m_GameFinishedDelegate += m_OtheloGame_GameFinished;
                m_BoardUiForm.PlayerSelected += new TokenClickEventHandler(player_Selected_EventHandler);
                m_OtheloGame.SetInitialSettings(gameSettingsForm.RequestedBoardSize, gameSettingsForm.OpponentType, null, null);
                m_BoardUiForm.ShowDialog();
            }
        }

        private void getGameFinishedWinnerMessage(out string o_GameFinishedWinnerMessage, Othelo.GameFinishedEventArgs e)
        {
            string winnerName;
            if (e.PlayerOneCurrentGameScore > e.PlayerTwoCurrentGameScore)
            {
                winnerName = ((Player)e.PlayerOne).PlayerName + " Won!!";
            }
            else if (e.PlayerOneCurrentGameScore < e.PlayerTwoCurrentGameScore)
            {
                winnerName = ((Player)e.PlayerTwo).PlayerName + " Won!!";
            }
            else
            {
                winnerName = "Tie!!";
            }

            o_GameFinishedWinnerMessage = string.Format(
@"{0} ({1}/{2}) ({3}/{4})
Would you like another round?",
                              winnerName,
                              e.PlayerOneCurrentGameScore,
                              e.PlayerTwoCurrentGameScore,
                              ((Player)e.PlayerOne).TotalGamesScore,
                              ((Player)e.PlayerTwo).TotalGamesScore);
        }

        private BoardForm m_BoardUiForm;
        private Othelo m_OtheloGame;

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose managed resources
                m_BoardUiForm.Dispose();
            }
            // free native resources
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
