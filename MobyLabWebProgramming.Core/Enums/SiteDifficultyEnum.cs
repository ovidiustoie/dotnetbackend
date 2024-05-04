using Ardalis.SmartEnum;
using Ardalis.SmartEnum.SystemTextJson;
using System.Text.Json.Serialization;

namespace MobyLabWebProgramming.Core.Enums;

[JsonConverter(typeof(SmartEnumNameConverter<SiteDifficultyEnum, string>))]
public sealed class SiteDifficultyEnum : SmartEnum<SiteDifficultyEnum, string>
{
    public static readonly SiteDifficultyEnum VeryEasy = new(nameof(VeryEasy), "VeryEasy");
    public static readonly SiteDifficultyEnum Easy = new(nameof(Easy), "Easy");
    public static readonly SiteDifficultyEnum Difficult = new(nameof(Difficult), "Difficult");
    public static readonly SiteDifficultyEnum VeryDifficult = new(nameof(VeryDifficult), "VeryDifficult");

    private SiteDifficultyEnum(string name, string value) : base(name, value)
    {
    }
}
