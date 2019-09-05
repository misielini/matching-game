using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Media;
using System.Windows.Forms;

namespace matchingGame
{
    public partial class Form1 : Form
    {

        private Score score;

        private int maxTime = Int32.MaxValue;
        private int remTime = Int32.MaxValue;

        //sound1 - first click
        //sound2 - second right click
        //sound3 - hint
        //sound4 - second wrong click
        private SoundPlayer clickSound1, clickSound2, clickSound3, clickSound4;

        //Resoult dialog
        private Form2 form2;

        private string fileName = Path.Combine(Environment.CurrentDirectory, "ResultsFile.txt");

        // firstClicked points to the first Label control
        // that the player clicks, but it will be null 
        // if the player hasn't clicked a label yet
        Label firstClicked = null;

        // secondClicked points to the second Label control 
        // that the player clicks
        Label secondClicked = null;

        // represents clicked square
        Label clickedLabel = null;

        // Use this Random object to choose random icons for the squares
        Random random = new Random();

        // Each of these letters is an interesting icon
        // in the Webdings font,
        // and each icon appears twice in this list
        List<string> icons = null;

        public Form1()
        {
            InitializeComponent();
            assignIconsToSquares();
            clickSound1 = new SoundPlayer(Properties.Resources.sound1);
            clickSound2 = new SoundPlayer(Properties.Resources.sound2);
            clickSound3 = new SoundPlayer(Properties.Resources.sound3);
            clickSound4 = new SoundPlayer(Properties.Resources.sound4);
            form2 = new Form2();
            score = new Score();
        }

        // Assign each icon from the list of icons to a random square
        private void assignIconsToSquares()
        {
            icons = new List<string>()
            {
            "!", "!", "N", "N", ",", ",", "k", "k",
            "b", "b", "v", "v", "w", "w", "z", "z"
            };

            // The TableLayoutPanel has 16 labels,
            // and the icon list has 16 icons,
            // so an icon is pulled at random from the list
            // and added to each label
            foreach (Control control in tableLayoutPanel1.Controls)
            {
                Label iconLabel = control as Label;
                if (iconLabel != null)
                {
                    int randomNumber = random.Next(icons.Count);
                    iconLabel.Text = icons[randomNumber];
                    iconLabel.ForeColor = iconLabel.BackColor;
                    icons.RemoveAt(randomNumber);
                }
            }
        }

        //Every label's Click event is handled by this event handler
        private void label_Click(object sender, EventArgs e)
        {
            clickedLabel = sender as Label;

            if (clickedLabel != null)
            {

                // The timer is on only after two non-matching 
                // icons have been shown to the player, 
                // so ignore any clicks if the timer is running
                if (timer1.Enabled == true) return;

                // If the clicked label is black, the player clicked
                // an icon that's already been revealed - ignore the click
                if (clickedLabel.ForeColor == Color.Black) return;

                // If firstClicked is null, this is the first icon 
                // in the pair that the player clicked, so start total time timer,
                // set firstClicked to the label that the player 
                // clicked, change its color to black, play sound1 and return
                if (firstClicked == null)
                {
                    if (totalTimeTimer.Enabled == false) totalTimeTimer.Enabled = true;
                    firstClicked = clickedLabel;
                    firstClicked.ForeColor = Color.Black;
                    playSound(clickSound1);
                    score.MyClicks++;
                    return;
                }

                // If the player gets this far, the timer isn't
                // running and firstClicked isn't null,
                // so this must be the second icon the player clicked
                // Set its color to black and increase the counter 
                secondClicked = clickedLabel;
                secondClicked.ForeColor = Color.Black;
                score.MyClicks++;

                // If the player clicked two matching icons, keep them 
                // black, play sound2 and reset firstClicked
                // and secondClicked so the player can click another icon
                if (firstClicked.Text == secondClicked.Text)
                {
                    //matches++;
                    score.MyMatches++;

                    playSound(clickSound2);
                    // Check to see if the player won
                    checkForWinner();
                    firstClicked = null;
                    secondClicked = null;

                    return;
                }

                // If the player gets this far, the player 
                // clicked two different icons, so start the 
                // timer (which will wait several hundred ms, and then
                // hide the icons)
                // If that isnt beginning of the game - play sound4 
                timer1.Start();
                if (score.MyClicks != 0) playSound(clickSound4);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Stop the timer
            timer1.Stop();

            // Hide both icons
            firstClicked.ForeColor = firstClicked.BackColor;
            secondClicked.ForeColor = secondClicked.BackColor;

            // Reset firstClicked and secondClicked 
            // so the next time a label is clicked,
            // the program knows it's the first click
            firstClicked = null;
            secondClicked = null;
        }

        // Check every icon to see if it is matched, by 
        // comparing its foreground color to its background color. 
        // If all of the icons are matched, the player wins
        private void checkForWinner()
        {
            // Go through all of the labels in the TableLayoutPanel
            // checking each one to see if its icon matched
            foreach (Control control in tableLayoutPanel1.Controls)
            {

                Label iconLabel = control as Label;

                if (iconLabel != null)
                {
                    if (totalTimeTimer.Enabled != true)
                    {
                        form2.ResultLabel = "Przegrałeś!";
                        break;
                    }
                    else if (iconLabel.ForeColor == iconLabel.BackColor) return;
                }

            }

            score.countPoints();
            totalTimeTimer.Enabled = false;

            // If the player gets this far, the player won
            // Message box show him nubmer of clicks, used hints, seconds
            // and it will ask him if he wants to keep playing

            form2.PointsLabel = "Wynik: " + score.MyPoints.ToString() + " pkt.";
            form2.ClickLabel = "Kliknięć: " + score.MyClicks.ToString();
            form2.MatchesLabel = "Dopasowań: " + score.MyMatches.ToString();
            form2.HintsLabel = "Podpowiedzi: " + score.MyHints.ToString();
            form2.TimeLabel = "Czas: " + score.MyTime.ToString() + " s.";

            DialogResult dr = form2.ShowDialog(this);
            form2.ResultLabel = "Wygrałeś!";

            if (dr == DialogResult.OK)
            {
                score.MyNick = form2.UserName;
                saveToTxtFile(fileName, score.ToString());
                gameOver();
            }
            else
            {
                gameOver();
            }
        }

        private void gameOver()
        {
            foreach (Control control in tableLayoutPanel1.Controls)
            {
                Label iconLabel = control as Label;
                iconLabel.ForeColor = Color.Black;
            }
        }

        // One clock cycle is one second.
        private void totalTimeTimer_Tick(object sender, EventArgs e)
        {
            setRemaingTime();

            score.MyTime++;
            if (score.MyTime == maxTime)
            {
                totalTimeTimer.Enabled = false;
                checkForWinner();
            }

        }

        // New game button from menu
        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            restartGame();
        }

