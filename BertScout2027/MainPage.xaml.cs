using BertScout2027.Database;
using BertScout2027.Models;
using static BertScout2027.Utilities.PermissionManagement;

namespace BertScout2027
{
    public partial class MainPage : ContentPage
    {
        private readonly MatchDatabase db = new();

        private readonly Color ColorButtonOn = Colors.Green;
        private readonly Color ColorButtonOff = Colors.LightGray;

        private readonly GlobalViewModel _global;

        private Match match = new();

        public MainPage(GlobalViewModel global)
        {
            InitializeComponent();
            _global = global;
            BindingContext = _global;
            var taskPerm = Task.Run(() => CheckAndRequestStoragePermissionsAsync());
            if (!taskPerm.Result)
            {
                ShowError("Storage Permissions have been denied\n" +
                    "Please turn on Storage permission in App Info / Permissions");
            }
            CommentPicker.Items.Clear();
            foreach (string s in CommentList)
            {
                CommentPicker.Items.Add(s);
            }
        }

        private void StartButtonClicked(object? sender, EventArgs e)
        {
            try
            {
                ErrorLabelTop.IsVisible = false;
                if (string.IsNullOrEmpty(_global.ScouterName))
                {
                    SetErrorLabelTop("Missing Scouter Name - Please Login");
                    return;
                }
                if (string.IsNullOrWhiteSpace(EntryMatchNumber.Text) ||
                    !int.TryParse(EntryMatchNumber.Text, out int matchNum) ||
                    matchNum <= 0)
                {
                    SetErrorLabelTop("Invalid Match Number");
                    return;
                }
                EntryMatchNumber.Text = matchNum.ToString();
                var existingMatch = Task.Run(() => db.GetMatchAsync(matchNum)).Result;
                if (existingMatch != null)
                {
                    match = existingMatch;
                    if (string.IsNullOrEmpty(EntryTeamNumber.Text))
                    {
                        EntryTeamNumber.Text = match.TeamNumber.ToString();
                    }
                    else if (int.TryParse(EntryTeamNumber.Text, out int newTeam) && newTeam > 0)
                    {
                        match.TeamNumber = newTeam;
                    }
                    if (string.IsNullOrEmpty(match.ScoutName))
                    {
                        match.ScoutName = _global.ScouterName;
                    }
                    FillFields();
                }
                else
                {
                    if (string.IsNullOrEmpty(EntryTeamNumber.Text) ||
                        !int.TryParse(EntryTeamNumber.Text, out int teamNumber) ||
                        teamNumber <= 0)
                    {
                        SetErrorLabelTop("Invalid Team Number");
                        return;
                    }
                    match = new Match(matchNum)
                    {
                        TeamNumber = teamNumber
                    };
                    ClearFields();
                }
                EntryMatchNumber.IsEnabled = false;
                EntryTeamNumber.IsEnabled = false;
                StartButton.IsEnabled = false;
                match.ScoutName = _global.ScouterName;
                ScoutingLayout.IsVisible = true;
            }
            catch (Exception)
            {
                SetErrorLabelTop("Error loading data from database");
            }
        }

        private void SetErrorLabelTop(string message)
        {
            ErrorLabelTop.Text = message;
            ErrorLabelTop.IsVisible = true;
        }

        private void ClearFields()
        {
            //SetAutoNumberOfCycles(0);
            //SetAutoShootingSpeed(0);
            //SetAutoBallsPerCycle(0);
            //SetAutoAccuracy(0);
            //SetAutoRobotSpeed(0);
            //SetAutoFloorPickup(false);
            //SetAutoOutpostPickup(false);
            //SetAutoRoute(0);
            //SetAutoClimbingLevel(0);
            //SetTeleNumberOfCycles(0);
            //SetTeleShootingSpeed(0);
            //SetTeleBallsPerCycle(0);
            //SetTeleAccuracy(0);
            //SetTeleRobotSpeed(0);
            //SetTeleFloorPickup(false);
            //SetTeleOutpostPickup(false);
            //SetTeleRoute(0);
            //SetTeleClimbingLevel(0);
            SetScoreStar(0);
            SetDefenseScoreStar(0);
            SetComments("");
        }

