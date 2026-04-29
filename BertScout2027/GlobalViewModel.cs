using BertScout2027.Models;
using System.ComponentModel;

namespace BertScout2027;

public partial class GlobalViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public string ScouterName
    {
        get
        {
            return Global.ScouterName;
        }
        set
        {
            if (Global.ScouterName != value)
            {
                Global.ScouterName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ScouterName)));
            }
        }
    }

    public int AirtableUploadCount
    {
        get
        {
            return Global.AirtableUploadCount;
        }
        set
        {
            if ((Global.AirtableUploadCount != value) && (value >= 0))
            {
                Global.AirtableUploadCount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AirtableUploadCount)));
            }
        }
    }

    public List<MatchSummary> MatchSummaries
    {
        get
        {
            return Global.MatchSummaries;
        }
        set
        {
            Global.MatchSummaries = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MatchSummaries)));
        }
    }

    public string TargetMatchNumber
    {
        get
        {
            return Global.TargetMatchNumber;
        }
        set
        {
            Global.TargetMatchNumber = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TargetMatchNumber)));
        }
    }
}