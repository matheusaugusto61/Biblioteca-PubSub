using Biblioteca.Model;
using Biblioteca.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Controllers
{
    [ApiController]
    [Route("/v1/books")]
    public class BookController : ControllerBase
    {
        private readonly BookRepository _bookRepository;
        public BookController()
        {
             _bookRepository = new BookRepository();
        }

        [HttpPost]
        public ActionResult Post([FromBody]Book book)
        {
            try
            {
                _bookRepository.SaveBook(book);
                return StatusCode(201);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
        }
    }
}