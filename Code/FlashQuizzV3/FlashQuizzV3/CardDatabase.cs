using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;
using FlashQuizzV3.Models;

namespace FlashQuizzV3.Data
{
    public class CardDatabase
    {
        private readonly SQLiteAsyncConnection _database;

        public CardDatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Card>().Wait();
        }

        public Task<List<Card>> GetCardsAsync()
        {
            return _database.Table<Card>().ToListAsync();
        }

        public Task<int> SaveCardAsync(Card card)
        {
            if (card.Id != 0)
            {
                return _database.UpdateAsync(card);
            }
            else
            {
                return _database.InsertAsync(card);
            }
        }

        public Task<int> DeleteCardAsync(Card card)
        {
            return _database.DeleteAsync(card);
        }

        public Task<int> UpdateCardAsync(Card card)
        {
            return _database.UpdateAsync(card);
        }

        // Method to reset IncorrectCount for all cards
        public async Task ResetIncorrectCountAsync()
        {
            var cards = await _database.Table<Card>().ToListAsync();
            foreach (var card in cards)
            {
                card.IncorrectCount = 0;
                await _database.UpdateAsync(card);
            }
        }
    }
}
