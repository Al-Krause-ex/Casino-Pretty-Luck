using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MediaPlayer;

namespace Casino
{
    public partial class LuckyTree : Form
    {
        SoundPlayer player = new SoundPlayer();

        Random rnd = new Random();

        Timer luckyTreeTime = new Timer();

        public string[] symbols = new string[2] { ">", "<" };
        Label[] labels = new Label[4];

        public int randomOne = 0;
        public int randomTwo = 0;

        int countXP;
        long countCash, countSubstractCash;

        bool flag = false;
        bool win = false;
        bool click = false;

        public int count = 0; //Счётчик для журнала истории (в форме слева-сверху)

        public string randomSymbol;

        int counter;
        char check = 'F';

        public LuckyTree()
        {
            InitializeComponent();

            buttonBack.Click += (obj, e) =>
            {
                Form1.SoundClickButton();
                luckyTreeTime.Stop();
            };
            buttonBack.DialogResult = DialogResult.OK;

            for (int i = 0; i < Information.Training.Length; i++)
            {
                if (check == Information.Training[i])
                {
                    counter = 0;
                    timer1.Interval = 100;
                    timer1.Enabled = true;
                    
                    timer1.Tick += new System.EventHandler(time_Tick);

                    Information.Training = Information.Training.Remove(i, 1);

                    File.WriteAllText(Form1.path_training, string.Empty);

                    using (StreamWriter sw = File.AppendText(Form1.path_training))
                    {
                        sw.WriteLine(Information.Training);
                    }
                }
            }

            Form1.Write();
            Form1.Encode();

            Start();
            Main();
            Mus_Sound();
            LuckyTreeTime();
        }

        public void Start()
        {
            ToolTip t = new ToolTip();
            t.SetToolTip(hintItemOne, "За каждую игру умножается полученный опыт в два раза");
            t.SetToolTip(hintItemTwo, "Бонус позволяет играть в игры, не тратя денег");
            t.SetToolTip(hintItemThree, "Последующие выигрышные деньги превращаются в опыт");
            t.SetToolTip(hintItemFour, "После каждой победы, дополнительно \nвносится 25% от выигрышной суммы");
            t.SetToolTip(hintItemFive, "Даёт пассивный эффект, после каждой игры, \nв бюджет возвращается 10% от потраченной суммы");

            SetLabels();

            labels = new Label[4] { label1, label2, label3, label4 };

            labelFinance.BackColor = Color.FromArgb(50, Color.Black);
            labelFinance.ForeColor = Color.FromArgb(5, Color.Blue);

            labelLvL.BackColor = Color.FromArgb(50, Color.Black);
            labelLvL.ForeColor = Color.FromArgb(5, Color.Blue);

            labelXP.BackColor = Color.FromArgb(50, Color.Black);
            labelXP.ForeColor = Color.FromArgb(5, Color.Blue);

            buttonStart.BringToFront();

            if (Information.StaticXP >= 8000)
                closeLuckyTreeUpAccount.Visible = false;

            if (Information.Finance < 10)
            {
                buttonStart.Enabled = false;
                topUpAccount.Enabled = true;
            }

            else
            {
                buttonStart.Enabled = true;
                topUpAccount.Enabled = false;
            }

            buttonStart.Click += (obj, e) =>
            {
                Form1.PayToTurn();

                if (Information.Finance >= 10)
                {
                    if (Information.ItemTwo_WW_Game > 0)
                    {
                        countSubstractCash = 0;
                        Information.Finance = Information.Finance;
                        labelSubstractCashOne.Text = "-" + countSubstractCash.ToString();
                    }
                    else
                    {
                        labelSubstractCashTwo.Text = "-" + countSubstractCash.ToString();
                        countSubstractCash = Information.Finance * 20 / 100;

                        Information.Finance -= Information.Finance * 20 / 100;

                        labelSubstractCashOne.Text = "-" + countSubstractCash.ToString();
                    }

                    if (Information.ItemFive_Pass_Accumulation)
                        Information.Finance += countSubstractCash * 10 / 100;

                    if (Information.ItemTwo_WW_Game > 0)
                        Information.ItemTwo_WW_Game--;

                    //Включение 2х кнопок
                    flag = true;

                    int index = rnd.Next(symbols.Length);
                    randomOne = rnd.Next(1000);
                    randomTwo = rnd.Next(1000);

                    randomSymbol = symbols[index];

                    txtBlockMain.Text = randomSymbol;

                    txtBlockLeft.Visible = false;
                    txtBlockRight.Visible = false;

                    //Пикчи знак "?"
                    blockLeft.Image = (Image)Properties.Resources.ResourceManager.GetObject("unknownBlock");
                    blockRight.Image = (Image)Properties.Resources.ResourceManager.GetObject("unknownBlock");

                    if (Information.Finance < 10)
                    {
                        buttonStart.Enabled = false;
                        topUpAccount.Enabled = true;
                    }
                    else
                    {
                        buttonStart.Enabled = true;
                        topUpAccount.Enabled = false;
                    }

                    SetLabels();
                }
            };
        }

