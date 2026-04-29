using BertScout2027.Database;
using BertScout2027.Models;

namespace BertScout2027;

public partial class ListMatchesPage : ContentPage
{
    private readonly GlobalViewModel _global;

    private readonly MatchDatabase db = new();

    public List<MatchSummary> Matches = [];

    public ListMatchesPage(GlobalViewModel global)
    {
        InitializeComponent();
        _global = global;
        BindingContext = _global;
        RefreshMatchSummaryList();
    }

    private async void RefreshMatchSummaryList()
    {
        var result = await db.GetMatchSummaryListAsync();
        _global.MatchSummaries = result;
    }

    //private void GoToMatchClicked(object? sender, EventArgs e)
    //{
    //    _global.TargetMatchNumber = ((Match)sender!).MatchNumber.ToString();
    //    Routing.RegisterRoute("MainPage", typeof(MainPage));
    //    var task = Task.Run(() => Shell.Current.GoToAsync("MainPage"));
    //    task.Wait();
    //}
}