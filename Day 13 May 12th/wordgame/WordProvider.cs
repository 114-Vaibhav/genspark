using System;
using System.Collections.Generic;

namespace wordgame
{
    internal class WordProvider
    {
        private List<string> EasyWords;
        private List<string> MediumWords;
        private List<string> HardWords;
        private Random random; 
        public WordProvider()
        {
            random = new Random();
            EasyWords = new List<string>
            {
                "CAT",
                "DOG",
                "SUN",
                "BOX",
                "PEN",
                "MAP",
                "CAR",
                "BAT",
                "CUP",
                "HAT"
            };

            MediumWords = new List<string>
            {
                "BOOK",
                "TREE",
                "FISH",
                "MOON",
                "STAR",
                "LION",
                "CAKE",
                "BALL",
                "WIND",
                "SHIP"
            };

            HardWords = new List<string>
            {
                "APPLE",
                "MANGO",
                "GRAPE",
                "TRAIN",
                "PLANT",
                "BRAIN",
                "HOUSE",
                "SMILE",
                "WATER",
                "LIGHT"
            };
        }
        public string getWord(string level)
        {
            if (level == "easy")
            {
                int index = random.Next(EasyWords.Count); 
                return EasyWords[index];
            }else
            if (level == "medium")
            {
                int index = random.Next(MediumWords.Count); 
                return MediumWords[index];
            }
            else
            {
                int index = random.Next(HardWords.Count); 
                return HardWords[index];     
            }
        }
    }
}