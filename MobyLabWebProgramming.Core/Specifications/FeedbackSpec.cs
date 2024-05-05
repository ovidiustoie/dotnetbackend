using MobyLabWebProgramming.Core.Entities;
using Ardalis.Specification;

namespace MobyLabWebProgramming.Core.Specifications;

public sealed class FeedbackSpec : BaseSpec<FeedbackSpec, Feedback>
{
    public FeedbackSpec(Guid id)
    {
        Query.Where(e => e.Id == id);
    }
}
