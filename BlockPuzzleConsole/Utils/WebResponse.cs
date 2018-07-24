using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace PuzzleBlock.Utils
{
    public class WebShape
    {
        public WebShape(int shapeIndex, string shapeId, string orientation, List<string> possiblePlacements)
        {
            ShapeIndex = shapeIndex;
            ShapeId = shapeId;
            Orientation = orientation;
            PossiblePlacements = possiblePlacements;
        }

        public int ShapeIndex { get; set; }
        public string ShapeId { get; set; }
        public string Orientation { get; set; }
        public IList<string> PossiblePlacements { get; set; }

    }

    public class WebResponse
    {
        public WebResponse(bool isGameOver, int score, bool [,] boardState, List<WebShape> shapes)
            : this(isGameOver, score, BoardToString(boardState), shapes)
        {
        }

        public WebResponse(bool isGameOver, int score, bool[][] boardState, List<WebShape> shapes)
            : this(isGameOver, score, BoardToString(boardState), shapes)
        {
        }

        public WebResponse(bool isGameOver, int score, string boardState, List<WebShape> shapes)
        {
            IsGameOver = isGameOver;
            Score = score;
            BoardState = boardState;
            Shapes = shapes;
        }

        public static string BoardToString(bool[,] board)
        {
            StringBuilder sb = new StringBuilder(board.GetLength(0) * board.GetLength(1));

            foreach (bool cell in board)
            {
                sb.Append(cell ? "1" : "0");
            }

            return sb.ToString();
        }

        public static string BoardToString(bool[][] board)
        {
            StringBuilder sb = new StringBuilder(board.GetLength(0) * board.GetLength(0));

            foreach (var row in board)
            {
                foreach (var cell in row)
                {
                    sb.Append(cell ? "1" : "0");
                }
            }

            return sb.ToString();
        }

        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public bool IsGameOver { get; set; }
        public int Score { get; set; }
        public string BoardState { get; set; }
        public IList<WebShape> Shapes { get; set; }
    }
}
