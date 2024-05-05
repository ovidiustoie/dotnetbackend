using System.Linq.Expressions;
using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Specifications;

public sealed class FeedbackProjectionSpec : BaseSpec<FeedbackProjectionSpec, Feedback, FeedbackDTO>
{
    protected override Expression<Func<Feedback, FeedbackDTO>> Spec => e => new()
    {
        Id = e.Id,
        SiteGoal = e.SiteGoal,
        Score = e.Score,
        SiteDificulty = e.SiteDificulty,
        Sugestion = e.Sugestion,
        RecommendToOthers = e.RecommendToOthers
    };

    public FeedbackProjectionSpec(bool orderByCreatedAt = true) : base(orderByCreatedAt)
    {
    }

    public FeedbackProjectionSpec(Guid id)
    {
        Query.Where(e => e.Id == id);
    }


    public FeedbackProjectionSpec(string? search)
    {
        search = !string.IsNullOrWhiteSpace(search) ? search.Trim() : null;

        if (search == null)
        {
            return;
        }

        var searchExpr = $"%{search.Replace(" ", "%")}%";

        Query.Where(e => EF.Functions.ILike(e.Sugestion, searchExpr));                                                            // Note that this will be translated to the database something like "where user.Name ilike '%str%'".
    }
}
