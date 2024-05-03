using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Requests;
using MobyLabWebProgramming.Core.Responses;
using MobyLabWebProgramming.Infrastructure.Authorization;
using MobyLabWebProgramming.Infrastructure.Extensions;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;

namespace MobyLabWebProgramming.Backend.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class LibrarianController : AuthorizedController 
{

    private readonly ILibrarianService _librarianService;

    public LibrarianController(IUserService userService, ILibrarianService librarianService) : base(userService)
    {
        _librarianService = librarianService;
    }


    [Authorize] 
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RequestResponse<LibrarianDTO>>> GetById([FromRoute] Guid id) 
    {
        var currentUser = await GetCurrentUser();

        return currentUser.Result != null ?
            this.FromServiceResponse(await _librarianService.GetLibrarian(id)) :
            this.ErrorMessageResult<LibrarianDTO>(currentUser.Error);
    }
    [Authorize]
    [HttpGet] 
    public async Task<ActionResult<RequestResponse<PagedResponse<LibrarianDTO>>>> GetPage([FromQuery] PaginationSearchQueryParams pagination)                                                                                             // the PaginationSearchQueryParams properties to the object in the method parameter.
    {
        var currentUser = await GetCurrentUser();

        return currentUser.Result != null ?
            this.FromServiceResponse(await _librarianService.GetLibrarians(pagination)) :
            this.ErrorMessageResult<PagedResponse<LibrarianDTO>>(currentUser.Error);
    }

    [Authorize]
    [HttpPost] 
    public async Task<ActionResult<RequestResponse>> Add([FromBody] LibrarianAddDTO librarian)
    {
        var currentUser = await GetCurrentUser();
        return currentUser.Result != null ?
            this.FromServiceResponse(await _librarianService.AddLibrarian(librarian, currentUser.Result)) :
            this.ErrorMessageResult(currentUser.Error);
    }

    [Authorize]
    [HttpPut] 
    public async Task<ActionResult<RequestResponse>> Update([FromBody] LibrarianUpdateDTO librarian)
    {
        var currentUser = await GetCurrentUser();

        return currentUser.Result != null ?
            this.FromServiceResponse(await _librarianService.UpdateLibrarian(librarian with
            {
            }, currentUser.Result)) :
            this.ErrorMessageResult(currentUser.Error);
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<RequestResponse>> Delete([FromRoute] Guid id)
    {
        var currentUser = await GetCurrentUser();

        return currentUser.Result != null ?
            this.FromServiceResponse(await _librarianService.DeleteLibrarian(id)) :
            this.ErrorMessageResult(currentUser.Error);
    }

}
