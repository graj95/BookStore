using BookStore.Context;
using BookStore.Models;
using BookStore.Repository.Interface;
using Dapper;
using System.Data;
using System.Data.Common;

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

        public async Task<BaseResponseStatus> GetAllBookModelPagination(int pageno, int pagesize, int? id, string? searchText)
        {
            BaseResponseStatus baseResponse = new BaseResponseStatus();
            PaginationModel pagination = new PaginationModel();
            List<BookModel> booksList = new List<BookModel>();
            if (pageno == 0)
            {
                pageno = 1;
            }
            if (pagesize == 0)
            {
                pagesize = 10;
            }
            IEnumerable<BookModel> li;
            using (var Connection = _context.CreateConnection())
            {
                string books;
                
                if (searchText == null && id==null)
                {                    
                    searchText = "";
                    li= await Connection.QueryAsync<BookModel>(@"
                    select * from book");
                }
                else if (id != null && searchText==null)
                {
                    books = "";
                    li = await Connection.QueryAsync<BookModel>(@"
                    select * from book where id=@id", new { id });
                }
                else 
                {
                    li = await Connection.QueryAsync<BookModel>(@"
                    select * from book where book_name like '%'+'@searchText'+'%'", new { searchText });

                }
                //else
                //{
                //    books = await Connection.QuerySingleAsync<string>(@"
                //    select * from book where id=@id", new { id });

                //}


                var sql = (@"Select ROW_NUMBER() OVER(ORDER BY Id desc) as SrNo, Id,book_name,descriptions,publication_date,
                                              CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsDeleted from book
                             where book_name like '%'+@searchText+'%' and descriptions like '%'+@searchText+'%' and IsDeleted=0 order by Id desc 
                           OFFSET(@pageno - 1) * @pagesize ROWS FETCH NEXT @pagesize ROWS ONLY ; 
                           select @pageno as PageNo, count(distinct Id) as TotalPages from book
                           
                             where book_name like '%'+@searchText+'%' and descriptions  like '%'+@searchText+'%' and IsDeleted=0
                            ");
                var result = await Connection.QueryMultipleAsync(sql, new { pageno = pageno, pagesize = pagesize, searchText = searchText });
                var cList = await result.ReadAsync<BookModel>();
                booksList = cList.ToList();
                var paginations = await result.ReadAsync<PaginationModel>();
                pagination = paginations.FirstOrDefault();

                int PageCount = 0;
                int last = 0;


                last = pagination.TotalPages % pagesize;
                PageCount = pagination.TotalPages / pagesize;
                pagination.PageCount = pagination.TotalPages;
                pagination.TotalPages = PageCount;


                if (last > 0)
                {
                    pagination.TotalPages = PageCount + 1;
                }



                baseResponse.ResponseData1 = li;
                baseResponse.ResponseData2 = pagination;
            }
            return baseResponse;
        
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