        private void FillFields()
        {
            //SetAutoNumberOfCycles(match.AutoNumberOfCycles);
            //SetAutoShootingSpeed(match.AutoShootingSpeed);
            //SetAutoBallsPerCycle(match.AutoBallsPerCycle);
            //SetAutoAccuracy(match.AutoAccuracy);
            //SetAutoRobotSpeed(match.AutoRobotSpeed);
            //SetAutoFloorPickup(match.AutoFloorPickup);
            //SetAutoOutpostPickup(match.AutoOutpostPickup);
            //SetAutoRoute(match.AutoRoute);
            //SetAutoClimbingLevel(match.AutoClimbingLevel);
            //SetTeleNumberOfCycles(match.TeleNumberOfCycles);
            //SetTeleShootingSpeed(match.TeleShootingSpeed);
            //SetTeleBallsPerCycle(match.TeleBallsPerCycle);
            //SetTeleAccuracy(match.TeleAccuracy);
            //SetTeleRobotSpeed(match.TeleRobotSpeed);
            //SetTeleFloorPickup(match.TeleFloorPickup);
            //SetTeleOutpostPickup(match.TeleOutpostPickup);
            //SetTeleRoute(match.TeleRoute);
            //SetTeleClimbingLevel(match.TeleClimbingLevel);
            SetScoreStar(match.Score);
            SetDefenseScoreStar(match.DefenseScore);
            SetComments(match.Comments);
        }

        private void SaveButtonClicked(object? sender, EventArgs e)
        {
            if (match.TeamNumber <= 0)
            {
                return;
            }
            SaveData();
            RefreshMatchSummaryList();
            var newMatchNum = match.MatchNumber + 1;
            StartButton.BackgroundColor = ColorButtonOn;
            ScoutingLayout.IsVisible = false;
            EntryTeamNumber.Text = "";
            var existingMatch = Task.Run(() => db.GetMatchAsync(newMatchNum)).Result;
            if (existingMatch != null)
            {
                match = existingMatch;
                if (string.IsNullOrEmpty(EntryTeamNumber.Text))
                {
                    EntryTeamNumber.Text = match.TeamNumber.ToString();
                }
                if (string.IsNullOrEmpty(match.ScoutName))
                {
                    match.ScoutName = _global.ScouterName;
                }
                FillFields();
            }
            else
            {
                match = new Match(newMatchNum);
                ClearFields();
            }
            EntryMatchNumber.Text = newMatchNum.ToString();
            EntryTeamNumber.IsEnabled = true;
            EntryMatchNumber.IsEnabled = true;
            StartButton.IsEnabled = true;
            EntryTeamNumber.Focus();
        }

