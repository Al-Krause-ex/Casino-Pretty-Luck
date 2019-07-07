using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Casino
{
    class Personal_data
    {
        private static int wins;
        private static int losses;
        private static int items_bought;
        private static bool opened_extra_app;
        private static bool achieve;

        private static int all_time;
        private static int slotMachine_time;
        private static int yourRandom_time;
        private static int luckyRandom_time;
        private static int luckyTree_time;

        public static int Wins
        {
            get { return wins; }
            set { wins = value; }
        }

        public static int Losses
        {
            get { return losses; }
            set { losses = value; }
        }

        public static int Items_bought
        {
            get { return items_bought; }
            set { items_bought = value; }
        }

        public static bool Opened_extra_app
        {
            get { return opened_extra_app; }
            set { opened_extra_app = value; }
        }

        public static bool Achieve
        {
            get { return achieve; }
            set { achieve = value; }
        }


        public static int All_time
        {
            get { return all_time; }
            set { all_time = value; }
        }

        public static int SlotMachine_time
        {
            get { return slotMachine_time; }
            set { slotMachine_time = value; }
        }

        public static int YourRandom_time
        {
            get { return yourRandom_time; }
            set { yourRandom_time = value; }
        }

        public static int LuckyRandom_time
        {
            get { return luckyRandom_time; }
            set { luckyRandom_time = value; }
        }

        public static int LuckyTree_time
        {
            get { return luckyTree_time; }
            set { luckyTree_time = value; }
        }
    }
}