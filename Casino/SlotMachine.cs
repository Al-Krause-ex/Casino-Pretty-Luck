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
    public partial class SlotMachine : Form
    {
        SoundPlayer player = new SoundPlayer();
        Random rnd = new Random();

        Timer timer;
        Timer timerAnimation;
        Timer slotMachineTime = new Timer();

        int count = 0;
        int countAnimationPictures = 0;

        Color[] colours = new Color[] { Color.AntiqueWhite, Color.Aqua, Color.Blue, Color.BlueViolet, Color.Brown,
            Color.BurlyWood, Color.Chartreuse, Color.Coral, Color.CornflowerBlue, Color.Crimson, Color.Cyan,
            Color.DeepPink, Color.Fuchsia, Color.LightPink, Color.LightGreen, Color.LightSalmon, Color.LightSkyBlue,
            Color.Moccasin, Color.Orange, Color.Yellow};

        int counter;

        char check = 'B';

        public SlotMachine()
        {
            InitializeComponent();
            buttonBack.Click += (obj, e) =>
            {
                Form1.SoundClickButton();
                slotMachineTime.Stop();
            };
            buttonBack.DialogResult = DialogResult.OK;

            for (int i = 0; i < Information.Training.Length; i++)
            {
                if (check == Information.Training[i])
                {
                    counter = 0;
                    timer2.Interval = 100;
                    timer2.Enabled = true;
                    // Hook up timer's tick event handler.  
                    timer2.Tick += new System.EventHandler(time_Tick);

                    Information.Training = Information.Training.Remove(i, 1);

                    File.WriteAllText(Form1.path_training, string.Empty);

                    using (StreamWriter sw = File.AppendText(Form1.path_training))
                    {
                        sw.WriteLine(Information.Training);
                    }
                }
            }

            DataInitialization();
            InitTimer();
            Main();
            Mus_Sound();

            SlotMachineTime();
        }

        private void time_Tick(object sender, System.EventArgs e)
        {
            if (counter == 10)
            {
                timer2.Stop();
                MessageBox.Show("Добро пожаловать в Slot Machine!.\n" +
                    "В этой игре нет ничего замудрённого, только может ставки?\n" +
                    "Ты можешь увеличить ставку и линии. Стоимость прокрутки: ставка * линии\n" +
                    "Рядом есть кубик, объяснять ничего не буду, наведи мышкой на него.\n" +
                    "Снизу твои бонусы, если ты забыл какой бонус что значит, наведи на иконку.\n" +
                    "А справа информация о расходе, доходе опыта и денег.");
                timer2.Enabled = false;
            }
            else
            {
                counter = counter + 1;
            }
        }

        public void DataInitialization()
        {
            ((TextBox)nud_Bets.Controls[1]).MaxLength = 7;
            SlotMachineLogic.Lines = 1;
            SlotMachineLogic.Bets = 10;

            ToolTip t = new ToolTip();
            t.SetToolTip(pictureRandomBonus, "Lucky-Dice. Стоит 50% твоего бюджета. " +
                "\nПри покупке даются 3 попытки, в случае победы, \n50%, которые вы отдали, увеличиваются в 2 раза");
            t.SetToolTip(hintItemOne, "За каждую игру умножается полученный опыт в два раза");
            t.SetToolTip(hintItemTwo, "Бонус позволяет играть в игры, не тратя денег");
            t.SetToolTip(hintItemThree, "Последующие выигрышные деньги превращаются в опыт");
            t.SetToolTip(hintItemFour, "После каждой победы, дополнительно \nвносится 25% от выигрышной суммы");
            t.SetToolTip(hintItemFive, "Даёт пассивный эффект, после каждой игры, \nв бюджет возвращается 10% от потраченной суммы");

            BackColor = Color.FromArgb(64, 64, 64);

            SetLabels();

            if (SlotMachineLogic.Blinking)
                Time();

            if (Information.Finance <= 0)
                buttonTurn.Enabled = false;

            if (Information.EarnedMoney <= 0)
                buttonTakeMoney.Enabled = false;

            if (SlotMachineLogic.Attempts > 0 && SlotMachineLogic.Bonus)
                infoAttempts.Visible = true;
        }

        public void Main()
        {
            //string[] lines = File.ReadAllLines(Form1.path);
            //Information.Finance = int.Parse(lines[1]);

            if (Information.Finance > 0 && Information.EarnedMoney == 0)
                buttonTakeMoney.Enabled = false;

            if (Information.Finance < 10)
                buttonTurn.Enabled = false;
            else
                buttonTurn.Enabled = true;

            buttonTurn.Click += (obj, e) =>
            {
                if (nud_Bets.Value > Information.Finance)
                    nud_Bets.Value = Information.Finance;
                if (Information.Animation)
                    Animation();
                else
                {
                    if (Information.Sound)
                    {
                        player.SoundLocation = @"./Sounds/turn_short.wav";
                        player.Play();
                    }

                    MainLogic();
                }
            };

            pictureRandomBonus.Click += (obj, e) =>
            {
                if (!SlotMachineLogic.Bonus && Information.Finance > 15)
                {
                    Form1.PayToTurn();

                    if (SlotMachineLogic.Attempts == 0)
                        SlotMachineLogic.Attempts = 3;

                    SlotMachineLogic.moneyRandomBox = Information.Finance * 50 / 100;
                    Information.Finance -= Information.Finance * 50 / 100;

                    labelFinance.Text = ($"Бюджет: {Information.Finance}");

                    SlotMachineLogic.Bonus = true;

                    infoAttempts.Text = "Количество попыток: " + SlotMachineLogic.Attempts.ToString();
                    infoAttempts.Visible = true;

                    if (!SlotMachineLogic.Blinking)
                        Time();

                    nudValue();
                }
            };

            buttonTakeMoney.Click += (obj, e) =>
            {
                Information.Finance += Information.EarnedMoney;
                Information.EarnedMoney = 0;

                labelEarnedMoney.Text = ($"Заработанные деньги: {Information.EarnedMoney}");
                labelFinance.Text = ($"Бюджет: {Information.Finance}");

                if (Information.Finance > 9)
                    buttonTurn.Enabled = true;

                if (Information.EarnedMoney == 0)
                    buttonTakeMoney.Enabled = false;

                nudValue();

                Form1.Write();
                Form1.Encode();
            };

            buttonAccept.Click += (obj, e) =>
            {
                nudValue();
            };

            buttonAddLine.Click += (obj, e) =>
            {
                if (SlotMachineLogic.Lines < 3)
                {
                    Form1.SoundClickButton();
                    SlotMachineLogic.Lines++;

                    if (SlotMachineLogic.Lines == 2)
                    {
                        pictureBoxSlotOne.Image = (Image)Properties.Resources.ResourceManager.GetObject("unknownBlock");
                        pictureBoxSlotTwo.Image = (Image)Properties.Resources.ResourceManager.GetObject("unknownBlock");
                        pictureBoxSlotThree.Image = (Image)Properties.Resources.ResourceManager.GetObject("unknownBlock");
                    }

                    if (SlotMachineLogic.Lines == 3)
                    {
                        pictureBoxSlotSeven.Image = (Image)Properties.Resources.ResourceManager.GetObject("unknownBlock");
                        pictureBoxSlotEight.Image = (Image)Properties.Resources.ResourceManager.GetObject("unknownBlock");
                        pictureBoxSlotNine.Image = (Image)Properties.Resources.ResourceManager.GetObject("unknownBlock");
                    }
                }

                labelLines.Text = ($"Линий: {SlotMachineLogic.Lines}");

                nudValue();
            };

            buttonSubstractLine.Click += (obj, e) =>
            {
                if (SlotMachineLogic.Lines > 1)
                {
                    Form1.SoundClickButton();
                    SlotMachineLogic.Lines--;
                }

                if (SlotMachineLogic.Lines == 2)
                {
                    SlotMachineLogic.ThirdBlock = false;
                    panelSlot_3_1.BackColor = Color.White;
                    panelSlot_3_2.BackColor = Color.White;
                    panelSlot_3_3.BackColor = Color.White;

                    pictureBoxSlotSeven.Image = (Image)Properties.Resources.ResourceManager.GetObject("picture_close");
                    pictureBoxSlotEight.Image = (Image)Properties.Resources.ResourceManager.GetObject("picture_close");
                    pictureBoxSlotNine.Image = (Image)Properties.Resources.ResourceManager.GetObject("picture_close");
                }

                if (SlotMachineLogic.Lines == 1)
                {
                    SlotMachineLogic.FirstBlock = false;
                    panelSlot_1_1.BackColor = Color.White;
                    panelSlot_1_2.BackColor = Color.White;
                    panelSlot_1_3.BackColor = Color.White;

                    pictureBoxSlotOne.Image = (Image)Properties.Resources.ResourceManager.GetObject("picture_close");
                    pictureBoxSlotTwo.Image = (Image)Properties.Resources.ResourceManager.GetObject("picture_close");
                    pictureBoxSlotThree.Image = (Image)Properties.Resources.ResourceManager.GetObject("picture_close");
                }

                labelLines.Text = ($"Линий: {SlotMachineLogic.Lines}");

                nudValue();
            };

            buttonBack.Click += (obj, e) =>
            {
                Information.Finance += Information.EarnedMoney;
                Information.EarnedMoney = 0;

                labelEarnedMoney.Text = ($"Заработанные деньги: {Information.EarnedMoney}");
                labelFinance.Text = ($"Бюджет: {Information.Finance}");

                if (Information.EarnedMoney == 0)
                    buttonTakeMoney.Enabled = false;

                Form1.Write();
                Form1.Encode();
            };

            buttonAnimationOnOff.Click += (obj, e) =>
            {
                if (Information.Animation)
                {
                    Information.Animation = false;

                    labelAnimationOnOff.Text = "Animation: OFF";
                    buttonAnimationOnOff.Text = "ON";
                }
                else
                {
                    Information.Animation = true;
                    labelAnimationOnOff.Text = "Animation: ON";
                    buttonAnimationOnOff.Text = "OFF";
                }

                Form1.Write();
                Form1.Encode();
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

        void MainLogic()
        {
            SlotMachineLogic slot = new SlotMachineLogic(
                    rnd.Next() % 30, rnd.Next() % 30, rnd.Next() % 30,
                    rnd.Next() % 30, rnd.Next() % 30, rnd.Next() % 30,
                    rnd.Next() % 30, rnd.Next() % 30, rnd.Next() % 30);

            if ((SlotMachineLogic.Bets * SlotMachineLogic.Lines) <= Information.Finance)
            {
                if (Information.ItemTwo_WW_Game <= 0)
                    Information.Finance -= SlotMachineLogic.Bets * SlotMachineLogic.Lines;

                labelSubstractCash.Text = "-" + (SlotMachineLogic.Bets * SlotMachineLogic.Lines).ToString();

                labelFinance.Text = ($"Бюджет: {Information.Finance}");

                slot.VirtualNumberCheck();

                slot.WinCheck();
                labelAddCash.Text = "+" + SlotMachineLogic.CountCash.ToString();
                labelAddXp.Text = "+" + SlotMachineLogic.CountXP.ToString();

                if (Information.EarnedMoney > 0)
                    buttonTakeMoney.Enabled = true;

                labelEarnedMoney.Text = ($"Заработанные деньги: {Information.EarnedMoney}");

                if (SlotMachineLogic.Attempts > 0 && SlotMachineLogic.Bonus)
                {
                    SlotMachineLogic.Attempts -= 1;
                    infoAttempts.Text = "Количество попыток: " + SlotMachineLogic.Attempts.ToString();
                    //System.Diagnostics.Debug.WriteLine("Attempts: " + SlotMachineLogic.Attempts);

                    if (SlotMachineLogic.Attempts == 0)
                    {
                        SlotMachineLogic.Bonus = false;
                        SlotMachineLogic.Blinking = false;
                        SlotMachineLogic.Attempts = 3;

                        infoAttempts.Visible = false;
                    }
                }

                if (Information.ItemOne_2x_XP > 0)
                {
                    Information.ItemOne_2x_XP--;
                    labelInfoItemOne.Text = Information.ItemOne_2x_XP.ToString();
                }

                if (Information.ItemTwo_WW_Game > 0)
                {
                    Information.ItemTwo_WW_Game--;
                    labelInfoItemTwo.Text = Information.ItemTwo_WW_Game.ToString();
                }

                if (Information.ItemFour_XP_Cash > 0)
                {
                    Information.ItemFour_XP_Cash--;
                    labelInfoItemThree.Text = Information.ItemFour_XP_Cash.ToString();

                    labelAddCash.Text = "+0";

                    if (Information.Xp < Information.StaticXP)
                    {
                        Information.Xp += Information.EarnedMoney;
                    }
                    else
                    {
                        Information.Xp += Information.EarnedMoney;
                        Information.StaticXP += Information.EarnedMoney;
                    }

                    labelAddXp.Text = "+" + (SlotMachineLogic.CountXP += Information.EarnedMoney).ToString();

                    Information.EarnedMoney = 0;
                    labelEarnedMoney.Text = ($"Заработанные деньги: {Information.EarnedMoney}");
                    if (Information.EarnedMoney <= 0)
                        buttonTakeMoney.Enabled = false;
                }

                if (Information.ItemFive_Pass_Accumulation)
                {
                    Information.Finance += (SlotMachineLogic.Bets * SlotMachineLogic.Lines) * 10 / 100;
                    labelFinance.Text = ($"Бюджет: {Information.Finance}");
                }

                PaintPanels();

                if (SlotMachineLogic.Lines == 1)
                {
                    pictureBoxSlotFour.Image = (Image)Properties.Resources.ResourceManager.GetObject("picture" + slot.num4.ToString());
                    pictureBoxSlotFive.Image = (Image)Properties.Resources.ResourceManager.GetObject("picture" + slot.num5.ToString());
                    pictureBoxSlotSix.Image = (Image)Properties.Resources.ResourceManager.GetObject("picture" + slot.num6.ToString());
                }

                else if (SlotMachineLogic.Lines == 2)
                {
                    pictureBoxSlotOne.Image = (Image)Properties.Resources.ResourceManager.GetObject("picture" + slot.num1.ToString());
                    pictureBoxSlotTwo.Image = (Image)Properties.Resources.ResourceManager.GetObject("picture" + slot.num2.ToString());
                    pictureBoxSlotThree.Image = (Image)Properties.Resources.ResourceManager.GetObject("picture" + slot.num3.ToString());
                    pictureBoxSlotFour.Image = (Image)Properties.Resources.ResourceManager.GetObject("picture" + slot.num4.ToString());
                    pictureBoxSlotFive.Image = (Image)Properties.Resources.ResourceManager.GetObject("picture" + slot.num5.ToString());
                    pictureBoxSlotSix.Image = (Image)Properties.Resources.ResourceManager.GetObject("picture" + slot.num6.ToString());
                }

                else if (SlotMachineLogic.Lines == 3)
                {
                    pictureBoxSlotOne.Image = (Image)Properties.Resources.ResourceManager.GetObject("picture" + slot.num1.ToString());
                    pictureBoxSlotTwo.Image = (Image)Properties.Resources.ResourceManager.GetObject("picture" + slot.num2.ToString());
                    pictureBoxSlotThree.Image = (Image)Properties.Resources.ResourceManager.GetObject("picture" + slot.num3.ToString());
                    pictureBoxSlotFour.Image = (Image)Properties.Resources.ResourceManager.GetObject("picture" + slot.num4.ToString());
                    pictureBoxSlotFive.Image = (Image)Properties.Resources.ResourceManager.GetObject("picture" + slot.num5.ToString());
                    pictureBoxSlotSix.Image = (Image)Properties.Resources.ResourceManager.GetObject("picture" + slot.num6.ToString());
                    pictureBoxSlotSeven.Image = (Image)Properties.Resources.ResourceManager.GetObject("picture" + slot.num7.ToString());
                    pictureBoxSlotEight.Image = (Image)Properties.Resources.ResourceManager.GetObject("picture" + slot.num8.ToString());
                    pictureBoxSlotNine.Image = (Image)Properties.Resources.ResourceManager.GetObject("picture" + slot.num9.ToString());
                }

                if (nud_Bets.Value > Information.Finance)
                    buttonTurn.Enabled = false;

            }

            if (Information.Finance < 10)
                buttonTurn.Enabled = false;

            Form1.LvLCheck();
            SetLabels();
            Form1.Write();
            Form1.Encode();
        }

        async void Time()
        {
            SlotMachineLogic.Blinking = true;
            while (SlotMachineLogic.Bonus)
            {
                panelRandom.BackColor = colours[rnd.Next(0, 19)];
                await Task.Delay(150);

                panelRandom.BackColor = colours[rnd.Next(0, 19)];
                await Task.Delay(150);
            }
        }

        public void InitTimer()
        {
            timer = new Timer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = 5000;
            timer.Start();
        }

        public void Animation()
        {
            timerAnimation = new Timer();
            timerAnimation.Tick += new EventHandler(tick_tack);
            timerAnimation.Interval = 100;
            timerAnimation.Start();

            if (Information.Sound)
            {
                player.SoundLocation = @"./Sounds/turn.wav";
                player.Play();
            }
        }

        private void tick_tack(object sender, EventArgs e)
        {
            //buttonAddBet.Enabled = false;
            //buttonSubstractBet.Enabled = false;

            buttonTurn.Enabled = false;
            buttonTurn.Visible = false;
            buttonAddLine.Enabled = false;
            buttonSubstractLine.Enabled = false;
            buttonAnimationOnOff.Enabled = false;
            pictureRandomBonus.Enabled = false;
            buttonBack.Enabled = false;
            nud_Bets.Enabled = false;
            buttonAccept.Enabled = false;

            if (SlotMachineLogic.Lines == 1)
            {
                pictureBoxSlotFour.Image = (Image)Properties.Resources.ResourceManager.GetObject("picture" + rnd.Next(0, 5));
                pictureBoxSlotFive.Image = (Image)Properties.Resources.ResourceManager.GetObject("picture" + rnd.Next(0, 5));
                pictureBoxSlotSix.Image = (Image)Properties.Resources.ResourceManager.GetObject("picture" + rnd.Next(0, 5));
            }

            else if (SlotMachineLogic.Lines == 2)
            {
                pictureBoxSlotOne.Image = (Image)Properties.Resources.ResourceManager.GetObject("picture" + rnd.Next(0, 5));
                pictureBoxSlotTwo.Image = (Image)Properties.Resources.ResourceManager.GetObject("picture" + rnd.Next(0, 5));
                pictureBoxSlotThree.Image = (Image)Properties.Resources.ResourceManager.GetObject("picture" + rnd.Next(0, 5));
                pictureBoxSlotFour.Image = (Image)Properties.Resources.ResourceManager.GetObject("picture" + rnd.Next(0, 5));
                pictureBoxSlotFive.Image = (Image)Properties.Resources.ResourceManager.GetObject("picture" + rnd.Next(0, 5));
                pictureBoxSlotSix.Image = (Image)Properties.Resources.ResourceManager.GetObject("picture" + rnd.Next(0, 5));
            }

            else if (SlotMachineLogic.Lines == 3)
            {
                pictureBoxSlotOne.Image = (Image)Properties.Resources.ResourceManager.GetObject("picture" + rnd.Next(0, 5));
                pictureBoxSlotTwo.Image = (Image)Properties.Resources.ResourceManager.GetObject("picture" + rnd.Next(0, 5));
                pictureBoxSlotThree.Image = (Image)Properties.Resources.ResourceManager.GetObject("picture" + rnd.Next(0, 5));
                pictureBoxSlotFour.Image = (Image)Properties.Resources.ResourceManager.GetObject("picture" + rnd.Next(0, 5));
                pictureBoxSlotFive.Image = (Image)Properties.Resources.ResourceManager.GetObject("picture" + rnd.Next(0, 5));
                pictureBoxSlotSix.Image = (Image)Properties.Resources.ResourceManager.GetObject("picture" + rnd.Next(0, 5));
                pictureBoxSlotSeven.Image = (Image)Properties.Resources.ResourceManager.GetObject("picture" + rnd.Next(0, 5));
                pictureBoxSlotEight.Image = (Image)Properties.Resources.ResourceManager.GetObject("picture" + rnd.Next(0, 5));
                pictureBoxSlotNine.Image = (Image)Properties.Resources.ResourceManager.GetObject("picture" + rnd.Next(0, 5));
            }
            countAnimationPictures++;

            if (countAnimationPictures >= 19)
            {
                //buttonAddBet.Enabled = true;
                //buttonSubstractBet.Enabled = true;

                timerAnimation.Stop();
                buttonAddLine.Enabled = true;
                buttonSubstractLine.Enabled = true;
                buttonAnimationOnOff.Enabled = true;
                pictureRandomBonus.Enabled = true;
                buttonBack.Enabled = true;
                nud_Bets.Enabled = true;
                buttonAccept.Enabled = true;
                buttonTurn.Enabled = true;
                buttonTurn.Visible = true;

                countAnimationPictures = 0;

                MainLogic();
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (labelHint.Visible)
                labelHint.Visible = false;
            else
                labelHint.Visible = true;
            count++;

            if (count >= 10)
            {
                labelHint.Visible = true;
                timer.Stop();
            }
        }

        void SetLabels()
        {
            labelEarnedMoney.Text = ($"Заработанные деньги: {Information.EarnedMoney}");
            labelFinance.Text = ($"Бюджет: {Information.Finance}");
            labelBet.Text = "Ставка:";
            nud_Bets.Text = SlotMachineLogic.Bets.ToString();
            labelLines.Text = ($"Линий: {SlotMachineLogic.Lines}");
            infoAttempts.Text = ($"Количество попыток: {SlotMachineLogic.Attempts}");

            if (Information.Animation)
            {
                buttonAnimationOnOff.Text = "OFF";
                labelAnimationOnOff.Text = "Анимация: ON";
            }
            else
            {
                buttonAnimationOnOff.Text = "ON";
                labelAnimationOnOff.Text = "Анимация: OFF";
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

        public void PaintPanels()
        {
            if (!SlotMachineLogic.FirstBlock && !SlotMachineLogic.SecondBlock && !SlotMachineLogic.ThirdBlock)
            {
                panelSlot_1_1.BackColor = Color.White;
                panelSlot_1_2.BackColor = Color.White;
                panelSlot_1_3.BackColor = Color.White;
                panelSlot_2_1.BackColor = Color.White;
                panelSlot_2_2.BackColor = Color.White;
                panelSlot_2_3.BackColor = Color.White;
                panelSlot_3_1.BackColor = Color.White;
                panelSlot_3_2.BackColor = Color.White;
                panelSlot_3_3.BackColor = Color.White;
            }

            if (SlotMachineLogic.FirstBlock)
            {
                int index = rnd.Next(0, 19);

                panelSlot_1_1.BackColor = colours[index];
                panelSlot_1_2.BackColor = colours[index];
                panelSlot_1_3.BackColor = colours[index];

                SlotMachineLogic.FirstBlock = false;
            }

            if (SlotMachineLogic.SecondBlock)
            {
                int index = rnd.Next(0, 19);

                panelSlot_2_1.BackColor = colours[index];
                panelSlot_2_2.BackColor = colours[index];
                panelSlot_2_3.BackColor = colours[index];

                SlotMachineLogic.SecondBlock = false;
            }

            if (SlotMachineLogic.ThirdBlock)
            {
                int index = rnd.Next(0, 19);

                panelSlot_3_1.BackColor = colours[index];
                panelSlot_3_2.BackColor = colours[index];
                panelSlot_3_3.BackColor = colours[index];

                SlotMachineLogic.ThirdBlock = false;
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            nudValue();
        }

        public void nudValue()
        {
            SlotMachineLogic.Bets = int.Parse(nud_Bets.Value.ToString());

            if ((SlotMachineLogic.Bets * SlotMachineLogic.Lines) > Information.Finance)
            {
                buttonTurn.Enabled = false;
            }
            else
                buttonTurn.Enabled = true;

            labelBet.Text = "Ставка:";
            nud_Bets.Text = SlotMachineLogic.Bets.ToString();
        }

        void SlotMachineTime()
        {
            slotMachineTime.Interval = 1000;
            slotMachineTime.Enabled = true;
            slotMachineTime.Tick += new System.EventHandler(TimerOfSlotMachineTime);
        }

        void TimerOfSlotMachineTime(object sender, System.EventArgs e)
        {
            Personal_data.SlotMachine_time++;
            Form1.Write();
            Form1.Encode();
        }
    }
}