        /*
        #region AutoNumberOfCycles

        private void AutoNumberOfCyclesPlusClicked(object? sender, EventArgs e)
        {
            SetAutoNumberOfCycles(match.AutoNumberOfCycles + 1);
            SaveData();
        }

        private void AutoNumberOfCyclesMinusClicked(object? sender, EventArgs e)
        {
            SetAutoNumberOfCycles(match.AutoNumberOfCycles - 1);
            SaveData();
        }

        private void SetAutoNumberOfCycles(int value)
        {
            if (value >= 0)
            {
                match.AutoNumberOfCycles = value;
                AutoNumberOfCycles.Text = value.ToString();
            }
        }

        #endregion

        #region AutoShootingSpeed

        private void AutoShootingSpeedNone_Clicked(object? sender, EventArgs e)
        {
            SetAutoShootingSpeed(0);
            SaveData();
        }

        private void AutoShootingSpeedLow_Clicked(object? sender, EventArgs e)
        {
            SetAutoShootingSpeed(1);
            SaveData();
        }

        private void AutoShootingSpeedMedium_Clicked(object? sender, EventArgs e)
        {
            SetAutoShootingSpeed(2);
            SaveData();
        }

        private void AutoShootingSpeedHigh_Clicked(object? sender, EventArgs e)
        {
            SetAutoShootingSpeed(3);
            SaveData();
        }

        private void SetAutoShootingSpeed(int value)
        {
            match.AutoShootingSpeed = value;
            AutoShootingSpeedNone.BackgroundColor = value == 0 ? ColorButtonOn : ColorButtonOff;
            AutoShootingSpeedLow.BackgroundColor = value == 1 ? ColorButtonOn : ColorButtonOff;
            AutoShootingSpeedMedium.BackgroundColor = value == 2 ? ColorButtonOn : ColorButtonOff;
            AutoShootingSpeedHigh.BackgroundColor = value == 3 ? ColorButtonOn : ColorButtonOff;
        }

        #endregion

        #region AutoBallsPerCycle
        */
        /*
        private void AutoBallsPerCycleNone_Clicked(object? sender, EventArgs e)
        {
            SetAutoBallsPerCycle(0);
            SaveData();
        }

        private void AutoBallsPerCycleLow_Clicked(object? sender, EventArgs e)
        {
            SetAutoBallsPerCycle(1);
            SaveData();
        }

        private void AutoBallsPerCycleMedium_Clicked(object? sender, EventArgs e)
        {
            SetAutoBallsPerCycle(2);
            SaveData();
        }

        private void AutoBallsPerCycleHigh_Clicked(object? sender, EventArgs e)
        {
            SetAutoBallsPerCycle(3);
            SaveData();
        }

        private void SetAutoBallsPerCycle(int value)
        {
            match.AutoBallsPerCycle = value;
            AutoBallsPerCycleNone.BackgroundColor = value == 0 ? ColorButtonOn : ColorButtonOff;
            AutoBallsPerCycleLow.BackgroundColor = value == 1 ? ColorButtonOn : ColorButtonOff;
            AutoBallsPerCycleMedium.BackgroundColor = value == 2 ? ColorButtonOn : ColorButtonOff;
            AutoBallsPerCycleHigh.BackgroundColor = value == 3 ? ColorButtonOn : ColorButtonOff;
        }

        #endregion
        */

        /*
        #region AutoAccuracy

        private void AutoAccuracyNone_Clicked(object? sender, EventArgs e)
        {
            SetAutoAccuracy(0);
            SaveData();
        }

        private void AutoAccuracyLow_Clicked(object? sender, EventArgs e)
        {
            SetAutoAccuracy(1);
            SaveData();
        }

        private void AutoAccuracyMedium_Clicked(object? sender, EventArgs e)
        {
            SetAutoAccuracy(2);
            SaveData();
        }

        private void AutoAccuracyHigh_Clicked(object? sender, EventArgs e)
        {
            SetAutoAccuracy(3);
            SaveData();
        }

        private void SetAutoAccuracy(int value)
        {
            match.AutoAccuracy = value;
            AutoAccuracyNone.BackgroundColor = value == 0 ? ColorButtonOn : ColorButtonOff;
            AutoAccuracyLow.BackgroundColor = value == 1 ? ColorButtonOn : ColorButtonOff;
            AutoAccuracyMedium.BackgroundColor = value == 2 ? ColorButtonOn : ColorButtonOff;
            AutoAccuracyHigh.BackgroundColor = value == 3 ? ColorButtonOn : ColorButtonOff;
        }

        #endregion
        */

