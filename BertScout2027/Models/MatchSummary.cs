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
        var index = 0;
        var item = new MatchSummary
        {
            MatchNumber = reader.GetInt32(index++),
            TeamNumber = reader.GetInt32(index++),
            ScoutName = reader.GetString(index++),
            Uploaded = reader.GetString(index++),
        };
        return item;
    }
}
