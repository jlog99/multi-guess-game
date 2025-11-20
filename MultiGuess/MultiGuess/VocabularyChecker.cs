namespace MultiGuess
{
    public class VocabularyChecker : IVocabularyChecker
    {
        private List<string>? stringList;

        public VocabularyChecker()
        {
            ReadFile();
        }

        private void ReadFile()
        {
            try
            {
                using var stream = new StreamReader(new FileStream("wordlist.txt", FileMode.OpenOrCreate));
                var content = stream.ReadToEndAsync();
                stringList = content.Result.Split('\n').ToList();
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public bool Exists(string word)
        {
            return stringList?.Contains(word) == true;
        }
    }
}
