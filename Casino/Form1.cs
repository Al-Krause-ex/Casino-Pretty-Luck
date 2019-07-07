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
using WMPLib;

namespace Casino
{
    public partial class Form1 : Form
    {
        Label[] labels = new Label[4];

        public static Timer all_time = new Timer();

        static PictureBox closeYourRandom = new PictureBox();
        static PictureBox closeLuckyRandom = new PictureBox(); //location 384; 196
        static PictureBox closeShop = new PictureBox(); //loc 384; 448
        static PictureBox closeLuckyTree = new PictureBox(); //384; 574

        public static Random rnd = new Random();

        public string text = Information.Name;
        public static string userLvL;
        public static string userXP;

        public static string path_data = @"./data.cas";
        public static string path_statistics_data = @"./personal_data.cas";
        public static string path_statistics_times = @"./times.cas"; 
        public static string path_training = @"./training.cas";

        public static int[,] arrayLvLsAndXP = new int[,] { {0, 100 }, {1, 400 }, {2, 800 }, {3, 2000 }, { 4, 4000 },
        {5, 7000 }, {6, 10000 }, {7, 15000 }, {8, 30000 }, {9, 100000 }, {10, 1000000 }, {11, 9999999 }, {12, 99999999} };

        public static string[] trackList = new string[] { "sound_1", "sound_2", "sound_3", "sound_4",
        "sound_5", "sound_6", "sound_7", "sound_8", "sound_9", "sound_10"};

        public static string[] laughList = new string[] { "laugh_1", "laugh_2", "laugh_3", "laugh_4",
        "laugh_5", "laugh_6", "laugh_7", "laugh_8", "laugh_9"};

        public static string[] magicList = new string[] { "magic_1", "magic_2", "magic_3", "magic_4" };
        public static string[] chest = new string[] { "open_chest", "close_chest" };

        public static WindowsMediaPlayer Player;

        public static SoundPlayer sPlay = new SoundPlayer();

        int counter;
        public static int limitsOff = 0;

        char check = 'A';

        public Form1()
        {
            InitializeComponent();

            if (!File.Exists(path_data))
            {
                using (StreamWriter sw = File.CreateText(path_data))
                {
                    sw.WriteLine("Игрок"); //name
                    sw.WriteLine("1000"); //Finance
                    sw.WriteLine("0"); //lvl
                    sw.WriteLine("0"); //xp
                    sw.WriteLine("0"); //x2
                    sw.WriteLine("0"); //ww
                    sw.WriteLine("False"); //add_perc
                    sw.WriteLine("0"); //xp_cash
                    sw.WriteLine("False"); //pass_acc
                    sw.WriteLine("False"); //x2_top_up_account
                    sw.WriteLine("False"); //luckyTree
                    sw.WriteLine("0"); //wallpaper
                    sw.WriteLine("0"); //staticLvL
                    sw.WriteLine("0"); //staticXP
                    sw.WriteLine("0"); //current Track
                    sw.WriteLine("True"); //music
                    sw.WriteLine("True"); //sound
                    sw.WriteLine("True"); //animation
                    sw.WriteLine("False"); //game won
                }

                using (StreamWriter sw = File.CreateText(path_statistics_data))
                {
                    sw.WriteLine("0"); //wins
                    sw.WriteLine("0"); //losses
                    sw.WriteLine("0"); //items_bought
                    sw.WriteLine("False"); //extra_app
                    sw.WriteLine("False"); //achieve
                }

                using (StreamWriter sw = File.CreateText(path_statistics_times))
                {
                    sw.WriteLine("0"); //all_time
                    sw.WriteLine("0"); //slotMachine_time
                    sw.WriteLine("0"); //yourRandom_time
                    sw.WriteLine("0"); //luckyRandom_time
                    sw.WriteLine("0"); //luckyTree_time Track
                }

                using (StreamWriter sw = File.CreateText(path_training))
                {
                    sw.WriteLine(".ABCDEF"); 
                }

                Read();
                Encode();
            }

            string[] linesOfTraining = File.ReadAllLines(path_training);

            for (int i = 0; i < linesOfTraining.Length; i++)
            {
                Information.Training += linesOfTraining[i];
            }

            if (Information.Training.Length > 1 && Information.Training[1] != '\0')
            {
                if (check == Information.Training[1])
                {
                    counter = 0;
                    timer1.Interval = 100;
                    timer1.Enabled = true;
                    timer1.Tick += new System.EventHandler(timer1_Tick);

                    Information.Training = Information.Training.Remove(1, 1);

                    File.WriteAllText(path_training, string.Empty);

                    using (StreamWriter sw = File.AppendText(path_training))
                    {
                        sw.WriteLine(Information.Training);
                    }
                }
            }
            
            DataInitialization();
            Start();

            Information.Current_Track = rnd.Next(0, 10);

            PlayNextSound();
            Mus_Sound();
            CreateCloseElements();
            AllTime();
        }

        public static void PlayNextSound()
        {
            Player = new WindowsMediaPlayer();
            Player.PlayStateChange += (newState) =>
            {
                //System.Diagnostics.Debug.WriteLine("cTrack: " +currentTrack);
                if (newState == 1)
                {
                    Information.Current_Track++;

                    if (Information.Current_Track == 10)
                        Information.Current_Track = 0;

                    Write();
                    Encode();

                    Player.close();
                    PlayNextSound();
                }
                //System.Diagnostics.Debug.WriteLine("cTrack_after: " + currentTrack);
                //Player.settings.volume = 50;
            };

            Player.URL = $@"./Music/{trackList[Information.Current_Track]}.mp3";

            //label1.Text = trackList[currentTrack].ToString();

            Player.controls.play();
            //System.Diagnostics.Debug.WriteLine(Information.Current_Track);
        }

