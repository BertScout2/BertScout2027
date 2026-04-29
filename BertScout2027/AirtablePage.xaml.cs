using BertScout2027.Database;

namespace BertScout2027;

public partial class AirtablePage : ContentPage
{
    private readonly MatchDatabase db = new();
    private readonly GlobalViewModel _global;

    public AirtablePage(GlobalViewModel global)
    {
        InitializeComponent();
        _global = global;
        BindingContext = _global;
    }

    private async void AirtableSend_Clicked(object? sender, EventArgs e)
    {
        try
        {
            AirtableDoneLabel.Text = "";
            ErrorLabel.IsVisible = false;
            _global.AirtableUploadCount = 0;
            AirtableSend.IsEnabled = false;
            var matches = await db.GetMatchItemsAsync();
            _global.AirtableUploadCount = await AirtableDatabase.AirtableSendRecords(matches);
            foreach (var match in matches)
            {
                if (match.Changed)
                {
                    match.Changed = false;
                    await db.SaveMatchItemAsync(match);
                }
            }
            RefreshMatchSummaryList();
            AirtableDoneLabel.Text = "Done!";
            AirtableSend.IsEnabled = true;
        }
        catch (Exception ex)
        {
            ErrorLabel.Text = ex.Message;
            ErrorLabel.IsVisible = true;
        }
    }

    private async void RefreshMatchSummaryList()
    {
        var result = await db.GetMatchSummaryListAsync();
        _global.MatchSummaries = result;
    }
}