        /*
        #region AutoRobotSpeed

        private void AutoRobotSpeedNoMovementClicked(object? sender, EventArgs e)
        {
            SetAutoRobotSpeed(0);
            SaveData();
        }
        private void AutoRobotSpeedSlowClicked(object? sender, EventArgs e)
        {
            SetAutoRobotSpeed(1);
            SaveData();
        }
        private void AutoRobotSpeedMediumClicked(object? sender, EventArgs e)
        {
            SetAutoRobotSpeed(2);
            SaveData();
        }
        private void AutoRobotSpeedFastClicked(object? sender, EventArgs e)
        {
            SetAutoRobotSpeed(3);
            SaveData();
        }

        private void SetAutoRobotSpeed(int value)
        {
            match.AutoRobotSpeed = value;
            AutoRobotSpeedNoMovement.BackgroundColor = value == 0 ? ColorButtonOn : ColorButtonOff;
            AutoRobotSpeedSlow.BackgroundColor = value == 1 ? ColorButtonOn : ColorButtonOff;
            AutoRobotSpeedMedium.BackgroundColor = value == 2 ? ColorButtonOn : ColorButtonOff;
            AutoRobotSpeedFast.BackgroundColor = value == 3 ? ColorButtonOn : ColorButtonOff;
        }

        #endregion
        */

        /*
        #region AutoFloorPickup

        private void AutoFloorPickupFalseClicked(object? sender, EventArgs e)
        {
            SetAutoFloorPickup(false);
            SaveData();
        }
        private void AutoFloorPickupTrueClicked(object? sender, EventArgs e)
        {
            SetAutoFloorPickup(true);
            SaveData();
        }

        private void SetAutoFloorPickup(bool value)
        {
            match.AutoFloorPickup = value;
            AutoFloorPickupFalse.BackgroundColor = !value ? ColorButtonOn : ColorButtonOff;
            AutoFloorPickupTrue.BackgroundColor = value ? ColorButtonOn : ColorButtonOff;
        }

        #endregion
        */

        /*
        #region AutoOutpostPickup

        private void AutoOutpostPickupFalseClicked(object? sender, EventArgs e)
        {
            SetAutoOutpostPickup(false);
            SaveData();
        }
        private void AutoOutpostPickupTrueClicked(object? sender, EventArgs e)
        {
            SetAutoOutpostPickup(true);
            SaveData();
        }
        private void SetAutoOutpostPickup(bool value)
        {
            match.AutoOutpostPickup = value;
            AutoOutpostPickupFalse.BackgroundColor = !value ? ColorButtonOn : ColorButtonOff;
            AutoOutpostPickupTrue.BackgroundColor = value ? ColorButtonOn : ColorButtonOff;
        }

        #endregion
        */

        /*
        #region AutoRoute

        private void AutoRouteNone_Clicked(object? sender, EventArgs e)
        {
            SetAutoRoute(0);
        }
        private void AutoRouteOver_Clicked(object? sender, EventArgs e)
        {
            SetAutoRoute(1);
        }
        private void AutoRouteUnder_Clicked(object? sender, EventArgs e)
        {
            SetAutoRoute(2);
        }
        private void AutoRouteBoth_Clicked(object? sender, EventArgs e)
        {
            SetAutoRoute(3);
        }

        private void SetAutoRoute(int value)
        {
            match.AutoRoute = value;
            AutoRouteNone.BackgroundColor = value == 0 ? ColorButtonOn : ColorButtonOff;
            AutoRouteOver.BackgroundColor = value == 1 ? ColorButtonOn : ColorButtonOff;
            AutoRouteUnder.BackgroundColor = value == 2 ? ColorButtonOn : ColorButtonOff;
            AutoRouteBoth.BackgroundColor = value == 3 ? ColorButtonOn : ColorButtonOff;
        }

        #endregion
        */

