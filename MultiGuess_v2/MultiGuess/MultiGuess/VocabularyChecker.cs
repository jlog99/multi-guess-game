namespace MultiGuess
{
    internal class VocabularyChecker : IVocabularyChecker
    {
        List<string>? stringList; // private readonly 

        public VocabularyChecker() // could make this async and rename e.g. ReadFileAsync and even make the file name a param
        {
            StreamReader? reader = null; // move inside try block and change to using statement
            try
            {
                reader = new StreamReader(new FileStream("wordlist.txt", FileMode.OpenOrCreate)); // FileMode should be open

                var content = reader.ReadToEndAsync(); // can await this if using async and get result on the same line

                stringList = content.Result.Split('\n').ToList(); // check first to see if not null 
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message); // I would add some context to this to help identify quicker where the issue has come from
            }
            finally
            {
                reader?.Dispose(); // can remove this if switching to using statement as above
            }
        }

        public bool Exists(string word)
        {
            return stringList?.Contains(word) == true;
        }
    }
}
