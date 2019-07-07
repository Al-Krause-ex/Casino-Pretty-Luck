using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Casino
{
    public partial class Shop : Form
    {
        string[,] namesAndDescriptionsOfItems = new string[,] {
            {"x2 XP" , "За каждую игру, опыт \nумножается в два раза" },
            {"Win-win game", "Бонус позволяет \nиграть в игры, не тратя денег" },
            {"Additional percentage", "После каждой победы, \nдополнительно вносится 25% от \nвыигрышной суммы" },
            {"XP => Cash", "Последующие деньги, \nвыигранные вами, превратятся \nв опыт. Работает только в Slot \nMachine" },
            {"Passive accumulation", "Даёт пассивный \nэффект, после каждой игры, \nв бюджет возвращается 10% от \nпотраченной суммы" },
            {"x2 Top up account", "Деньги с функции \n" + '"' + "Пополнить счёт" + '"' + " удваиваются" },
            {"Cash += XP", "Весь опыт, который у \nВас есть, превращается в деньги, \n умноженные в 5 раз, а опыт \nобнуляется" },
            {"Open the secret App", "Открывает секретное \nприложение в Казино" }, };

        int[] costOfItems = new int[] { 2000, 5000, 25000, 20000, 15000, 5000, 100, /*50к и 10к для апа*/ 100000, 30000 };
        int[,] costOfBottles = new int[,] { { 1000, 10000 }, { 5000, 40000 }, { 10000, 75000 }, { 20000, 145000 } };
        int percent;

        string[] quantity = new string[] {
            "50 игр", "2 игры", "Пассивно", "10 игр",
            "Пассивно", "Пассивно", "неограниченно", "Одноразово" };

        PictureBox[] pictureBoxes = new PictureBox[8];
        Button[] buttonsBuyBottles = new Button[4];
        Label[] labelsCostBottles = new Label[4];

        int item, counter;
        char check = 'D';

        public Shop()
        {
            InitializeComponent();

            buttonBack.Click += (obj, e) =>
            {
                Form1.SoundClickButton();
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

            buttonBuyItem.Enabled = false;
            item = 0;

            SetDataItems();
            DataInitialization();
            Initilization();
            Main();
            Mus_Sound();

            System.Diagnostics.Debug.WriteLine("XP: " + Information.Xp);
            System.Diagnostics.Debug.WriteLine("XP: " + Information.StaticXP);
        }

        public void DataInitialization()
        {
            Form1.Decode();

            labelName.Text = Information.Name;
            labelFinance.Text = "Бюджет: " + Information.Finance + "$";

            Form1.LvLCheck();

            labelLvL.Text = "Уровень: " + Form1.userLvL;
            labelXP.Text = "Опыт: " + Form1.userXP;

            //CheckButtonAndSetDataLabels();
        }

        public void Main()
        {
            percent = int.Parse((Information.Xp * 25 / 100).ToString());
            buttonBuyItem.Click += (obj, e) =>
            {
                if (item >= 0)
                {
                    Form1.PayToTurn();
                    ActionForButtonBuy();
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

        public void SetDataItems()
        {
            pictureBoxes = new PictureBox[8] {
                pictureBoxItemOne, pictureBoxItemTwo, pictureBoxItemThree, pictureBoxItemFour,
                pictureBoxItemFive, pictureBoxItemSix, pictureBoxItemSeven, pictureBoxItemEight};

            buttonsBuyBottles = new Button[4] {
                buttonBuyFirstBottle, buttonBuySecondBottle, buttonBuyThirdBottle, buttonBuyFourthBottle };

            labelsCostBottles = new Label[4]
            {
                labelCostFirstCapacity, labelCostSecondCapacity, labelCostThirdCapacity, labelCostFourthCapacity
            };
            SetLabelsBottles();

            int q = 0;
            int i = 0;

            //For items
            for (q = 0; q < 8; q++)
            {
                //7 полей (localItem)
                int localItem = q;

                pictureBoxes[q].Click += (obj, e) =>
                {
                    item = localItem;
                    Form1.SoundClickButton();

                    CheckButtonAndSetDataLabels();
                };
            }

            //For bottles
            for (i = 0; i < 4; i++)
            {
                int localI = i;

                buttonsBuyBottles[i].Click += (obj, e) =>
                {
                    Form1.PayToTurn();

                    if (Information.Finance >= costOfBottles[localI, 1] + percent * 2)
                    {
                        Information.Finance -= costOfBottles[localI, 1] + percent * 2;

                        if (Information.Xp < Information.StaticXP)
                        {
                            Information.Xp += costOfBottles[localI, 0];
                        }
                        else if ((Information.Xp + costOfBottles[localI, 0] + percent) >= Information.StaticXP)
                        {
                            Information.Xp += costOfBottles[localI, 0];
                            Information.StaticXP += Information.Xp - Information.StaticXP;
                        }
                        else
                        {
                            Information.Xp += costOfBottles[localI, 0];
                            Information.StaticXP += costOfBottles[localI, 0];
                        }

                        //System.Diagnostics.Debug.WriteLine("XP: " + Information.Xp);
                        //System.Diagnostics.Debug.WriteLine("XP: " + Information.StaticXP);

                        SetLabelsBottles();
                        SetLabelsItems();
                        MessageBox.Show("Благодарим за покупку! Теперь у тебя больше опыта :3");
                        if (Information.Xp > Information.StaticXP)
                        {
                            Information.StaticXP = Information.Xp;
                        }

                        Form1.Write();
                        Form1.Encode();

                        DataInitialization();
                    }
                    else
                        MessageBox.Show($"Упс! Тебе не хватает {(Information.Finance - (costOfBottles[localI, 1] + percent)) * (-1)}$ чтобы купить этот предмет");
                };
            }
        }

        void Initilization()
        {
            if (Information.ItemThree_Additional_Perc)
            {
                pictureBoxItemThree.Enabled = false;
                closeItemOne.Image = (Image)Properties.Resources.ResourceManager.GetObject("closeItem");
                closeItemOne.BringToFront();

                Clean();
            }

            if (Information.ItemFive_Pass_Accumulation)
            {
                pictureBoxItemFive.Enabled = false;
                closeItemTwo.Image = (Image)Properties.Resources.ResourceManager.GetObject("closeItem");
                closeItemTwo.BringToFront();

                buttonBuyItem.Enabled = false;

                Clean();
            }

            if (Information.ItemSix_x2_TopUpAcc)
            {
                pictureBoxItemSix.Enabled = false;
                closeItemThree.Image = (Image)Properties.Resources.ResourceManager.GetObject("closeItem");
                closeItemThree.BringToFront();

                buttonBuyItem.Enabled = false;

                Clean();
            }

            if (Information.ItemEight_OpenApp)
            {
                pictureBoxItemEight.Enabled = false;
                closeItemFour.Image = (Image)Properties.Resources.ResourceManager.GetObject("closeItem");
                closeItemFour.BringToFront();

                buttonBuyItem.Enabled = false;

                Clean();
            }
        }

        void CheckButtonAndSetDataLabels()
        {
            if (Information.Finance >= costOfItems[item] + percent && item < 5)
                buttonBuyItem.Enabled = true;
            else if (Information.Xp >= costOfItems[item] + percent && item >= 5 && item < 7)
                buttonBuyItem.Enabled = true;
            else if (Information.Xp >= costOfItems[item + 1] + percent && Information.Finance >= costOfItems[item] + percent && item == 7 && !Information.ItemEight_OpenApp)
                buttonBuyItem.Enabled = true;
            else
                buttonBuyItem.Enabled = false;

            SetLabelsItems();

            //Выбор предмет визуально
            for (int i = 0; i < 8; i++)
                pictureBoxes[i].Image = null;
            pictureBoxes[item].Image = (Image)Properties.Resources.ResourceManager.GetObject("item_" + item + "_chosen");
            //System.Diagnostics.Debug.WriteLine(item);

        }

        void ActionForButtonBuy()
        {
            if (item == 5 || item == 6)
            {
                if (Information.Xp >= costOfItems[item] + percent)
                {
                    Information.Xp -= costOfItems[item] + percent;
                    buttonBuyItem.Enabled = false;
                    SetLabelsBottles();
                    SetLabelsItems();
                }

                else
                    buttonBuyItem.Enabled = false;
            }
            else if (item == 7)
            {
                if (Information.Finance >= costOfItems[item] + percent && Information.Xp >= costOfItems[item + 1] + percent)
                {
                    Information.Finance -= costOfItems[item] + percent;
                    Information.Xp -= costOfItems[item + 1] + percent;
                    SetLabelsBottles();
                    SetLabelsItems();
                }
                else
                    buttonBuyItem.Enabled = false;
            }
            else
            {
                if (Information.Finance >= costOfItems[item] + percent)
                {
                    Information.Finance -= costOfItems[item] + percent;
                    Form1.Write();
                    Form1.Encode();
                }
                else
                    buttonBuyItem.Enabled = false;
            }
            if (Information.Finance >= costOfItems[item] + percent && item < 5)
                buttonBuyItem.Enabled = true;
            else
                buttonBuyItem.Enabled = false;

            //ACTIONS//
            if (item == 0)
            {
                Information.ItemOne_2x_XP += 50;
                Personal_data.Items_bought += 50;
            }
            else if (item == 1)
            {
                Information.ItemTwo_WW_Game += 2;
                Personal_data.Items_bought += 2;
            }
            else if (item == 2)
            {
                Information.ItemThree_Additional_Perc = true;
                Initilization();
            }
            else if (item == 3)
            {
                Information.ItemFour_XP_Cash += 10;
                Personal_data.Items_bought += 10;
            }
            else if (item == 4)
            {
                Information.ItemFive_Pass_Accumulation = true;
                Initilization();
            }
            else if (item == 5)
            {
                Information.ItemSix_x2_TopUpAcc = true;
                Initilization();
                SetLabelsBottles();
            }
            else if (item == 6)
            {
                Information.Finance += Information.Xp * 5;
                Information.Xp = 0;
                SetLabelsBottles();
            }
            else
            {
                Information.ItemEight_OpenApp = true;
                Personal_data.Opened_extra_app = true;
                Initilization();
            }

            Form1.Write();
            Form1.Encode();
            DataInitialization();
        }

        void SetLabelsBottles()
        {
            percent = int.Parse((Information.Xp * 25 / 100).ToString());
            int j = 0;

            for (j = 0; j < 4; j++)
            {
                int localJ = j;

                labelsCostBottles[j].Text = $"Цена: {costOfBottles[localJ, 1] + percent * 2}$";
            }
        }

        void SetLabelsItems()
        {
            if (item >= 5 && item < 7)
                labelCostItem.Text = "Цена: " + (costOfItems[item] + percent) + " xp";
            else if (item == 7)
                labelCostItem.Text = "Цена: " + (costOfItems[item] + percent) + "$ и " + (costOfItems[item + 1] + percent) + " xp";
            else
                labelCostItem.Text = "Цена: " + (costOfItems[item] + percent) + "$";

            labelNameItem.Text = "Название: " + namesAndDescriptionsOfItems[item, 0];
            labelDescriptionItem.Text = "Описание: " + namesAndDescriptionsOfItems[item, 1];
            labelQuantityItem.Text = "Кол-во: " + quantity[item];
        }

        private void time_Tick(object sender, System.EventArgs e)
        {
            if (counter == 10)
            {
                timer1.Stop();
                MessageBox.Show("Ну тут даже рассказать нечего.\n" +
                    "Это магазин, да-да.\n" +
                    "Снизу бонусы, чтобы узнать какой бонус что значит, нажми на него и рядом появится описание.\n" +
                    "Выше разные ёмкости с очками опыта.");
                timer1.Enabled = false;
            }
            else
            {
                counter = counter + 1;
            }
        }

        void Clean()
        {
            for (int i = 0; i < 8; i++)
                pictureBoxes[i].Image = null;

            labelCostItem.Text = "Цена: ";
            labelNameItem.Text = "Название: ";
            labelDescriptionItem.Text = "Описание: ";
            labelQuantityItem.Text = "Кол-во: ";
        }
    }
}

