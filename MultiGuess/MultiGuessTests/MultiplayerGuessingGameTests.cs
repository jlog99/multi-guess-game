using NUnit.Framework;
using MultiGuess;

namespace MultiGuessTests
{
    [TestFixture]
    public class MultiplayerGuessingGameTests
    {
        private MultiplayerGuessingGame game;

        [SetUp]
        public void Setup()
        {
            var vocabularyChecker = new VocabularyChecker();

            var words = new List<string> { "poker", "cover", "pesto" };
            game = new MultiplayerGuessingGame(vocabularyChecker, words);
        }

        [Test]
        public void TestGetGameStrings()
        {
            var gameStrings = game.GetGameStrings();
            Assert.AreEqual(3, gameStrings.Count);
            foreach (var word in gameStrings)
            {
                var hiddenCount = word.Where(x => x == '*').Count();
                Assert.AreEqual(4, hiddenCount);
                Assert.AreEqual(5, word.Length);
                Assert.IsTrue(word.Contains('*'));
            }
        }

        [Test]
        public void TestSubmitGuess_ExactMatch()
        {
            int score = game.SubmitGuess("player1", "poker");
            Assert.AreEqual(10, score);
        }

        [Test]
        public void TestSubmitGuess_ValidGuess()
        {
            int score = game.SubmitGuess("player1", "power");
            Assert.Greater(score, 0);
        }

        [Test]
        public void TestSubmitGuess_InvalidGuess()
        {
            int score = game.SubmitGuess("player1", "xxxxx");
            Assert.AreEqual(0, score);
        }

        [Test]
        public void TestSubmitGuesses_ReturnsStringsInOriginalOrder()
        {
            var submission1 = "poker";
            var submission2 = "cover";
            var submission3 = "pesto";
            int score1 = game.SubmitGuess("player1", submission1);
            int score2 = game.SubmitGuess("player2", submission2);
            int score3 = game.SubmitGuess("player3", submission3);
            Assert.AreEqual(score1, 10);
            Assert.AreEqual(score2, 10);
            Assert.AreEqual(score3, 10);

            var gameStrings = game.GetGameStrings().ToArray();
            Assert.AreEqual(submission1, gameStrings[0]);
            Assert.AreEqual(submission2, gameStrings[1]);
            Assert.AreEqual(submission3, gameStrings[2]);
        }
    }
}