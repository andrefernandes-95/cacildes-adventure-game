namespace AF
{
    public class SerializedItem
    {
        public string itemID;
        public string resourcePath;
    }

    public class SerializedUpgradeableItem : SerializedItem
    {
        public int level;
    }

}
