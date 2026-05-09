namespace wordgame
{
    internal class GuessValidator
    {
        public bool ValidateGuess(string guess, int size)
{
            if (string.IsNullOrWhiteSpace(guess))
            {
                throw new InvalidGuessException("Input cannot be empty.");
            }

            if (guess.Length != size)
            {
                throw new InvalidGuessException($"Word must contain exactly {size} letters.");
            }

            if (!guess.All(c => char.IsLetter(c) && c <= 127))
            {
                throw new InvalidGuessException("Only English letters A-Z are allowed.");
            }

            return true;
        }
    }
}