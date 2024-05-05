using MobyLabWebProgramming.Core.Enums;

namespace MobyLabWebProgramming.Core.DataTransferObjects;
public class FeedbackDTO
{
    public Guid Id { get; set; }
    public int Score { get; set; } = default!; // How would you rate our website on a scale of 1 to 5

    public SiteDifficultyEnum SiteDificulty { get; set; } = default!; // How easy is it to find information on the site? 
    public bool RecommendToOthers { get; set; } = default!; //  Recommend our website to others?
    public SiteGoalEnum SiteGoal { get; set; } = default!; // Did you achieve  your goal ?
    public string Sugestion { get; set; } = default!;

}