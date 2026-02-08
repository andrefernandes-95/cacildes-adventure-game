using AF.Health;

namespace AF
{
    public struct DefenseComparisonResult
    {
        public Damage current;
        public Damage withItem;
        public int comparison; // -1 worse, 0 equal, 1 better
    }

}