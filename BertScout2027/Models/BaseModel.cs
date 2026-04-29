using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.Data.Sqlite;

namespace BertScout2027.Models;

public class BaseModel
{
    public const int BaseFieldCount = 4;

    public int Id { get; set; }

    public string Uuid { get; set; } = "";

    public string AirtableId { get; set; } = "";

    public bool Changed { get; set; }

    protected readonly JsonSerializerOptions WriteOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public static string BaseCreateTableFields()
    {
        return
            @"Id INTEGER PRIMARY KEY AUTOINCREMENT
            , Uuid TEXT NOT NULL
            , AirtableId TEXT NOT NULL
            , Changed INTEGER NOT NULL";
    }

    public static string BaseFieldsWithID()
    {
        return
            @$"Id
            , {BaseFields()}";
    }

    public static string BaseFields()
    {
        return
            @"Uuid
            , AirtableId
            , Changed";
    }

    public string BaseAddValues()
    {
        return
            @$"'{(Uuid != "" ? Uuid : Guid.NewGuid().ToString())}'
            , '{AirtableId}'
            , {(Changed ? 1 : 0)}";
    }

    public string BaseUpdateValues()
    {
        return
            @$"Uuid = '{(Uuid != "" ? Uuid : Guid.NewGuid().ToString())}'
            , AirtableId = '{AirtableId}'
            , Changed = {(Changed ? 1 : 0)}";
    }

    public void BaseFromReader(SqliteDataReader reader)
    {
        Id = reader.GetInt32(0);
        Uuid = reader.GetString(1);
        AirtableId = reader.GetString(2);
        Changed = reader.GetInt32(3) == 1;
    }

    public static string SQLInjectionFix(string value)
    {
        return value.Replace("'", "''");
    }
}