        private void restartGame()
        {
            score = new Score();
            remTime = maxTime;
            setRemaingTime();
            totalTimeTimer.Enabled = false;
            firstClicked = null;
            secondClicked = null;
            clickedLabel = null;
            assignIconsToSquares();
        }

        // Hint button from menu
        private void hintToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // To get a hint the player must click some squere
            if (clickedLabel != null)
            {
                // Ignore that situation if player clicked a second time
                // of just used a hint
                if (secondClicked != null || clickedLabel.ForeColor != Color.Black) return;

                // Check every icon to check if they are the same and not clicked
                // and then change the color of the correct icon to white,
                // play sound3, increase hints counter and return.
                foreach (Control control in tableLayoutPanel1.Controls)
                {
                    Label iconLabel = control as Label;

                    if (iconLabel.Equals(clickedLabel)) continue;
                    else if (iconLabel.Text == clickedLabel.Text && iconLabel.ForeColor == iconLabel.BackColor)
                    {
                        iconLabel.ForeColor = Color.White;
                        playSound(clickSound3);
                        score.MyHints++;
                        return;
                    }
                }

            }
        }

        private void changeColorOfLayout(Color color, ToolStripMenuItem oldMenuItem, ToolStripMenuItem newMenuItem)
        {
            DialogResult dr = MessageBox.Show("Zmiana ustawień spowoduje utratę rozgywki! \nCzy napewno chcesz to zrobić?",
            this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            switch (dr)
            {
                case DialogResult.Yes:
                    {
                        tableLayoutPanel1.BackColor = color;
                        tableLayoutPanel1.ForeColor = color;

                        foreach (Control control in tableLayoutPanel1.Controls)
                        {
                            Label label = control as Label;
                            label.ForeColor = Color.CornflowerBlue;
                        }
                        newMenuItem.Enabled = false;
                        oldMenuItem.Enabled = true;
                        newMenuItem.Checked = true;
                        oldMenuItem.Checked = false;
                        restartGame();
                        break;
                    }
                case DialogResult.No: break;
            }
        }

        // A button from the menu that changes board color to blue
        // If player clicks this, button will be checked and invisible
        // and result of the game will be lost
        private void blueToolStripMenuItem_Click(object sender, EventArgs e)
        {

            changeColorOfLayout(Color.CornflowerBlue, redToolStripMenuItem, blueToolStripMenuItem);

        }

        // A button from the menu that changes board color to red
        // If player clicks this, button will be checked and invisible
        // and result of the game will be lost
        private void redToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changeColorOfLayout(Color.Crimson, blueToolStripMenuItem, redToolStripMenuItem);

        }

