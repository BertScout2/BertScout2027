using AirtableApiClient;
using BertScout2027.Models;
using System.Reflection;
using System.Text;

namespace BertScout2027.Database;

public class AirtableDatabase
{
    // To find Airtable identifiers, create the table in your "base" (database)
    // then look under Help, API Documentation. It contains all the exact info
    // for your identifiers and table, as well as all API calls to get and send
    // data to/from Airtable. It also has a link to get a personal access token
    // which should have a scope of "data.records:write". Copy it immediately
    // and save it somewhere, then encrypt with Base64 and paste it here.
    // Don't put the unencrypted personal access token into GitHub anywhere!

    // identifier for Airtable BertScout2027 database ("base")
    private const string AIRTABLE_BASE = "appZplIjMnsIy50Ku";
    // identifier for Airtable BertScout2027 "Matches" table
    private const string AIRTABLE_MATCHES_TABLE = "tbloentIMHSxGJRjX";

    // Token is encrypted base64 to avoid GitHub searches for plain text Airtable
    // tokens. Not great, but better than an unencrypted string. Any symetrical
    // encryption/decryption will do as well.
    private const string AIRTABLE_TOKEN_BASE64 = "cGF0WjJKR05Kak80VXE2SEEuNWQ5YzFkZWQyM2Q1NzAwNTgyMTJkYjUzYWYzZTY1MDBhNzgyZGI2Mzg4NzRkMjVmN2QzMTEzZTI4NjBjYTdhNA==";
    // unencrypted value only in memory
    private static string AIRTABLE_TOKEN = "";

    private static string AirtableToken()
    {
        if (AIRTABLE_TOKEN == "")
        {
            var base64EncodedBytes = Convert.FromBase64String(AIRTABLE_TOKEN_BASE64);
            AIRTABLE_TOKEN = Encoding.UTF8.GetString(base64EncodedBytes);
        }
        return AIRTABLE_TOKEN;
    }

