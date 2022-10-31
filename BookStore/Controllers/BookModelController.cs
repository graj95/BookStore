using BookStore.Models;
using BookStore.Repository;
using BookStore.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Server;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookModelController : ControllerBase
    {
        private readonly IBookModelRepositoty _bookModelRepositoty;

        public BookModelController(IBookModelRepositoty bookModelRepositoty)
        {
            _bookModelRepositoty = bookModelRepositoty;
        }

       /* [HttpGet]
        public async Task<IActionResult> GetAllBooks()
        {
            try
            {
                var book = await _bookModelRepositoty.GetBookModels();
                return Ok(book);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }*/
        [HttpPost]

        public async Task<IActionResult> SaveBookModel(BookModel bookModel)
        {
            try
            {
                var result = await _bookModelRepositoty.SaveBookModel(bookModel);

                if (result == 0)
                {
                    return StatusCode(409, "The request could not be processed because of conflict in the request");
                }
                else
                {
                    return StatusCode(200, string.Format("Record Inserted Successfuly with compnay Id {0}", result));
                }
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPut]

        public async Task<IActionResult> UpdateBookModel(BookModel bookModel)
        {
            try
            {
                var result = await _bookModelRepositoty.UpdateBookModel(bookModel);
                if (result == 0)
                {
                    return StatusCode(409, "The request could not be processed because of conflict in the request");
                }
                else
                {
                    return StatusCode(200, string.Format("Record Updated Successfuly"));
                }
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBookModel(int id)
        {
            try
            {
                var result = await _bookModelRepositoty.DeleteBookModel(id);
                if (result == 0)
                {
                    return StatusCode(409, "The request could not be processed because of conflict in the request");
                }
                else
                {
                    return StatusCode(200, string.Format("Record Deleted Successfuly"));
                }
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetAllBookModelPagination")]
        public async Task<ActionResult> GetAllBookModelPagination(int pageno, int pagesize, int id, string? searchText)
        {
            BaseResponseStatus responseDetails = new BaseResponseStatus();
            var booklist = await _bookModelRepositoty.GetAllBookModelPagination(pageno, pagesize, id, searchText);
           List<BookModel> bookModels = (List<BookModel>)booklist.ResponseData1;
            if (bookModels.Count == 0)
            {
                var returnmsg = string.Format("No records are available for BookModel");
               
                responseDetails.StatusCode = StatusCodes.Status404NotFound.ToString();
                responseDetails.StatusMessage = returnmsg;
                return Ok(responseDetails);
            }
            var rtnmsg = string.Format("All Booklist records are fetched successfully.");
          
            responseDetails.StatusCode = StatusCodes.Status200OK.ToString();
            responseDetails.StatusMessage = rtnmsg;
            responseDetails.ResponseData = booklist.ResponseData1;
            responseDetails.ResponseData1 = booklist.ResponseData2;
            return Ok(responseDetails);
        }

    }
}