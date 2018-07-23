﻿using System.Collections.Generic;
using System.Linq;
using ConsoleApp1.Utils;
using PuzzleBlock.Utils;

namespace PuzzleBlock.Players
{
    class MultiFactorSingleStepPlayer : IPlayer
    {
        public void MakeAMove(out int shapeId, out string placement, Board board, IDictionary<int, Shape> shapes, IGameDrawer renderer)
        {
            placement = "";
            shapeId = 0;
            var candidates = new List<Candidate>();
            foreach (var shape in shapes)
            {
                for (int x = 0; x < 8; x++)
                for (int y = 0; y < 8; y++)
                {
                    var newBoard = new Board(board);
                    var curPlacement = "" + (char)(97 + x) + (char)(49 + y);
                    if (newBoard.TryPlace(shape.Value, curPlacement))
                    {
                        var candidate = new Candidate()
                        {
                            Placement = curPlacement,
                            ShapeId = shape.Key,
                            ScoreGain = (newBoard.Score - shape.Value.Score) - board.Score,
                            CellsGain = newBoard.CellCount() - board.CellCount(),
                            LinesScore = newBoard.LinesScore(),
                            MaxArea = MaxArea.MaximalRectangle(newBoard.Cells),
                            FragScore = Fragmentation.GetFragmentationScore(newBoard.Cells)
                        };
                        candidates.Add(candidate);
                    }
                }
            }

            var maxAreaList = from x in candidates orderby x.MaxArea descending, x.FragScore descending select x;
            var fragScoreList = from x in candidates orderby x.FragScore descending, x.MaxArea descending select x;

            var maxArea = maxAreaList.First();
            var fragScore = fragScoreList.First();

            var final = fragScore.MaxArea < 0.32F ? maxArea : fragScore;

            placement = final.Placement;
            shapeId = final.ShapeId;
        }

        public void OnMoveComplete()
        {
        }
    }

    class Candidate
    {
        public string Placement { get; set; }
        public int ShapeId { get; set; }
        public int ScoreGain { get; set; }
        public int CellsGain { get; set; }
        public int LinesScore { get; set; }
        public float PlacementScore
        {
            get
            {
                float mult1 = 0;
                float mult2 = 0;

                if (Placement[0] == 'a' || Placement[0] == 'h')
                    mult1 = 1;
                if (Placement[0] == 'b' || Placement[0] == 'g')
                    mult1 = 0.75F;
                if (Placement[0] == 'c' || Placement[0] == 'f')
                    mult1 = 0.5F;
                if (Placement[0] == 'd' || Placement[0] == 'e')
                    mult1 = 0.25F;

                if (Placement[1] == '1' || Placement[1] == '8')
                    mult2 = 1;
                if (Placement[1] == '2' || Placement[1] == '7')
                    mult2 = 0.75F;
                if (Placement[1] == '3' || Placement[1] == '6')
                    mult2 = 0.5F;
                if (Placement[1] == '4' || Placement[1] == '5')
                    mult2 = 0.25F;

                return mult1 * mult2;
            }
        }
        public int MaxArea { get; set; }
        public float FragScore { get; set; }
    }
}