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
using Casino.Properties;

namespace Casino
{
    public partial class LuckyRandom : Form
    {
        SoundPlayer player = new SoundPlayer();

        bool quit = true;

        Timer timer;
        Timer luckyRandomTime = new Timer();

        Random rnd = new Random();

        List<int> DefValues = new List<int>(25) { 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 3, 3, 3, 4 }; //25 nums
        List<int> copyValues = new List<int>(25) { 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 3, 3, 3, 4 };
        List<int> DefValuesLvLTwo = new List<int>(25) { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 2, 2, 2, 3, 3, 3, 3, 3, 3, 4, 4, 4, 5, 5 };
        List<int> DefValuesLvLThree = new List<int>(25) { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5 };

        Color[] colours = new Color[] { Color.AliceBlue, Color.YellowGreen, Color.AntiqueWhite, Color.Aqua, Color.Aquamarine,
        Color.Azure, Color.Beige, Color.Bisque, Color.BlanchedAlmond, Color.Blue, Color.BlueViolet, Color.Brown, Color.BurlyWood,
        Color.CadetBlue, Color.Chartreuse, Color.Chocolate, Color.Coral, Color.CornflowerBlue, Color.Cornsilk, Color.Crimson,
        Color.Cyan, Color.DarkBlue, Color.LightCoral, Color.LightCyan, Color.Lime, Color.LimeGreen, Color.Linen, Color.Magenta,
        Color.Maroon, Color.MediumAquamarine, Color.MistyRose, Color.Navy, Color.Orange, Color.Red, Color.Yellow, Color.Snow};

        List<int> score = new List<int>();

        int[,] values = new int[5, 5];

        long starter;
        long availableEarn;
        long money_Won;
        int difficult = 1;
        long countCash_1, countCash_2,
            countXP_1, countXP_2,
            countSubstractCash_1, countSubstractCash_2;

        //Массив отвечающий за множители
        int[] factors = new int[5];

        //Поле необходимо для расчёта алгоритма = "финансы * множитель1 / множитель2 * множитель3
        int iteration = 0;

        //Сколько можно открыть картинок
        int attempts = 0;

        PictureBox[,] arr = new PictureBox[5, 5];

        int counter;
        char check = 'E';

