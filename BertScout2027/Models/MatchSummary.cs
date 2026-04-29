using Microsoft.Data.Sqlite;

namespace BertScout2027.Models;

public class MatchSummary
{
    public int MatchNumber { get; set; }
    public int TeamNumber { get; set; }
    public string ScoutName { get; set; } = "";
    public string Uploaded { get; set; } = "";

    public static MatchSummary FromReader(SqliteDataReader reader)
    {
        var item = new MatchSummary
        {
            MatchNumber = reader.GetInt32(0),
            TeamNumber = reader.GetInt32(1),
            ScoutName = reader.GetString(2),
            Uploaded = reader.GetString(3),
        };
        return item;
    }
}
