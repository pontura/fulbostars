class CharacterRolData
{
    public CharacterRolData(int characterID, bool isGoalKeeper)
    {
        this.characterID = characterID;
        this.isGoalkeeper = isGoalKeeper;
    }
    public int characterID;
    public bool isGoalkeeper;
    public int totalStats;
    public string positionText;
    public int uniqueID;
}
