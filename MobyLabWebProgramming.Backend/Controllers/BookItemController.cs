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
public class BookItemController : AuthorizedController
{

    private readonly IBookItemService _service;

    public BookItemController(IUserService userService, IBookItemService bookService) : base(userService)
    {
        _service = bookService;
    }
    // [Authorize] 
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RequestResponse<BookItemDTO>>> GetById([FromRoute] Guid id)
    {
        var currentUser = await GetCurrentUser();

        return currentUser.Result != null ?
            this.FromServiceResponse(await _service.GetBookItem(id)) :
            this.ErrorMessageResult<BookItemDTO>(currentUser.Error);

    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<RequestResponse<PagedResponse<BookItemDTO>>>> GetPage([FromQuery] Guid bookId, [FromQuery] PaginationSearchQueryParams pagination)
    {
        var currentUser = await GetCurrentUser();

        return currentUser.Result != null ?
            this.FromServiceResponse(await _service.GetBookItems(bookId, pagination)) :
            this.ErrorMessageResult<PagedResponse<BookItemDTO>>(currentUser.Error);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<RequestResponse>> Add([FromBody] BookItemDTO bookItem)
    {
        var currentUser = await GetCurrentUser();
        return currentUser.Result != null ?
            this.FromServiceResponse(await _service.AddBookItem(bookItem, currentUser.Result)) :
            this.ErrorMessageResult(currentUser.Error);
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<RequestResponse>> Delete([FromRoute] Guid id)
    {
        var currentUser = await GetCurrentUser();

        return currentUser.Result != null ?
            this.FromServiceResponse(await _service.DeleteBookItem(id)) :
            this.ErrorMessageResult(currentUser.Error);
    }
    [Authorize]
    [HttpPut]
    public async Task<ActionResult<RequestResponse>> Update([FromBody] BookItemUpdateDTO bookItem)
    {
        var currentUser = await GetCurrentUser();
        return currentUser.Result != null ?
            this.FromServiceResponse(await _service.UpdateBookItem(bookItem, currentUser.Result)) :
            this.ErrorMessageResult(currentUser.Error);
    }

}
