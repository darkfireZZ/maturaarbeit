using Cubing;
using Cubing.ThreeByThree;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CubingVisual
{
    public partial class Form1 : Form
    {
        private Random random = null;
        private ICollection<CsTimerData> collectedData = null;
        private string path = null;

        public Form1()
        {
            InitializeComponent();
            random = new Random();
            collectedData = new List<CsTimerData>();
        }

        private void enterTimeBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (enterTimeBox.Text == "") { }
                else if (enterTimeBox.Text == "next")
                {
                    scrambleLabel.Text = Alg.FromRandomMoves(random.Next(15, 25), random).ToString();
                    enterTimeBox.Text = "";
                }
                else if (enterTimeBox.Text.All(char.IsDigit))
                {
                    collectedData.Add(new CsTimerData()
                    {
                        Number = collectedData.Count + 1,
                        Date = DateTime.Now,
                        Scramble = scrambleLabel.Text,
                        Milliseconds = int.Parse(enterTimeBox.Text)
                    });
                    scrambleLabel.Text = Alg.FromRandomMoves(random.Next(15, 25), random).ToString();
                    enterTimeBox.Text = "";
                }
                else if (File.Exists(enterTimeBox.Text))
                {
                    if (path != null) CsTimerData.WriteCsTimerData(path, collectedData);
                    path = enterTimeBox.Text;
                    collectedData.Clear();
                    CsTimerData.ReadCsTimerData(path, it => collectedData.Add(it));
                    enterTimeBox.Text = "";
                    scrambleLabel.Text = Alg.FromRandomMoves(random.Next(15, 25), random).ToString();
                }
                else
                {
                    enterTimeBox.Text = "Invalid: " + enterTimeBox.Text;
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (path != null) CsTimerData.WriteCsTimerData(path, collectedData);
        }
    }
}