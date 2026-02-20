namespace GoapRpgPoC.Core
{
    public class Location : Entity
    {
        public Location(string name, int x, int y) : base(name, new Vector2(x, y))
        {
        }

        public override string ToString() => $"{Name} at {Position}";
    }
}
