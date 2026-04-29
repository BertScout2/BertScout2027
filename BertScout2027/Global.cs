using BertScout2027.Models;

namespace BertScout2027;

public static class Global
{
    public static string ScouterName { get; set; } = "";

    public static string TargetMatchNumber { get; set; } = "";

    public static int AirtableUploadCount { get; set; } = 0;

    public static List<MatchSummary> MatchSummaries { get; set; } = [];
}