        public LuckyRandom()
        {
            InitializeComponent();
            buttonBack.Click += (obj, e) =>
            {
                Form1.SoundClickButton();
                luckyRandomTime.Stop();
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
            Mus_Sound();

            LuckyRandomTime();
        }

        public void Start()
        {
            ToolTip t = new ToolTip();
            t.SetToolTip(hintItemOne, "За каждую игру умножается полученный опыт в два раза");
            t.SetToolTip(hintItemTwo, "Бонус позволяет играть в игры, не тратя денег");
            t.SetToolTip(hintItemThree, "Последующие выигрышные деньги превращаются в опыт");
            t.SetToolTip(hintItemFour, "После каждой победы, дополнительно \nвносится 25% от выигрышной суммы");
            t.SetToolTip(hintItemFive, "Даёт пассивный эффект, после каждой игры, \nв бюджет возвращается 10% от потраченной суммы");

            if (Form1.limitsOff < 2)
            {
                availableEarn = 30000 * Information.StaticLvL * difficult;
                starter = availableEarn;
            }

            InitTimer();

            if (Information.StaticXP >= 3000)
                closeTopUpAccount.Visible = false;

            if (Information.Finance < 10)
            {
                buttonTurn.Enabled = false;
                topUpAccount.Enabled = true;
            }

            else
            {
                buttonTurn.Enabled = true;
                topUpAccount.Enabled = false;
            }

            SetLabels();

            buttonSubstractDifficult.Click += (obj, e) =>
            {
                if (difficult > 1)
                {
                    Form1.SoundClickButton();
                    difficult--;

                    if (Form1.limitsOff < 2)
                    {
                        if (difficult == 2 && availableEarn <= Information.StaticLvL * 30000 * 3 && availableEarn >= Information.StaticLvL * 30000 * 2)
                            availableEarn = Information.StaticLvL * 30000 * 2;
                        if (difficult == 1 && availableEarn <= Information.StaticLvL * 30000 * 2 && availableEarn >= Information.StaticLvL * 30000)
                            availableEarn = Information.StaticLvL * 30000;

                        if (availableEarn <= 0)
                            availableEarn = 0;
                    }

                }

                SetLabels();
            };

            buttonAddDifficult.Click += (obj, e) =>
            {
                if (difficult < 3)
                {
                    Form1.SoundClickButton();
                    difficult++;

                    if (Form1.limitsOff < 2)
                    {
                        if (availableEarn >= Information.StaticLvL * 30000 * (difficult - 1))
                            availableEarn = Information.StaticLvL * 30000 * (difficult);

                        if (availableEarn > 0 && availableEarn < Information.StaticLvL * 30000 * (difficult - 1))
                        {
                            availableEarn = (Information.StaticLvL * 30000 * (difficult) - (starter - availableEarn));
                            starter = availableEarn;
                        }

                        if (availableEarn <= 0)
                            availableEarn = 0;
                    }

                }
                SetLabels();
            };

            arr = new PictureBox[5, 5]{
                {box1_1, box1_2, box1_3, box1_4, box1_5},
                {box2_1, box2_2, box2_3, box2_4, box2_5},
                {box3_1, box3_2, box3_3, box3_4, box3_5},
                {box4_1, box4_2, box4_3, box4_4, box4_5},
                {box5_1, box5_2, box5_3, box5_4, box5_5}
            };

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    values[i, j] = DefValues[rnd.Next(DefValues.Count)];
                    DefValues.Remove(values[i, j]); //Удаление получившего значения из списка
                    //System.Diagnostics.Debug.Write(values[i, j]);
                }
            }

            int q = 0;
            int w = 0;

