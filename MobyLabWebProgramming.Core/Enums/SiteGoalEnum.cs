using Ardalis.SmartEnum;
using Ardalis.SmartEnum.SystemTextJson;
using System.Text.Json.Serialization;

namespace MobyLabWebProgramming.Core.Enums;

[JsonConverter(typeof(SmartEnumNameConverter<SiteGoalEnum, string>))]
public sealed class SiteGoalEnum : SmartEnum<SiteGoalEnum, string>
{
    public static readonly SiteGoalEnum Yes = new(nameof(Yes), "Yes");
    public static readonly SiteGoalEnum Partially = new(nameof(Partially), "Partially");
    public static readonly SiteGoalEnum No = new(nameof(No), "No");
   
    private SiteGoalEnum(string name, string value) : base(name, value)
    {
    }
}

