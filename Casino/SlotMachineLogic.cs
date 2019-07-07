using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace Casino
{
    class SlotMachineLogic
    {
        Random rnd = new Random();

        static int bets = 10;
        static int lines = 1;
        static int attempts = 3;
        static long countCash, countXP;

        static bool bonusCheck = false;
        static bool isBlinking = false;
        static bool isWonFirstBlock = false;
        static bool isWonSecondBlock = false;
        static bool isWonThirdBlock = false;

        public int num1, num2, num3, num4, num5, num6, num7, num8, num9;
        public static long moneyRandomBox;

        public SlotMachineLogic(
            int num1, int num2, int num3,
            int num4, int num5, int num6,
            int num7, int num8, int num9)
        {
            this.num1 = num1;
            this.num2 = num2;
            this.num3 = num3;
            this.num4 = num4;
            this.num5 = num5;
            this.num6 = num6;
            this.num7 = num7;
            this.num8 = num8;
            this.num9 = num9;
        }

        public static int Bets
        {
            get { return bets; }
            set { bets = value; }
        }

        public static int Lines
        {
            get { return lines; }
            set { lines = value; }
        }

        public static int Attempts
        {
            get { return attempts; }
            set { attempts = value; }
        }

        public static long CountCash
        {
            get { return countCash; }
            set { countCash = value; }
        }

        public static long CountXP
        {
            get { return countXP; }
            set { countXP = value; }
        }

        public static bool Bonus
        {
            get { return bonusCheck; }
            set { bonusCheck = value; }
        }

        public static bool Blinking
        {
            get { return isBlinking; }
            set { isBlinking = value; }
        }

        public static bool FirstBlock
        {
            get { return isWonFirstBlock; }
            set { isWonFirstBlock = value; }
        }

        public static bool SecondBlock
        {
            get { return isWonSecondBlock; }
            set { isWonSecondBlock = value; }
        }

        public static bool ThirdBlock
        {
            get { return isWonThirdBlock; }
            set { isWonThirdBlock = value; }
        }

        public void VirtualNumberCheck()
        {
            if (num1 >= 0 && num1 <= 5)
                num1 = 0;
            else if (num1 >= 6 && num1 <= 11)
                num1 = 1;
            else if (num1 >= 12 && num1 <= 17)
                num1 = 2;
            else if (num1 >= 18 && num1 <= 23)
                num1 = 3;
            else if (num1 >= 24 && num1 <= 29)
                num1 = 4;
            else
                num1 = 5;

            if (num2 >= 0 && num2 <= 5)
                num2 = 0;
            else if (num2 >= 6 && num2 <= 11)
                num2 = 1;
            else if (num2 >= 12 && num2 <= 17)
                num2 = 2;
            else if (num2 >= 18 && num2 <= 23)
                num2 = 3;
            else if (num2 >= 24 && num2 <= 29)
                num2 = 4;
            else
                num2 = 5;

            if (num3 >= 0 && num3 <= 5)
                num3 = 0;
            else if (num3 >= 6 && num3 <= 11)
                num3 = 1;
            else if (num3 >= 12 && num3 <= 17)
                num3 = 2;
            else if (num3 >= 18 && num3 <= 23)
                num3 = 3;
            else if (num3 >= 24 && num3 <= 29)
                num3 = 4;
            else
                num3 = 5;


            if (num4 >= 0 && num4 <= 5)
                num4 = 0;
            else if (num4 >= 6 && num4 <= 11)
                num4 = 1;
            else if (num4 >= 12 && num4 <= 17)
                num4 = 2;
            else if (num4 >= 18 && num4 <= 23)
                num4 = 3;
            else if (num4 >= 24 && num4 <= 29)
                num4 = 4;
            else
                num4 = 5;

            if (num5 >= 0 && num5 <= 5)
                num5 = 0;
            else if (num5 >= 6 && num5 <= 11)
                num5 = 1;
            else if (num5 >= 12 && num5 <= 17)
                num5 = 2;
            else if (num5 >= 18 && num5 <= 23)
                num5 = 3;
            else if (num5 >= 24 && num5 <= 29)
                num5 = 4;
            else
                num5 = 5;

            if (num6 >= 0 && num6 <= 5)
                num6 = 0;
            else if (num6 >= 6 && num6 <= 11)
                num6 = 1;
            else if (num6 >= 12 && num6 <= 17)
                num6 = 2;
            else if (num6 >= 18 && num6 <= 23)
                num6 = 3;
            else if (num6 >= 24 && num6 <= 29)
                num6 = 4;
            else
                num6 = 5;


            if (num7 >= 0 && num7 <= 5)
                num7 = 0;
            else if (num7 >= 6 && num7 <= 11)
                num7 = 1;
            else if (num7 >= 12 && num7 <= 17)
                num7 = 2;
            else if (num7 >= 18 && num7 <= 23)
                num7 = 3;
            else if (num7 >= 24 && num7 <= 29)
                num7 = 4;
            else
                num7 = 5;

            if (num8 >= 0 && num8 <= 5)
                num8 = 0;
            else if (num8 >= 6 && num8 <= 11)
                num8 = 1;
            else if (num8 >= 12 && num8 <= 17)
                num8 = 2;
            else if (num8 >= 18 && num8 <= 23)
                num8 = 3;
            else if (num8 >= 24 && num8 <= 29)
                num8 = 4;
            else
                num8 = 5;

            if (num9 >= 0 && num9 <= 5)
                num9 = 0;
            else if (num9 >= 6 && num9 <= 11)
                num9 = 1;
            else if (num9 >= 12 && num9 <= 17)
                num9 = 2;
            else if (num9 >= 18 && num9 <= 23)
                num9 = 3;
            else if (num9 >= 24 && num9 <= 29)
                num9 = 4;
            else
                num9 = 5;
        }

        public void WinCheck()
        {
            if (lines == 1)
            {
                if (num4 == num5 && num5 == num6)
                {
                    if (Information.Sound)
                        Form1.GetToCash();

                    isWonSecondBlock = true;

                    Information.EarnedMoney += bets * 4 + bets * lines;
                    countCash += bets * 4 + bets * lines;

                    Personal_data.Wins++;

                    if (Information.ItemThree_Additional_Perc)
                        Information.EarnedMoney += (bets * 4 + bets * lines) * 25 / 100;

                    if (bonusCheck == true)
                    {
                        Information.EarnedMoney += moneyRandomBox * 2;
                        countCash += moneyRandomBox * 2;

                        if (Information.ItemThree_Additional_Perc)
                            Information.EarnedMoney += (moneyRandomBox * 2) * 25 / 100;
                    }

                    if (Information.ItemOne_2x_XP > 0)
                    {
                        //Information.Xp += (5 + (bets * 10 / 100)) * 2;
                        //Information.StaticXP += (5 + (bets * 10 / 100)) * 2;

                        if (Information.Xp < Information.StaticXP)
                        {
                            Information.Xp += (5 + (bets * 10 / 100)) * 2;
                        }
                        else
                        {
                            Information.Xp += (5 + (bets * 10 / 100)) * 2;
                            Information.StaticXP += (5 + (bets * 10 / 100)) * 2;
                        }

                        countXP = (5 + (bets * 10 / 100)) * 2;
                    }

                    else
                    {
                        //Information.Xp += 5 + (bets * 10 / 100);
                        //Information.StaticXP += 5 + (bets * 10 / 100);

                        if (Information.Xp < Information.StaticXP)
                        {
                            Information.Xp += 5 + (bets * 10 / 100);
                        }
                        else
                        {
                            Information.Xp += 5 + (bets * 10 / 100);
                            Information.StaticXP += 5 + (bets * 10 / 100);
                        }

                        countXP = 5 + (bets * 10 / 100);
                    }

                    Form1.Write();
                    Form1.Encode();
                }
                else
                {
                    if (Information.Sound)
                        Form1.LaughMethod();

                    countCash = 0;
                    Personal_data.Losses++;

                    if (bets > 100)
                    {
                        if (Information.ItemOne_2x_XP > 0)
                        {
                            if (Information.Xp < Information.StaticXP)
                            {
                                Information.Xp += (bets * 7 / 100) * 2;
                            }
                            else
                            {
                                Information.Xp += (bets * 7 / 100) * 2;
                                Information.StaticXP += (bets * 7 / 100) * 2;
                            }

                            countXP = (bets * 7 / 100) * 2;
                        }

                        else
                        {
                            if (Information.Xp < Information.StaticXP)
                            {
                                Information.Xp += bets * 7 / 100;
                            }
                            else
                            {
                                Information.Xp += bets * 7 / 100;
                                Information.StaticXP += bets * 7 / 100;
                            }

                            countXP = bets * 7 / 100;
                        }

                        Form1.Write();
                        Form1.Encode();
                    }
                    else
                    {
                        if (Information.ItemOne_2x_XP > 0)
                        {
                            if (Information.Xp < Information.StaticXP)
                            {
                                Information.Xp += bets * 10 / 100 * 2;
                            }
                            else
                            {
                                Information.Xp += bets * 10 / 100 * 2;
                                Information.StaticXP += bets * 10 / 100 * 2;
                            }

                            countXP = bets * 10 / 100 * 2;
                        }

                        else
                        {
                            if (Information.Xp < Information.StaticXP)
                            {
                                Information.Xp += bets * 10 / 100;
                            }
                            else
                            {
                                Information.Xp += bets * 10 / 100;
                                Information.StaticXP += bets * 10 / 100;
                            }

                            countXP = bets * 10 / 100;
                        }

                        Form1.Write();
                        Form1.Encode();
                    }
                }
            }

            if (lines == 2)
            {
                if (num1 == num2 && num2 == num3)
                {
                    if (Information.Sound)
                        Form1.GetToCash();

                    Information.EarnedMoney += bets * 4 + bets * lines;
                    countCash += bets * 4 + bets * lines;

                    Personal_data.Wins++;

                    if (Information.ItemThree_Additional_Perc)
                        Information.EarnedMoney += (bets * 4 + bets * lines) * 25 / 100;

                    isWonFirstBlock = true;

                    if (bonusCheck == true)
                    {
                        Information.EarnedMoney += moneyRandomBox * 2;
                        countCash += moneyRandomBox * 2;

                        if (Information.ItemThree_Additional_Perc)
                            Information.EarnedMoney += (moneyRandomBox * 2) * 25 / 100;
                    }

                    if (Information.ItemOne_2x_XP > 0)
                    {
                        if (Information.Xp < Information.StaticXP)
                        {
                            Information.Xp += (5 + (bets * 10 / 100)) * 2;
                        }
                        else
                        {
                            Information.Xp += (5 + (bets * 10 / 100)) * 2;
                            Information.StaticXP += (5 + (bets * 10 / 100)) * 2;
                        }

                        countXP = (5 + (bets * 10 / 100)) * 2;
                    }

                    else
                    {
                        if (Information.Xp < Information.StaticXP)
                        {
                            Information.Xp += 5 + (bets * 10 / 100);
                        }
                        else
                        {
                            Information.Xp += 5 + (bets * 10 / 100);
                            Information.StaticXP += 5 + (bets * 10 / 100);
                        }

                        countXP = 5 + (bets * 10 / 100);
                    }

                    Form1.Write();
                    Form1.Encode();
                }

                else if (num4 == num5 && num5 == num6)
                {
                    if (Information.Sound)
                        Form1.GetToCash();

                    isWonSecondBlock = true;

                    Information.EarnedMoney += bets * 4 + bets * lines;
                    countCash += bets * 4 + bets * lines;

                    Personal_data.Wins++;

                    if (Information.ItemThree_Additional_Perc)
                        Information.EarnedMoney += (bets * 4 + bets * lines) * 25 / 100;

                    if (bonusCheck == true)
                    {
                        Information.EarnedMoney += moneyRandomBox * 2;
                        countCash += moneyRandomBox * 2;

                        if (Information.ItemThree_Additional_Perc)
                            Information.EarnedMoney += (moneyRandomBox * 2) * 25 / 100;
                    }

                    if (Information.ItemOne_2x_XP > 0)
                    {
                        if (Information.Xp < Information.StaticXP)
                        {
                            Information.Xp += (5 + (bets * 10 / 100)) * 2;
                        }
                        else
                        {
                            Information.Xp += (5 + (bets * 10 / 100)) * 2;
                            Information.StaticXP += (5 + (bets * 10 / 100)) * 2;
                        }

                        countXP = (5 + (bets * 10 / 100)) * 2;
                    }

                    else
                    {
                        if (Information.Xp < Information.StaticXP)
                        {
                            Information.Xp += 5 + (bets * 10 / 100);
                        }
                        else
                        {
                            Information.Xp += 5 + (bets * 10 / 100);
                            Information.StaticXP += 5 + (bets * 10 / 100);
                        }

                        countXP = 5 + (bets * 10 / 100);
                    }

                    Form1.Write();
                    Form1.Encode();
                }
                else
                {
                    if (Information.Sound)
                        Form1.LaughMethod();

                    countCash = 0;

                    Personal_data.Losses++;

                    if (bets > 100)
                    {
                        if (Information.ItemOne_2x_XP > 0)
                        {
                            if (Information.Xp < Information.StaticXP)
                            {
                                Information.Xp += (bets * 7 / 100) * 2;
                            }
                            else
                            {
                                Information.Xp += (bets * 7 / 100) * 2;
                                Information.StaticXP += (bets * 7 / 100) * 2;
                            }

                            countXP = (bets * 7 / 100) * 2;
                        }

                        else
                        {
                            if (Information.Xp < Information.StaticXP)
                            {
                                Information.Xp += bets * 7 / 100;
                            }
                            else
                            {
                                Information.Xp += bets * 7 / 100;
                                Information.StaticXP += bets * 7 / 100;
                            }

                            countXP = bets * 7 / 100;
                        }

                        Form1.Write();
                        Form1.Encode();
                    }
                    else
                    {
                        if (Information.ItemOne_2x_XP > 0)
                        {
                            if (Information.Xp < Information.StaticXP)
                            {
                                Information.Xp += bets * 10 / 100 * 2;
                            }
                            else
                            {
                                Information.Xp += bets * 10 / 100 * 2;
                                Information.StaticXP += bets * 10 / 100 * 2;
                            }

                            countXP = bets * 10 / 100 * 2;
                        }

                        else
                        {
                            if (Information.Xp < Information.StaticXP)
                            {
                                Information.Xp += bets * 10 / 100;
                            }
                            else
                            {
                                Information.Xp += bets * 10 / 100;
                                Information.StaticXP += bets * 10 / 100;
                            }

                            countXP = bets * 10 / 100;
                        }

                        Form1.Write();
                        Form1.Encode();
                    }
                }
            }

            if (lines == 3)
            {
                if (num1 == num2 && num2 == num3)
                {
                    if (Information.Music)
                        Form1.GetToCash();

                    Information.EarnedMoney += bets * 4 + bets * lines;
                    countCash += bets * 4 + bets * lines;

                    Personal_data.Wins++;

                    if (Information.ItemThree_Additional_Perc)
                        Information.EarnedMoney += (bets * 4 + bets * lines) * 25 / 100;

                    isWonFirstBlock = true;

                    if (bonusCheck == true)
                    {
                        Information.EarnedMoney += moneyRandomBox * 2;
                        countCash += moneyRandomBox * 2;

                        if (Information.ItemThree_Additional_Perc)
                            Information.EarnedMoney += (moneyRandomBox * 2) * 25 / 100;
                    }

                    if (Information.ItemOne_2x_XP > 0)
                    {
                        if (Information.Xp < Information.StaticXP)
                        {
                            Information.Xp += (5 + (bets * 10 / 100)) * 2;
                        }
                        else
                        {
                            Information.Xp += (5 + (bets * 10 / 100)) * 2;
                            Information.StaticXP += (5 + (bets * 10 / 100)) * 2;
                        }

                        countXP = (5 + (bets * 10 / 100)) * 2;
                    }

                    else
                    {
                        if (Information.Xp < Information.StaticXP)
                        {
                            Information.Xp += 5 + (bets * 10 / 100);
                        }
                        else
                        {
                            Information.Xp += 5 + (bets * 10 / 100);
                            Information.StaticXP += 5 + (bets * 10 / 100);
                        }

                        countXP = 5 + (bets * 10 / 100);
                    }

                    Form1.Write();
                    Form1.Encode();
                }

                else if (num4 == num5 && num5 == num6)
                {
                    if (Information.Music)
                        Form1.GetToCash();

                    isWonSecondBlock = true;

                    Information.EarnedMoney += bets * 4 + bets * lines;
                    countCash += bets * 4 + bets * lines;

                    Personal_data.Wins++;

                    if (Information.ItemThree_Additional_Perc)
                        Information.EarnedMoney += (bets * 4 + bets * lines) * 25 / 100;

                    if (bonusCheck == true)
                    {
                        Information.EarnedMoney += moneyRandomBox * 2;
                        countCash += moneyRandomBox * 2;

                        if (Information.ItemThree_Additional_Perc)
                            Information.EarnedMoney += (moneyRandomBox * 2) * 25 / 100;
                    }

                    if (Information.ItemOne_2x_XP > 0)
                    {
                        if (Information.Xp < Information.StaticXP)
                        {
                            Information.Xp += (5 + (bets * 10 / 100)) * 2;
                        }
                        else
                        {
                            Information.Xp += (5 + (bets * 10 / 100)) * 2;
                            Information.StaticXP += (5 + (bets * 10 / 100)) * 2;
                        }

                        countXP = (5 + (bets * 10 / 100)) * 2;
                    }

                    else
                    {
                        if (Information.Xp < Information.StaticXP)
                        {
                            Information.Xp += 5 + (bets * 10 / 100);
                        }
                        else
                        {
                            Information.Xp += 5 + (bets * 10 / 100);
                            Information.StaticXP += 5 + (bets * 10 / 100);
                        }

                        countXP = 5 + (bets * 10 / 100);
                    }

                    Form1.Write();
                    Form1.Encode();
                }

                else if (num7 == num8 && num8 == num9)
                {
                    if (Information.Music)
                        Form1.GetToCash();

                    isWonThirdBlock = true;

                    Information.EarnedMoney += bets * 4 + bets * lines;
                    countCash += bets * 4 + bets * lines;

                    Personal_data.Wins++;

                    if (Information.ItemThree_Additional_Perc)
                        Information.EarnedMoney += (bets * 4 + bets * lines) * 25 / 100;

                    if (bonusCheck == true)
                    {
                        Information.EarnedMoney += moneyRandomBox * 2;
                        countCash += moneyRandomBox * 2;

                        if (Information.ItemThree_Additional_Perc)
                            Information.EarnedMoney += (moneyRandomBox * 2) * 25 / 100;
                    }

                    if (Information.ItemOne_2x_XP > 0)
                    {
                        if (Information.Xp < Information.StaticXP)
                        {
                            Information.Xp += (5 + (bets * 10 / 100)) * 2;
                        }
                        else
                        {
                            Information.Xp += (5 + (bets * 10 / 100)) * 2;
                            Information.StaticXP += (5 + (bets * 10 / 100)) * 2;
                        }

                        countXP = (5 + (bets * 10 / 100)) * 2;
                    }

                    else
                    {
                        if (Information.Xp < Information.StaticXP)
                        {
                            Information.Xp += 5 + (bets * 10 / 100);
                        }
                        else
                        {
                            Information.Xp += 5 + (bets * 10 / 100);
                            Information.StaticXP += 5 + (bets * 10 / 100);
                        }

                        countXP = 5 + (bets * 10 / 100);
                    }

                    Form1.Write();
                    Form1.Encode();
                }
                else
                {
                    if (Information.Sound)
                        Form1.LaughMethod();

                    countCash = 0;

                    Personal_data.Losses++;

                    if (bets > 100)
                    {
                        if (Information.ItemOne_2x_XP > 0)
                        {
                            if (Information.Xp < Information.StaticXP)
                            {
                                Information.Xp += bets * 7 / 100 * 2;
                            }
                            else
                            {
                                Information.Xp += bets * 7 / 100 * 2;
                                Information.StaticXP += bets * 7 / 100 * 2;
                            }

                            countXP = bets * 7 / 100 * 2;
                        }

                        else
                        {
                            if (Information.Xp < Information.StaticXP)
                            {
                                Information.Xp += bets * 7 / 100;
                            }
                            else
                            {
                                Information.Xp += bets * 7 / 100;
                                Information.StaticXP += bets * 7 / 100;
                            }

                            countXP = bets * 7 / 100;
                        }

                        Form1.Write();
                        Form1.Encode();
                    }
                    else
                    {
                        if (Information.ItemOne_2x_XP > 0)
                        {
                            if (Information.Xp < Information.StaticXP)
                            {
                                Information.Xp += bets * 10 / 100 * 2;
                            }
                            else
                            {
                                Information.Xp += bets * 10 / 100 * 2;
                                Information.StaticXP += bets * 10 / 100 * 2;
                            }

                            countXP = bets * 10 / 100 * 2;
                        }

                        else
                        {
                            if (Information.Xp < Information.StaticXP)
                            {
                                Information.Xp += bets * 10 / 100;
                            }
                            else
                            {
                                Information.Xp += bets * 10 / 100;
                                Information.StaticXP += bets * 10 / 100;
                            }

                            countXP = bets * 10 / 100;
                        }

                        Form1.Write();
                        Form1.Encode();
                    }
                }
            }
        }
    }
}