        public void Main()
        {
            blockLeft.Click += (obj, e) =>
            {
                if (flag == true && Information.Finance > 0)
                {
                    blockLeft.Image = (Image)Properties.Resources.ResourceManager.GetObject("closeBlock");
                    blockRight.Image = (Image)Properties.Resources.ResourceManager.GetObject("closeBlock");

                    txtBlockLeft.Text = "" + randomOne;
                    txtBlockRight.Text = "" + randomTwo;

                    txtBlockLeft.Visible = true;
                    txtBlockRight.Visible = true;

                    if (randomSymbol == ">")
                    {
                        if (randomOne > randomTwo)
                        {
                            win = true;
                            CashAndXP();

                            for (int i = 0; i < labels.Length; i++)
                                labels[i].BackColor = Color.White;

                            labels[count].BackColor = Color.Aqua;

                            labels[count].Text = ($"{randomOne} {randomSymbol} {randomTwo} — победа");

                            count++;
                            if (count == 4)
                                count = 0;

                            Form1.Write();
                            Form1.Encode();
                        }

                        else if (randomOne < randomTwo)
                        {
                            win = false;
                            CashAndXP();

                            System.Diagnostics.Debug.WriteLine($"Count: {count}");

                            for (int i = 0; i < labels.Length; i++)
                                labels[i].BackColor = Color.White;

                            labels[count].BackColor = Color.Aqua;

                            labels[count].Text = ($"{randomOne} {randomSymbol} {randomTwo} — проигрыш");
                            count++;
                            if (count == 4)
                                count = 0;

                            Form1.Write();
                            Form1.Encode();
                        }
                    }
                    else if (randomSymbol == "<")
                    {
                        if (randomOne < randomTwo)
                        {
                            win = true;
                            CashAndXP();

                            System.Diagnostics.Debug.WriteLine($"ONE {randomOne} {randomSymbol} TWO {randomTwo}");
                            System.Diagnostics.Debug.WriteLine($"Count: {count}");

                            for (int i = 0; i < labels.Length; i++)
                                labels[i].BackColor = Color.White;

                            labels[count].BackColor = Color.Aqua;

                            labels[count].Text = ($"{randomOne} {randomSymbol} {randomTwo} — победа");
                            count++;
                            if (count == 4)
                                count = 0;

                            Form1.Write();
                            Form1.Encode();
                        }

                        else if (randomOne > randomTwo)
                        {
                            win = false;
                            CashAndXP();

                            System.Diagnostics.Debug.WriteLine($"ONE {randomOne} {randomSymbol} TWO {randomTwo}");
                            System.Diagnostics.Debug.WriteLine($"Count: {count}");

                            for (int i = 0; i < labels.Length; i++)
                                labels[i].BackColor = Color.White;

                            labels[count].BackColor = Color.Aqua;

                            labels[count].Text = ($"{randomOne} {randomSymbol} {randomTwo} — проигрыш");
                            count++;
                            if (count == 4)
                                count = 0;
                        }
                    }

                    if (Information.Finance < 10)
                        buttonStart.Enabled = false;
                    else
                        buttonStart.Enabled = true;

                    Form1.LvLCheck();
                    SetLabels();

                    if (Information.StaticXP >= 8000)
                        closeLuckyTreeUpAccount.Visible = false;

                    flag = false;
                }
            };

            blockRight.Click += (obj, e) =>
            {
                if (flag == true && Information.Finance > 0)
                {
                    blockRight.Image = (Image)Properties.Resources.ResourceManager.GetObject("closeBlock");
                    blockLeft.Image = (Image)Properties.Resources.ResourceManager.GetObject("closeBlock");

                    txtBlockLeft.Text = "" + randomOne;
                    txtBlockRight.Text = "" + randomTwo;

                    txtBlockLeft.Visible = true;
                    txtBlockRight.Visible = true;

                    if (randomSymbol == ">")
                    {
                        if (randomTwo > randomOne)
                        {
                            win = true;
                            CashAndXP();

                            System.Diagnostics.Debug.WriteLine($"TWO {randomTwo} {randomSymbol} ONE {randomOne}");

                            for (int i = 0; i < labels.Length; i++)
                                labels[i].BackColor = Color.White;

                            labels[count].BackColor = Color.Aqua;

                            labels[count].Text = ($"{randomTwo} {randomSymbol} {randomOne} — победа");
                            count++;
                            if (count == 4)
                                count = 0;

                            Form1.Write();
                            Form1.Encode();
                        }

                        else if (randomTwo < randomOne)
                        {
                            win = false;
                            CashAndXP();

                            System.Diagnostics.Debug.WriteLine($"TWO {randomTwo} {randomSymbol} ONE {randomOne}");

                            for (int i = 0; i < labels.Length; i++)
                                labels[i].BackColor = Color.White;

                            labels[count].BackColor = Color.Aqua;

                            labels[count].Text = ($"{randomTwo} {randomSymbol} {randomOne} - проигрыш");
                            count++;
                            if (count == 4)
                                count = 0;

                            Form1.Write();
                            Form1.Encode();
                        }
                    }
                    else if (randomSymbol == "<")
                    {
                        if (randomTwo < randomOne)
                        {
                            win = true;
                            CashAndXP();

                            System.Diagnostics.Debug.WriteLine($"TWO {randomTwo} {randomSymbol} ONE {randomOne}");

                            for (int i = 0; i < labels.Length; i++)
                                labels[i].BackColor = Color.White;

                            labels[count].BackColor = Color.Aqua;

                            labels[count].Text = ($"{randomTwo} {randomSymbol} {randomOne} - победа");
                            count++;
                            if (count == 4)
                                count = 0;

                            Form1.Write();
                            Form1.Encode();
                        }

                        else if (randomTwo > randomOne)
                        {
                            win = false;
                            CashAndXP();

                            System.Diagnostics.Debug.WriteLine($"TWO {randomTwo} {randomSymbol} ONE {randomOne}");

                            for (int i = 0; i < labels.Length; i++)
                                labels[i].BackColor = Color.White;

                            labels[count].BackColor = Color.Aqua;

                            labels[count].Text = ($"{randomTwo} {randomSymbol} {randomOne} - проигрыш");
                            count++;
                            if (count == 4)
                                count = 0;

                            Form1.Write();
                            Form1.Encode();
                        }
                    }

                    if (Information.Finance < 10)
                        buttonStart.Enabled = false;
                    else
                        buttonStart.Enabled = true;

                    Form1.LvLCheck();
                    SetLabels();

                    if (Information.StaticXP >= 8000)
                        closeLuckyTreeUpAccount.Visible = false;

                    flag = false;
                }
            };

            chestRandom.Click += (obj, e) =>
            {
                if (Information.Finance >= 1000 + (Information.Xp * 25 / 100))
                {
                    Information.Finance -= 1000 + (Information.Xp * 25 / 100);

                    if (!click)
                    {
                        int randomItem = rnd.Next(0, 5);
                        int getRandom;

                        DialogResult result = DialogResult.None;

                        player.SoundLocation = @"./Sounds/open_chest.wav";

                        if (Information.Sound)
                            player.Play();

                        chestRandom.Image = (Image)Properties.Resources.ResourceManager.GetObject("openedChest");
                        chestRandom.Location = new Point(67, 394);

                        switch (randomItem)
                        {
                            case 0:
                                result = MessageBox.Show("Ты получил: НИЧЕГО ");
                                break;
                            case 1:
                                result = MessageBox.Show("Ты получил: " + (getRandom = rnd.Next(1, 10000)) + " $");
                                Information.Finance += getRandom;
                                break;
                            case 2:
                                result = MessageBox.Show("Ты получил: предмет x2 XP - " + (getRandom = rnd.Next(1, 30)) + "шт");
                                Information.ItemOne_2x_XP += getRandom;
                                break;
                            case 3:
                                result = MessageBox.Show("Ты получил: предмет Win-win game - " + (getRandom = rnd.Next(1, 8)) + "шт");
                                Information.ItemTwo_WW_Game += getRandom;
                                break;
                            case 4:
                                result = MessageBox.Show("Ты получил: " + (getRandom = rnd.Next(1, 500)) + " XP");

                                if (Information.Xp < Information.StaticXP)
                                {
                                    Information.Xp += getRandom;
                                }
                                else
                                {
                                    Information.Xp += getRandom;
                                    Information.StaticXP += getRandom;
                                }

                                break;
                            case 5:
                                result = MessageBox.Show("Ты получил: предмет XP => Cash - " + (getRandom = rnd.Next(1, 3)) + "шт");
                                Information.ItemFour_XP_Cash += getRandom;
                                break;
                        }

                        getRandom = 0;
                        click = true;

                        if (result == DialogResult.OK)
                        {
                            player.SoundLocation = @"./Sounds/close_chest.wav";

                            if (Information.Sound)
                                player.Play();

                            chestRandom.Image = (Image)Properties.Resources.ResourceManager.GetObject("closedChest");
                            chestRandom.Location = new Point(57, 426);

                            click = false;
                        }
                    }

                    Form1.LvLCheck();
                    SetLabels();

                    if (Information.StaticXP >= 8000)
                        closeLuckyTreeUpAccount.Visible = false;

                    Form1.Write();
                    Form1.Encode();
                }
            };

            topUpAccount.Click += (obj, e) =>
            {
                if (Information.Finance <= 9)
                {
                    if (Information.Lvl == 0)
                    {
                        if (Information.ItemSix_x2_TopUpAcc)
                            Information.Finance += (250 * (Information.Lvl + 1)) * 2;
                        else
                            Information.Finance += 250 * (Information.Lvl + 1);
                    }

                    else
                    {
                        if (Information.ItemSix_x2_TopUpAcc)
                            Information.Finance += (250 + (100 * Information.Lvl) * Information.Lvl) * 2;
                        else
                            Information.Finance += 250 + (100 * Information.Lvl) * Information.Lvl;
                    }

                    labelFinance.Text = "Бюджет: " + Information.Finance.ToString();

                    buttonStart.Enabled = true;

                    Form1.Write();
                    Form1.Encode();

                    if (Information.Finance > 0)
                        topUpAccount.Enabled = false;
                }
            };

            btnMusicOffOn.Click += (obj, e) =>
            {
                if (Information.Music)
                {
                    Information.Music = false;

                    Form1.Write();
                    Form1.Encode();

                    if (!Information.Music)
                        Form1.Player.close();

                    btnMusicOffOn.BackgroundImage = (Image)Properties.Resources.ResourceManager.GetObject("music_off");
                }

                else
                {
                    Information.Music = true;

                    Form1.Write();
                    Form1.Encode();

                    Form1.PlayNextSound();
                    btnMusicOffOn.BackgroundImage = (Image)Properties.Resources.ResourceManager.GetObject("music_on");
                }
            };

            btnSoundOffOn.Click += (obj, e) =>
            {
                if (Information.Sound)
                {
                    Information.Sound = false;

                    Form1.Write();
                    Form1.Encode();

                    btnSoundOffOn.BackgroundImage = (Image)Properties.Resources.ResourceManager.GetObject("sound_off");
                }

                else
                {
                    Information.Sound = true;

                    Form1.Write();
                    Form1.Encode();

                    btnSoundOffOn.BackgroundImage = (Image)Properties.Resources.ResourceManager.GetObject("sound_on");
                }
            };
        }

