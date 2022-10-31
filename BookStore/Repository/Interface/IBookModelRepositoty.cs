using BookStore.Models;

namespace BookStore.Repository.Interface
{
    public interface IBookModelRepositoty
    {
      //  public Task<IEnumerable<BookModel>> GetBookModels();
        /*  public Task<BookModel> GetBookModelById( int id);
          public Task<BookModel> SaveBook();   */

        public Task<int> SaveBookModel(BookModel bookModel);

        public Task<int> UpdateBookModel(BookModel bookModel);

        public Task<int> DeleteBookModel(int id);

        public Task<BaseResponseStatus> GetAllBookModelPagination(int pageno, int pagesize,int id, string? searchText);
    }
}
