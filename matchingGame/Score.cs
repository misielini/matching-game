using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matchingGame
{
    public class Score
    {
        // Player name and counters
        private string nick;
        private int points;
        private int time;
        private int clicks;
        private int hints;
        private int matches;

        public Score()
        {
            this.nick = "";
            this.points = 0;
            this.time = 0;
            this.matches = 0;
            this.clicks = 0;
            this.hints = 0;
        }

        public Score(string nick, int points, int time, int matches, int clicks, int hints)
        {
            this.nick = nick;
            this.points = points;
            this.time = time;
            this.matches = matches;
            this.clicks = clicks;
            this.hints = hints;
        }

        public Score(string s)
        {
            String[] splitedLine = s.Split(',');
            this.nick = splitedLine[0];
            this.points = Int32.Parse(splitedLine[1]);
            this.time = Int32.Parse(splitedLine[2]);
            this.matches = Int32.Parse(splitedLine[3]);
            this.clicks = Int32.Parse(splitedLine[4]);
            this.hints = Int32.Parse(splitedLine[5]);
        }

        // Setters and getters
        #region Setters and getters
        public int MyPoints
        {
            get { return points; }
            set { points = value; }
        }

        public string MyNick
        {
            get { return nick; }
            set { nick = value; }
        }

        public int MyTime
        {
            get { return time; }
            set { time = value; }
        }

        public int MyMatches
        {
            get { return matches; }
            set { matches = value; }
        }

        public int MyHints
        {
            get { return hints; }
            set { hints = value; }
        }

        public int MyClicks
        {
            get { return clicks; }
            set { clicks = value; }
        }

        #endregion

        // Points counter 
        public void countPoints()
        {
            this.points = (this.time * (-1) + this.matches * 100 + this.hints * (-99) + this.clicks * (-1));
            if (this.points < 0)
            {
                this.points = 0;
            }
        }


        public override string ToString()
        {
            return nick + "," + points + "," + time + "," + matches + "," + clicks + "," + hints;
        }

        public override bool Equals(object obj)
        {
            var score = obj as Score;
            return score != null &&
                   nick == score.nick &&
                   points == score.points &&
                   time == score.time &&
                   clicks == score.clicks &&
                   hints == score.hints &&
                   matches == score.matches &&
                   MyPoints == score.MyPoints;
        }

        public override int GetHashCode()
        {
            var hashCode = -1827612781;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(nick);
            hashCode = hashCode * -1521134295 + points.GetHashCode();
            hashCode = hashCode * -1521134295 + time.GetHashCode();
            hashCode = hashCode * -1521134295 + clicks.GetHashCode();
            hashCode = hashCode * -1521134295 + hints.GetHashCode();
            hashCode = hashCode * -1521134295 + matches.GetHashCode();
            hashCode = hashCode * -1521134295 + MyPoints.GetHashCode();
            return hashCode;
        }

    }
}
