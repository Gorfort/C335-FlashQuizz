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
            return _database.InsertAsync(card);
        }
        public Task<int> DeleteCardAsync(Card card)
        {
            return _database.DeleteAsync(card);
        }

        public Task<int> UpdateCardAsync(Card card)
        {
            return _database.UpdateAsync(card);
        }
    }
}