        void Mus_Sound()
        {
            if (!Information.Music)
            {
                Form1.Player.close();

                btnMusicOffOn.BackgroundImage = (Image)Properties.Resources.ResourceManager.GetObject("music_off");
            }
            else
            {
                if (Information.Music)
                {
                    btnMusicOffOn.BackgroundImage = (Image)Properties.Resources.ResourceManager.GetObject("music_on");
                }
            }

            if (!Information.Sound)
            {
                btnSoundOffOn.BackgroundImage = (Image)Properties.Resources.ResourceManager.GetObject("sound_off");
            }

            else
            {
                btnSoundOffOn.BackgroundImage = (Image)Properties.Resources.ResourceManager.GetObject("sound_on");
            }
        }

        void CashAndXP()
        {
            if (win)
            {
                Form1.GetToCash();

                if (Information.ItemOne_2x_XP > 0)
                {
                    labelAddXpTwo.Text = "+" + countXP.ToString();
                    countXP = 10;

                    if (Information.Xp < Information.StaticXP)
                    {
                        Information.Xp += countXP;
                    }
                    else
                    {
                        Information.Xp += countXP;
                        Information.StaticXP += countXP;
                    }

                    Information.ItemOne_2x_XP--;
                    labelAddXpOne.Text = "+" + countXP.ToString();
                }
                else
                {
                    labelAddXpTwo.Text = "+" + countXP.ToString();
                    countXP = 5;

                    if (Information.Xp < Information.StaticXP)
                    {
                        Information.Xp += countXP;
                    }
                    else
                    {
                        Information.Xp += countXP;
                        Information.StaticXP += countXP;
                    }

                    labelAddXpOne.Text = "+" + countXP.ToString();
                }

                labelAddCashTwo.Text = "+" + countCash.ToString();
                countCash = Information.Finance * 40 / 100;
                Information.Finance += Information.Finance * 40 / 100;
                labelAddCashOne.Text = "+" + countCash.ToString();

                if (Information.ItemThree_Additional_Perc)
                    Information.Finance += countCash * 25 / 100;
            }
            else
            {
                player.SoundLocation = @"./Sounds/lose.wav";
                if (Information.Sound)
                    player.Play();

                if (Information.ItemOne_2x_XP > 0)
                {
                    labelAddXpTwo.Text = "+" + countXP.ToString();
                    countXP = 2;

                    if (Information.Xp < Information.StaticXP)
                    {
                        Information.Xp += countXP;
                    }
                    else
                    {
                        Information.Xp += countXP;
                        Information.StaticXP += countXP;
                    }

                    Information.ItemOne_2x_XP--;
                    labelAddXpOne.Text = "+" + countXP.ToString();
                }
                else
                {
                    labelAddXpTwo.Text = "+" + countXP.ToString();
                    countXP = 1;

                    if (Information.Xp < Information.StaticXP)
                    {
                        Information.Xp += countXP;
                    }
                    else
                    {
                        Information.Xp += countXP;
                        Information.StaticXP += countXP;
                    }

                    labelAddXpOne.Text = "+" + countXP.ToString();
                }
            }
        }

