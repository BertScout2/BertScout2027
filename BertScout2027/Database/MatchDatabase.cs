using BertScout2027.Models;
using Microsoft.Data.Sqlite;

namespace BertScout2027.Database;

public class MatchDatabase : BaseDatabase
{
    private const string MatchDBFilename = "MatchScout2027.db3";
    private const string TableName = Match.TableName;
    private SqliteConnection Database = new();
    private string? databasePath;
    private bool created = false;

    public MatchDatabase()
    {
    }

    private async Task InitMatchDB()
    {
        if (created)
        {
            return;
        }
        try
        {
            databasePath = Path.Combine(DatabasePath, MatchDBFilename);
            Database = new SqliteConnection("Data Source=" + databasePath);
            await Database.OpenAsync();
            var createTableCmd = Database.CreateCommand();
            createTableCmd.CommandText = Match.CreateTableCommand();
            await createTableCmd.ExecuteNonQueryAsync();
            var createIndexCmd = Database.CreateCommand();
            createIndexCmd.CommandText = Match.CreateTableIndexCommand();
            await createIndexCmd.ExecuteNonQueryAsync();
            Database.Close();
            created = true;
        }
        catch (Exception ex)
        {
            throw new SystemException($"Error initializing Matches database: {databasePath}\r\n{ex.Message}");
        }
    }

    public async Task<List<Match>> GetMatchItemsAsync()
    {
        await InitMatchDB();
        await Database.OpenAsync();
        var selectCmd = Database.CreateCommand();
        selectCmd.CommandText =
            @$"SELECT
            {Match.MatchFieldsWithId()}
            FROM {TableName}
            ORDER BY MatchNumber";
        await using var reader = await selectCmd.ExecuteReaderAsync();
        var matches = new List<Match>();
        while (await reader.ReadAsync())
        {
            matches.Add(Match.FromReader(reader));
        }
        return matches;
    }

    // get short info for listing
    public async Task<List<MatchSummary>> GetMatchSummaryListAsync()
    {
        await InitMatchDB();
        await Database.OpenAsync();
        var selectCmd = Database.CreateCommand();
        selectCmd.CommandText =
            @$"SELECT
            MatchNumber
            , TeamNumber
            , ScoutName
            , CASE
                WHEN AirtableID IS NOT NULL AND AirtableID != '' THEN
                    CASE WHEN Changed = 0 THEN 'Uploaded' ELSE 'Changed' END
                ELSE ''
            END as Uploaded
            FROM {TableName}
            ORDER BY MatchNumber";
        await using var reader = await selectCmd.ExecuteReaderAsync();
        var matches = new List<MatchSummary>();
        while (await reader.ReadAsync())
        {
            matches.Add(MatchSummary.FromReader(reader));
        }
        return matches;
    }

    public async Task<List<Match>> GetChangedMatchItemsAsync()
    {
        await InitMatchDB();
        await Database.OpenAsync();
        var selectCmd = Database.CreateCommand();
        selectCmd.CommandText =
            @$"SELECT
            {Match.MatchFieldsWithId()}
            FROM {TableName}
            WHERE Changed = 1
            ORDER BY MatchNumber";
        await using var reader = await selectCmd.ExecuteReaderAsync();
        var matches = new List<Match>();
        while (await reader.ReadAsync())
        {
            matches.Add(Match.FromReader(reader));
        }
        return matches;
    }

    public async Task<List<Match>> GetTeamAllMatches(int team)
    {
        await InitMatchDB();
        await Database.OpenAsync();
        var selectCmd = Database.CreateCommand();
        selectCmd.CommandText =
            @$"SELECT
            {Match.MatchFieldsWithId()}
            FROM {TableName}
            WHERE TeamNumber = @team
            ORDER BY MatchNumber";
        selectCmd.Parameters.AddWithValue("@team", team);
        await using var reader = await selectCmd.ExecuteReaderAsync();
        var matches = new List<Match>();
        while (await reader.ReadAsync())
        {
            matches.Add(Match.FromReader(reader));
        }
        return matches;
    }

    public async Task<Match?> GetMatchAsync(int match)
    {
        await InitMatchDB();
        await Database.OpenAsync();
        var selectCmd = Database.CreateCommand();
        selectCmd.CommandText =
            @$"SELECT
            {Match.MatchFieldsWithId()}
            FROM {TableName}
            WHERE MatchNumber = @match";
        selectCmd.Parameters.AddWithValue("@match", match);
        await using var reader = await selectCmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return Match.FromReader(reader);
        }
        return null;
    }

    public async Task<int> SaveMatchItemAsync(Match item)
    {
        await InitMatchDB();
        await Database.OpenAsync();
        var cmd = Database.CreateCommand();
        if (item.Id != 0)
        {
            cmd.CommandText = item.UpdateCommand();
        }
        else
        {
            var oldItem = await GetMatchAsync(item.MatchNumber);
            if (oldItem != null)
            {
                item.Id = oldItem.Id;
                item.Uuid = oldItem.Uuid;
                if (!string.IsNullOrWhiteSpace(oldItem.AirtableId))
                {
                    item.AirtableId = oldItem.AirtableId;
                }
                cmd.CommandText = item.UpdateCommand();
            }
            else
            {
                cmd.CommandText = item.AddCommand();
            }
        }
        var count = await cmd.ExecuteNonQueryAsync();
        Database.Close();
        return count;
    }

    //public async Task<int> DeleteMatchItemAsync(int match)
    //{
    //    await InitMatchDB();
    //    await Database.OpenAsync();
    //    var cmd = Database.CreateCommand();
    //    cmd.CommandText =
    //        @$"DELETE FROM {TableName}
    //        WHERE MatchNumber = @match";
    //    cmd.Parameters.AddWithValue("@match", match);
    //    var count = await cmd.ExecuteNonQueryAsync();
    //    Database.Close();
    //    return count;
    //}
}
