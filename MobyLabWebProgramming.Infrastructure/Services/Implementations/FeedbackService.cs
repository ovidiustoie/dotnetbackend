using System.Net;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;
using MobyLabWebProgramming.Core.Enums;
using MobyLabWebProgramming.Core.Errors;
using MobyLabWebProgramming.Core.Requests;
using MobyLabWebProgramming.Core.Responses;
using MobyLabWebProgramming.Core.Specifications;
using MobyLabWebProgramming.Infrastructure.Authorization;
using MobyLabWebProgramming.Infrastructure.Database;
using MobyLabWebProgramming.Infrastructure.Repositories.Interfaces;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;

namespace MobyLabWebProgramming.Infrastructure.Services.Implementations;

public class FeedbackService : IFeedbackService

{
    private readonly IRepository<WebAppDatabaseContext> _repository;

    /// <summary>
    /// Inject the required services through the constructor.
    /// </summary>
    public FeedbackService(IRepository<WebAppDatabaseContext> repository)
    {
        _repository = repository;
    }

    public async Task<ServiceResponse<FeedbackDTO>> GetFeedback(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _repository.GetAsync(new FeedbackProjectionSpec(id), cancellationToken);

        return result != null ?
            ServiceResponse<FeedbackDTO>.ForSuccess(result) :
            ServiceResponse<FeedbackDTO>.FromError(CommonErrors.FeedbackNotFound);
    }

    public async Task<ServiceResponse<PagedResponse<FeedbackDTO>>> GetFeedbacks(PaginationSearchQueryParams pagination, CancellationToken cancellationToken = default)
    {
        var result = await _repository.PageAsync(pagination, new FeedbackProjectionSpec(pagination.Search), cancellationToken);

        return ServiceResponse<PagedResponse<FeedbackDTO>>.ForSuccess(result);
    }


    public async Task<ServiceResponse<int>> GetFeedbackCount(CancellationToken cancellationToken = default) =>
        ServiceResponse<int>.ForSuccess(await _repository.GetCountAsync<Feedback>(cancellationToken));

    public async Task<ServiceResponse> AddFeedback(FeedbackDTO feedback, UserDTO? requestingUser, CancellationToken cancellationToken = default)
    {
        if (String.IsNullOrEmpty(feedback.SiteGoal))
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "SiteGoal name is mandatory", ErrorCodes.CannotAdd));

        }
        if (String.IsNullOrEmpty(feedback.SiteDificulty))
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "SiteDificulty is mandatory", ErrorCodes.CannotAdd));

        }
        var feedbackDb = new Feedback()
        {
            Score = (feedback.Score | 0),
            RecommendToOthers = (feedback.RecommendToOthers | false),
            SiteDificulty = feedback.SiteDificulty,
            SiteGoal = feedback.SiteGoal,
            Sugestion = feedback.Sugestion,
        };
        
        await _repository.AddAsync(feedbackDb, cancellationToken); // A new entity is created and persisted in the database.

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> UpdateFeedback(FeedbackUpdateDTO feedback, UserDTO? requestingUser, CancellationToken cancellationToken = default)
    {
        if (requestingUser != null && requestingUser.Role != UserRoleEnum.Admin)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Only the admin or personal can update the librarian!", ErrorCodes.CannotUpdate));
        }
        var entity = await _repository.GetAsync(new FeedbackSpec(feedback.Id), cancellationToken);

        if (entity != null)
        {
            entity.Score = feedback.Score ?? entity.Score;
            entity.SiteDificulty = feedback.SiteDificulty ?? entity.SiteDificulty;
            entity.SiteGoal = feedback.SiteGoal ?? entity.SiteGoal;
            entity.Sugestion = feedback.Sugestion ?? entity.Sugestion;
            await _repository.UpdateAsync(entity, cancellationToken); // Update the entity and persist the changes.
        }

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> DeleteFeedback(Guid id, UserDTO? requestingUser = default, CancellationToken cancellationToken = default)
    {
        if (requestingUser != null && requestingUser.Role != UserRoleEnum.Admin)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Only the admin  can remove feedbacks!", ErrorCodes.CannotUpdate));
        }
        await _repository.DeleteAsync<Feedback>(id, cancellationToken); // Delete the entity.

        return ServiceResponse.ForSuccess();
    }
}


