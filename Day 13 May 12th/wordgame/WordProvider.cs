using System;
using System.Collections.Generic;

namespace wordgame
{
    internal class WordProvider
    {
        WordRepository wordRepository;
        public WordProvider()
        {
            wordRepository = new WordRepository();

        }
        public void addWord(string word, string level)
        {
            wordRepository.addWordIntoDatabase(word, level);
        }
        public string getWord(string level)
        {
                Console.WriteLine("Fetching a word for you...");
                return wordRepository.getWordFromDatabase(level);
        }
    }
}