        /*
        #region AutoClimbingLevel

        private void AutoClimbingNoClimbClicked(object? sender, EventArgs e)
        {
            SetAutoClimbingLevel(0);
            SaveData();
        }
        private void AutoClimbingLevel1Clicked(object? sender, EventArgs e)
        {
            SetAutoClimbingLevel(1);
            SaveData();
        }
        private void AutoClimbingLevel2Clicked(object? sender, EventArgs e)
        {
            SetAutoClimbingLevel(2);
            SaveData();
        }
        private void AutoClimbingLevel3Clicked(object? sender, EventArgs e)
        {
            SetAutoClimbingLevel(3);
            SaveData();
        }
        private void SetAutoClimbingLevel(int value)
        {
            match.AutoClimbingLevel = value;
            AutoClimbingNoClimb.BackgroundColor = value == 0 ? ColorButtonOn : ColorButtonOff;
            AutoClimbingLevel1.BackgroundColor = value == 1 ? ColorButtonOn : ColorButtonOff;
            AutoClimbingLevel2.BackgroundColor = value == 2 ? ColorButtonOn : ColorButtonOff;
            AutoClimbingLevel3.BackgroundColor = value == 3 ? ColorButtonOn : ColorButtonOff;
        }

        #endregion
                */

        /*

        #region TeleNumberOfCycles

        private void TeleNumberOfCyclesPlusClicked(object? sender, EventArgs e)
        {
            SetTeleNumberOfCycles(match.TeleNumberOfCycles + 1);
            SaveData();
        }

        private void TeleNumberOfCyclesMinusClicked(object? sender, EventArgs e)
        {
            SetTeleNumberOfCycles(match.TeleNumberOfCycles - 1);
            SaveData();
        }
        private void SetTeleNumberOfCycles(int value)
        {
            if (value >= 0)
            {
                match.TeleNumberOfCycles = value;
                TeleNumberOfCycles.Text = value.ToString();
            }
        }

        #endregion
        */

        /*

        #region TeleShootingSpeed

        private void TeleShootingSpeedNone_Clicked(object? sender, EventArgs e)
        {
            SetTeleShootingSpeed(0);
            SaveData();
        }

        private void TeleShootingSpeedLow_Clicked(object? sender, EventArgs e)
        {
            SetTeleShootingSpeed(1);
            SaveData();
        }

        private void TeleShootingSpeedMedium_Clicked(object? sender, EventArgs e)
        {
            SetTeleShootingSpeed(2);
            SaveData();
        }

        private void TeleShootingSpeedHigh_Clicked(object? sender, EventArgs e)
        {
            SetTeleShootingSpeed(3);
            SaveData();
        }

        private void SetTeleShootingSpeed(int value)
        {
            match.TeleShootingSpeed = value;
            TeleShootingSpeedNone.BackgroundColor = value == 0 ? ColorButtonOn : ColorButtonOff;
            TeleShootingSpeedLow.BackgroundColor = value == 1 ? ColorButtonOn : ColorButtonOff;
            TeleShootingSpeedMedium.BackgroundColor = value == 2 ? ColorButtonOn : ColorButtonOff;
            TeleShootingSpeedHigh.BackgroundColor = value == 3 ? ColorButtonOn : ColorButtonOff;
        }

        #endregion
        */

        /*

        #region TeleBallsPerCycle

        private void TeleBallsPerCycleNone_Clicked(object? sender, EventArgs e)
        {
            SetTeleBallsPerCycle(0);
            SaveData();
        }

        private void TeleBallsPerCycleLow_Clicked(object? sender, EventArgs e)
        {
            SetTeleBallsPerCycle(1);
            SaveData();
        }

        private void TeleBallsPerCycleMedium_Clicked(object? sender, EventArgs e)
        {
            SetTeleBallsPerCycle(2);
            SaveData();
        }

        private void TeleBallsPerCycleHigh_Clicked(object? sender, EventArgs e)
        {
            SetTeleBallsPerCycle(3);
            SaveData();
        }

        private void SetTeleBallsPerCycle(int value)
        {
            match.TeleBallsPerCycle = value;
            TeleBallsPerCycleNone.BackgroundColor = value == 0 ? ColorButtonOn : ColorButtonOff;
            TeleBallsPerCycleLow.BackgroundColor = value == 1 ? ColorButtonOn : ColorButtonOff;
            TeleBallsPerCycleMedium.BackgroundColor = value == 2 ? ColorButtonOn : ColorButtonOff;
            TeleBallsPerCycleHigh.BackgroundColor = value == 3 ? ColorButtonOn : ColorButtonOff;
        }

        #endregion
        */

