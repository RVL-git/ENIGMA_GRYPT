using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Media.Animation;


namespace enigma
{

    public partial class MainWindow : Window
    {
        int[,] Roters = new int[5, 26];
        int[,] ReverserRoters = new int[5, 26];
        public int[] Comm = new int[26];
        public int[] key = { 0,0,0,0,0};
        bool tumbler = false;
        bool Block = false;
        int WhatRoterTurn = 0;
        public MainWindow()
        {
            InitializeComponent();
            FirstRoter.Text = " " + (char)('a' + key[1]);
            SecondRoter.Text = " " + (char)('a' + key[2]);
            ThirdRoter.Text = " " + (char)('a' + key[3]);
            StreamReader reader = new StreamReader("maping.txt");


            for (int j = 0; j < 26; j++)
                Comm[j] = -1;
            string str;
            for (int count = 0; (str = reader.ReadLine()) != null; count++)
                for (int i = 0; i < 26; i++)
                    Roters[count, i] = (int)(str[i] - 'a');
            reader.Close();


            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 26; j++)
                    ReverserRoters[i, Roters[i, j]] = j;
            for (int j = 0; j < 26; j++)
                ReverserRoters[3, j] = Roters[3, j];
        }

        public int Coder(int symbol)
        {
            symbol = Comm[symbol];
            for (int i = 0; i < 4; i++)
            {
                symbol += (key[i + 1] - key[i]);
                if (symbol >= 26)
                    symbol -= 26;
                if (symbol < 0)
                    symbol += 26;
                symbol = Roters[i, symbol];
            }

            for (int i = 4; i > 0; i--)
            {
                symbol -= (key[i] - key[i - 1]);
                if (symbol >= 26)
                    symbol -= 26;
                if (symbol < 0)
                    symbol += 26;
                if (i > 1)
                    symbol = ReverserRoters[i - 2, symbol];
            }

            return symbol;
        }

        private void TurnForward()
        {

            key[WhatRoterTurn + 1]++;
            if (26 == key[WhatRoterTurn + 1])
            {
                key[WhatRoterTurn%3 + 1] = 0;
                switch (WhatRoterTurn)
                {
                    case 0: FirstRoter.Text = " " + (char)('a' + key[1]); break;
                    case 1: SecondRoter.Text = " " + (char)('a' + key[2]); break;
                    case 2: ThirdRoter.Text = " " + (char)('a' + key[3]); break;
                }
                WhatRoterTurn = WhatRoterTurn == 2 ? WhatRoterTurn = 0: WhatRoterTurn + 1;
            }
            else
                switch (WhatRoterTurn)
                {
                    case 0: FirstRoter.Text = " " + (char)('a' + key[1]); break;
                    case 1: SecondRoter.Text = " " + (char)('a' + key[2]); break;
                    case 2: ThirdRoter.Text = " " + (char)('a' + key[3]); break;
                }
        }

        private void TurnBack()
        {
            key[WhatRoterTurn + 1]--;
            if (-1 == key[WhatRoterTurn + 1])
            {
                WhatRoterTurn = WhatRoterTurn == 0 ? WhatRoterTurn = 2 : WhatRoterTurn - 1;
                key[WhatRoterTurn + 1] = 25;
            }
            switch (WhatRoterTurn)
            {
                case 0: FirstRoter.Text = " " + (char)('a' + key[1]); break;
                case 1: SecondRoter.Text = " " + (char)('a' + key[2]); break;
                case 2: ThirdRoter.Text = " " + (char)('a' + key[3]); break;
            }
        }

