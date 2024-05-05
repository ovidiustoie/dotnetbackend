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
public class FeedbackController : AuthorizedController
{

    private readonly IFeedbackService _service;

    public FeedbackController(IUserService userService, IFeedbackService feedbackService) : base(userService)
    {
        _service = feedbackService;
    }
    // [Authorize] 
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RequestResponse<FeedbackDTO>>> GetById([FromRoute] Guid id)
    {
        var currentUser = await GetCurrentUser();

        return currentUser.Result != null ?
            this.FromServiceResponse(await _service.GetFeedback(id)) :
            this.ErrorMessageResult<FeedbackDTO>(currentUser.Error);

    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<RequestResponse<PagedResponse<FeedbackDTO>>>> GetPage([FromQuery] PaginationSearchQueryParams pagination)
    {
        var currentUser = await GetCurrentUser();

        return currentUser.Result != null ?
            this.FromServiceResponse(await _service.GetFeedbacks(pagination)) :
            this.ErrorMessageResult<PagedResponse<FeedbackDTO>>(currentUser.Error);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<RequestResponse>> Add([FromBody] FeedbackDTO feedback)
    {
        var currentUser = await GetCurrentUser();
        return currentUser.Result != null ?
            this.FromServiceResponse(await _service.AddFeedback(feedback, currentUser.Result)) :
            this.ErrorMessageResult(currentUser.Error);
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<RequestResponse>> Delete([FromRoute] Guid id)
    {
        var currentUser = await GetCurrentUser();

        return currentUser.Result != null ?
            this.FromServiceResponse(await _service.DeleteFeedback(id)) :
            this.ErrorMessageResult(currentUser.Error);
    }
    [Authorize]
    [HttpPut]
    public async Task<ActionResult<RequestResponse>> Update([FromBody] FeedbackUpdateDTO feedback)
    {
        var currentUser = await GetCurrentUser();
        return currentUser.Result != null ?
            this.FromServiceResponse(await _service.UpdateFeedback(feedback, currentUser.Result)) :
            this.ErrorMessageResult(currentUser.Error);
    }

}