        /*

        #region TeleAccuracy

        private void TeleAccuracyNone_Clicked(object? sender, EventArgs e)
        {
            SetTeleAccuracy(0);
            SaveData();
        }

        private void TeleAccuracyLow_Clicked(object? sender, EventArgs e)
        {
            SetTeleAccuracy(1);
            SaveData();
        }

        private void TeleAccuracyMedium_Clicked(object? sender, EventArgs e)
        {
            SetTeleAccuracy(2);
            SaveData();
        }

        private void TeleAccuracyHigh_Clicked(object? sender, EventArgs e)
        {
            SetTeleAccuracy(3);
            SaveData();
        }

        private void SetTeleAccuracy(int value)
        {
            match.TeleAccuracy = value;
            TeleAccuracyNone.BackgroundColor = value == 0 ? ColorButtonOn : ColorButtonOff;
            TeleAccuracyLow.BackgroundColor = value == 1 ? ColorButtonOn : ColorButtonOff;
            TeleAccuracyMedium.BackgroundColor = value == 2 ? ColorButtonOn : ColorButtonOff;
            TeleAccuracyHigh.BackgroundColor = value == 3 ? ColorButtonOn : ColorButtonOff;
        }

        #endregion
        */

        /*

        #region TeleRobotSpeed

        private void TeleRobotSpeedNoMovementClicked(object? sender, EventArgs e)
        {
            SetTeleRobotSpeed(0);
            SaveData();
        }
        private void TeleRobotSpeedSlowClicked(object? sender, EventArgs e)
        {
            SetTeleRobotSpeed(1);
            SaveData();
        }
        private void TeleRobotSpeedMediumClicked(object? sender, EventArgs e)
        {
            SetTeleRobotSpeed(2);
            SaveData();
        }
        private void TeleRobotSpeedFastClicked(object? sender, EventArgs e)
        {
            SetTeleRobotSpeed(3);
            SaveData();
        }

        private void SetTeleRobotSpeed(int value)
        {
            match.TeleRobotSpeed = value;
            TeleRobotSpeedNoMovement.BackgroundColor = value == 0 ? ColorButtonOn : ColorButtonOff;
            TeleRobotSpeedSlow.BackgroundColor = value == 1 ? ColorButtonOn : ColorButtonOff;
            TeleRobotSpeedMedium.BackgroundColor = value == 2 ? ColorButtonOn : ColorButtonOff;
            TeleRobotSpeedFast.BackgroundColor = value == 3 ? ColorButtonOn : ColorButtonOff;
        }

        #endregion
*/

        /*
        #region TeleFloorPickup

        private void TeleFloorPickupFalseClicked(object? sender, EventArgs e)
        {
            SetTeleFloorPickup(false);
            SaveData();
        }
        private void TeleFloorPickupTrueClicked(object? sender, EventArgs e)
        {
            SetTeleFloorPickup(true);
            SaveData();
        }

        private void SetTeleFloorPickup(bool value)
        {
            match.TeleFloorPickup = value;
            TeleFloorPickupFalse.BackgroundColor = !value ? ColorButtonOn : ColorButtonOff;
            TeleFloorPickupTrue.BackgroundColor = value ? ColorButtonOn : ColorButtonOff;
        }

        #endregion
        */

        /*
        #region TeleOutpostPickup

        private void TeleOutpostPickupFalseClicked(object? sender, EventArgs e)
        {
            SetTeleOutpostPickup(false);
            SaveData();
        }
        private void TeleOutpostPickupTrueClicked(object? sender, EventArgs e)
        {
            SetTeleOutpostPickup(true);
            SaveData();
        }

        private void SetTeleOutpostPickup(bool value)
        {
            match.TeleOutpostPickup = value;
            TeleOutpostPickupFalse.BackgroundColor = !value ? ColorButtonOn : ColorButtonOff;
            TeleOutpostPickupTrue.BackgroundColor = value ? ColorButtonOn : ColorButtonOff;
        }

        #endregion
        */

