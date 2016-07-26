using System;
using System.Collections.Generic;
using System.Threading;

namespace OtheloBLL
{
    /// <summary>
    ///   This is the container that holds Othelo game logic. It is used to initiate gaming sessions and keep them running as long as the user wants.
    ///   It is also the only class that interacts directly with the UI.
    /// </summary>
    public sealed class Othelo
    {
        /// <summary>
        ///   Definitions
        /// </summary>
        public const int k_MaxMatrixSize = 14;
        public const int k_MinMatrixSize = 6;
        private const int k_NumberOfPlayers = 2;

        public enum ePlayers
        {
            PlayerOne = 0,
            PlayerTwo
        }

        /// <summary>
        ///   Delegates and Events
        /// </summary>
        public delegate void BoardChangedDelegate(object sender, BoardChangedEventArgs e);

        public delegate void CurrentPlayerChangedDelegate(object sender, PlayerChangedEventArgs e);

        public delegate void GameFinishedChangedDelegate(object sender, GameFinishedEventArgs e);

        public BoardChangedDelegate m_BoardChangedDelegate;

        public CurrentPlayerChangedDelegate m_CurrentPlayerChangedDelegate;

        public GameFinishedChangedDelegate m_GameFinishedDelegate;

        private void onBoardChanged(List<Board.TokenLocation> i_AvailableMoves)
        {
            if (m_BoardChangedDelegate != null)
            {
                BoardChangedEventArgs boardChangedArgs = new BoardChangedEventArgs(m_GameEngine.CurrentTableState, i_AvailableMoves);
                m_BoardChangedDelegate.Invoke(this, boardChangedArgs);
            }
        }

        private void onCurrentPlayerChanged(Player i_CurrentPlayer, bool i_HasAvailableMoves)
        {
            if (m_CurrentPlayerChangedDelegate != null)
            {
                PlayerChangedEventArgs playerChangedArgs = new PlayerChangedEventArgs(i_CurrentPlayer, i_HasAvailableMoves);
                m_CurrentPlayerChangedDelegate.Invoke(this, playerChangedArgs);
            }
        }

        private void onGameFinished(Player i_PlayerOne, int i_PlayerOneCurrentGameScore, Player i_PlayerTwo, int i_PlayerTwoCurrentGameScore)
        {
            if (m_GameFinishedDelegate != null)
            {
                GameFinishedEventArgs gameFinishedArgs = new GameFinishedEventArgs(i_PlayerOne, i_PlayerOneCurrentGameScore, i_PlayerTwo, i_PlayerTwoCurrentGameScore);
                m_GameFinishedDelegate.Invoke(this, gameFinishedArgs);
            }
        }

        public class BoardChangedEventArgs : EventArgs
        {
            public BoardChangedEventArgs(Board.eTokenMarks[,] i_BoardState, List<Board.TokenLocation> i_AvailableMoves)
            {
                this.BoardState = i_BoardState;
                this.AvailableMoves = i_AvailableMoves;
            }

            public readonly Board.eTokenMarks[,] BoardState;
            public readonly List<Board.TokenLocation> AvailableMoves;
        }

        public class PlayerChangedEventArgs : EventArgs
        {
            public PlayerChangedEventArgs(Player i_Player, bool i_HasAvailableMoves)
            {
                this.CurrentPlayer = i_Player;
                this.HasMoves = i_HasAvailableMoves;
            }

            public readonly Player CurrentPlayer;
            public readonly bool HasMoves;
        }

        public class GameFinishedEventArgs : EventArgs
        {
            public GameFinishedEventArgs(Player i_PlayerOne, int i_PlayerOneCurrentGameScore, Player i_PlayerTwo, int i_PlayerTwoCurrentGameScore)
            {
                this.PlayerOne = i_PlayerOne;
                this.PlayerOneCurrentGameScore = i_PlayerOneCurrentGameScore;
                this.PlayerTwo = i_PlayerTwo;
                this.PlayerTwoCurrentGameScore = i_PlayerTwoCurrentGameScore;
            }

            public readonly Player PlayerOne;
            public int PlayerOneCurrentGameScore { get; private set; }

            public readonly Player PlayerTwo;
            public int PlayerTwoCurrentGameScore { get; private set; }
        }

        /// <summary>
        ///   Methods
        /// </summary>
        public Othelo()
        {
            m_Players = new Player[k_NumberOfPlayers];
        }

        /// <remarks>
        ///   Wanted to keep Console UI support. In Console UI it is possible to change the names of players unlike the Graphic version (not in the requirments).
        ///   That is why this method can be called with null instead of names.
        /// </remarks>
        /// <param name="i_BoardSize"></param>
        /// <param name="i_PlayerOneName"> When called from Windows forms UI - will be null </param>
        /// <param name="i_PlayerTwoName"> When called from Windows forms UI - will be null</param>
        public void SetInitialSettings(int i_BoardSize, Player.ePlayerType i_OpponentType, string i_PlayerOneName, string i_PlayerTwoName)
        {
            this.m_TableSize = i_BoardSize;
            string firstPlayerName = (i_PlayerOneName == null) ? "Black" : i_PlayerOneName;
            string secondPlayerName = (i_PlayerTwoName == null) ? "White" : i_PlayerTwoName;
            this.m_Players[0] = new Player(firstPlayerName, Player.ePlayerType.Human, Board.eTokenMarks.PlayerOneToken);
            this.m_Players[1] = new Player(secondPlayerName, i_OpponentType, Board.eTokenMarks.PlayerTwoToken);
            StartNewGame();
        }