        public void DataInitialization()
        {
            Random rnd = new Random();

            ToolTip t = new ToolTip();
            t.SetToolTip(hintItemOne, "За каждую игру умножается полученный опыт в два раза");
            t.SetToolTip(hintItemTwo, "Бонус позволяет играть в игры, не тратя денег");
            t.SetToolTip(hintItemThree, "После каждой победы, дополнительно \nвносится 25% от выигрышной суммы");
            t.SetToolTip(hintItemFour, "Последующие выигрышные деньги превращаются в опыт");
            t.SetToolTip(hintItemFive, "Даёт пассивный эффект, после каждой игры, \nв бюджет возвращается 10% от потраченной суммы");
            t.SetToolTip(hintItemSix, "Деньги с функции Пополнить счёт удваиваются");

            panelChangeName.BackColor = Color.FromArgb(31, 31, 31);

            labels = new Label[] { userName, userFinance, labelUserLvL, labelUserXP };

            string[] lines = File.ReadAllLines(path_data);

            Decode();

            for (int i = 0; i < 4; i++)
            {
                labels[i].Text = lines[i];
            }

            LvLCheck();

            if (Information.StaticLvL == 7)
                limitsOff = 1;
            else if (Information.StaticLvL >= 8)
                limitsOff = 2;

            labelUserLvL.Text = userLvL;
            labelUserXP.Text = userXP;

            SetWallpaper();

            userName.Text = "Имя: " + Information.Name;
            userFinance.Text = "Бюджет: " + Information.Finance;

            if (Information.ItemThree_Additional_Perc)
                labelInfoItem_Three.Text = "= Включён";
            else
                labelInfoItem_Three.Text = "= Отключён";

            if (Information.ItemFive_Pass_Accumulation)
                labelInfoItem_Five.Text = "= Включён";
            else
                labelInfoItem_Five.Text = "= Отключён";

            if (Information.ItemSix_x2_TopUpAcc)
                labelInfoItem_Six.Text = "= Включён";
            else
                labelInfoItem_Six.Text = "= Отключён";

            labelInfoItem_One.Text = "= " + Information.ItemOne_2x_XP.ToString();
            labelInfoItem_Two.Text = "= " + Information.ItemTwo_WW_Game.ToString();
            labelInfoItem_Four.Text = "= " + Information.ItemFour_XP_Cash.ToString();

            
            if (Information.Xp >= 50000 && Information.Finance >= 1000000)
            {
                labelRemainderToWin.Text = "";
                Information.Win = true;
                btnEndGame.Visible = true;

                Write();
                Encode();
            }

            if (Information.Win)
            {
                labelRemainderToWin.Text = "";
                Information.Win = true;
                btnEndGame.Visible = true;
            }
            else
            {
                labelRemainderToWin.Text = ($"Осталось: {(50000 - Information.Xp <= 0 ? 0 : 50000 - Information.Xp)} xp и { (1000000 - Information.Finance <= 0 ? 0 : 1000000 - Information.Finance)}$");
            }
        }

        public void Start()
        {
            if (Information.Finance > 10)
                topUpAccount.Enabled = false;
            else
                topUpAccount.Enabled = true;

            turnOnOff.Click += (obj, e) =>
            {
                SoundClickButton();

                Information.Wallpaper++;

                Information.Finance -= 100;
                userFinance.Text = "Бюджет: " + Information.Finance;

                if (Information.Finance > 100)
                    turnOnOff.Enabled = true;
                else
                    turnOnOff.Enabled = false;

                if (Information.Finance < 10)
                    topUpAccount.Enabled = true;

                SetWallpaper();

                if (Information.Xp >= 50000 && Information.Finance >= 1000000)
                {
                    labelRemainderToWin.Text = "";
                    Information.Win = true;
                    btnEndGame.Visible = true;
                    Write();
                    Encode();
                }
             
                if (Information.Win)
                {
                    labelRemainderToWin.Text = "";
                    btnEndGame.Visible = true;
                }
                else
                {
                    labelRemainderToWin.Text = ($"Осталось: {(50000 - Information.Xp <= 0 ? 0 : 50000 - Information.Xp)} xp и { (1000000 - Information.Finance <= 0 ? 0 : 1000000 - Information.Finance)}$");
                }

            };

            btnMusicOffOn.Click += (obj, e) =>
            {
                if (Information.Music)
                {
                    Information.Music = false;

                    Write();
                    Encode();

                    if (!Information.Music)
                        Player.close();

                    btnMusicOffOn.BackgroundImage = (Image)Properties.Resources.ResourceManager.GetObject("music_off");
                }

                else
                {
                    Information.Music = true;

                    Write();
                    Encode();

                    PlayNextSound();

                    btnMusicOffOn.BackgroundImage = (Image)Properties.Resources.ResourceManager.GetObject("music_on");
                }

            };

            btnSoundOffOn.Click += (obj, e) =>
            {
                if (Information.Sound)
                {
                    Information.Sound = false;

                    Write();
                    Encode();

                    btnSoundOffOn.BackgroundImage = (Image)Properties.Resources.ResourceManager.GetObject("sound_off");
                }

                else
                {
                    Information.Sound = true;

                    Write();
                    Encode();

                    btnSoundOffOn.BackgroundImage = (Image)Properties.Resources.ResourceManager.GetObject("sound_on");
                }
            };

            buttonGameOne.Click += (obj, e) =>
            {
                SlotMachine formSlotMachine = new SlotMachine();

                Hide();

                SoundClickButton();

                if (formSlotMachine.ShowDialog() == DialogResult.OK)
                {
                    DataInitialization();
                    LvLCheck();

                    if (Information.Finance < 10)
                        topUpAccount.Enabled = true;
                    Mus_Sound();
                    Show();
                }
            };

            buttonGameTwo.Click += (obj, e) =>
            {
                Form formLuckyRandom = new LuckyRandom();

                Hide();

                SoundClickButton();

                if (formLuckyRandom.ShowDialog() == DialogResult.OK)
                {
                    DataInitialization();
                    if (Information.Finance < 10)
                        topUpAccount.Enabled = true;
                    Mus_Sound();
                    Show();
                }
            };

            buttonGameThree.Click += (obj, e) =>
            {
                Form formYourRandom = new YourRandom();

                Hide();

                SoundClickButton();

                if (formYourRandom.ShowDialog() == DialogResult.OK)
                {
                    DataInitialization();
                    if (Information.Finance < 10)
                        topUpAccount.Enabled = true;
                    Mus_Sound();
                    Show();
                }
            };

            buttonGameFour.Click += (obj, e) =>
            {
                Form formLuckyTree = new LuckyTree();

                Hide();

                SoundClickButton();

                if (formLuckyTree.ShowDialog() == DialogResult.OK)
                {
                    DataInitialization();
                    if (Information.Finance < 10)
                        topUpAccount.Enabled = true;
                    Mus_Sound();
                    Show();
                }
            };

            buttonShop.Click += (obj, e) =>
            {
                Form formShop = new Shop();

                Hide();

                SoundClickButton();

                if (formShop.ShowDialog() == DialogResult.OK)
                {
                    DataInitialization();
                    if (Information.Finance < 10)
                        topUpAccount.Enabled = true;
                    Mus_Sound();
                    Show();
                    CreateCloseElements();
                }
            };

            btnEndGame.Click += (obj, e) =>
            {
                Form endForm = new EndForm();

                Hide();

                SoundClickButton();

                if (endForm.ShowDialog() == DialogResult.OK)
                {
                    DataInitialization();
                    if (Information.Finance < 10)
                        topUpAccount.Enabled = true;
                    Mus_Sound();
                    Show();
                    CreateCloseElements();
                    //TimerOnOff();
                }
            };

            changeName.Click += (obj, e) =>
            {
                if (nameBox.Text != "" && Information.Finance >= (50 * nameBox.Text.Length))
                {
                    Information.Name = nameBox.Text;
                    userName.Text = "Имя: " + Information.Name;

                    Information.Finance -= 50 * nameBox.Text.Length;
                    userFinance.Text = "Бюджет: " + Information.Finance.ToString();

                    Write();
                    Encode();

                    if (Information.Xp >= 50000 && Information.Finance >= 1000000)
                    {
                        labelRemainderToWin.Text = "";
                        Information.Win = true;
                        btnEndGame.Visible = true;
                        Write();
                        Encode();
                    }

                    if (Information.Win)
                    {
                        labelRemainderToWin.Text = "";
                        btnEndGame.Visible = true;
                    }
                    else
                    {
                        labelRemainderToWin.Text = ($"Осталось: {(50000 - Information.Xp <= 0 ? 0 : 50000 - Information.Xp)} xp и { (1000000 - Information.Finance <= 0 ? 0 : 1000000 - Information.Finance)}$");
                    }

                    nameBox.Text = "";

                    PayToTurn();

                    changeName.Enabled = false;
                }
            };

            topUpAccount.Click += (obj, e) =>
            {
                if (Information.Finance <= 9)
                {
                    GetToCash();

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

                    userFinance.Text = "Бюджет: " + Information.Finance.ToString();

                    Write();
                    Encode();

                    if (Information.Finance > 0)
                        topUpAccount.Enabled = false;

                    //System.Diagnostics.Debug.WriteLine("Счёт пополнен. Бюджет: " + Information.Finance);
                    turnOnOff.Enabled = true;
                }

                if (Information.Xp >= 50000 && Information.Finance >= 1000000)
                {
                    labelRemainderToWin.Text = "";
                    Information.Win = true;
                    btnEndGame.Visible = true;
                    Write();
                    Encode();
                }

                if (Information.Win)
                {
                    labelRemainderToWin.Text = "";
                    btnEndGame.Visible = true;
                }
                else
                {
                    labelRemainderToWin.Text = ($"Осталось: {(50000 - Information.Xp <= 0 ? 0 : 50000 - Information.Xp)} xp и { (1000000 - Information.Finance <= 0 ? 0 : 1000000 - Information.Finance)}$");
                }
            };
        }

