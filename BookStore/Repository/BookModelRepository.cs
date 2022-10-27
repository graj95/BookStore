using BookStore.Context;
using BookStore.Models;
using BookStore.Repository.Interface;
using Dapper;

namespace BookStore.Repository
{
    public class BookModelRepository : IBookModelRepositoty
    {
        private readonly DapperContext _context;

        public BookModelRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<int> DeleteBookModel(int id)
        {
            int result = 0;
            var query = @" Update book set 
                            isDeleted = 1,ModifiedDate=getdate(), ModifiedBy = 1 
                            where Id = @Id "; 
                          
            using (var connection = _context.CreateConnection())
            {
                result = await connection.ExecuteAsync(query, new { id = id });
                return result;
            }
        }

        public async  Task<IEnumerable<BookModel>> GetBookModels()
        {
            List<BookModel> bookModels = new List<BookModel>();
            var query = "select ROW_NUMBER() OVER(ORDER BY Id desc) as SrNo, * from book";

            using(var Connection = _context.CreateConnection())
            {
                var regelt= await Connection.QueryAsync<BookModel>(query); 
                bookModels= regelt.ToList();

                return bookModels;
            }
        }

        public async Task<int> SaveBookModel(BookModel bookModel)
        {
            int result = 0;
            var query = @"INSERT INTO Book (book_name,descriptions,publication_date,CreatedBy,CreatedDate,IsDeleted) 
                          VALUES (@book_name,@descriptions,@publication_date,1,@CreatedDate,0);
                          SELECT CAST(SCOPE_IDENTITY() as int)";

            using (var connection = _context.CreateConnection())
            {
                result = await connection.QuerySingleAsync<int>(query, bookModel);
               
                return result;
            }

        }

        public async Task<int> UpdateBookModel(BookModel bookModel)
        {
            int result = 0;
            var query = @"UPDATE Book SET book_name=@book_name, descriptions=@descriptions,
                          publication_date=@publication_date, ModifiedDate=@ModifiedDate, ModifiedBy = 1 WHERE Id=@Id";

            using (var connection = _context.CreateConnection())
            {
                result = await connection.ExecuteAsync(query, bookModel);
               
                return result;
            }
        }
    }
    
}