        public void StartNewGame()
        {
            this.m_GameEngine = new GameEngine(this.m_TableSize);

            // 50% chance for each player to begin
            this.m_CurrentPlayerTurn = RandomDecision.GetRandomNumber(0, 1) == 0 ? ePlayers.PlayerOne : ePlayers.PlayerTwo;
            playRound();
        }

        /// <remarks>
        ///   This method is called when a human player select a move.
        /// </remarks>
        public void PlayHumanRound(Board.TokenLocation i_SelectedLocation)
        {
            if (this.m_Players[(int)m_CurrentPlayerTurn].PlayerType != Player.ePlayerType.Human)
            {
                throw new InvalidOperationException("This method should only be called for human players.");
            }

            int numberOfTokensReplaced;
            Board.eTokenMarks[,] newBoardState;
            this.m_GameEngine.GetHumanPlayerMoveSelection(
                this.m_Players[(int)m_CurrentPlayerTurn],
                (Board.TokenLocation)i_SelectedLocation,
                out newBoardState,
                out numberOfTokensReplaced);
            updateBoardState(newBoardState, null);
            playRound();
        }

        /// <summary>
        ///   Select the next player that can actually play (has moves available). If none is found, o_GameFinished will be marked true; 
        /// </summary>
        private void getNextPlayer()
        {
            bool hasAvailableMoves;
            changePlayer(out hasAvailableMoves);
            if (hasAvailableMoves)
            {
                m_CurrentGameFinished = false;
            }
            else
            {
                changePlayer(out hasAvailableMoves);
                m_CurrentGameFinished = !hasAvailableMoves;
            }
        }

        private void changePlayer(out bool o_HasAvailableMoves)
        {
            List<Board.TokenLocation> availableMoves = null;
            m_CurrentPlayerTurn = m_CurrentPlayerTurn == ePlayers.PlayerOne ? ePlayers.PlayerTwo : ePlayers.PlayerOne;
            m_GameEngine.GetPossibleMoves(m_Players[(int)m_CurrentPlayerTurn].TokenMark, out availableMoves);
            o_HasAvailableMoves = availableMoves != null;
            onCurrentPlayerChanged(m_Players[(int)m_CurrentPlayerTurn], o_HasAvailableMoves);
        }

        private void playRound()
        {
            getNextPlayer();
            Board.eTokenMarks[,] newBoardState;

            // If the current player is human, nothing to do but show available moves.
            // Else, if computer, play entire round and then switch turn for human player in order to show available moves.
            if (!m_CurrentGameFinished)
            {
                switch (m_Players[(int)m_CurrentPlayerTurn].PlayerType)
                {
                    case Player.ePlayerType.Human:
                        updateAvailableMovesForHumanPlayer();
                        break;
                    case Player.ePlayerType.Computer:
                        m_GameEngine.GetBestComputerMove(m_Players[(int)m_CurrentPlayerTurn].TokenMark, out newBoardState);
                        updateBoardState(newBoardState, null);
                        getNextPlayer();
                        if (!m_CurrentGameFinished)
                        {
                            updateAvailableMovesForHumanPlayer();
                        }
                        break;
                }
            }

            if (m_CurrentGameFinished)
            {
                finishGame();
            }
        }

        private void finishGame()
        {
            int playerOneScore;
            int playerTwoScore;
            m_GameEngine.GetCurrentScore(m_Players[(int)ePlayers.PlayerOne], m_Players[(int)ePlayers.PlayerTwo], out playerOneScore, out playerTwoScore);
            m_Players[(int)ePlayers.PlayerOne].TotalGamesScore += playerOneScore > playerTwoScore ? 1 : 0;
            m_Players[(int)ePlayers.PlayerTwo].TotalGamesScore += playerTwoScore > playerOneScore ? 1 : 0;
            m_CurrentGameFinished = false;
            onGameFinished(m_Players[(int)ePlayers.PlayerOne], playerOneScore, m_Players[(int)ePlayers.PlayerTwo], playerTwoScore);
        }

        private void updateAvailableMovesForHumanPlayer()
        {
            List<Board.TokenLocation> availableMoves = null;
            m_GameEngine.GetPossibleMoves(m_Players[(int)m_CurrentPlayerTurn].TokenMark, out availableMoves);
            updateBoardState(m_GameEngine.CurrentTableState, availableMoves);
        }

        private void updateBoardState(Board.eTokenMarks[,] newBoardState, List<Board.TokenLocation> i_AvailableMoves)
        {
            m_GameEngine.UpdateCurrentBoardState(newBoardState);
            onBoardChanged(i_AvailableMoves);
        }

        /// <summary>
        ///   Members
        /// </summary>
        private int m_TableSize;
        private GameEngine m_GameEngine;
        private Player[] m_Players;
        private ePlayers m_CurrentPlayerTurn;
        private bool m_CurrentGameFinished;
    }
}