        public static void LvLCheck()
        {
            if (Information.Xp < arrayLvLsAndXP[Information.Lvl, 1])
            {
                Information.Lvl = arrayLvLsAndXP[Information.Lvl, 0];
                userLvL = Information.Lvl.ToString();
                userXP = Information.Xp + $" / {arrayLvLsAndXP[Information.Lvl, 1]}";
            }

            if (Information.Xp >= arrayLvLsAndXP[Information.Lvl, 1])
            {
                Information.Lvl++;

                if (Information.Lvl == 1)
                {
                    Information.Finance += 500;
                    MessageBox.Show($"Поздравляю! Ты достиг {Information.Lvl} уровня\nТы получаешь: 500$", "LvL UP");
                }

                if (Information.Lvl == 2)
                {
                    Information.Finance += 1500;
                    MessageBox.Show($"Поздравляю! Ты достиг {Information.Lvl} уровня\nТы получаешь: 1500$", "LvL UP");
                }

                if (Information.Lvl == 3)
                {
                    Information.Finance += 2000;
                    Information.ItemOne_2x_XP += 10;
                    MessageBox.Show($"Поздравляю! Ты достиг {Information.Lvl} уровня\nТы получаешь: 2000$ и 10 (x2 XP)", "LvL UP");
                }

                if (Information.Lvl == 4)
                {
                    Information.Finance += 3000;
                    Information.ItemOne_2x_XP += 5;
                    Information.ItemTwo_WW_Game++;
                    MessageBox.Show($"Поздравляю! Ты достиг {Information.Lvl} уровня\nТы получаешь: 3000$, 5 (x2 XP) и 1 (Win-Win)", "LvL UP");
                }

                if (Information.Lvl == 5)
                {
                    Information.Finance += 5000;
                    Information.ItemFour_XP_Cash += 3;
                    MessageBox.Show($"Поздравляю! Ты достиг {Information.Lvl} уровня\nТы получаешь: 5000$ и 1 (XP => Cash)", "LvL UP");
                }

                if (Information.Lvl == 6)
                {
                    Information.Finance += 10000;
                    Information.ItemOne_2x_XP += 50;
                    MessageBox.Show($"Поздравляю! Ты достиг {Information.Lvl} уровня\nТы получаешь: 10000$ и 50 (x2 XP)", "LvL UP");
                }

                if (Information.Lvl == 7)
                {
                    Information.Finance += 15000;
                    Information.ItemTwo_WW_Game += 5;
                    Information.ItemFour_XP_Cash += 2;
                    MessageBox.Show($"Поздравляю! Ты достиг {Information.Lvl} уровня\nТы получаешь:15000$, 5 (Win-Win) и 2 (XP => Cash)", "LvL UP");
                }

                if (Information.Lvl == 8)
                {
                    Information.Finance += 50000;
                    Information.ItemFour_XP_Cash += 5;
                    MessageBox.Show($"Поздравляю! Ты достиг {Information.Lvl} уровня\nТы получаешь: 30000$ и 5 (XP => Cash)", "LvL UP");
                }

                if (Information.Lvl == 9)
                {
                    Information.Finance += 100000;
                    Information.ItemOne_2x_XP += 100;
                    Information.ItemTwo_WW_Game += 10;
                    Information.ItemFour_XP_Cash += 5;
                    MessageBox.Show($"Поздравляю! Ты достиг {Information.Lvl} уровня\nТы получаешь: 50000$, 100 (x2 XP), 10 (Win-win) и 5 (XP => Cash)", "LvL UP");
                }

                if (Information.Lvl == 10)
                {
                    Information.Finance += 1000000;
                    Information.ItemOne_2x_XP += 1000;
                    Information.ItemFour_XP_Cash += 20;
                    MessageBox.Show($"Поздравляю! Ты достиг {Information.Lvl} уровня\nТы получаешь: 100000$, 1000 (x2 XP) и 20 (XP => Cash)", "LvL UP");
                }

                if (Information.Lvl == 11)
                {
                    Information.Finance += 10000000;
                    Information.ItemOne_2x_XP += 10000;
                    Information.ItemFour_XP_Cash += 200;
                    MessageBox.Show($"Поздравляю! Ты достиг {Information.Lvl} уровня\nТы получаешь: 10000000, 10000 (x2 XP) и 200 (XP => Cash)", "LvL UP");
                }

                if (Information.Xp >= 9999999)
                {
                    MessageBox.Show($"Поздравляю! Тебе нечем заняться, друг. Я обнулю твой прогресс :3");
                    Information.Finance = 0;
                    Information.Lvl = 0;
                    Information.Xp = 0;
                }

                userLvL = Information.Lvl.ToString();
                userXP = Information.Xp + $" / {arrayLvLsAndXP[Information.Lvl, 1]}";

                LvLCheck();
            }

            if (Information.StaticXP < arrayLvLsAndXP[Information.StaticLvL, 1])
                Information.StaticLvL = arrayLvLsAndXP[Information.StaticLvL, 0];

            if (Information.StaticXP >= arrayLvLsAndXP[Information.StaticLvL, 1])
            {
                Information.StaticLvL++;

                if (Information.StaticLvL == 3)
                {
                    MessageBox.Show("Ты открыл форму 'Your Random'");
                    closeYourRandom.Visible = false;
                }

                if (Information.StaticLvL == 4)
                {
                    MessageBox.Show("Ты открыл Магазин");
                    closeShop.Visible = false;
                }

                if (Information.StaticLvL == 6)
                {
                    MessageBox.Show("Ты открыл форму 'Lucky Random'");
                    closeLuckyRandom.Visible = false;
                }

                if (Information.StaticLvL == 7)
                {
                    MessageBox.Show("Лимит в 'Your Random' отключен!");
                    limitsOff = 1;
                }

                if (Information.StaticLvL == 8)
                {
                    MessageBox.Show("Лимит в'Lucky Random' отключен!");
                    limitsOff = 2;
                }

                if (Information.StaticLvL == 10)
                {
                    MessageBox.Show("Поздравляем! Ты достиг уровня Великого игрока!");
                    Personal_data.Achieve = true;
                }

                LvLCheck();
            }
            //System.Diagnostics.Debug.WriteLine($"Уровень: {Information.Lvl} и опыт: {Information.Xp}");
            Write();
            Encode();
        }