        /*
        #region TeleRoute

        private void TeleRouteNone_Clicked(object? sender, EventArgs e)
        {
            SetTeleRoute(0);
        }
        private void TeleRouteOver_Clicked(object? sender, EventArgs e)
        {
            SetTeleRoute(1);
        }
        private void TeleRouteUnder_Clicked(object? sender, EventArgs e)
        {
            SetTeleRoute(2);
        }
        private void TeleRouteBoth_Clicked(object? sender, EventArgs e)
        {
            SetTeleRoute(3);
        }

        private void SetTeleRoute(int value)
        {
            match.TeleRoute = value;
            TeleRouteNone.BackgroundColor = value == 0 ? ColorButtonOn : ColorButtonOff;
            TeleRouteOver.BackgroundColor = value == 1 ? ColorButtonOn : ColorButtonOff;
            TeleRouteUnder.BackgroundColor = value == 2 ? ColorButtonOn : ColorButtonOff;
            TeleRouteBoth.BackgroundColor = value == 3 ? ColorButtonOn : ColorButtonOff;
        }

        #endregion
        */

        /*
        #region TeleClimbingLevel

        private void TeleClimbingNoClimbClicked(object? sender, EventArgs e)
        {
            SetTeleClimbingLevel(0);
            SaveData();
        }
        private void TeleClimbingLevel1Clicked(object? sender, EventArgs e)
        {
            SetTeleClimbingLevel(1);
            SaveData();
        }
        private void TeleClimbingLevel2Clicked(object? sender, EventArgs e)
        {
            SetTeleClimbingLevel(2);
            SaveData();
        }
        private void TeleClimbingLevel3Clicked(object? sender, EventArgs e)
        {
            SetTeleClimbingLevel(3);
            SaveData();
        }

        private void SetTeleClimbingLevel(int value)
        {
            match.TeleClimbingLevel = value;
            TeleClimbingNoClimb.BackgroundColor = value == 0 ? ColorButtonOn : ColorButtonOff;
            TeleClimbingLevel1.BackgroundColor = value == 1 ? ColorButtonOn : ColorButtonOff;
            TeleClimbingLevel2.BackgroundColor = value == 2 ? ColorButtonOn : ColorButtonOff;
            TeleClimbingLevel3.BackgroundColor = value == 3 ? ColorButtonOn : ColorButtonOff;
        }

        #endregion
        */

        #region Comments

        private void Comments_TextChanged(object? sender, TextChangedEventArgs e)
        {
            var value = Comments.Text;
            if (value.Length > 250)
            {
                value = value[0..250];
            }
            if (match.Comments == value)
            {
                return;
            }
            SetComments(value);
            SaveData();
        }

        private void SetComments(string value)
        {
            if (value.Length > 250)
            {
                value = value[0..250];
            }
            match.Comments = value;
            if (Comments.Text == value)
            {
                return;
            }
            Comments.Text = value;
        }

        #endregion
        private readonly List<string> CommentList =
        [
            "Can pick from the outpost.",
            "Picks up fuel fast.",
            "Got a fuel stuck inside.",
            "Missed shooting fuel into hub a lot.",
            "Good feeder to endzone.",
            "Dropped fuel a lot.",
            "Tried to climb but failed.",
            "Can go underneath trench.",
            "Played defense.",
            "Caused a penalty.",
            "Had technical issues.",
            "Hits wall often",
            "Broke down.",
            "Never moved.",
            "DON'T PICK!",
        ];

        #region Score

