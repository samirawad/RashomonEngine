namespace GoapRpgPoC.Core
{
    public class Location
    {
        public string Name { get; private set; }
        public Vector2 Position { get; private set; }

        public Location(string name, int x, int y)
        {
            Name = name;
            Position = new Vector2(x, y);
        }

        public override string ToString() => $"{Name} at {Position}";
    }
}