        public static void Write()
        {
            File.WriteAllText(path_data, string.Empty);

            using (StreamWriter sw = File.AppendText(path_data))
            {
                sw.WriteLine(Information.Name);
                sw.WriteLine(Information.Finance);
                sw.WriteLine(Information.Lvl);
                sw.WriteLine(Information.Xp);
                sw.WriteLine(Information.ItemOne_2x_XP);
                sw.WriteLine(Information.ItemTwo_WW_Game);
                sw.WriteLine(Information.ItemThree_Additional_Perc);
                sw.WriteLine(Information.ItemFour_XP_Cash);
                sw.WriteLine(Information.ItemFive_Pass_Accumulation);
                sw.WriteLine(Information.ItemSix_x2_TopUpAcc);
                sw.WriteLine(Information.ItemEight_OpenApp);
                sw.WriteLine(Information.Wallpaper);
                sw.WriteLine(Information.StaticLvL);
                sw.WriteLine(Information.StaticXP);
                sw.WriteLine(Information.Training);
                sw.WriteLine(Information.Current_Track);
                sw.WriteLine(Information.Music);
                sw.WriteLine(Information.Sound);
                sw.WriteLine(Information.Animation);
                sw.WriteLine(Information.Win);
            }

            File.WriteAllText(path_statistics_data, string.Empty);

            using (StreamWriter sw = File.AppendText(path_statistics_data))
            {
                sw.WriteLine(Personal_data.Wins);
                sw.WriteLine(Personal_data.Losses);
                sw.WriteLine(Personal_data.Items_bought);
                sw.WriteLine(Personal_data.Opened_extra_app);
                sw.WriteLine(Personal_data.Achieve);
            }

            File.WriteAllText(path_statistics_times, string.Empty);

            using (StreamWriter sw = File.AppendText(path_statistics_times))
            {
                sw.WriteLine(Personal_data.All_time);
                sw.WriteLine(Personal_data.SlotMachine_time);
                sw.WriteLine(Personal_data.YourRandom_time);
                sw.WriteLine(Personal_data.LuckyRandom_time);
                sw.WriteLine(Personal_data.LuckyTree_time);
            }
        }

        public static void Read()
        {
            string[] linesOfData = File.ReadAllLines(path_data);

            Information.Name = linesOfData[0];
            Information.Finance = long.Parse(linesOfData[1]);
            Information.Lvl = int.Parse(linesOfData[2]);
            Information.Xp = int.Parse(linesOfData[3]);
            Information.ItemOne_2x_XP = int.Parse(linesOfData[4]);
            Information.ItemTwo_WW_Game = int.Parse(linesOfData[5]);
            Information.ItemThree_Additional_Perc = bool.Parse(linesOfData[6]);
            Information.ItemFour_XP_Cash = int.Parse(linesOfData[7]);
            Information.ItemFive_Pass_Accumulation = bool.Parse(linesOfData[8]);
            Information.ItemSix_x2_TopUpAcc = bool.Parse(linesOfData[9]);
            Information.ItemEight_OpenApp = bool.Parse(linesOfData[10]);
            Information.Wallpaper = int.Parse(linesOfData[11]);
            Information.StaticLvL = int.Parse(linesOfData[12]);
            Information.StaticXP = int.Parse(linesOfData[13]);
            Information.Current_Track = int.Parse(linesOfData[14]);
            Information.Music = bool.Parse(linesOfData[15]);
            Information.Sound = bool.Parse(linesOfData[16]);
            Information.Animation = bool.Parse(linesOfData[17]);
            Information.Win = bool.Parse(linesOfData[18]);

            string[] linesOfStatistics = File.ReadAllLines(path_statistics_data);

            Personal_data.Wins = int.Parse(linesOfStatistics[0]);
            Personal_data.Losses = int.Parse(linesOfStatistics[1]);
            Personal_data.Items_bought = int.Parse(linesOfStatistics[2]);
            Personal_data.Opened_extra_app = bool.Parse(linesOfStatistics[3]);
            Personal_data.Achieve = bool.Parse(linesOfStatistics[4]);

            string[] linesOfTimes = File.ReadAllLines(path_statistics_times);

            Personal_data.All_time = int.Parse(linesOfTimes[0]);
            Personal_data.SlotMachine_time = int.Parse(linesOfTimes[1]);
            Personal_data.YourRandom_time = int.Parse(linesOfTimes[2]);
            Personal_data.LuckyRandom_time = int.Parse(linesOfTimes[3]);
            Personal_data.LuckyTree_time = int.Parse(linesOfTimes[4]);
        }

        private void NameBox_TextChanged(object sender, EventArgs e)
        {
            if (nameBox.Text != "" && Information.Finance >= (50 * nameBox.Text.Length))
            {
                changeName.Enabled = true;

                if (Information.Finance > 10)
                    topUpAccount.Enabled = false;
                else
                    topUpAccount.Enabled = true;
            }

            else
            {
                changeName.Enabled = false;

                if (Information.Finance > 10)
                    topUpAccount.Enabled = false;
                else
                    topUpAccount.Enabled = true;
            }
        }

