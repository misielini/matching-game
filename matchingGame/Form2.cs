using System;
using System.Windows.Forms;

namespace matchingGame
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        // Setters and getters
        #region Setters and getters
        public string ResultLabel
        {
            get { return resultLabel.Text; }
            set { resultLabel.Text = value; }
        }

        public string ClickLabel
        {
            get { return clicksLabel.Text; }
            set { clicksLabel.Text = value; }
        }

        public string MatchesLabel
        {
            get { return matchesLabel.Text; }
            set { matchesLabel.Text = value; }
        }

        public string HintsLabel
        {
            get { return hintsLabel.Text; }
            set { hintsLabel.Text = value; }
        }

        public string TimeLabel
        {
            get { return timeLabel.Text; }
            set { timeLabel.Text = value; }
        }

        public string PointsLabel
        {
            get { return pointsLabel.Text; }
            set { pointsLabel.Text = value; }
        }

        public string UserName
        {
            get { return textBox1.Text; }
            set { textBox1.Text = value; }
        }
        #endregion
    }
}
