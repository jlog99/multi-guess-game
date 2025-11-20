using System.Collections.Concurrent;

namespace MultiGuess
{
    public class MultiplayerGuessingGame : IMultiplayerGuessingGame
    {
        private readonly IVocabularyChecker _vocabularyChecker;
        private readonly List<string> _originalWords;
        private readonly ConcurrentDictionary<int, char[]> _currentWords;

        public MultiplayerGuessingGame(IVocabularyChecker vocabularyChecker, List<string> words)
        {
            _vocabularyChecker = vocabularyChecker;
            _originalWords = words.ToList();
            _currentWords = new ConcurrentDictionary<int, char[]>();
            var random = new Random();

            var index = 0;
            foreach (var word in _originalWords)
            {
                var hiddenWord = new string('*', word.Length).ToCharArray();
                int revealIndex = random.Next(word.Length);
                hiddenWord[revealIndex] = word[revealIndex];
                _currentWords.TryAdd(index++, hiddenWord);
            }
        }

        public IList<string> GetGameStrings()
        {
            return _currentWords.OrderBy(kvp => kvp.Key).Select(kvp => new string(kvp.Value)).ToList();
        }

        public int SubmitGuess(string playerName, string submission)
        {
            // add lock here 

            if (!_vocabularyChecker.Exists(submission) || submission.Length != _originalWords[0].Length)
            {
                return 0;
            }

            bool exactMatch = false;
            int score = 0;

            for (int i = 0; i < _originalWords.Count; i++)
            {
                if (!_currentWords[i].Contains('*'))
                {
                    continue; // if current word is fully revealed then skip
                }

                if (_originalWords[i] == submission)
                {
                    exactMatch = true;
                    score = 10;
                    break; // exact match found break loop
                }

                bool validGuess = true;
                for (int j = 0; j < submission.Length; j++)
                {
                    if (_currentWords[i][j] != '*' && _currentWords[i][j] != submission[j])
                    {
                        validGuess = false;
                        break; // if any of the current words positions are revealed and equal to submission then break
                    }
                }

                if (validGuess)
                {
                    for (int j = 0; j < submission.Length; j++)
                    {
                        if (_originalWords[i][j] == submission[j] && _currentWords[i][j] == '*')
                        {
                            _currentWords[i][j] = submission[j];
                            score++; // increment score for each correctly guessed character
                        }
                    }
                }
            }

            if (exactMatch)
            {
                for (int i = 0; i < _originalWords.Count; i++)
                {
                    if (_originalWords[i] == submission)
                    {
                        _currentWords[i] = _originalWords[i].ToCharArray(); // reveal the current word for other players to see
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