        private void SetWallpaper()
        {
            if (Information.Wallpaper == 1)
            {
                Write();
                Encode();

                BackgroundImage = (Image)Properties.Resources.ResourceManager.GetObject("Casino_menu_blue");
                pictureBoxMain_Info.Image = (Image)Properties.Resources.ResourceManager.GetObject("informationBlue");
                pictureBoxMain_Games.Image = (Image)Properties.Resources.ResourceManager.GetObject("mainBlue");
                pictureBoxMain_Bonuses.Image = (Image)Properties.Resources.ResourceManager.GetObject("bonusesBlue");
                userName.ForeColor = Color.FromArgb(251, 0, 162);
                userFinance.ForeColor = Color.FromArgb(251, 0, 162);
                labelLevel.ForeColor = Color.FromArgb(251, 0, 162);
                labelUserLvL.ForeColor = Color.FromArgb(251, 0, 162);
                labelExp.ForeColor = Color.FromArgb(251, 0, 162);
                labelUserXP.ForeColor = Color.FromArgb(251, 0, 162);

                labelInfoItem_One.ForeColor = Color.FromArgb(153, 153, 255);
                labelInfoItem_Two.ForeColor = Color.FromArgb(153, 153, 255);
                labelInfoItem_Three.ForeColor = Color.FromArgb(153, 153, 255);
                labelInfoItem_Four.ForeColor = Color.FromArgb(153, 153, 255);
                labelInfoItem_Five.ForeColor = Color.FromArgb(153, 153, 255);
                labelInfoItem_Six.ForeColor = Color.FromArgb(153, 153, 255);

                nameBox.BackColor = Color.FromArgb(31, 31, 31);
                nameBox.ForeColor = Color.FromArgb(251, 0, 162);

                changeName.FlatAppearance.MouseDownBackColor = Color.FromArgb(251, 0, 162);
                changeName.FlatAppearance.MouseOverBackColor = Color.FromArgb(249, 129, 207);

                topUpAccount.FlatAppearance.MouseDownBackColor = Color.FromArgb(242, 0, 244);
                topUpAccount.FlatAppearance.MouseOverBackColor = Color.FromArgb(203, 49, 206);

                buttonGameOne.FlatAppearance.MouseDownBackColor = Color.Gold;
                buttonGameTwo.FlatAppearance.MouseDownBackColor = Color.Gold;
                buttonGameThree.FlatAppearance.MouseDownBackColor = Color.Gold;
                buttonGameFour.FlatAppearance.MouseDownBackColor = Color.Gold;
                buttonShop.FlatAppearance.MouseDownBackColor = Color.Gold;

                buttonGameOne.FlatAppearance.MouseOverBackColor = Color.Yellow;
                buttonGameTwo.FlatAppearance.MouseOverBackColor = Color.Yellow;
                buttonGameThree.FlatAppearance.MouseOverBackColor = Color.Yellow;
                buttonGameFour.FlatAppearance.MouseOverBackColor = Color.Yellow;
                buttonShop.FlatAppearance.MouseOverBackColor = Color.Yellow;
            }

            else if (Information.Wallpaper == 2)
            {
                Write();
                Encode();

                BackgroundImage = (Image)Properties.Resources.ResourceManager.GetObject("Casino_menu_green");
                pictureBoxMain_Info.Image = (Image)Properties.Resources.ResourceManager.GetObject("informationGreen");
                pictureBoxMain_Games.Image = (Image)Properties.Resources.ResourceManager.GetObject("mainGreen");
                pictureBoxMain_Bonuses.Image = (Image)Properties.Resources.ResourceManager.GetObject("bonusesGreen");
                userName.ForeColor = Color.FromArgb(28, 113, 255);
                userFinance.ForeColor = Color.FromArgb(28, 113, 255);
                labelLevel.ForeColor = Color.FromArgb(28, 113, 255);
                labelUserLvL.ForeColor = Color.FromArgb(28, 113, 255);
                labelExp.ForeColor = Color.FromArgb(28, 113, 255);
                labelUserXP.ForeColor = Color.FromArgb(28, 113, 255);

                labelInfoItem_One.ForeColor = Color.FromArgb(178, 0, 255);
                labelInfoItem_Two.ForeColor = Color.FromArgb(178, 0, 255);
                labelInfoItem_Three.ForeColor = Color.FromArgb(178, 0, 255);
                labelInfoItem_Four.ForeColor = Color.FromArgb(178, 0, 255);
                labelInfoItem_Five.ForeColor = Color.FromArgb(178, 0, 255);
                labelInfoItem_Six.ForeColor = Color.FromArgb(178, 0, 255);

                nameBox.BackColor = Color.FromArgb(31, 31, 31);
                nameBox.ForeColor = Color.FromArgb(28, 113, 255);

                changeName.FlatAppearance.MouseDownBackColor = Color.FromArgb(28, 113, 255);
                changeName.FlatAppearance.MouseOverBackColor = Color.FromArgb(28, 167, 255);

                topUpAccount.FlatAppearance.MouseDownBackColor = Color.FromArgb(28, 113, 255);
                topUpAccount.FlatAppearance.MouseOverBackColor = Color.FromArgb(28, 167, 255);

                buttonGameOne.FlatAppearance.MouseDownBackColor = Color.OrangeRed;
                buttonGameTwo.FlatAppearance.MouseDownBackColor = Color.OrangeRed;
                buttonGameThree.FlatAppearance.MouseDownBackColor = Color.OrangeRed;
                buttonGameFour.FlatAppearance.MouseDownBackColor = Color.OrangeRed;
                buttonShop.FlatAppearance.MouseDownBackColor = Color.OrangeRed;

                buttonGameOne.FlatAppearance.MouseOverBackColor = Color.Orange;
                buttonGameTwo.FlatAppearance.MouseOverBackColor = Color.Orange;
                buttonGameThree.FlatAppearance.MouseOverBackColor = Color.Orange;
                buttonGameFour.FlatAppearance.MouseOverBackColor = Color.Orange;
                buttonShop.FlatAppearance.MouseOverBackColor = Color.Orange;
            }

            else if (Information.Wallpaper == 3)
            {
                Write();
                Encode();

                BackgroundImage = (Image)Properties.Resources.ResourceManager.GetObject("Casino_menu_pink");
                pictureBoxMain_Info.Image = (Image)Properties.Resources.ResourceManager.GetObject("informationPink");
                pictureBoxMain_Games.Image = (Image)Properties.Resources.ResourceManager.GetObject("mainPink");
                pictureBoxMain_Bonuses.Image = (Image)Properties.Resources.ResourceManager.GetObject("bonusesPink");
                userName.ForeColor = Color.FromArgb(255, 0, 0);
                userFinance.ForeColor = Color.FromArgb(255, 0, 0);
                labelLevel.ForeColor = Color.FromArgb(255, 0, 0);
                labelUserLvL.ForeColor = Color.FromArgb(255, 0, 0);
                labelExp.ForeColor = Color.FromArgb(255, 0, 0);
                labelUserXP.ForeColor = Color.FromArgb(255, 0, 0);

                labelInfoItem_One.ForeColor = Color.FromArgb(255, 216, 0);
                labelInfoItem_Two.ForeColor = Color.FromArgb(255, 216, 0);
                labelInfoItem_Three.ForeColor = Color.FromArgb(255, 216, 0);
                labelInfoItem_Four.ForeColor = Color.FromArgb(255, 216, 0);
                labelInfoItem_Five.ForeColor = Color.FromArgb(255, 216, 0);
                labelInfoItem_Six.ForeColor = Color.FromArgb(255, 216, 0);

                nameBox.BackColor = Color.FromArgb(31, 31, 31);
                nameBox.ForeColor = Color.FromArgb(255, 0, 0);

                changeName.FlatAppearance.MouseDownBackColor = Color.FromArgb(255, 0, 0);
                changeName.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 81, 81);

                topUpAccount.FlatAppearance.MouseDownBackColor = Color.FromArgb(255, 0, 0);
                topUpAccount.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 81, 81);

