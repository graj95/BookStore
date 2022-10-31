using BookStore.Context;
using BookStore.Models;
using BookStore.Repository.Interface;
using Dapper;
using System.Data;
using System.Data.Common;
using System.Xml.Linq;

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

        public async Task<BaseResponseStatus> GetAllBookModelPagination(int pageno, int pagesize,int id, string? searchText)
        {
            BaseResponseStatus baseResponse = new BaseResponseStatus();
            PaginationModel pagination1 = new PaginationModel();
            List<BookModel> books = new List<BookModel>();
            if (pageno == 0)
            {
                pageno = 1;
            }
            if (pagesize == 0)
            {
                pagesize = 10;
            }
            using (var Connection = _context.CreateConnection())
            {
              
                if (searchText == null)
                {
                    searchText = "";
                }
               
                var sql = (@"Select ROW_NUMBER() OVER(ORDER BY Id desc) as SrNo,Id,book_name,descriptions,publication_date,
                            CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsDeleted from book 
                            where (book_name like '%'+@searchText+'%'  OR descriptions like '%'+@searchText+'%') AND (Id=@id or @id=0) AND IsDeleted=0 order by Id desc
                           OFFSET(@pageno - 1) * @pagesize ROWS FETCH NEXT @pagesize ROWS ONLY; 
                           select @pageno as PageNo, count(distinct Id) as TotalPages from book  
                               where isDeleted=0");
                var result = await Connection.QueryMultipleAsync(sql, new { pageno = pageno, pagesize = pagesize, searchText = searchText,id=id });
                var dataList = await result.ReadAsync<BookModel>();
                var pagination = await result.ReadAsync<PaginationModel>();
                books = dataList.ToList();
                pagination1 = pagination.FirstOrDefault();
                int PageCount = 0;
                int last = 0;
                last = pagination1.TotalPages % pagesize;
                PageCount = pagination1.TotalPages / pagesize;
                pagination1.PageCount = pagination1.TotalPages;
                if (last > 0)
                {
                    pagination1.TotalPages = PageCount + 1;
                }
                baseResponse.ResponseData1 = books;
                baseResponse.ResponseData2 = pagination;
            }
            return baseResponse;
        }

      /*  public async Task<IEnumerable<BookModel>> GetBookModels()
        {
            List<BookModel> bookModels = new List<BookModel>();
            var query = "select ROW_NUMBER() OVER(ORDER BY Id desc) as SrNo, * from book";

            using (var Connection = _context.CreateConnection())
            {
                var regelt = await Connection.QueryAsync<BookModel>(query);
                bookModels = regelt.ToList();

                return bookModels;
            }
        }*/

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
