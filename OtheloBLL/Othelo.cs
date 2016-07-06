using System;
using System.Collections.Generic;

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
                this.m_BoardState = i_BoardState;
                this.m_AvailableMoves = i_AvailableMoves;
            }

            public Board.eTokenMarks[,] BoardState
            {
                get { return this.m_BoardState; }
            }

            public List<Board.TokenLocation> AvailableMoves
            {
                get { return this.m_AvailableMoves; }
            }

            private Board.eTokenMarks[,] m_BoardState;
            private List<Board.TokenLocation> m_AvailableMoves;
        }

        public class PlayerChangedEventArgs : EventArgs
        {
            public PlayerChangedEventArgs(Player i_Player, bool i_HasAvailableMoves)
            {
                this.m_Player = i_Player;
                this.m_HasAvailableMoves = i_HasAvailableMoves;
            }

            public Player CurrentPlayer
            {
                get { return this.m_Player; }
            }

            public bool HasMoves
            {
                get { return this.m_HasAvailableMoves; }
            }

            private Player m_Player;
            private bool m_HasAvailableMoves;
        }

        public class GameFinishedEventArgs : EventArgs
        {
            public GameFinishedEventArgs(Player i_PlayerOne, int i_PlayerOneCurrentGameScore, Player i_PlayerTwo, int i_PlayerTwoCurrentGameScore)
            {
                this.m_PlayerOne = i_PlayerOne;
                this.m_PlayerOneCurrentGameScore = i_PlayerOneCurrentGameScore;
                this.m_PlayerTwo = i_PlayerTwo;
                this.m_PlayerTwoCurrentGameScore = i_PlayerTwoCurrentGameScore;
            }

            public Player PlayerOne
            {
                get { return this.m_PlayerOne; }
            }

            public int PlayerOneCurrentGameScore
            {
                get { return this.m_PlayerOneCurrentGameScore; }
            }

            public Player PlayerTwo
            {
                get { return this.m_PlayerTwo; }
            }

            public int PlayerTwoCurrentGameScore
            {
                get { return this.m_PlayerTwoCurrentGameScore; }
            }

            private Player m_PlayerOne;
            private int m_PlayerOneCurrentGameScore;
            private Player m_PlayerTwo;
            private int m_PlayerTwoCurrentGameScore;
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
            string firstPlayerName = i_PlayerOneName == null ? "Black" : i_PlayerOneName;
            string secondPlayerName = i_PlayerTwoName == null ? "White" : i_PlayerTwoName;
            this.m_Players[0] = new Player(firstPlayerName, Player.ePlayerType.Human, Board.eTokenMarks.PlayerOneToken);
            this.m_Players[1] = new Player(secondPlayerName, i_OpponentType, Board.eTokenMarks.PlayerTwoToken);
            StartNewGame();
        }

        public void StartNewGame()
        {
            this.m_GameEngine = new GameEngine(this.m_TableSize);

            // 50% chance for each player to begin
            this.m_CurrentPlayerTurn = GameEngine.s_RandMechanisem.Next() % 2 == 0 ? ePlayers.PlayerOne : ePlayers.PlayerTwo;
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
        private void getNextPlayer(out bool o_GameFinished)
        {
            bool hasAvailableMoves;
            changePlayer(out hasAvailableMoves);
            if (hasAvailableMoves)
            {
                o_GameFinished = false;
            }
            else
            {
                changePlayer(out hasAvailableMoves);
                o_GameFinished = !hasAvailableMoves;
            }
        }

        private void changePlayer(out bool o_HasAvailableMoves)
        {
            Board.eTokenMarks[,] newBoardState;
            List<Board.TokenLocation> availableMoves = null;
            m_CurrentPlayerTurn = m_CurrentPlayerTurn == ePlayers.PlayerOne ? ePlayers.PlayerTwo : ePlayers.PlayerOne;
            m_GameEngine.GetBestPossibleMove(m_Players[(int)m_CurrentPlayerTurn].TokenMark, out newBoardState, out availableMoves);
            o_HasAvailableMoves = availableMoves != null;
            onCurrentPlayerChanged(m_Players[(int)m_CurrentPlayerTurn], o_HasAvailableMoves);
        }

        private void playRound()
        {
            bool gameFinished;
            getNextPlayer(out gameFinished);
            Board.eTokenMarks[,] newBoardState;
            List<Board.TokenLocation> availableMoves = null;

            // If the current player is human, nothing to do but show available moves.
            // Else, if computer, play entire round and then switch turn for human player in order to show available moves.
            if (!gameFinished && m_Players[(int)m_CurrentPlayerTurn].PlayerType == Player.ePlayerType.Human)
            {
                updateAvailableMovesForHumanPlayer();
            }
            else if (!gameFinished && m_Players[(int)m_CurrentPlayerTurn].PlayerType == Player.ePlayerType.Computer)
            {
                m_GameEngine.GetBestPossibleMove(m_Players[(int)m_CurrentPlayerTurn].TokenMark, out newBoardState, out availableMoves);
                updateBoardState(newBoardState, null);
                getNextPlayer(out gameFinished);
                if (!gameFinished)
                {
                    updateAvailableMovesForHumanPlayer();
                }
            }

            if (gameFinished)
            {
                finishGame();
            }
        }

        private void finishGame()
        {
            int playerOneScore;
            int playerTwoScore;
            m_GameEngine.GetCurrentScore(m_Players[(int)ePlayers.PlayerOne], m_Players[(int)ePlayers.PlayerTwo], out playerOneScore, out playerTwoScore);
            m_Players[(int)ePlayers.PlayerOne].Score += playerOneScore > playerTwoScore ? 1 : 0;
            m_Players[(int)ePlayers.PlayerTwo].Score += playerTwoScore > playerOneScore ? 1 : 0;
            onGameFinished(m_Players[(int)ePlayers.PlayerOne], playerOneScore, m_Players[(int)ePlayers.PlayerTwo], playerTwoScore);
        }

        private void updateAvailableMovesForHumanPlayer()
        {
            Board.eTokenMarks[,] newBoardState;
            List<Board.TokenLocation> availableMoves = null;
            m_GameEngine.GetBestPossibleMove(m_Players[(int)m_CurrentPlayerTurn].TokenMark, out newBoardState, out availableMoves);
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
    }
}