                buttonGameOne.FlatAppearance.MouseDownBackColor = Color.Green;
                buttonGameTwo.FlatAppearance.MouseDownBackColor = Color.Green;
                buttonGameThree.FlatAppearance.MouseDownBackColor = Color.Green;
                buttonGameFour.FlatAppearance.MouseDownBackColor = Color.Green;
                buttonShop.FlatAppearance.MouseDownBackColor = Color.Green;

                buttonGameOne.FlatAppearance.MouseOverBackColor = Color.LimeGreen;
                buttonGameTwo.FlatAppearance.MouseOverBackColor = Color.LimeGreen;
                buttonGameThree.FlatAppearance.MouseOverBackColor = Color.LimeGreen;
                buttonGameFour.FlatAppearance.MouseOverBackColor = Color.LimeGreen;
                buttonShop.FlatAppearance.MouseOverBackColor = Color.LimeGreen;
            }

            else if (Information.Wallpaper == 4)
            {
                Write();
                Encode();

                BackgroundImage = (Image)Properties.Resources.ResourceManager.GetObject("Casino_menu_red");
                pictureBoxMain_Info.Image = (Image)Properties.Resources.ResourceManager.GetObject("informationRed");
                pictureBoxMain_Games.Image = (Image)Properties.Resources.ResourceManager.GetObject("mainRed");
                pictureBoxMain_Bonuses.Image = (Image)Properties.Resources.ResourceManager.GetObject("bonusesRed");
                userName.ForeColor = Color.FromArgb(0, 255, 33);
                userFinance.ForeColor = Color.FromArgb(0, 255, 33);
                labelLevel.ForeColor = Color.FromArgb(0, 255, 33);
                labelUserLvL.ForeColor = Color.FromArgb(0, 255, 33);
                labelExp.ForeColor = Color.FromArgb(0, 255, 33);
                labelUserXP.ForeColor = Color.FromArgb(0, 255, 33);

                labelInfoItem_One.ForeColor = Color.FromArgb(128, 255, 255);
                labelInfoItem_Two.ForeColor = Color.FromArgb(128, 255, 255);
                labelInfoItem_Three.ForeColor = Color.FromArgb(128, 255, 255);
                labelInfoItem_Four.ForeColor = Color.FromArgb(128, 255, 255);
                labelInfoItem_Five.ForeColor = Color.FromArgb(128, 255, 255);
                labelInfoItem_Six.ForeColor = Color.FromArgb(128, 255, 255);

                nameBox.BackColor = Color.FromArgb(31, 31, 31);
                nameBox.ForeColor = Color.FromArgb(0, 255, 33);

                changeName.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 255, 33);
                changeName.FlatAppearance.MouseOverBackColor = Color.FromArgb(102, 255, 119);