            for (q = 0; q < 5; q++)
            {
                for (w = 0; w < 5; w++)
                {
                    int iCopy = q;
                    int jCopy = w;

                    //Лямбда-функция выполняется после того, как все итерации пройдут. 
                    //Поэтому я объявил новые "копи" поля счётчиков (iCopy), (jCopy)
                    arr[iCopy, jCopy].Click += (obj, e) =>
                    {
                        iteration++;
                        if (attempts > 0)
                        {
                            arr[iCopy, jCopy].Image = (Image)Properties.Resources.ResourceManager.GetObject("scoreX" + values[iCopy, jCopy]);
                            arr[iCopy, jCopy].Enabled = false;

                            score.Add(values[iCopy, jCopy]);

                            factors[iCopy] = values[iCopy, jCopy];

                            if (iteration == 1)
                            {
                                if (score[0] != 0)
                                {
                                    if (Information.Sound)
                                    {
                                        if (values[iCopy, jCopy] == 1)
                                        {
                                            player.SoundLocation = @"./Sounds/magic_1.wav";
                                            player.Play();
                                        }
                                        else if (values[iCopy, jCopy] == 2)
                                        {
                                            player.SoundLocation = @"./Sounds/magic_2.wav";
                                            player.Play();
                                        }
                                        else if (values[iCopy, jCopy] == 3)
                                        {
                                            player.SoundLocation = @"./Sounds/magic_3.wav";
                                            player.Play();
                                        }
                                        else if (values[iCopy, jCopy] == 4)
                                        {
                                            player.SoundLocation = @"./Sounds/applause.wav";
                                            player.Play();
                                        }
                                        else if (values[iCopy, jCopy] == 5)
                                        {
                                            player.SoundLocation = @"./Sounds/triangle_win.wav";
                                            player.Play();
                                        }
                                    }

                                    countCash_1 = (Information.Finance * 5 / 100) * factors[iCopy] * difficult;
                                    //Information.Finance += (Information.Finance * 10 / 100) * factors[iCopy] * difficult;
                                    money_Won += (Information.Finance * 5 / 100) * factors[iCopy] * difficult;

                                    if (Form1.limitsOff < 2)
                                        availableEarn -= ((Information.Finance + money_Won) * 5 / 100) * factors[iCopy] * difficult;

                                    labelAddCashOne.Text = "+" + countCash_1.ToString();

                                    if (Information.ItemThree_Additional_Perc)
                                    {
                                        //Information.Finance += (Information.Finance * 10 / 100) * factors[iCopy] * difficult * 25 / 100;
                                        money_Won += (Information.Finance * 5 / 100) * factors[iCopy] * difficult * 25 / 100;

                                        if (Form1.limitsOff < 2)
                                            availableEarn -= ((Information.Finance + money_Won) * 5 / 100) * factors[iCopy] * difficult * 25 / 100;
                                    }

                                    if (Information.ItemOne_2x_XP > 0)
                                    {
                                        countXP_1 = (factors[iCopy] + difficult) * 2;

                                        if (Information.Xp < Information.StaticXP)
                                        {
                                            Information.Xp += (factors[iCopy] + difficult) * 2;
                                        }
                                        else
                                        {
                                            Information.Xp += (factors[iCopy] + difficult) * 2;
                                            Information.StaticXP += (factors[iCopy] + difficult) * 2;
                                        }

                                        labelAddXpOne.Text = "+" + countXP_1.ToString();
                                    }

                                    else
                                    {
                                        countXP_1 = factors[iCopy] + difficult;

                                        if (Information.Xp < Information.StaticXP)
                                        {
                                            Information.Xp += factors[iCopy] + difficult;
                                        }
                                        else
                                        {
                                            Information.Xp += factors[iCopy] + difficult;
                                            Information.StaticXP += factors[iCopy] + difficult;
                                        }

                                        labelAddXpOne.Text = "+" + countXP_1.ToString();
                                    }

                                    labelMoneyWon.Text = "Выиграно: " + money_Won.ToString();

                                    Personal_data.Wins++;

                                    Form1.LvLCheck();
                                    Form1.Write();
                                    Form1.Encode();
                                }
                                else
                                {
                                    if (values[iCopy, jCopy] == 0)
                                    {
                                        Form1.LaughMethod();
                                    }

                                    if (Information.ItemTwo_WW_Game > 0)
                                    {
                                        countSubstractCash_1 = 0;
                                        Information.Finance = Information.Finance;
                                        labelSubstractCashOne.Text = "-" + countSubstractCash_1.ToString();
                                    }

                                    else
                                    {
                                        countSubstractCash_1 = Information.Finance * ((difficult + 2) * 5) / 100;
                                        Information.Finance -= Information.Finance * ((difficult + 2) * 5) / 100;
                                        labelSubstractCashOne.Text = "-" + countSubstractCash_1.ToString();
                                    }

                                    Personal_data.Losses++;

                                    Form1.LvLCheck();
                                    Form1.Write();
                                    Form1.Encode();

                                    if (Information.StaticXP >= 3000)
                                        closeTopUpAccount.Visible = false;

                                    if (score[0] == 0)
                                    {
                                        attempts = 0;
                                        money_Won = 0;

                                        labelMoneyWon.Text = "Выиграно: " + money_Won.ToString();
                                    }
                                }
                            }
                            else if (iteration == 2)
                            {
                                if (score[1] != 0)
                                {
                                    if (Information.Sound)
                                    {
                                        if (values[iCopy, jCopy] == 1)
                                        {
                                            player.SoundLocation = @"./Sounds/magic_1.wav";
                                            player.Play();
                                        }
                                        else if (values[iCopy, jCopy] == 2)
                                        {
                                            player.SoundLocation = @"./Sounds/magic_2.wav";
                                            player.Play();
                                        }
                                        else if (values[iCopy, jCopy] == 3)
                                        {
                                            player.SoundLocation = @"./Sounds/magic_3.wav";
                                            player.Play();
                                        }
                                        else if (values[iCopy, jCopy] == 4)
                                        {
                                            player.SoundLocation = @"./Sounds/applause.wav";
                                            player.Play();
                                        }
                                        else if (values[iCopy, jCopy] == 5)
                                        {
                                            player.SoundLocation = @"./Sounds/triangle_win.wav";
                                            player.Play();
                                        }
                                    }

                                    labelAddCashTwo.Text = "+" + countCash_1.ToString();
                                    countCash_2 = (Information.Finance * 5 / 100) * factors[iCopy] * difficult;

                                    //Information.Finance += (Information.Finance * 10 / 100) * factors[iCopy] * difficult;
                                    money_Won += (Information.Finance * 5 / 100) * factors[iCopy] * difficult;

                                    if (Form1.limitsOff < 2)
                                        availableEarn -= ((Information.Finance + money_Won) * 5 / 100) * factors[iCopy] * difficult;

                                    labelAddCashOne.Text = "+" + countCash_2.ToString();

                                    if (Information.ItemThree_Additional_Perc)
                                    {
                                        //Information.Finance += (Information.Finance * 10 / 100) * factors[iCopy] * difficult * 25 / 100;

                                        money_Won += (Information.Finance * 5 / 100) * factors[iCopy] * difficult * 25 / 100;

                                        if (Form1.limitsOff < 2)
                                            availableEarn -= ((Information.Finance + money_Won) * 5 / 100) * factors[iCopy] * difficult * 25 / 100;
                                    }

                                    if (Information.ItemOne_2x_XP > 0)
                                    {
                                        labelAddXpTwo.Text = "+" + countXP_1.ToString();
                                        countXP_2 = (factors[iCopy] + difficult) * 2;

                                        if (Information.Xp < Information.StaticXP)
                                        {
                                            Information.Xp += (factors[iCopy] + difficult) * 2;
                                        }
                                        else
                                        {
                                            Information.Xp += (factors[iCopy] + difficult) * 2;
                                            Information.StaticXP += (factors[iCopy] + difficult) * 2;
                                        }

                                        labelAddXpOne.Text = "+" + countXP_2.ToString();
                                    }

                                    else
                                    {
                                        labelAddXpTwo.Text = "+" + countXP_1.ToString();
                                        countXP_2 = factors[iCopy] + difficult;

                                        if (Information.Xp < Information.StaticXP)
                                        {
                                            Information.Xp += factors[iCopy] + difficult;
                                        }
                                        else
                                        {
                                            Information.Xp += factors[iCopy] + difficult;
                                            Information.StaticXP += factors[iCopy] + difficult;
                                        }

                                        labelAddXpOne.Text = "+" + countXP_2.ToString();
                                    }

                                    if (Information.StaticXP >= 3000)
                                        closeTopUpAccount.Visible = false;

                                    labelMoneyWon.Text = "Выиграно: " + money_Won.ToString();

                                    Personal_data.Wins++;

                                    Form1.LvLCheck();
                                    Form1.Write();
                                    Form1.Encode();
                                }
                                else
                                {
                                    if (values[iCopy, jCopy] == 0)
                                    {
                                        Form1.LaughMethod();
                                    }

                                    if (Information.ItemTwo_WW_Game > 0)
                                    {
                                        labelSubstractCashTwo.Text = "-" + countSubstractCash_1.ToString();

                                        countSubstractCash_2 = 0;
                                        Information.Finance = Information.Finance;
                                        labelSubstractCashOne.Text = "-" + countSubstractCash_2.ToString();
                                    }

                                    else
                                    {
                                        labelSubstractCashTwo.Text = "-" + countSubstractCash_1.ToString();

                                        countSubstractCash_2 = Information.Finance * ((difficult + 2) * 5) / 100;
                                        Information.Finance -= Information.Finance * ((difficult + 2) * 5) / 100;
                                        labelSubstractCashOne.Text = "-" + countSubstractCash_2.ToString();
                                    }

                                    if (Information.StaticXP >= 3000)
                                        closeTopUpAccount.Visible = false;

                                    Personal_data.Losses++;

                                    Form1.LvLCheck();
                                    Form1.Write();
                                    Form1.Encode();

                                    if (score[1] == 0)
                                    {
                                        attempts = 0;
                                        money_Won = 0;

                                        labelMoneyWon.Text = "Выиграно: " + money_Won.ToString();
                                    }
                                }
                                //label3.Text = ($"Расчёт: {Information.Finance} / {factors[iCopy]} = {Information.Finance}")
                            }
                            else if (iteration == 3)
                            {
                                if (score[2] != 0)
                                {
                                    if (Information.Sound)
                                    {
                                        if (values[iCopy, jCopy] == 1)
                                        {
                                            player.SoundLocation = @"./Sounds/magic_1.wav";
                                            player.Play();
                                        }
                                        else if (values[iCopy, jCopy] == 2)
                                        {
                                            player.SoundLocation = @"./Sounds/magic_2.wav";
                                            player.Play();
                                        }
                                        else if (values[iCopy, jCopy] == 3)
                                        {
                                            player.SoundLocation = @"./Sounds/magic_3.wav";
                                            player.Play();
                                        }
                                        else if (values[iCopy, jCopy] == 4)
                                        {
                                            player.SoundLocation = @"./Sounds/applause.wav";
                                            player.Play();
                                        }
                                        else if (values[iCopy, jCopy] == 5)
                                        {
                                            player.SoundLocation = @"./Sounds/triangle_win.wav";
                                            player.Play();
                                        }
                                    }

                                    labelAddCashThree.Text = "+" + countCash_1.ToString();
                                    labelAddCashTwo.Text = "+" + countCash_2.ToString();

                                    countCash_1 = (Information.Finance * 5 / 100) * factors[iCopy] * difficult;
                                    //Information.Finance += (Information.Finance * 10 / 100) * factors[iCopy] * difficult;
                                    money_Won += (Information.Finance * 5 / 100) * factors[iCopy] * difficult;

                                    if (Form1.limitsOff < 2)
                                        availableEarn -= ((Information.Finance + money_Won) * 5 / 100) * factors[iCopy] * difficult;

                                    labelAddCashOne.Text = "+" + countCash_1.ToString();

                                    if (Information.ItemThree_Additional_Perc)
                                    {
                                        //Information.Finance += (Information.Finance * 10 / 100) * factors[iCopy] * difficult * 25 / 100;
                                        money_Won += (Information.Finance * 5 / 100) * factors[iCopy] * difficult * 25 / 100;

                                        if (Form1.limitsOff < 2)
                                            availableEarn -= ((Information.Finance + money_Won) * 5 / 100) * factors[iCopy] * difficult * 25 / 100;
                                    }

                                    if (Information.ItemOne_2x_XP > 0)
                                    {
                                        labelAddXpThree.Text = "+" + countXP_1.ToString();
                                        labelAddXpTwo.Text = "+" + countXP_2.ToString();

                                        countXP_1 = (factors[iCopy] + difficult) * 2;

                                        if (Information.Xp < Information.StaticXP)
                                        {
                                            Information.Xp += (factors[iCopy] + difficult) * 2;
                                        }
                                        else
                                        {
                                            Information.Xp += (factors[iCopy] + difficult) * 2;
                                            Information.StaticXP += (factors[iCopy] + difficult) * 2;
                                        }

                                        labelAddXpOne.Text = "+" + countXP_1.ToString();
                                    }

                                    else
                                    {
                                        labelAddXpThree.Text = "+" + countXP_1.ToString();
                                        labelAddXpTwo.Text = "+" + countXP_2.ToString();

                                        countXP_1 = factors[iCopy] + difficult;

                                        if (Information.Xp < Information.StaticXP)
                                        {
                                            Information.Xp += factors[iCopy] + difficult;
                                        }
                                        else
                                        {
                                            Information.Xp += factors[iCopy] + difficult;
                                            Information.StaticXP += factors[iCopy] + difficult;
                                        }

                                        labelAddXpOne.Text = "+" + countXP_1.ToString();
                                    }

                                    if (Information.StaticXP >= 3000)
                                        closeTopUpAccount.Visible = false;

                                    labelMoneyWon.Text = "Выиграно: " + money_Won.ToString();

                                    Personal_data.Wins++;

                                    Form1.LvLCheck();
                                    Form1.Write();
                                    Form1.Encode();
                                }
                                else
                                {
                                    if (values[iCopy, jCopy] == 0)
                                    {
                                        Form1.LaughMethod();
                                    }

                                    if (Information.ItemTwo_WW_Game > 0)
                                    {
                                        labelSubstractCashTwo.Text = "-" + countXP_2.ToString();

                                        countSubstractCash_1 = 0;
                                        Information.Finance = Information.Finance;
                                        labelSubstractCashOne.Text = "-" + countSubstractCash_1.ToString();
                                    }

                                    else
                                    {
                                        labelSubstractCashTwo.Text = "-" + countXP_2.ToString();

                                        countSubstractCash_1 = Information.Finance * ((difficult + 2) * 5) / 100;
                                        Information.Finance -= Information.Finance * ((difficult + 2) * 5) / 100;
                                        labelSubstractCashOne.Text = "-" + countSubstractCash_1.ToString();
                                    }

                                    if (Information.StaticXP >= 3000)
                                        closeTopUpAccount.Visible = false;

                                    Personal_data.Losses++;

                                    Form1.LvLCheck();
                                    Form1.Write();
                                    Form1.Encode();

                                    if (score[2] == 0)
                                    {
                                        attempts = 0;
                                        money_Won = 0;

                                        labelMoneyWon.Text = "Выиграно: " + money_Won.ToString();
                                    }
                                }

                                Information.Finance += money_Won;
                            }

                            if (Information.ItemOne_2x_XP > 0)
                                Information.ItemOne_2x_XP--;

                            if (attempts > 0)
                                attempts--;

                            SetLabels();

                            if (attempts <= 0 && Information.Finance > 0)
                            {
                                buttonTurn.Visible = true;
                                buttonSubstractDifficult.Visible = true;
                                buttonAddDifficult.Visible = true;
                            }

                            else
                            {
                                buttonTurn.Visible = false;
                                buttonSubstractDifficult.Visible = false;
                                buttonAddDifficult.Visible = false;
                            }

                            if (attempts == 0)
                                quit = true;
                            if (quit)
                                buttonBack.Enabled = true;
                            else
                                buttonBack.Enabled = false;

                            if (Information.Finance < 10)
                            {
                                buttonTurn.Enabled = false;
                                topUpAccount.Enabled = true;
                            }
                            else
                            {
                                buttonTurn.Enabled = true;
                                topUpAccount.Enabled = false;
                            }

                            if (Form1.limitsOff < 2)
                            {
                                if (availableEarn <= 0)
                                    buttonTurn.Enabled = false;
                            }

                            if (Information.Finance < 0)
                            {
                                Information.Finance = 0;
                                MessageBox.Show("Упс...");
                            }

                            Form1.LvLCheck();
                            Form1.Write();
                            Form1.Encode();
                        }
                    };
                }
            }