        private void time_Tick(object sender, System.EventArgs e)
        {
            if (counter == 10)
            {
                timer1.Stop();
                MessageBox.Show("Ты наверное уже плачешь от того, что увидел. Да, прости.\n" +
                    "Я старался нарисовать адекватно, но ты попробуй не обращать внимание,\n" +
                    "посмотри на небо и подумай о хорошем.");

                MessageBox.Show("А теперь о игре, нажав играть, сверху генерируется случайный знак неравенства, " +
                    "относительно знака, тебе необходимо выбрать значение, которое истинно в этом неравенстве.\n" +
                    "А ещё слева сверху ты можешь увидеть историю игр.");

                MessageBox.Show("Ох, кстати, чуть не забыл, на травушке есть сундук, " +
                    "стоимость его открытия 1000$ + 25% от твоего опыта.\n" +
                    "Открыв его, тебе выпадает случайная вещь.");

                timer1.Enabled = false;
            }
            else
            {
                counter = counter + 1;
            }
        }

        void LuckyTreeTime()
        {
            luckyTreeTime.Interval = 1000;
            luckyTreeTime.Enabled = true;
            luckyTreeTime.Tick += new System.EventHandler(TimerOfLuckyTreeTime);
        }

        void TimerOfLuckyTreeTime(object sender, System.EventArgs e)
        {
            Personal_data.LuckyTree_time++;
            Form1.Write();
            Form1.Encode();
        }

        void SetLabels()
        {
            labelFinance.Text = "Бюджет: " + Information.Finance;
            labelLvL.Text = "Уровень: " + Information.Lvl;
            labelXP.Text = "Опыт: " + Information.Xp;

            labelInfoItemOne.Text = Information.ItemOne_2x_XP.ToString();
            labelInfoItemTwo.Text = Information.ItemTwo_WW_Game.ToString();
            labelInfoItemThree.Text = Information.ItemFour_XP_Cash.ToString();

            if (Information.ItemFive_Pass_Accumulation)
                labelInfoItemFive.Text = "Вкл";
            else
                labelInfoItemFour.Text = "Откл";

            if (Information.ItemThree_Additional_Perc)
                labelInfoItemFour.Text = "Вкл";
            else
                labelInfoItemFour.Text = "Откл";
        }
    }
}