        private void changeGameTimeMenuItem(int number, int maxTime)
        {

            DialogResult dr = MessageBox.Show("Zmiana ustawień spowoduje utratę rozgywki! \nCzy napewno chcesz to zrobić?",
            this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            switch (dr)
            {
                case DialogResult.Yes:
                    {
                        number--;

                        ToolStripMenuItem[] items = new ToolStripMenuItem[6];

                        items[0] = gameTime1ToolStripMenuItem;
                        items[1] = gameTime2ToolStripMenuItem;
                        items[2] = gameTime3ToolStripMenuItem;
                        items[3] = gameTime4ToolStripMenuItem;
                        items[4] = gameTime5ToolStripMenuItem;
                        items[5] = gameTime6ToolStripMenuItem;

                        items[number].Enabled = false;
                        items[number].Checked = true;


                        for (int i = 0; i < items.Length; i++)
                        {
                            if (i != number)
                            {
                                items[i].Enabled = true;
                                items[i].Checked = false;
                            }
                        }

                        if (number == 5) remToolStripMenuItem.Visible = false;
                        else remToolStripMenuItem.Visible = true;

                        this.maxTime = maxTime;
                        restartGame();
                        break;
                    }
                case DialogResult.No: break;
            }

        }

        private void gameTime1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changeGameTimeMenuItem(1, 3);
        }

        private void gameTime2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changeGameTimeMenuItem(2, 45);

        }

        private void gameTime3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changeGameTimeMenuItem(3, 60);
        }

        private void gameTime4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changeGameTimeMenuItem(4, 75);

        }

        private void gameTime5ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changeGameTimeMenuItem(5, 90);

        }

        private void gameTime6ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changeGameTimeMenuItem(6, Int32.MaxValue);

        }

        // Play the sound received in the argument
        // if player didnt click turn on/off sound button
        private void playSound(SoundPlayer sound)
        {
            if (soundToolStripMenuItem.Checked)
            {
                sound.Play();
            }

        }

        private void scoresListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String line;
            int i = 0;
            List<Score> scoreList = new List<Score>();
            PointsComparer pc = new PointsComparer();
            try
            {
                using (StreamReader sr = new StreamReader(fileName, true))
                {

                    while ((line = sr.ReadLine()) != null)
                    {
                        scoreList.Add(new Score(line));

                    }
                }

                line = "";
                scoreList.Sort(pc);
                scoreList.Reverse();

                foreach (Score item in scoreList)
                {
                    if (i < 5)
                    {
                        i++;
                        line += i + ". " + item.MyNick.ToString() + ", " + item.MyPoints.ToString() + " pkt." + "\n";
                    }
                    else break;
                }

                MessageBox.Show(line);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd odczytu pliku" + fileName + "(" + ex.Message + ")");
            }
        }

        // Turn on/off sound button
        private void soundToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private static void saveToTxtFile(string fileName, string text)
        {
            using (StreamWriter sw = new StreamWriter(fileName, true))
            {
                //if the file doesn't exist, create it
                if (!File.Exists(fileName)) File.Create(fileName);
                sw.WriteLine(text);
            }
        }

        private void setRemaingTime()
        {

            if (remTime > 90 || remTime < 0) return;

            remToolStripMenuItem.Text = "Pozostały czas: " + remTime + " sek.";
            remTime--;
        }

    }
}

/*
       * 5X5 
       List<string> icons = new List<string>()
       {
       "!", "!", "N", "N", ",", ",", "k", "k", "Ń", "Ń", 
       "b", "b", "v", "v", "w", "w", "z", "z", "-", "-",
       "e", "e", "Y", "Y", "U", "U"
       };
*/

/*
    * 6X6 
   List<string> icons = new List<string>()
   {
   "!", "!", "N", "N", ",", ",", "k", "k", "Ń", "Ń", "ó", "ó",
   "b", "b", "v", "v", "w", "w", "z", "z", "-", "-", "~", "~",
   "e", "e", "Y", "Y", "U", "U", "O", "O", "@", "@", "#", "#"
   };
*/

/*
    * 8X8 
   List<string> icons = new List<string>()
   {
   "!", "!", "N", "N", ",", ",", "k", "k", "Ń", "Ń", "ó", "ó",
   "b", "b", "v", "v", "w", "w", "z", "z", "-", "-", "~", "~",
   "e", "e", "Y", "Y", "U", "U", "O", "O", "@", "@", "#", "#",
   "P", "P", "S", "S", "f", "f", "G", "G", "$", "$", "%", "%",
   "Z", "Z", "ą", "ą", "Ą", "Ą", "ć", "ć", "A", "A", "H", "H",
   "h", "h", "j", "j"

   };
*/