        private void ScoreStar0Clicked(object? sender, EventArgs e)
        {
            SetScoreStar(0);
            SaveData();
        }
        private void ScoreStar1Clicked(object? sender, EventArgs e)
        {
            SetScoreStar(1);
            SaveData();
        }
        private void ScoreStar2Clicked(object? sender, EventArgs e)
        {
            SetScoreStar(2);
            SaveData();
        }
        private void ScoreStar3Clicked(object? sender, EventArgs e)
        {
            SetScoreStar(3);
            SaveData();
        }
        private void ScoreStar4Clicked(object? sender, EventArgs e)
        {
            SetScoreStar(4);
            SaveData();
        }
        private void ScoreStar5Clicked(object? sender, EventArgs e)
        {
            SetScoreStar(5);
            SaveData();
        }
        private void SetScoreStar(int value)
        {
            match.Score = value;
            ScoreStar0.BackgroundColor = value == 0 ? ColorButtonOn : ColorButtonOff;
            ScoreStar1.BackgroundColor = value == 1 ? ColorButtonOn : ColorButtonOff;
            ScoreStar2.BackgroundColor = value == 2 ? ColorButtonOn : ColorButtonOff;
            ScoreStar3.BackgroundColor = value == 3 ? ColorButtonOn : ColorButtonOff;
            ScoreStar4.BackgroundColor = value == 4 ? ColorButtonOn : ColorButtonOff;
            ScoreStar5.BackgroundColor = value == 5 ? ColorButtonOn : ColorButtonOff;
        }

        #endregion

        #region DefenseScore

        private void DefenseScoreStar0Clicked(object? sender, EventArgs e)
        {
            SetDefenseScoreStar(0);
            SaveData();
        }
        private void DefenseScoreStar1Clicked(object? sender, EventArgs e)
        {
            SetDefenseScoreStar(1);
            SaveData();
        }
        private void DefenseScoreStar2Clicked(object? sender, EventArgs e)
        {
            SetDefenseScoreStar(2);
            SaveData();
        }
        private void DefenseScoreStar3Clicked(object? sender, EventArgs e)
        {
            SetDefenseScoreStar(3);
            SaveData();
        }
        private void DefenseScoreStar4Clicked(object? sender, EventArgs e)
        {
            SetDefenseScoreStar(4);
            SaveData();
        }
        private void DefenseScoreStar5Clicked(object? sender, EventArgs e)
        {
            SetDefenseScoreStar(5);
            SaveData();
        }
        private void SetDefenseScoreStar(int value)
        {
            match.DefenseScore = value;
            DefenseScoreStar0.BackgroundColor = value == 0 ? ColorButtonOn : ColorButtonOff;
            DefenseScoreStar1.BackgroundColor = value == 1 ? ColorButtonOn : ColorButtonOff;
            DefenseScoreStar2.BackgroundColor = value == 2 ? ColorButtonOn : ColorButtonOff;
            DefenseScoreStar3.BackgroundColor = value == 3 ? ColorButtonOn : ColorButtonOff;
            DefenseScoreStar4.BackgroundColor = value == 4 ? ColorButtonOn : ColorButtonOff;
            DefenseScoreStar5.BackgroundColor = value == 5 ? ColorButtonOn : ColorButtonOff;
        }

        #endregion

        private void SaveData()
        {
            try
            {
                match.Changed = true;
                var taskSave = Task.Run(() => db.SaveMatchItemAsync(match));
                taskSave.Wait();
            }
            catch (Exception)
            {
                SetErrorLabelTop("Error saving data to database");
            }
        }

        private void ShowError(string message)
        {
            MainLayout.IsVisible = false;
            ErrorLayout.IsVisible = true;
            ErrorMsg.Text = message;
        }

        private void CommentPicker_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (CommentPicker.SelectedIndex < 0)
                return;
            if (Comments.Text == null)
                Comments.Text = "";
            else if (Comments.Text.Length > 0 && !Comments.Text.EndsWith(' '))
                Comments.Text += " ";
            Comments.Text += CommentPicker.SelectedItem.ToString() + " ";
            CommentPicker.SelectedIndex = -1;
            SaveData();
        }

        private async void RefreshMatchSummaryList()
        {
            var result = await db.GetMatchSummaryListAsync();
            _global.MatchSummaries = result;
        }
    }
}
