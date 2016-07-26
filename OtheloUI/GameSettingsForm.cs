using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using OtheloBLL;

namespace OtheloUI
{
    /// <summary>
    ///   The opening window, used for choosing table size and selecting the type of opponent.
    /// </summary>
    internal class GameSettingsForm : Form
    {
        /// <summary>
        /// This method will be called once, just before the first time the form is displayed
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.InitializeComponent();
            this.InitCustomLayout();
        }

        internal GameSettingsForm()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void SetBoardSizeButtonText()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GameSettingsForm));
            string resourceString = resources.GetString("m_ButtonBoardSize.Text");
            this.m_ButtonBoardSize.Text = string.Format(resourceString, this.m_RequestedBoardSize);
        }

        /// <summary>
        /// Uppon clicking on board size - increase board size that will be used when the game will start.
        /// </summary>
        private void m_ButtonBoardSize_Click(object sender, EventArgs e)
        {
            this.m_RequestedBoardSize += this.m_RequestedBoardSize < 14 ? 2 : 0;
            SetBoardSizeButtonText();
        }

        /// <summary>
        /// Uppon start game - check which button was clicked and according to it either start the game vs computer or human.
        /// </summary>
        private void m_ButtonStartGame_Click(object sender, EventArgs e)
        {
            Button buttonClicked = sender as Button;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GameSettingsForm));
            this.m_ButtonBoardSize = new System.Windows.Forms.Button();
            this.m_ButtonPlayAgainstComputer = new System.Windows.Forms.Button();
            this.m_ButtonPlayAgainstHuman = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_ButtonBoardSize
            // 
            resources.ApplyResources(this.m_ButtonBoardSize, "m_ButtonBoardSize");
            this.m_ButtonBoardSize.Name = "m_ButtonBoardSize";
            this.m_ButtonBoardSize.Click += new System.EventHandler(this.m_ButtonBoardSize_Click);
            // 
            // m_ButtonPlayAgainstComputer
            // 
            resources.ApplyResources(this.m_ButtonPlayAgainstComputer, "m_ButtonPlayAgainstComputer");
            this.m_ButtonPlayAgainstComputer.Name = "m_ButtonPlayAgainstComputer";
            this.m_ButtonPlayAgainstComputer.Click += new System.EventHandler(this.m_ButtonStartGame_Click);
            // 
            // m_ButtonPlayAgainstHuman
            // 
            resources.ApplyResources(this.m_ButtonPlayAgainstHuman, "m_ButtonPlayAgainstHuman");
            this.m_ButtonPlayAgainstHuman.Name = "m_ButtonPlayAgainstHuman";
            this.m_ButtonPlayAgainstHuman.Click += new System.EventHandler(this.m_ButtonStartGame_Click);
            // 
            // GameSettingsForm
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.m_ButtonBoardSize);
            this.Controls.Add(this.m_ButtonPlayAgainstComputer);
            this.Controls.Add(this.m_ButtonPlayAgainstHuman);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "GameSettingsForm";
            this.ResumeLayout(false);

        }

        private void InitCustomLayout()
        {
            this.m_RequestedBoardSize = 6;
            SetBoardSizeButtonText();
        }

        public int RequestedBoardSize
        {
            get { return this.m_RequestedBoardSize; }
        }

        private Button m_ButtonBoardSize;
        private Button m_ButtonPlayAgainstComputer;
        private Button m_ButtonPlayAgainstHuman;
        private int m_RequestedBoardSize;
    }
}
