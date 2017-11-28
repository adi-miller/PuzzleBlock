namespace PuzzleBlock
{
    public class Shape
    {
        public bool[,] Bits;

        public enum Type { Singler, SmallL, LargeL, ThreeLiner, FourLiner, FiveLiner, LongTail, SmallSquare, LargeSquare }
        public enum ShapeOrientation { N, W, S, E }

        private Type ShapeType { get; }
        private ShapeOrientation Orientation { get; }

        public Shape(Type type, ShapeOrientation orientation)
        {
            ShapeType = type;
            Orientation = orientation;
            UpdateBits();

            for (int x = 0; x <= Bits.GetUpperBound(0); x++)
            {
                for (int y = 0; y <= Bits.GetUpperBound(1); y++)
                {
                    if (Bits[x, y])
                        Score++;
                }
            }
        }

        public string Name => $"{ShapeType.ToString()} [{Orientation}]";

        public int Score { get; internal set; }

        private void UpdateBits()
        {
            switch (ShapeType)
            {
                case Shape.Type.FiveLiner:
                    if (Orientation == Shape.ShapeOrientation.W || Orientation == Shape.ShapeOrientation.E)
                    {
                        // Console.WriteLine("XXXXX");
                        Bits = new bool[1, 5];
                        for (int i = 0; i < 5; i++)
                            Bits[0, i] = true;
                    }
                    else
                    {
                        Bits = new bool[5, 1];
                        for (int i = 0; i < 5; i++)
                            Bits[i, 0] = true;
                    }
                    break;
                case Shape.Type.Singler:
                    // Console.WriteLine("X");
                    Bits = new bool[1, 1];
                    Bits[0, 0] = true;
                    break;
                case Shape.Type.SmallL:
                    Bits = new bool[2, 2];
                    switch (Orientation)
                    {
                        case Shape.ShapeOrientation.N:
                            // Console.WriteLine("XX");
                            // Console.WriteLine("X");
                            Bits[0, 0] = true; Bits[0, 1] = true;
                            Bits[1, 0] = true; Bits[1, 1] = false;
                            break;
                        case Shape.ShapeOrientation.W:
                            // Console.WriteLine("XX");
                            // Console.WriteLine(" X");
                            Bits[0, 0] = true; Bits[0, 1] = true;
                            Bits[1, 0] = false; Bits[1, 1] = true;
                            break;
                        case Shape.ShapeOrientation.S:
                            // Console.WriteLine(" X");
                            // Console.WriteLine("XX");
                            Bits[0, 0] = false; Bits[0, 1] = true;
                            Bits[1, 0] = true; Bits[1, 1] = true;
                            break;
                        case Shape.ShapeOrientation.E:
                            // Console.WriteLine("X");
                            // Console.WriteLine("XX");
                            Bits[0, 0] = true; Bits[0, 1] = false;
                            Bits[1, 0] = true; Bits[1, 1] = true;
                            break;
                    }
                    break;
                case Shape.Type.LargeL:
                    Bits = new bool[3, 3];
                    switch (Orientation)
                    {
                        case Shape.ShapeOrientation.N:
                            // Console.WriteLine("XXX");
                            // Console.WriteLine("X");
                            // Console.WriteLine("X");
                            Bits[0, 0] = true; Bits[0, 1] = true; Bits[0, 2] = true;
                            Bits[1, 0] = true; Bits[1, 1] = false; Bits[1, 2] = false;
                            Bits[2, 0] = true; Bits[2, 1] = false; Bits[2, 2] = false;
                            break;
                        case Shape.ShapeOrientation.W:
                            // Console.WriteLine("XXX");
                            // Console.WriteLine("  X");
                            // Console.WriteLine("  X");
                            Bits[0, 0] = true; Bits[0, 1] = true; Bits[0, 2] = true;
                            Bits[1, 0] = false; Bits[1, 1] = false; Bits[1, 2] = true;
                            Bits[2, 0] = false; Bits[2, 1] = false; Bits[2, 2] = true;
                            break;
                        case Shape.ShapeOrientation.S:
                            // Console.WriteLine("  X");
                            // Console.WriteLine("  X");
                            // Console.WriteLine("XXX");
                            Bits[0, 0] = false; Bits[0, 1] = false; Bits[0, 2] = true;
                            Bits[1, 0] = false; Bits[1, 1] = false; Bits[1, 2] = true;
                            Bits[2, 0] = true; Bits[2, 1] = true; Bits[2, 2] = true;
                            break;
                        case Shape.ShapeOrientation.E:
                            // Console.WriteLine("X");
                            // Console.WriteLine("X");
                            // Console.WriteLine("XXX");
                            Bits[0, 0] = true; Bits[0, 1] = false; Bits[0, 2] = false;
                            Bits[1, 0] = true; Bits[1, 1] = false; Bits[1, 2] = false;
                            Bits[2, 0] = true; Bits[2, 1] = true; Bits[2, 2] = true;
                            break;
                    }
                    break;
                case Shape.Type.ThreeLiner:
                    if (Orientation == Shape.ShapeOrientation.W || Orientation == Shape.ShapeOrientation.E)
                    {
                        // Console.WriteLine("XXX");
                        Bits = new bool[1, 3];
                        for (int i = 0; i < 3; i++)
                            Bits[0, i] = true;
                    }
                    else
                    {
                        Bits = new bool[3, 1];
                        for (int i = 0; i < 3; i++)
                            Bits[i, 0] = true;
                    }
                    break;
                case Shape.Type.FourLiner:
                    if (Orientation == Shape.ShapeOrientation.W || Orientation == Shape.ShapeOrientation.E)
                    {
                        // Console.WriteLine("XXXX");
                        Bits = new bool[1, 4];
                        for (int i = 0; i < 4; i++)
                            Bits[0, i] = true;
                    }
                    else
                    {
                        Bits = new bool[4, 1];
                        for (int i = 0; i < 4; i++)
                            Bits[i, 0] = true;
                    }
                    break;
                case Shape.Type.LongTail:
                    switch (Orientation)
                    {
                        case Shape.ShapeOrientation.N:
                            // Console.WriteLine("XXX");
                            // Console.WriteLine("X");
                            Bits = new bool[2, 3];
                            Bits[0, 0] = true; Bits[0, 1] = true; Bits[0, 2] = true;
                            Bits[1, 0] = true; Bits[1, 1] = false; Bits[1, 2] = false;
                            break;
                        case Shape.ShapeOrientation.W:
                            // Console.WriteLine("XX");
                            // Console.WriteLine(" X");
                            // Console.WriteLine(" X");
                            Bits = new bool[3, 2];
                            Bits[0, 0] = true; Bits[0, 1] = true; 
                            Bits[1, 0] = false; Bits[1, 1] = true; 
                            Bits[2, 0] = false; Bits[2, 1] = true; 
                            break;
                        case Shape.ShapeOrientation.S:
                            // Console.WriteLine("  X");
                            // Console.WriteLine("XXX");
                            Bits = new bool[2, 3];
                            Bits[0, 0] = false; Bits[0, 1] = false; Bits[0, 2] = true;
                            Bits[1, 0] = true; Bits[1, 1] = true; Bits[1, 2] = true;
                            break;
                        case Shape.ShapeOrientation.E:
                            // Console.WriteLine("X");
                            // Console.WriteLine("X");
                            // Console.WriteLine("XX");
                            Bits = new bool[3, 2];
                            Bits[0, 0] = true; Bits[0, 1] = false;
                            Bits[1, 0] = true; Bits[1, 1] = false;
                            Bits[2, 0] = true; Bits[2, 1] = true;
                            break;
                    }
                    break;
                case Shape.Type.SmallSquare:
                    // Console.WriteLine("XX");
                    // Console.WriteLine("XX");
                    Bits = new bool[2, 2];
                    Bits[0, 0] = true; Bits[0, 1] = true;
                    Bits[1, 0] = true; Bits[1, 1] = true;
                    break;
                case Shape.Type.LargeSquare:
                    // Console.WriteLine("XXX");
                    // Console.WriteLine("XXX");
                    // Console.WriteLine("XXX");
                    Bits = new bool[3, 3];
                    Bits[0, 0] = true; Bits[0, 1] = true; Bits[0, 2] = true;
                    Bits[1, 0] = true; Bits[1, 1] = true; Bits[1, 2] = true;
                    Bits[2, 0] = true; Bits[2, 1] = true; Bits[2, 2] = true;
                    break;
            }
        }
    }
}
