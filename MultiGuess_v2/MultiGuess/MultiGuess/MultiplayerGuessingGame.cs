using System.Collections.Concurrent;

namespace MultiGuess
{
    public class MultiplayerGuessingGame : IMultiplayerGuessingGame
    {
        private readonly IVocabularyChecker vocabularyChecker;
        private readonly List<string> originalWords;
        private readonly ConcurrentDictionary<int, char[]> currentWords;
        private readonly Random? random;

        public MultiplayerGuessingGame(List<string> words)
        {
            vocabularyChecker = new VocabularyChecker();
            originalWords = words.ToList();
            currentWords = new ConcurrentDictionary<int, char[]>();
            random = new Random();

            var index = 0;
            foreach (var word in originalWords)
            {
                var hiddenWord = new string('*', word.Length).ToCharArray();
                int revealIndex = random.Next(word.Length);
                hiddenWord[revealIndex] = word[revealIndex];
                currentWords.TryAdd(index++, hiddenWord);
            }
        }

        public IList<string> GetGameStrings()
        {
            return currentWords.OrderBy(kvp => kvp.Key).Select(kvp => new string(kvp.Value)).ToList();
        }

        public int SubmitGuess(string playerName, string submission)
        {
            if (!vocabularyChecker.Exists(submission) || submission.Length != originalWords[0].Length)
            {
                return 0;
            }

            bool exactMatch = false;
            int score = 0;

            for (int i = 0; i < originalWords.Count; i++)
            {
                if (new string(currentWords[i]) == originalWords[i])
                {
                    continue; // if current word is fully revealed then skip
                }

                if (originalWords[i] == submission)
                {
                    exactMatch = true;
                    score = 10;
                    break; // exact match found break loop
                }

                bool validGuess = true;
                for (int j = 0; j < submission.Length; j++)
                {
                    if (currentWords[i][j] != '*' && currentWords[i][j] != submission[j])
                    {
                        validGuess = false;
                        break; // if any of the current words positions are revealed and equal to submission then break
                    }
                }

                if (validGuess)
                {
                    for (int j = 0; j < submission.Length; j++)
                    {
                        if (originalWords[i][j] == submission[j] && currentWords[i][j] == '*')
                        {
                            currentWords[i][j] = submission[j];
                            score++; // increment score for each correctly guessed character
                        }
                    }
                }
            }

            if (exactMatch)
            {
                for (int i = 0; i < originalWords.Count; i++)
                {
                    if (originalWords[i] == submission)
                    {
                        currentWords[i] = originalWords[i].ToCharArray(); // reveal the current word for other players to see
                    }
                }
            }

            if (score > 0)
            {
                //Update player leaderboard data
                //send notification or event for <playerName>
            }

            return score;
        }
    }
}
