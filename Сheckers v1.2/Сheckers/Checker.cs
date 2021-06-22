namespace Сheckers
{
    internal class Checker
    {
        public Checker(int x, int y, bool isWhite, bool isQueen = false, 
            bool isEnabled = true, int movesCount = 0)
        {
            X = x;
            Y = y;
            IsWhite = isWhite;
            IsQueen = isQueen;
            IsEnabled = isEnabled;
            MovesCount = movesCount;
        }

        internal bool IsQueen { get; set; }
        internal bool IsEnabled { get; set; }
        internal bool IsWhite { get; }
        internal int X { get; private set; }
        internal int Y { get; private set; }
        internal int MovesCount { get; private set; }

        public void SetPosition(int x, int y)
        {
            X = x;
            Y = y;
            MovesCount++;
        }

        public override string ToString()
        {
            string asString = IsWhite ? "w" : "b";
            asString = IsQueen ? asString.ToUpper() : asString;
            return asString;
        }

        public Checker Copy()
        {
            return new Checker(X, Y, IsWhite, IsQueen, IsEnabled, MovesCount);
            // return (Checker) MemberwiseClone();
        }
    }
}