                topUpAccount.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 255, 33);
                topUpAccount.FlatAppearance.MouseOverBackColor = Color.FromArgb(102, 255, 119);

                buttonGameOne.FlatAppearance.MouseDownBackColor = Color.Red;
                buttonGameTwo.FlatAppearance.MouseDownBackColor = Color.Red;
                buttonGameThree.FlatAppearance.MouseDownBackColor = Color.Red;
                buttonGameFour.FlatAppearance.MouseDownBackColor = Color.Red;
                buttonShop.FlatAppearance.MouseDownBackColor = Color.Red;

                buttonGameOne.FlatAppearance.MouseOverBackColor = Color.IndianRed;
                buttonGameTwo.FlatAppearance.MouseOverBackColor = Color.IndianRed;
                buttonGameThree.FlatAppearance.MouseOverBackColor = Color.IndianRed;
                buttonGameFour.FlatAppearance.MouseOverBackColor = Color.IndianRed;
                buttonShop.FlatAppearance.MouseOverBackColor = Color.IndianRed;
            }

            else if (Information.Wallpaper == 5)
            {
                Write();
                Encode();

                BackgroundImage = (Image)Properties.Resources.ResourceManager.GetObject("Casino_menu_yellow");
                pictureBoxMain_Info.Image = (Image)Properties.Resources.ResourceManager.GetObject("informationYellow");
                pictureBoxMain_Games.Image = (Image)Properties.Resources.ResourceManager.GetObject("mainYellow");
                pictureBoxMain_Bonuses.Image = (Image)Properties.Resources.ResourceManager.GetObject("bonusesYellow");
                userName.ForeColor = Color.FromArgb(0, 255, 255);
                userFinance.ForeColor = Color.FromArgb(0, 255, 255);
                labelLevel.ForeColor = Color.FromArgb(0, 255, 255);
                labelUserLvL.ForeColor = Color.FromArgb(0, 255, 255);
                labelExp.ForeColor = Color.FromArgb(0, 255, 255);
                labelUserXP.ForeColor = Color.FromArgb(0, 255, 255);

                labelInfoItem_One.ForeColor = Color.FromArgb(0, 163, 255);
                labelInfoItem_Two.ForeColor = Color.FromArgb(0, 163, 255);
                labelInfoItem_Three.ForeColor = Color.FromArgb(0, 163, 255);
                labelInfoItem_Four.ForeColor = Color.FromArgb(0, 163, 255);
                labelInfoItem_Five.ForeColor = Color.FromArgb(0, 163, 255);
                labelInfoItem_Six.ForeColor = Color.FromArgb(0, 163, 255);

                nameBox.BackColor = Color.FromArgb(31, 31, 31);
                nameBox.ForeColor = Color.FromArgb(0, 255, 255);

                changeName.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 255, 255);
                changeName.FlatAppearance.MouseOverBackColor = Color.FromArgb(151, 255, 255);

                topUpAccount.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 255, 255);
                topUpAccount.FlatAppearance.MouseOverBackColor = Color.FromArgb(151, 255, 255);

                buttonGameOne.FlatAppearance.MouseDownBackColor = Color.Blue;
                buttonGameTwo.FlatAppearance.MouseDownBackColor = Color.Blue;
                buttonGameThree.FlatAppearance.MouseDownBackColor = Color.Blue;
                buttonGameFour.FlatAppearance.MouseDownBackColor = Color.Blue;
                buttonShop.FlatAppearance.MouseDownBackColor = Color.Blue;

                buttonGameOne.FlatAppearance.MouseOverBackColor = Color.DeepSkyBlue;
                buttonGameTwo.FlatAppearance.MouseOverBackColor = Color.DeepSkyBlue;
                buttonGameThree.FlatAppearance.MouseOverBackColor = Color.DeepSkyBlue;
                buttonGameFour.FlatAppearance.MouseOverBackColor = Color.DeepSkyBlue;
                buttonShop.FlatAppearance.MouseOverBackColor = Color.DeepSkyBlue;

                Information.Wallpaper = 0;
            }

            changeName.FlatAppearance.BorderColor = Color.Black;
            changeName.FlatAppearance.BorderSize = 2;

            topUpAccount.FlatAppearance.BorderColor = Color.Black;
            topUpAccount.FlatAppearance.BorderSize = 2;
        }

        public static void SoundClickButton()
        {
            Form1.sPlay.SoundLocation = @"./Sounds/click.wav";

            if (Information.Sound)
                Form1.sPlay.Play();
        }
        public static void PayToTurn()
        {
            Form1.sPlay.SoundLocation = @"./Sounds/pay.wav";

            if (Information.Sound)
                Form1.sPlay.Play();
        }
        public static void GetToCash()
        {
            Form1.sPlay.SoundLocation = @"./Sounds/win_cash.wav";

            if (Information.Sound)
                Form1.sPlay.Play();
        }
        public static void LaughMethod()
        {
            Form1.sPlay.SoundLocation = @"./Sounds/" + Form1.laughList[rnd.Next(0, 9)] + ".wav";

            if (Information.Sound)
                Form1.sPlay.Play();
        }

        private void timer1_Tick(object sender, System.EventArgs e)
        {
            if (counter == 10)
            {
                timer1.Stop();
                MessageBox.Show("Привет! Прошу не ругайся на графику этого приложения,\n" +
                    "если это можно назвать графикой.\nВ свою защиту, скажу, что это мой первый проект. ");

                MessageBox.Show("Вкратце расскажу тебе, что здесь и к чему.\n" +
                    "Главное меню делится на 3 блока.\n" +
                    "Первый блок (левый) содержит в себе информацию о тебе:\n" +
                    "Имя, бюджет, уровень, очки опыта.\nНиже находится окошко, в котором можно изменить имя," +
                    "для этого введи свой ник и нажми изменить имя, однако это не бесплатно.\n" +
                    "Стоимость смены ника зависит от длины имени.\n" +
                    "Также на этом окошке ты можешь увидеть сколько тебе осталось заработать очков опыта и денег для победы.\n" +
                    "Ну и самое приятное, это бесплатное пополнение счёта, суть проста," +
                    "когда твой бюджет будет меньше 10$, ты можешь пополнить свой счёт.\n" +
                    "Чем выше твой уровень, тем больше денег ты получишь.");

                MessageBox.Show("Второй блок (центр) содержит в себе формы (приложения), к несчастью/счастью (выбирай сам)" +
                    " формы будут закрыты, кроме игрового автомата.\n" +
                    "Формы будут открываться с повышением твоего уровня.\n" +
                    "YourRandom - 3 lvl. Shop - 4 lvl. LuckyRandom - 6 lvl.");

                MessageBox.Show("Третий блок (последний) содержит в себе информацию о бонусах.\n" +
                    "Ты можешь прочесть про бонусы, наведя мышкой на иконку бонуса.\n" +
                    "А внизу есть кнопка смены интерфейса, а сверху справа ты можешь выключить/включить музыку/звук.\n" +
                    "И да, я знаю, я кэп.");
                timer1.Enabled = false;
            }
            else
            {
                // Run your procedure here.  
                // Increment counter.  
                counter = counter + 1;
            }
        }

        public void CreateCloseElements()
        {
            LvLCheck();

            if (Information.StaticLvL < 3 || Information.StaticXP < 800)
            {
                closeYourRandom.Location = new Point(384, 322);
                closeYourRandom.Size = new Size(334, 115);
                closeYourRandom.BackColor = Color.Transparent;
                closeYourRandom.BackgroundImage = (Image)Properties.Resources.ResourceManager.GetObject("close_forms_menu");
                Controls.Add(closeYourRandom);
                closeYourRandom.BringToFront();
            }

            if (Information.StaticLvL >= 3 || Information.StaticXP >= 800)
            {
                closeYourRandom.Visible = false;
            }

            if (Information.StaticLvL < 4 || Information.StaticLvL < 2000)
            {
                closeShop.Location = new Point(384, 448);
                closeShop.Size = new Size(334, 115);
                closeShop.BackColor = Color.Transparent;
                closeShop.BackgroundImage = (Image)Properties.Resources.ResourceManager.GetObject("close_forms_menu");
                Controls.Add(closeShop);
                closeShop.BringToFront();
            }

            if (Information.StaticLvL >= 4 || Information.StaticXP >= 2000)
            {
                closeShop.Visible = false;
            }

            if (Information.StaticLvL < 6 || Information.StaticLvL < 7000)
            {
                closeLuckyRandom.Location = new Point(384, 196);
                closeLuckyRandom.Size = new Size(334, 115);
                closeLuckyRandom.BackColor = Color.Transparent;
                closeLuckyRandom.BackgroundImage = (Image)Properties.Resources.ResourceManager.GetObject("close_forms_menu");
                Controls.Add(closeLuckyRandom);
                closeLuckyRandom.BringToFront();
            }

            if (Information.StaticLvL >= 6 || Information.StaticXP >= 7000)
            {
                closeLuckyRandom.Visible = false;
            }

            if (!Information.ItemEight_OpenApp)
            {
                closeLuckyTree.Location = new Point(384, 574);
                closeLuckyTree.Size = new Size(334, 115);
                closeLuckyTree.BackColor = Color.Transparent;
                closeLuckyTree.BackgroundImage = (Image)Properties.Resources.ResourceManager.GetObject("close_forms_menu");
                Controls.Add(closeLuckyTree);
                closeLuckyTree.BringToFront();

                buttonGameFour.Enabled = false;
            }
            if (Information.ItemEight_OpenApp)
            {
                closeLuckyTree.Visible = false;
                buttonGameFour.Enabled = true;
            }
        }

        void Mus_Sound()
        {
            if (!Information.Music)
            {
                Player.close();

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

        public static void Encode()
        {
            File.WriteAllText(path_data, string.Empty);

            using (StreamWriter sw = File.AppendText(path_data))
            {
                sw.WriteLine(EncoderAndDecoder.Encoder(Information.Name));
                sw.WriteLine(EncoderAndDecoder.Encoder(Information.Finance.ToString()));
                sw.WriteLine(EncoderAndDecoder.Encoder(Information.Lvl.ToString()));
                sw.WriteLine(EncoderAndDecoder.Encoder(Information.Xp.ToString()));
                sw.WriteLine(EncoderAndDecoder.Encoder(Information.ItemOne_2x_XP.ToString()));
                sw.WriteLine(EncoderAndDecoder.Encoder(Information.ItemTwo_WW_Game.ToString()));
                sw.WriteLine(EncoderAndDecoder.Encoder(Information.ItemThree_Additional_Perc.ToString()));
                sw.WriteLine(EncoderAndDecoder.Encoder(Information.ItemFour_XP_Cash.ToString()));
                sw.WriteLine(EncoderAndDecoder.Encoder(Information.ItemFive_Pass_Accumulation.ToString()));
                sw.WriteLine(EncoderAndDecoder.Encoder(Information.ItemSix_x2_TopUpAcc.ToString()));
                sw.WriteLine(EncoderAndDecoder.Encoder(Information.ItemEight_OpenApp.ToString()));
                sw.WriteLine(EncoderAndDecoder.Encoder(Information.Wallpaper.ToString()));
                sw.WriteLine(EncoderAndDecoder.Encoder(Information.StaticLvL.ToString()));
                sw.WriteLine(EncoderAndDecoder.Encoder(Information.StaticXP.ToString()));
                sw.WriteLine(EncoderAndDecoder.Encoder(Information.Current_Track.ToString()));
                sw.WriteLine(EncoderAndDecoder.Encoder(Information.Music.ToString()));
                sw.WriteLine(EncoderAndDecoder.Encoder(Information.Sound.ToString()));
                sw.WriteLine(EncoderAndDecoder.Encoder(Information.Animation.ToString()));
                sw.WriteLine(EncoderAndDecoder.Encoder(Information.Win.ToString()));
            }

            File.WriteAllText(path_statistics_data, string.Empty);

            using (StreamWriter sw = File.AppendText(path_statistics_data))
            {
                sw.WriteLine(EncoderAndDecoder.Encoder(Personal_data.Wins.ToString()));
                sw.WriteLine(EncoderAndDecoder.Encoder(Personal_data.Losses.ToString()));
                sw.WriteLine(EncoderAndDecoder.Encoder(Personal_data.Items_bought.ToString()));
                sw.WriteLine(EncoderAndDecoder.Encoder(Personal_data.Opened_extra_app.ToString()));
                sw.WriteLine(EncoderAndDecoder.Encoder(Personal_data.Achieve.ToString()));
            }

            File.WriteAllText(path_statistics_times, string.Empty);

            using (StreamWriter sw = File.AppendText(path_statistics_times))
            {
                sw.WriteLine(EncoderAndDecoder.Encoder(Personal_data.All_time.ToString()));
                sw.WriteLine(EncoderAndDecoder.Encoder(Personal_data.SlotMachine_time.ToString()));
                sw.WriteLine(EncoderAndDecoder.Encoder(Personal_data.YourRandom_time.ToString()));
                sw.WriteLine(EncoderAndDecoder.Encoder(Personal_data.LuckyRandom_time.ToString()));
                sw.WriteLine(EncoderAndDecoder.Encoder(Personal_data.LuckyTree_time.ToString()));
            }
        }

        public static void Decode()
        {
            string[] lines = File.ReadAllLines(path_data);

            Information.Name = EncoderAndDecoder.Decoder(lines[0]);
            Information.Finance = long.Parse(EncoderAndDecoder.Decoder(lines[1]));
            Information.Lvl = int.Parse(EncoderAndDecoder.Decoder(lines[2]));
            Information.Xp = int.Parse(EncoderAndDecoder.Decoder(lines[3]));
            Information.ItemOne_2x_XP = int.Parse(EncoderAndDecoder.Decoder(lines[4]));
            Information.ItemTwo_WW_Game = int.Parse(EncoderAndDecoder.Decoder(lines[5]));
            Information.ItemThree_Additional_Perc = bool.Parse(EncoderAndDecoder.Decoder(lines[6]));
            Information.ItemFour_XP_Cash = int.Parse(EncoderAndDecoder.Decoder(lines[7]));
            Information.ItemFive_Pass_Accumulation = bool.Parse(EncoderAndDecoder.Decoder(lines[8]));
            Information.ItemSix_x2_TopUpAcc = bool.Parse(EncoderAndDecoder.Decoder(lines[9]));
            Information.ItemEight_OpenApp = bool.Parse(EncoderAndDecoder.Decoder(lines[10]));
            Information.Wallpaper = int.Parse(EncoderAndDecoder.Decoder(lines[11]));
            Information.StaticLvL = int.Parse(EncoderAndDecoder.Decoder(lines[12]));
            Information.StaticXP = int.Parse(EncoderAndDecoder.Decoder(lines[13]));
            Information.Current_Track = int.Parse(EncoderAndDecoder.Decoder(lines[14]));
            Information.Music = bool.Parse(EncoderAndDecoder.Decoder(lines[15]));
            Information.Sound = bool.Parse(EncoderAndDecoder.Decoder(lines[16]));
            Information.Animation = bool.Parse(EncoderAndDecoder.Decoder(lines[17]));
            Information.Win = bool.Parse(EncoderAndDecoder.Decoder(lines[18]));

            string[] linesOfStatistics = File.ReadAllLines(path_statistics_data);

            Personal_data.Wins = int.Parse(EncoderAndDecoder.Decoder(linesOfStatistics[0]));
            Personal_data.Losses = int.Parse(EncoderAndDecoder.Decoder(linesOfStatistics[1]));
            Personal_data.Items_bought = int.Parse(EncoderAndDecoder.Decoder(linesOfStatistics[2]));
            Personal_data.Opened_extra_app = bool.Parse(EncoderAndDecoder.Decoder(linesOfStatistics[3]));
            Personal_data.Achieve = bool.Parse(EncoderAndDecoder.Decoder(linesOfStatistics[4]));

            string[] linesOfTimes = File.ReadAllLines(path_statistics_times);

            Personal_data.All_time = int.Parse(EncoderAndDecoder.Decoder(linesOfTimes[0]));
            Personal_data.SlotMachine_time = int.Parse(EncoderAndDecoder.Decoder(linesOfTimes[1]));
            Personal_data.YourRandom_time = int.Parse(EncoderAndDecoder.Decoder(linesOfTimes[2]));
            Personal_data.LuckyRandom_time = int.Parse(EncoderAndDecoder.Decoder(linesOfTimes[3]));
            Personal_data.LuckyTree_time = int.Parse(EncoderAndDecoder.Decoder(linesOfTimes[4]));
        }

        public static void AllTime()
        {
            Form1.all_time.Interval = 1000;
            Form1.all_time.Enabled = true;
            Form1.all_time.Tick += new System.EventHandler(TimerOfAllTime);
            System.Diagnostics.Debug.WriteLine("Start timer");
        }

        public static void TimerOfAllTime(object sender, System.EventArgs e)
        {
            Personal_data.All_time++;
            Form1.Write();
            Form1.Encode();
        }
    }
}