        private void KeyBoardDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.A && e.Key <= Key.Z)
            {
                if (!tumbler)
                {
                    TextIn.Text += e.Key.ToString();
                    TextOut.Text +=  (char)('A' + (Coder((int)(e.Key.ToString()[0] - 'A'))));
                }
                else
                {
                    TextOut.Text += e.Key.ToString();
                    TextIn.Text += (char)('A' + (Coder((int)(e.Key.ToString()[0] - 'A'))));
                }
                TurnForward();
            }
            if (e.Key == Key.Space)
            {
                    TextIn.Text += " ";
                    TextOut.Text += " ";

            }
            if (e.Key == Key.Back)
            {
                if (TextIn.Text.Length > 0)
                {
                    TextIn.Text = TextIn.Text.Substring(0, TextIn.Text.Length - 1);
                    TextOut.Text = TextOut.Text.Substring(0, TextOut.Text.Length - 1);
                    TurnBack();
                }
            }
        }

        void TumblerChange(object sender, MouseButtonEventArgs e)
        {
            if (!tumbler)
            {
                tumbler = true;
                TransformTumbler.BeginAnimation(RotateTransform.AngleProperty, new DoubleAnimation { From = 0, To = -90, Duration = TimeSpan.FromSeconds(0.1) });
            }
            else
            {
                tumbler = false;
                TransformTumbler.BeginAnimation(RotateTransform.AngleProperty, new DoubleAnimation { From = -90, To = 0, Duration = TimeSpan.FromSeconds(0.1) });
            }
            TextOut.Text = "";
            TextIn.Text = "";
            for (int i = 0; i < 5; i++)
                key[i] = 0;
            FirstRoter.Text = " " + (char)('a' + key[1]);
            SecondRoter.Text = " " + (char)('a' + key[2]);
            ThirdRoter.Text = " " + (char)('a' + key[3]);
        }

        void BlockChange(object sender, MouseButtonEventArgs e)
        {
            if (!Block)
            {
                Block = true;
                TransformBlock.BeginAnimation(RotateTransform.AngleProperty, new DoubleAnimation { From = 0, To = -90, Duration = TimeSpan.FromSeconds(0.1) });
            }
            else
            {
                Block = false;
                TransformBlock.BeginAnimation(RotateTransform.AngleProperty, new DoubleAnimation { From = -90, To = 0, Duration = TimeSpan.FromSeconds(0.1) });
            }
        }

        void KeyClick(object sender, RoutedEventArgs e)
        {
            string name = ((Control)sender).Name;
            if(name == "space")
            {
                TextIn.Text += " ";
                TextOut.Text += " ";
                return;
            }
            if (name == "Dot")
            {
                TextIn.Text += ".";
                TextOut.Text += ".";
                return;
            }
            if (name == "BackSpace")
            {
                if (TextIn.Text.Length > 0)
                {
                    TextIn.Text = TextIn.Text.Substring(0, TextIn.Text.Length - 1);
                    TextOut.Text = TextOut.Text.Substring(0, TextOut.Text.Length - 1);
                    TurnBack();
                }
                return;
            }
            if (!tumbler)
            {
                TextIn.Text += name;
                TextOut.Text += (char)('A' + (Coder((int)(name[0] - 'A'))));
            }
            else
            {
                TextOut.Text += name;
                TextIn.Text += (char)('A' + (Coder((int)(name[0] - 'A'))));
            }
            TurnForward();
        }

        void Commutator(object sender, RoutedEventArgs e)
        {
            Window2 win2 = new Window2();
            win2.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            win2.Show();
        }

        void RoterUp(object sender, RoutedEventArgs e)
        {
          
            if (Block)
                return;
            string name = ((Control)sender).Name;

            if (name == "FirstRoterUp")
            {
                key[1] = key[1] == 25 ? 0 : (key[1] + 1);
                FirstRoter.Text = " " + (char)('a' + key[1]);
            }
            else if (name == "SecondRoterUp")
            {
                key[2] = key[2] == 25 ? 0 : (key[2] + 1);
                SecondRoter.Text = " " + (char)('a' + key[2]);
            }
            else
            {
                key[3] = key[3] == 25 ? 0 : (key[3] + 1);
                ThirdRoter.Text = " " + (char)('a' + key[3]);
            }


        }

        void RoterDown(object sender, RoutedEventArgs e)
        {
            if (Block)
                return;
            string name = ((Control)sender).Name; ;
            if (name == "FirstRoterDown")
            {
                key[1] = key[1] == 0 ? 25 : (key[1] - 1);
                FirstRoter.Text = " " + (char)('a' + key[1]);
            }
            else if (name == "SecondRoterDown")
            {
                key[2] = key[2] == 0 ? 25 : (key[2] - 1);
                SecondRoter.Text = " " + (char)('a' + key[2]);
            }
            else
            {
                key[3] = key[3] == 0 ? 25 : (key[3] - 1);
                ThirdRoter.Text = " " + (char)('a' + key[3]);
            }
        }


    }
}
