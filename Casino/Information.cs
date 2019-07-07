using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Casino
{
    class Information
    {
        private static long finance;
        private static long earnedMoney;
        private static string name;
        private static int lvl;
        private static long xp;
        private static int staticLvL;
        private static long staticXp;
        private static int countWallpaper;
        private static int current_Track;
        private static bool music;
        private static bool sound;
        private static bool animation;
        private static bool win;
        private static string training;

        private static int itemOne_x2_XP;
        private static int itemTwo_WWGame;
        private static bool itemThree_AddPercent;
        private static int itemFour_XP_Cash;
        private static bool itemFive_Pass_Accumulation;
        private static bool itemSix_x2_TopUpAcc;
        private static bool itemEight_OpenApp;

        public static long Finance
        {
            get { return finance; }
            set { finance = value; }
        }

        public static long EarnedMoney
        {
            get { return earnedMoney; }
            set { earnedMoney = value; }
        }

        public static string Name
        {
            get { return name; }
            set { name = value; }
        }

        public static int Lvl
        {
            get { return lvl; }
            set { lvl = value; }
        }

        public static long Xp
        {
            get { return xp; }
            set { xp = value; }
        }

        public static int StaticLvL
        {
            get { return staticLvL; }
            set { staticLvL = value; }
        }

        public static long StaticXP
        {
            get { return staticXp; }
            set { staticXp = value; }
        }


        public static int Wallpaper
        {
            get { return countWallpaper; }
            set { countWallpaper = value; }
        }

        public static string Training
        {
            get { return training; }
            set { training = value; }
        }

        public static int Current_Track
        {
            get { return current_Track; }
            set { current_Track = value; }
        }

        public static bool Music
        {
            get { return music; }
            set { music = value; }
        }

        public static bool Sound
        {
            get { return sound; }
            set { sound = value; }
        }

        public static bool Animation
        {
            get { return animation; }
            set { animation = value; }
        }

        public static bool Win
        {
            get { return win; }
            set { win = value; }
        }


        public static int ItemOne_2x_XP
        {
            get { return itemOne_x2_XP; }
            set { itemOne_x2_XP = value; }
        }

        public static int ItemTwo_WW_Game
        {
            get { return itemTwo_WWGame; }
            set { itemTwo_WWGame = value; }
        }

        public static bool ItemThree_Additional_Perc
        {
            get { return itemThree_AddPercent; }
            set { itemThree_AddPercent = value; }
        }

        public static int ItemFour_XP_Cash
        {
            get { return itemFour_XP_Cash; }
            set { itemFour_XP_Cash = value; }
        }

        public static bool ItemFive_Pass_Accumulation
        {
            get { return itemFive_Pass_Accumulation; }
            set { itemFive_Pass_Accumulation = value; }
        }

        public static bool ItemSix_x2_TopUpAcc
        {
            get { return itemSix_x2_TopUpAcc; }
            set { itemSix_x2_TopUpAcc = value; }
        }

        public static bool ItemEight_OpenApp
        {
            get { return itemEight_OpenApp; }
            set { itemEight_OpenApp = value; }
        }
    }
}
