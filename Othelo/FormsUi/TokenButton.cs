using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Othelo
{
    /// <summary>
    ///   This class represent a token button that a user can press on.
    /// </summary>
    internal delegate void TokenClickEventHandler(object sender, TokenButton.TokenClickedEventArgs e);

    internal class TokenButton : Button
    {
        public const int k_ButtonSize = 50;
        public const int k_ThickBorder = 3;
        public const int k_ThinBorder = 1;

        public event TokenClickEventHandler ClickOccured;

        protected override void OnMouseEnter(EventArgs e)
        {
            this.OnEnter(e);
            if (this.Enabled)
            {
                this.FlatAppearance.BorderColor = Color.Turquoise;
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            this.OnEnter(e);
            if (this.Enabled)
            {
                this.FlatAppearance.BorderColor = Color.White;
            }
        }

        /// <remarks>
        ///   As it turns out, ForeColor for disabled buttons can not be changed in the normal way. Had to override OnPaint in order to draw
        ///   the circle inside the black and white boxes.
        /// </remarks>
        /// <param name="pevent"></param>
        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);
            SolidBrush drawBrush = new SolidBrush(this.ForeColor); // Use the ForeColor property
            // Draw string to screen.
            if (!this.Enabled && this.BackColor != SystemColors.Control)
            {
                pevent.Graphics.DrawString("O", this.Font, drawBrush, (this.ClientSize.Width / 2) - this.Font.Size, (this.ClientSize.Height / 2) - this.Font.Size);
            }
        }

        public class TokenClickedEventArgs : EventArgs
        {
            public TokenClickedEventArgs(Board.TokenLocation i_TokenLocation)
            {
                this.m_TokenLocation = i_TokenLocation;
            }

            public Board.TokenLocation TokenLocation
            {
                get { return this.m_TokenLocation; }
            }

            private Board.TokenLocation m_TokenLocation;
        }

        protected override void OnClick(EventArgs e)
        {
            if (this.ClickOccured != null)
            {
                this.ClickOccured(this, this.m_TokenLocationEventArgs);
            }
        }

        public TokenButton(Board.TokenLocation i_TokenLocation)
        {
            this.m_TokenLocationEventArgs = new TokenClickedEventArgs(i_TokenLocation);
            this.ClientSize = new Size(k_ButtonSize, k_ButtonSize);
            this.SetStyle(ControlStyles.UserPaint, true);
        }

        public void SetButtonStatus(Board.eTokenMarks i_Token)
        {
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = k_ThickBorder;
            this.FlatAppearance.BorderColor = Color.White;

            switch (i_Token)
            {
                case Board.eTokenMarks.Blank:
                    this.BackColor = SystemColors.Control;
                    this.FlatAppearance.BorderSize = k_ThinBorder;
                    this.FlatAppearance.BorderColor = Color.Gray;
                    break;
                case Board.eTokenMarks.PlayerTwoToken:
                    this.BackColor = Color.White;
                    this.ForeColor = Color.Black;
                    break;
                case Board.eTokenMarks.PlayerOneToken:
                    this.BackColor = Color.Black;
                    this.ForeColor = Color.White;
                    break;
                default:
                    throw new ArgumentException("Unrecognized mark found in board state");
            }

            this.Enabled = false;
        }

        public void SetAvailableMove()
        {
            this.Enabled = true;
            this.BackColor = Color.Gray;
            this.FlatAppearance.BorderSize = k_ThickBorder;
            this.FlatAppearance.BorderColor = Color.White;
        }

        private readonly TokenClickedEventArgs m_TokenLocationEventArgs; // No need to recreate event args, button location doesn't change. 
    }
}
