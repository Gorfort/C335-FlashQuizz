using SQLite;

namespace FlashQuizzV3.Models
{
    public class Card
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public int KnowledgeLevel { get; set; }
    }
}
