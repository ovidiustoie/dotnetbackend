using MobyLabWebProgramming.Core.Enums;

namespace MobyLabWebProgramming.Core.DataTransferObjects;
public record FeedbackUpdateDTO(Guid Id, int? Score = default, SiteDifficultyEnum? SiteDificulty = default, bool? RecommendToOthers = default, SiteGoalEnum? SiteGoal = default, string? Sugestion = default);