            //System.Diagnostics.Debug.WriteLine($"{q},{w}");
            buttonTurn.Click += (sender, e) =>
            {
                Form1.PayToTurn();

                if (Information.Finance > 0 && availableEarn > 0 || Information.Finance > 0 && Form1.limitsOff >= 2)
                {
                    ResetBoxes();
                    labelMoneyWon.Text = "Выиграно: 0";

                    labelAddCashOne.Text = "+0";
                    labelAddCashTwo.Text = "+0";
                    labelAddCashThree.Text = "+0";

                    labelAddXpOne.Text = "+0";
                    labelAddXpTwo.Text = "+0";
                    labelAddXpThree.Text = "+0";

                    if (Information.ItemTwo_WW_Game > 0)
                        Information.ItemTwo_WW_Game--;
                }
                //System.Diagnostics.Debug.WriteLine(Information.ItemTwo_WW_Game);
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

                    buttonTurn.Enabled = true;

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

        public void ResetBoxes()
        {
            if (difficult == 1)
                DefValues = copyValues;
            if (difficult == 2)
                DefValues = DefValuesLvLTwo;
            if (difficult == 3)
                DefValues = DefValuesLvLThree;

            InitTimer();

            //Закрасить картинки в чёрный цвет
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    arr[i, j].Image = (Image)Properties.Resources.ResourceManager.GetObject("closeBlock");
                    arr[i, j].Enabled = true;
                }
            }
            //Случайно раскинуть значения картинкам
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    values[i, j] = DefValues[rnd.Next(DefValues.Count)];
                }
            }

            buttonTurn.Visible = false;
            buttonSubstractDifficult.Visible = false;
            buttonAddDifficult.Visible = false;

            if (Information.Finance > 0)
            {
                if (Information.Finance > 0 && Information.Finance < 10000000)
                {
                    if (Information.ItemTwo_WW_Game > 0)
                    {
                        countSubstractCash_1 = 0;
                        Information.Finance = Information.Finance;
                        labelSubstractCashOne.Text = "-" + countSubstractCash_1.ToString();
                    }
                    else
                    {
                        countSubstractCash_1 = Information.Finance - (Information.Finance * (100 - difficult * 15) / 100);
                        Information.Finance = (Information.Finance * (100 - difficult * 15) / 100);
                        labelSubstractCashOne.Text = "-" + countSubstractCash_1.ToString();
                    }

                    if (Information.ItemFive_Pass_Accumulation)
                        Information.Finance += (Information.Finance * (100 - difficult * 15) / 100) * 10 / 100;

                    Form1.Write();
                    Form1.Encode();
                }
            }

            iteration = 0;
            attempts = 3;
            score.Clear();

            quit = false;

            if (!quit)
                buttonBack.Enabled = false;

            SetLabels();
        }

        void SetLabels()
        {
            labelAvailableBoxes.Text = "Доступно: " + attempts;
            labelFinance.Text = "Бюджет: " + Information.Finance;
            labelDifficult.Text = "Сложность: " + difficult;
            labelLvL.Text = "Уровень: " + Information.Lvl;
            labelXP.Text = "Опыт: " + Information.Xp;

            if (Form1.limitsOff < 2)
            {
                if (availableEarn <= 0)
                {
                    labelAvailableEarn.BackColor = Color.Black;
                    labelAvailableEarn.ForeColor = Color.Red;
                    buttonTurn.Enabled = false;
                    labelAvailableEarn.Text = $"Лимит исчерпан :'(";
                }
                else
                {
                    labelAvailableEarn.Text = $"Лимит: {availableEarn}$";
                    buttonTurn.Enabled = true;
                }
            }
            else
                labelAvailableEarn.Text = "Неограниченно";


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
                MessageBox.Show("Эта форма поинтереснее предыдущих, отчасти похоже на старую добрую игру Сапёр. \n" +
                    "Принцип такой, начинаешь игру, выбираешь 3 блока и в них тебе выпадает значение от плохого до хорошого.");
                timer1.Enabled = false;
            }
            else
            {
                counter = counter + 1;
            }
        }

        void LuckyRandomTime()
        {
            luckyRandomTime.Interval = 1000;
            luckyRandomTime.Enabled = true;
            luckyRandomTime.Tick += new System.EventHandler(TimerOfLuckyRandomTime);
        }

        void TimerOfLuckyRandomTime(object sender, System.EventArgs e)
        {
            Personal_data.LuckyRandom_time++;
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

            h /= 60;
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
            panelWall.BackColor = HsvToRgb(h, 1f, 1f);
        }
    }
}
