namespace wordgame
{
    internal class FeedbackGenerator
    {
        public string generateFeedback(string guess,string answer,int size)
        {   
            
            
            answer = answer.ToUpper();
            guess = guess.ToUpper();
            char[] chars = new char[size];
            Array.Fill(chars, 'X');
    
            for(int i = 0; i < size; i++)
            {
                if(guess[i]==answer[i]) chars[i]='G';
                else if(answer.Contains(guess[i])) chars[i]='Y';
            }
            return new string(chars);
        }
    }
}