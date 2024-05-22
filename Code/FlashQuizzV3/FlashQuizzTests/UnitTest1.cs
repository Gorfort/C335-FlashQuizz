using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace FlashQuizzTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            // Arrange
            var database = new CardDatabase("database_path");

            // Act
            var cards = await database.GetCardsAsync();

            // Assert
            Assert.IsNotNull(cards);
            Assert.IsTrue(cards.Any()); // V�rifie si la liste n'est pas vide
        }

        [TestMethod]
        public async Task TestMethod2()
        {
            // Arrange
            var database = new CardDatabase("database_path");
            var existingCard = new Card { Id = 1, Question = "Old Question", Answer = "Old Answer" };
            await database.AddCardAsync(existingCard); // Ajoute la carte existante dans la base de donn�es

            // Act
            existingCard.Question = "New Question";
            existingCard.Answer = "New Answer";
            await database.UpdateCardAsync(existingCard);

            // Assert
            var updatedCard = await database.GetCardByIdAsync(existingCard.Id);
            Assert.AreEqual(existingCard.Question, updatedCard.Question);
            Assert.AreEqual(existingCard.Answer, updatedCard.Answer);
        }

        [TestMethod]
        public async Task TestMethod3()
        {
            // Arrange
            var database = new CardDatabase("database_path");
            var cardToDelete = new Card { Id = 1, Question = "Question to delete", Answer = "Answer to delete" };
            await database.AddCardAsync(cardToDelete); // Ajoute la carte � supprimer dans la base de donn�es

            // Act
            await database.DeleteCardAsync(cardToDelete);

            // Assert
            var deletedCard = await database.GetCardByIdAsync(cardToDelete.Id);
            Assert.IsNull(deletedCard); // V�rifie si la carte a bien �t� supprim�e
        }
    }
}
