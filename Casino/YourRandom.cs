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

namespace Casino
{
    public partial class YourRandom : Form
    {
        Random rnd = new Random();
        Timer timer;
        Timer yourRandomTime = new Timer();

        long levy;
        long availableEarn;
        long starter;

        int difficult = 1;
        int countXP;
        int xpGained;

        long countCash, countSubstractCash, counter;

        char check = 'C';

        public YourRandom()
        {
            InitializeComponent();
            buttonBack.Click += (obj, e) =>
            {
                Form1.SoundClickButton();
                yourRandomTime.Stop();
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

            InitTimer();
            Start();
            Mus_Sound();

            YourRandomTime();
        }

        public void Start()
        {
            ToolTip t = new ToolTip();
            t.SetToolTip(hintItemOne, "За каждую игру умножается полученный опыт в два раза");
            t.SetToolTip(hintItemTwo, "Бонус позволяет играть в игры, не тратя денег");
            t.SetToolTip(hintItemThree, "Последующие выигрышные деньги превращаются в опыт");
            t.SetToolTip(hintItemFour, "После каждой победы, дополнительно \nвносится 25% от выигрышной суммы");
            t.SetToolTip(hintItemFive, "Даёт пассивный эффект, после каждой игры, \nв бюджет возвращается 10% от потраченной суммы");

            BackColor = Color.FromArgb(25, 25, 25);

            if (Form1.limitsOff < 1)
            {
                availableEarn = Information.StaticLvL * 20000 * difficult;
                starter = availableEarn;
            }

            numericUpDown1.TextChanged += numericUpDown1_TextChanged;

            ((TextBox)numericUpDown1.Controls[1]).MaxLength = 2;

            if (Information.StaticXP >= 2000)
                closeTopUpAccount.Visible = false;

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

            buttonStart.Click += (obj, e) =>
            {
                if (Information.Finance >= 10 && availableEarn > 0 || Information.Finance >= 10 && Form1.limitsOff >= 1)
                {
                    labelAddCashOne.Text = "+0";
                    levy = Information.Finance * (10 * difficult) / 100;

                    if (Information.ItemTwo_WW_Game > 0)
                    {
                        countSubstractCash = 0;
                        Information.Finance = Information.Finance;
                        labelSubstractCashOne.Text = "-" + countSubstractCash.ToString();
                    }
                    else
                    {
                        if (difficult == 1)
                        {
                            labelSubstractCashTwo.Text = "-" + countSubstractCash.ToString();
                            countSubstractCash = 200 + rnd.Next(0, 200);
                            Information.Finance -= countSubstractCash;
                            labelSubstractCashOne.Text = "-" + countSubstractCash.ToString();
                        }
                        else if (difficult == 2)
                        {
                            labelSubstractCashTwo.Text = "-" + countSubstractCash.ToString();
                            countSubstractCash = Information.Xp * 10 * difficult / 100 * 2 + 200 + rnd.Next(0, 200);
                            Information.Finance -= countSubstractCash;
                            labelSubstractCashOne.Text = "-" + countSubstractCash.ToString();
                        }
                        else
                        {
                            labelSubstractCashTwo.Text = "-" + countSubstractCash.ToString();
                            countSubstractCash = levy;
                            Information.Finance -= levy;
                            labelSubstractCashOne.Text = "-" + countSubstractCash.ToString();
                        }
                    }

                    if (Information.ItemFive_Pass_Accumulation)
                        Information.Finance += levy * 10 / 100;

                    Form1.PayToTurn();

                    xpGained++;

                    Form1.Write();
                    Form1.Encode();
                    labelFinance.Text = ($"Бюджет: {Information.Finance}");

                    trackBar1.Value = rnd.Next(1, trackBar1.Maximum);
                    labelCount.Text = "Значение: " + trackBar1.Value.ToString();

                    if (difficult == 1)
                    {
                        if (numericUpDown1.Value >= trackBar1.Value - 2 && numericUpDown1.Value <= trackBar1.Value + 2)
                        {
                            labelAddCashTwo.Text = "+" + countCash.ToString();
                            countCash = 500 + rnd.Next(0, 150);
                            Information.Finance += countCash;

                            if (Form1.limitsOff < 1)
                                availableEarn -= countCash;

                            labelAddCashOne.Text = "+" + countCash.ToString();

                            if (Information.ItemThree_Additional_Perc)
                            {
                                Information.Finance += countCash * 25 / 100;

                                if (Form1.limitsOff < 1)
                                    availableEarn -= countCash;
                            }

                            if (trackBar1.Value - numericUpDown1.Value == 2 || trackBar1.Value - numericUpDown1.Value == -2)
                                xpGained += 1 * difficult;
                            else if (trackBar1.Value - numericUpDown1.Value == 1 || trackBar1.Value - numericUpDown1.Value == -1)
                                xpGained += 2 * difficult;
                            else if (trackBar1.Value - numericUpDown1.Value == 0)
                                xpGained += 3 * difficult;

                            Personal_data.Wins++;

                            Form1.LvLCheck();
                            Form1.Write();
                            Form1.Encode();
                            labelFinance.Text = ($"Бюджет: {Information.Finance}");

                            Form1.GetToCash();

                            //MessageBox.Show("WIN");
                        }
                        else
                        {
                            labelAddCashOne.Text = "+0";
                            labelAddCashTwo.Text = "+" + countCash.ToString();

                            Personal_data.Losses++;

                            Form1.LvLCheck();
                            Form1.Write();
                            Form1.Encode();
                        }
                    }

                    else if (difficult == 2)
                    {
                        if (numericUpDown1.Value >= trackBar1.Value - 5 && numericUpDown1.Value <= trackBar1.Value + 5)
                        {
                            labelAddCashTwo.Text = "+" + countCash.ToString();
                            countCash = Information.Xp * 10 * difficult / 100 * 2 * difficult + 500 + rnd.Next(0, 150);
                            Information.Finance += countCash;

                            if (Form1.limitsOff < 1)
                                availableEarn -= countCash;
                            labelAddCashOne.Text = "+" + countCash.ToString();

                            if (Information.ItemThree_Additional_Perc)
                            {
                                Information.Finance += countCash * 25 / 100;

                                if (Form1.limitsOff < 1)
                                    availableEarn -= countCash;
                            }

                            if (trackBar1.Value - numericUpDown1.Value == 5 || trackBar1.Value - numericUpDown1.Value == -5)
                                xpGained += 1 * difficult;
                            else if (trackBar1.Value - numericUpDown1.Value == 4 || trackBar1.Value - numericUpDown1.Value == -4)
                                xpGained += 2 * difficult;
                            else if (trackBar1.Value - numericUpDown1.Value == 3 || trackBar1.Value - numericUpDown1.Value == -3)
                                xpGained += 3 * difficult;
                            else if (trackBar1.Value - numericUpDown1.Value == 2 || trackBar1.Value - numericUpDown1.Value == -2)
                                xpGained += 4 * difficult;
                            else if (trackBar1.Value - numericUpDown1.Value == 1 || trackBar1.Value - numericUpDown1.Value == -1)
                                xpGained += 5 * difficult;
                            else if (trackBar1.Value - numericUpDown1.Value == 0)
                                xpGained += 6 * difficult;

                            Personal_data.Wins++;

                            Form1.LvLCheck();
                            Form1.Write();
                            Form1.Encode();
                            labelFinance.Text = ($"Бюджет: {Information.Finance}");

                            Form1.GetToCash();

                            //MessageBox.Show("WIN");
                        }
                        else
                        {
                            Personal_data.Losses++;

                            Form1.LvLCheck();
                            Form1.Write();
                            Form1.Encode();
                        }
                    }

                    else if (difficult == 3)
                    {
                        if (numericUpDown1.Value >= trackBar1.Value - 10 && numericUpDown1.Value <= trackBar1.Value + 10)
                        {
                            labelAddCashTwo.Text = "+" + countCash.ToString();
                            countCash = levy * 2 * difficult;
                            Information.Finance += levy * 2 * difficult;

                            if (Form1.limitsOff < 1)
                                availableEarn -= levy * 2 * difficult;
                            labelAddCashOne.Text = "+" + countCash.ToString();

                            if (Information.ItemThree_Additional_Perc)
                            {
                                Information.Finance += countCash * 25 / 100;

                                if (Form1.limitsOff < 1)
                                    availableEarn -= levy * 2 * difficult;
                            }

                            if (trackBar1.Value - numericUpDown1.Value == 10 || trackBar1.Value - numericUpDown1.Value == -10)
                                xpGained += 1 * difficult;
                            else if (trackBar1.Value - numericUpDown1.Value == 9 || trackBar1.Value - numericUpDown1.Value == -9)
                                xpGained += 2 * difficult;
                            else if (trackBar1.Value - numericUpDown1.Value == 8 || trackBar1.Value - numericUpDown1.Value == -8)
                                xpGained += 3 * difficult;
                            else if (trackBar1.Value - numericUpDown1.Value == 7 || trackBar1.Value - numericUpDown1.Value == -7)
                                xpGained += 4 * difficult;
                            else if (trackBar1.Value - numericUpDown1.Value == 6 || trackBar1.Value - numericUpDown1.Value == -6)
                                xpGained += 5 * difficult;
                            else if (trackBar1.Value - numericUpDown1.Value == 5 || trackBar1.Value - numericUpDown1.Value == -5)
                                xpGained += 6 * difficult;
                            else if (trackBar1.Value - numericUpDown1.Value == 4 || trackBar1.Value - numericUpDown1.Value == -4)
                                xpGained += 7 * difficult;
                            else if (trackBar1.Value - numericUpDown1.Value == 3 || trackBar1.Value - numericUpDown1.Value == -3)
                                xpGained += 8 * difficult;
                            else if (trackBar1.Value - numericUpDown1.Value == 2 || trackBar1.Value - numericUpDown1.Value == -2)
                                xpGained += 9 * difficult;
                            else if (trackBar1.Value - numericUpDown1.Value == 1 || trackBar1.Value - numericUpDown1.Value == -1)
                                xpGained += 10 * difficult;
                            else if (trackBar1.Value - numericUpDown1.Value == 0)
                                xpGained += 11 * difficult;

                            Personal_data.Wins++;

                            Form1.LvLCheck();
                            Form1.Write();
                            Form1.Encode();
                            labelFinance.Text = ($"Бюджет: {Information.Finance}");

                            Form1.GetToCash();

                            //MessageBox.Show("WIN");
                        }
                        else
                        {
                            Personal_data.Losses++;

                            Form1.LvLCheck();
                            Form1.Write();
                            Form1.Encode();
                        }
                    }

                    if (Information.ItemTwo_WW_Game > 0)
                        Information.ItemTwo_WW_Game--;

                    if (Information.ItemOne_2x_XP > 0)
                    {
                        labelAddXpTwo.Text = "+" + countXP.ToString();
                        countXP = xpGained * 2;

                        if (Information.Xp < Information.StaticXP)
                        {
                            Information.Xp += xpGained * 2;
                        }
                        else
                        {
                            Information.Xp += xpGained * 2;
                            Information.StaticXP += xpGained * 2;
                        }

                        Information.ItemOne_2x_XP--;
                        labelAddXpOne.Text = "+" + countXP.ToString();
                    }
                    else
                    {
                        labelAddXpTwo.Text = "+" + countXP.ToString();
                        countXP = xpGained;

                        if (Information.Xp < Information.StaticXP)
                        {
                            Information.Xp += xpGained;
                        }
                        else
                        {
                            Information.Xp += xpGained;
                            Information.StaticXP += xpGained;
                        }

                        labelAddXpOne.Text = "+" + countXP.ToString();
                    }

                    xpGained = 0;

                    if (Information.Finance < 0)
                        Information.Finance = 0;

                    Form1.LvLCheck();
                    Form1.Write();
                    Form1.Encode();
                    SetLabels();
                }

                if (Information.StaticXP >= 1000)
                    closeTopUpAccount.Visible = false;

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

                if (Form1.limitsOff < 1)
                {
                    if (availableEarn <= 0)
                        buttonStart.Enabled = false;
                }

            };

            buttonSubstractDiff.Click += (obj, e) =>
            {
                if (difficult > 1)
                {
                    Form1.SoundClickButton();
                    difficult--;
                    CheckDifficult();
                    numericUpDown1.Value = 1;

                    if (Form1.limitsOff < 1)
                    {
                        if (difficult == 2 && availableEarn <= Information.StaticLvL * 20000 * 3 && availableEarn >= Information.StaticLvL * 20000 * 2)
                            availableEarn = Information.StaticLvL * 20000 * 2;
                        if (difficult == 1 && availableEarn <= Information.StaticLvL * 20000 * 2 && availableEarn >= Information.StaticLvL * 20000)
                            availableEarn = Information.StaticLvL * 20000;

                        if (availableEarn <= 0)
                            availableEarn = 0;
                    }
                }

                SetLabels();
            };

            buttonAddDiff.Click += (obj, e) =>
            {
                if (difficult < 3)
                {
                    Form1.SoundClickButton();
                    difficult++;
                    CheckDifficult();

                    if (Form1.limitsOff < 1)
                    {
                        if (availableEarn >= Information.StaticLvL * 20000 * (difficult - 1))
                            availableEarn = Information.StaticLvL * 20000 * (difficult);

                        if (availableEarn > 0 && availableEarn < Information.StaticLvL * 20000 * (difficult - 1))
                        {
                            availableEarn = (Information.StaticLvL * 20000 * (difficult) - (starter - availableEarn));
                            starter = availableEarn;
                        }

                        if (availableEarn <= 0)
                            availableEarn = 0;
                    }
                }

                SetLabels();
            };

            topUpAccount.Click += (obj, e) =>
            {
                if (Information.Finance <= 9)
                {
                    Form1.GetToCash();

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

        void numericUpDown1_TextChanged(object sender, EventArgs e)
        {
            if (!Valid(numericUpDown1.Text) && numericUpDown1.Text != "")
            {
                labelHint.Text = "Попытка ввести неверное значение";
                buttonStart.Enabled = false;
            }
            else
            {
                labelHint.Text = "";
                buttonStart.Enabled = true;
            }

            if (numericUpDown1.Text == "")
                numericUpDown1.Text = "1";

            if (Form1.limitsOff < 1)
            {
                if (availableEarn <= 0)
                    buttonStart.Enabled = false;
            }

        }

        public void CheckDifficult()
        {
            if (difficult == 1)
            {
                trackBar1.Maximum = 10;
                labelInformation.Text = "Выбери число от 1 до " + trackBar1.Maximum;
                ((TextBox)numericUpDown1.Controls[1]).MaxLength = 2;

                trackBar1.Value = 1;
                labelCount.Text = "Значение: " + trackBar1.Value.ToString();
            }

            if (difficult == 2)
            {
                trackBar1.Maximum = 50;
                labelInformation.Text = "Выбери число от 1 до " + trackBar1.Maximum;
                ((TextBox)numericUpDown1.Controls[1]).MaxLength = 2;

                trackBar1.Value = 1;
                labelCount.Text = "Значение: " + trackBar1.Value.ToString();
            }

            if (difficult == 3)
            {
                trackBar1.Maximum = 100;
                labelInformation.Text = "Выберите число от 1 до " + trackBar1.Maximum;
                ((TextBox)numericUpDown1.Controls[1]).MaxLength = 3;

                trackBar1.Value = 1;
                labelCount.Text = "Значение: " + trackBar1.Value.ToString();
            }
        }

        bool Valid(string text)
        {
            int i = 0;
            if (!int.TryParse(text, out i))
                return false;
            if (i < 0 || i > trackBar1.Maximum)
                return false;
            else return true;
        }

        void SetLabels()
        {
            labelHint.Text = "";
            labelDifficult.Text = "Сложность: " + difficult;
            labelFinance.Text = ($"Бюджет: {Information.Finance}");
            labelLvL.Text = "Уровень: " + Information.Lvl;
            labelXP.Text = "Опыт: " + Information.Xp;

            if (Form1.limitsOff < 1)
            {
                if (availableEarn <= 0)
                {
                    labelAvailableEarn.BackColor = Color.Black;
                    labelAvailableEarn.ForeColor = Color.Red;
                    labelAvailableEarn.Text = $"Лимит исчерпан :'(";
                    buttonStart.Enabled = false;
                }
                else
                {
                    labelAvailableEarn.Text = $"Лимит: {availableEarn}$";
                    buttonStart.Enabled = true;
                }
            }
            else
            {
                labelAvailableEarn.Text = "Неограниченно";
            }

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

        private void time_Tick(object sender, System.EventArgs e)
        {
            if (counter == 10)
            {
                timer1.Stop();
                MessageBox.Show("Здесь победа на 50% зависит от тебя или может на 30%, хотя, может и 10%, " +
                    "не знаю, думай сам.\n" +
                    "Принцип игры: ты выбираешь число, которое как ты считаешь может выпасть, " +
                    "нажимаешь играть и если твоё число близко к выпавшему числу, то ты победил. Всё :)");
                timer1.Enabled = false;
            }
            else
            {
                counter = counter + 1;
            }
        }

        void YourRandomTime()
        {
            yourRandomTime.Interval = 1000;
            yourRandomTime.Enabled = true;
            yourRandomTime.Tick += new System.EventHandler(TimerOfYourRandomTime);
        }

        void TimerOfYourRandomTime(object sender, System.EventArgs e)
        {
            Personal_data.YourRandom_time++;
            Form1.Write();
            Form1.Encode();
        }

        public void InitTimer()
        {
            timer = new Timer();
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Start();
        }

        static Color HsvToRgb(float h, float s, float v)
        {
            int i;
            float f, p, q, t;

            if (s < float.Epsilon)
            {
                int c = (int)(v * 255);
                return Color.FromArgb(c, c, c);
            }

            h /= 10;
            i = (int)Math.Floor(h);
            f = h - i;
            p = v * (1 - s);
            q = v * (1 - s * f);
            t = v * (1 - s * (1 - f));

            float r, g, b;
            switch (i)
            {
                case 0: r = v; g = t; b = p; break;
                case 1: r = q; g = v; b = p; break;
                case 2: r = p; g = v; b = t; break;
                case 3: r = p; g = q; b = v; break;
                case 4: r = t; g = p; b = v; break;
                default: r = v; g = p; b = q; break;
            }

            return Color.FromArgb((int)(r * 255), (int)(g * 255), (int)(b * 255));
        }

        float h = 0;
        private void Timer_Tick(object sender, EventArgs e)
        {
            h++;
            if (h >= 360) h = 0;
            panel1.BackColor = HsvToRgb(h, 1f, 1f);
        }
    }
}
