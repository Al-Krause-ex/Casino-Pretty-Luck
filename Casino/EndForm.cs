using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Casino
{
    public partial class EndForm : Form
    {
        Timer time = new Timer();

        public EndForm()
        {
            InitializeComponent();

            buttonBack.Click += (obj, e) =>
            {
                Form1.SoundClickButton();
                time.Stop();
                //Form1.TimerOnOff();
            };
            buttonBack.DialogResult = DialogResult.OK;

            Main();
            Timer();
        }

        public void Main()
        {
            SetLabels();
            //Form1.TimerOnOff();

            buttonIncreaseFinance.Click += (obj, e) =>
            {
                if (Information.Finance < 1000000)
                {
                    Information.Finance += 1000;
                    MessageBox.Show($"Ты увеличил свой счёт. Твой бюджет: {Information.Finance}");
                }
                else
                    MessageBox.Show($"Увы, но у тебя уже {Information.Finance}$.");

                Form1.Write();
                Form1.Encode();

                SetLabels();
            };

            buttonIncreaseXp.Click += (obj, e) =>
            {
                if (Information.Xp < 50000)
                {
                    Information.Xp += 100;
                    MessageBox.Show($"Ты увеличил свой опыт. Твой опыт: {Information.Xp}");
                }
                else
                    MessageBox.Show($"Увы, но у тебя уже {Information.Xp} xp.");

                Form1.LvLCheck();
                Form1.Write();
                Form1.Encode();

                SetLabels();
            };

            buttonResetFinance.Click += (obj, e) =>
            {
                Information.Finance = 0;
                MessageBox.Show("Ты обнулил свой счёт!");

                Form1.Write();
                Form1.Encode();

                SetLabels();
            };

            buttonResetXpLvL.Click += (obj, e) =>
            {
                Information.Xp = 0;
                Information.Lvl = 0;
                MessageBox.Show("Ты обнулил свой уровень и опыт!");

                Form1.Write();
                Form1.Encode();

                SetLabels();
            };
        }

        void Timer()
        {
            time.Enabled = true;
            time.Interval = 1000;
            time.Tick += timer_Tick;
        }

        void timer_Tick(object sender, System.EventArgs e)
        {
            int h, m, s;
            SetLabels();
            if (Personal_data.All_time >= 3600)
            {
                h = Personal_data.All_time / 3600;
                m = Personal_data.All_time / 60 % 60;
                s = Personal_data.All_time % 60;
                labelAllTime.Text = $"Общее: {h} ч {m} мин {s} сек";
            }
            else
            {
                h = 0;
                m = Personal_data.All_time / 60;
                s = Personal_data.All_time % 60;
                labelAllTime.Text = $"Общее: {h} ч {m} мин {s} сек";
            }
            //labelAllTime.Text = $"Общее: {Personal_data.All_time / 60 / 60} ч {(Personal_data.All_time / 60 >= 60 ? (Personal_data.All_time - Convert.ToInt32(Math.Pow(60, Personal_data.All_time / 60/60)) ) / 60 : Personal_data.All_time / 60)} мин {Personal_data.All_time % 60} сек";
        }

        void SetLabels()
        {
            labelWins.Text = $"Побед: {Personal_data.Wins}";
            labelLosses.Text = $"Проигрышей: {Personal_data.Losses}";
            labelItemsBought.Text = $"Куплено предметов: {Personal_data.Items_bought}";

            Timers();

            if (Personal_data.Opened_extra_app)
                labelOpenedExtraForm.Text = "Открыта дополнительная форма: Да";
            else
                labelOpenedExtraForm.Text = "Открыта дополнительная форма: Нет";

            if (Personal_data.Achieve)
                labelAchieve.Text = "Достижение 'Великий игрок': Да";
            else
                labelAchieve.Text = "Достижение 'Великий игрок': Нет";
        }

        void Timers()
        {
            int h, m, s;

            if (Personal_data.All_time >= 3600)
            {
                h = Personal_data.All_time / 3600;
                m = Personal_data.All_time / 60 % 60;
                s = Personal_data.All_time % 60;
                labelAllTime.Text = $"Общее: {h} ч {m} мин {s} сек";
            }
            else
            {
                h = 0;
                m = Personal_data.All_time / 60;
                s = Personal_data.All_time % 60;
                labelAllTime.Text = $"Общее: {h} ч {m} мин {s} сек";
            }

            if (Personal_data.SlotMachine_time >= 3600)
            {
                h = Personal_data.SlotMachine_time / 3600;
                m = Personal_data.SlotMachine_time / 60 % 60;
                s = Personal_data.SlotMachine_time % 60;
                labelSlotMachineTime.Text = $"Slot Machine: {h} ч {m} мин {s} сек";
            }
            else
            {
                h = 0;
                m = Personal_data.SlotMachine_time / 60;
                s = Personal_data.SlotMachine_time % 60;
                labelSlotMachineTime.Text = $"Slot Machine: {h} ч {m} мин {s} сек";
            }

            if (Personal_data.YourRandom_time >= 3600)
            {
                h = Personal_data.YourRandom_time / 3600;
                m = Personal_data.YourRandom_time / 60 % 60;
                s = Personal_data.YourRandom_time % 60;
                labelYourRandomTime.Text = $"Your Random: {h} ч {m} мин {s} сек";
            }
            else
            {
                h = 0;
                m = Personal_data.YourRandom_time / 60;
                s = Personal_data.YourRandom_time % 60;
                labelYourRandomTime.Text = $"Your Random: {h} ч {m} мин {s} сек";
            }

            if (Personal_data.LuckyRandom_time >= 3600)
            {
                h = Personal_data.LuckyRandom_time / 3600;
                m = Personal_data.LuckyRandom_time / 60 % 60;
                s = Personal_data.LuckyRandom_time % 60;
                labelLuckyRandom.Text = $"Lucky Random: {h} ч {m} мин {s} сек";
            }
            else
            {
                h = 0;
                m = Personal_data.LuckyRandom_time / 60;
                s = Personal_data.LuckyRandom_time % 60;
                labelLuckyRandom.Text = $"Lucky Random: {h} ч {m} мин {s} сек";
            }

            if (Personal_data.LuckyTree_time >= 3600)
            {
                h = Personal_data.LuckyTree_time / 3600;
                m = Personal_data.LuckyTree_time / 60 % 60;
                s = Personal_data.LuckyTree_time % 60;
                labelLuckyTree.Text = $"Lucky Tree: {h} ч {m} мин {s} сек";
            }
            else
            {
                h = 0;
                m = Personal_data.LuckyTree_time / 60;
                s = Personal_data.LuckyTree_time % 60;
                labelLuckyTree.Text = $"Lucky Tree: {h} ч {m} мин {s} сек";
            }
        }
    }
}