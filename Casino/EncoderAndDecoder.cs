using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Casino
{
    class EncoderAndDecoder
    {
        public static string key = "987";
        public static char[] fullAlp = new char[] {'а', 'б', 'в', 'г', 'д', 'е', 'ё', 'ж', 'з', 'и', 'й', 'к',
        'л', 'м', 'н', 'о', 'п', 'р','с', 'т', 'у', 'ф', 'х', 'ц','ч', 'ш', 'щ', 'ъ', 'ы', 'ь', 'э', 'ю', 'я',
        'А', 'Б', 'В', 'Г', 'Д', 'Е', 'Ё', 'Ж', 'З', 'И', 'Й', 'К', 'Л', 'М', 'Н', 'О', 'П', 'Р','С', 'Т',
        'У', 'Ф', 'Х', 'Ц','Ч', 'Ш', 'Щ', 'Ъ', 'Ы', 'Ь', 'Э', 'Ю', 'Я',

        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u',
        'v', 'w', 'x', 'y', 'z',
        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U',
        'V', 'W', 'X', 'Y', 'Z',

        '1', '2', '3', '4', '5', '6', '7', '8', '9', '0'};

        //static void Main(string[] args)
        //{
        //    string secret = "Шифратор by Kr :3";

        //    var encode = Encoder(secret);
        //    Console.WriteLine("Шифр: " + encode);

        //    var decode = Decoder(encode);
        //    Console.WriteLine("Декод: " + decode);

        //    Console.ReadKey();
        //}

        public static string Encoder(string arg)
        {
            int indexToKey = 0;
            for (int j = 0; j < arg.Length; j++)
            {
                //System.Diagnostics.Debug.WriteLine($"iteration: {j}");
                for (int i = 0; i < fullAlp.Length; i++)
                {
                    if (fullAlp[i] == arg[j])
                    {
                        char originalChar = arg[j];
                        arg = arg.Remove(j, 1);

                        int stableI = i;
                        int newKey = (int)Char.GetNumericValue(key[indexToKey]);

                        if (i + (int)Char.GetNumericValue(key[indexToKey]) >= 128)
                        {
                            newKey = (128 - (i + (int)Char.GetNumericValue(key[indexToKey]))) * (-1);
                            stableI = 0;
                        }

                        //System.Diagnostics.Debug.WriteLine("NEWKEY IN ENCODER: " + newKey);
                        //System.Diagnostics.Debug.WriteLine("STABLEI IN ENCODER: " + stableI);

                        string newChar = Convert.ToString(fullAlp[stableI + newKey]);

                        //System.Diagnostics.Debug.WriteLine($"Start: {originalChar}. End: {newChar}");

                        arg = arg.Insert(j, newChar);

                        indexToKey++;

                        if (indexToKey >= 3)
                            indexToKey = 0;

                        break;
                    }
                }
            }
            return arg;
        }

        public static string Decoder(string arg)
        {
            int indexToKey = 0;
            for (int j = 0; j < arg.Length; j++)
            {
                //System.Diagnostics.Debug.WriteLine($"iteration: {j}");
                for (int i = 0; i < fullAlp.Length; i++)
                {
                    if (fullAlp[i] == arg[j])
                    {
                        char originalChar = arg[j];
                        arg = arg.Remove(j, 1);

                        int stableI = i;
                        int newKey = (int)Char.GetNumericValue(key[indexToKey]);

                        if (i - (int)Char.GetNumericValue(key[indexToKey]) <= -1)
                        {
                            newKey = 0;
                            newKey = (i - (int)Char.GetNumericValue(key[indexToKey])) * (-1);
                            stableI = 128;
                        }

                        string newChar = Convert.ToString(fullAlp[stableI - newKey]);

                        //System.Diagnostics.Debug.WriteLine($"Start: {originalChar}. End: {newChar}");

                        arg = arg.Insert(j, newChar);

                        indexToKey++;

                        if (indexToKey >= 3)
                            indexToKey = 0;

                        break;
                    }
                }
            }
            return arg;
        }
    }
}