    public static async Task<int> AirtableSendRecords(List<Match> matches)
    {
        int NewCount = 0;
        int UpdatedCount = 0;
        List<Fields> newRecordList = [];
        List<IdFields> updatedRecordList = [];
        FieldInfo[] myFieldInfo;

        // apparently matchType.GetFields() does not include BaseFields, so get separately and combine
        Type matchType = typeof(Match);
        Type baseType = typeof(Models.BaseModel);
        var matchList = matchType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).ToList();
        var baseList = baseType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).ToList();
        matchList.AddRange(baseList);
        myFieldInfo = [.. matchList];

        using AirtableBase airtableBase = new(AirtableToken(), AIRTABLE_BASE);

        foreach (Match match in matches)
        {
            if (match.Uuid == null) continue;
            if (string.IsNullOrEmpty(match.AirtableId))
            {
                Fields fields = new();
                foreach (FieldInfo fi in myFieldInfo)
                {
                    var name = FieldInfoName(fi);
                    if (string.IsNullOrEmpty(name)) continue;
                    if (ExcludeName(name)) continue;
                    object? value = fi.GetValue(match);
                    if (value == null) continue;
                    if (value is bool v) // change to integers
                    {
                        value = v ? 1 : 0;
                    }
                    fields.AddField(name, value);
                }
                newRecordList.Add(fields);
            }
            else if (match.Changed)
            {
                // use the AirtableId to identify changed records
                IdFields idFields = new(match.AirtableId);
                foreach (FieldInfo fi in myFieldInfo)
                {
                    var name = FieldInfoName(fi);
                    if (string.IsNullOrEmpty(name)) continue;
                    if (ExcludeName(name)) continue;
                    object? value = fi.GetValue(match);
                    if (value == null) continue;
                    if (value is bool v) // change to integers
                    {
                        value = v ? 1 : 0;
                    }
                    idFields.AddField(name, value);
                }
                updatedRecordList.Add(idFields);
            }

            if (newRecordList.Count > 0)
            {
                int tempCount = await AirtableSendNewRecords(airtableBase, newRecordList, matches);
                if (tempCount < 0)
                {
                    tempCount = 0; // error, don't count
                }
                NewCount += tempCount;
            }

            if (updatedRecordList.Count > 0)
            {
                int tempCount = await AirtableSendUpdatedRecords(airtableBase, updatedRecordList);
                if (tempCount < 0)
                {
                    tempCount = 0; // error, don't count
                }
                UpdatedCount += tempCount;
            }
        }

        return NewCount + UpdatedCount;
    }

    private static string FieldInfoName(FieldInfo fi)
    {
        var name = "";
        if (fi.Name.Contains('<') && fi.Name.Contains('>'))
        {
            // name is "<name>stuff", so just get the name part
            int pos1 = fi.Name.IndexOf('<') + 1;
            int pos2 = fi.Name.IndexOf('>');
            name = fi.Name[pos1..pos2];
        }
        return name;
    }

    private static bool ExcludeName(string name)
    {
        // these fields are not sent to Airtable
        if (name.Equals(nameof(Match.Id), StringComparison.OrdinalIgnoreCase)) return true;
        if (name.Equals(nameof(Match.AirtableId), StringComparison.OrdinalIgnoreCase)) return true;
        if (name.Equals(nameof(Match.Changed), StringComparison.OrdinalIgnoreCase)) return true;
        return false;
    }

    private static async Task<int> AirtableSendNewRecords(
        AirtableBase airtableBase,
        List<Fields> newRecordList,
        List<Match> matches)
    {
        AirtableCreateUpdateReplaceMultipleRecordsResponse result;
        List<Fields> sendList = [];
        int finalCount = 0;
        while (newRecordList.Count > 0)
        {
            sendList.Clear();
            do
            {
                sendList.Add(newRecordList[0]);
                newRecordList.RemoveAt(0);
            } while (newRecordList.Count > 0 && sendList.Count < 10);
            result = await airtableBase.CreateMultipleRecords(AIRTABLE_MATCHES_TABLE, sendList.ToArray());
            if (result == null || !result.Success)
            {
                return finalCount; // some may have sent
            }
            foreach (AirtableRecord rec in result.Records ?? [])
            {
                var uuid = rec.GetField("Uuid")?.ToString() ?? "";
                foreach (Match match in matches
                    .Where(x => x.Uuid.Equals(uuid, StringComparison.OrdinalIgnoreCase)))
                {
                    // Airtable sends back its own "Id" field which we save in AirtableId
                    match.AirtableId = rec.Id ?? "";
                    match.Changed = true;
                    finalCount++;
                }
            }
            if (newRecordList.Count > 0)
            {
                // Can only send 5 batches of 1 record per second.
                // Make sure that doesn't happen by sleeping 1/4 of a second
                // between each batch.
                Thread.Sleep(250);
            }
        }
        return finalCount;
    }

    private static async Task<int> AirtableSendUpdatedRecords(
        AirtableBase airtableBase,
        List<IdFields> updatedRecordList)
    {
        AirtableCreateUpdateReplaceMultipleRecordsResponse result;
        List<IdFields> sendList = [];
        int finalCount = 0;
        while (updatedRecordList.Count > 0)
        {
            sendList.Clear();
            do
            {
                sendList.Add(updatedRecordList[0]);
                updatedRecordList.RemoveAt(0);
            } while (updatedRecordList.Count > 0 && sendList.Count < 10);
            result = await airtableBase.UpdateMultipleRecords(AIRTABLE_MATCHES_TABLE, sendList.ToArray());
            if (!result.Success)
            {
                return finalCount; // some may have sent
            }
            foreach (AirtableRecord rec in result.Records ?? [])
            {
                finalCount++;
            }
            if (updatedRecordList.Count > 0)
            {
                // can only send 5 batches per second, make sure that doesn't happen
                Thread.Sleep(250);
            }
        }
        return finalCount;
    }
}
