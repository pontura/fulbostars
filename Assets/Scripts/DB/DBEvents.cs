namespace Fulbo.DB
{
    public static class DBEvents
    {
        public static System.Action<DBMatches.MatchData, System.Action<bool, string>> Track = delegate { };

        public static System.Action<int, Settings.stat, System.Action<bool, string>> UpgradeStat = delegate { };
        //characterID, position, Callback
        public static System.Action<int, int, System.Action<bool, string>> OnChangeCharacterPosition = delegate { }; 

        public static System.Action<string> OnSaveToken = delegate { };
        public static System.Action<System.Action> LoadMatches = delegate { };
        public static System.Action<System.Action> LoadUserData = delegate { };
        public static System.Action<DBUserData.UserData, System.Action<bool, string>> SaveUserData = delegate { };
        //public static System.Action<System.Action<bool, string>> UpdateCharacters = delegate { };
        public static System.Action<int, int, System.Action> LoadMatchesPerLevel = delegate { };
        public static System.Action<string, string, System.Action<string>> Login = delegate { };
        public static System.Action<DBRegisterTeam.RData, System.Action<bool, string>> OnRegisterTeam = delegate { };
    }
}