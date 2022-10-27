namespace BookStore.Models
{
    public class BookModel:BaseModel
    {
        public int SrNo { get; set; }
        public int id { get; set; }
        public string? book_name { get; set; } 
        public string? Descriptions { get; set; }

        public string ? publication_date { get; set; }
    }
}
