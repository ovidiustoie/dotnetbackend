using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Requests;
using MobyLabWebProgramming.Core.Responses;
using MobyLabWebProgramming.Infrastructure.Authorization;
using MobyLabWebProgramming.Infrastructure.Extensions;
using MobyLabWebProgramming.Infrastructure.Services.Implementations;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;

namespace MobyLabWebProgramming.Backend.Controllers;

[ApiController] 
[Route("api/[controller]/[action]")] 
public class BookController : AuthorizedController 
{
  
    private readonly IBookService _service;

    public BookController(IUserService userService, IBookService bookService) : base(userService)
    {
        _service = bookService;
    }
    [Authorize] 
    [HttpGet("{id:guid}")] 
    public async Task<ActionResult<RequestResponse<BookAddDTO>>> GetById([FromRoute] Guid id) 
    {
        var currentUser = await GetCurrentUser();

        return currentUser.Result != null ?
            this.FromServiceResponse(await _service.GetBook(id)) :
            this.ErrorMessageResult<BookAddDTO>(currentUser.Error);
        
    }

    [Authorize]
    [HttpGet] 
    public async Task<ActionResult<RequestResponse<PagedResponse<BookDTO>>>> GetPage([FromQuery] PaginationSearchQueryParams pagination) 
    {
        var currentUser = await GetCurrentUser();

        return currentUser.Result != null ?
            this.FromServiceResponse(await _service.GetBooks(pagination)) :
            this.ErrorMessageResult<PagedResponse<BookDTO>>(currentUser.Error);
    }

    [Authorize]
    [HttpPost] 
    public async Task<ActionResult<RequestResponse>> Add([FromBody] BookAddDTO book)
    {
        var currentUser = await GetCurrentUser();
        return currentUser.Result != null ?
            this.FromServiceResponse(await _service.AddBook(book, currentUser.Result)) :
            this.ErrorMessageResult(currentUser.Error);
    }

    [Authorize]
    [HttpDelete("{id:guid}")] 
    public async Task<ActionResult<RequestResponse>> Delete([FromRoute] Guid id) 
    {
        var currentUser = await GetCurrentUser();

        return currentUser.Result != null ?
            this.FromServiceResponse(await _service.DeleteBook(id)) :
            this.ErrorMessageResult(currentUser.Error);
    }
    [Authorize]
    [HttpPut] 
    public async Task<ActionResult<RequestResponse>> Update([FromBody] BookUpdateDTO book) 
    {
        var currentUser = await GetCurrentUser();
        return currentUser.Result != null ?
            this.FromServiceResponse(await _service.UpdateBook(book, currentUser.Result)) :
            this.ErrorMessageResult(currentUser.Error);
    }

}
