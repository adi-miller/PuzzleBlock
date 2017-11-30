﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace PuzzleBlock.Players
{
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
    }

    class GamePath
    {
        public int CellsGain { get; set; }
        public float CellGainNorm => ((float)(1 - ((float)CellsGain - (-43)) / ((float)27 - (-43))));
        public int ScoreGain { get; set; }
        public float ScoreGainNorm => ((float)(ScoreGain - 0)) / (127);
        public int CellCount { get; set; }
        public float PlacementScore { get; set; }
        public int MaxArea { get; set; }

        public float Rank => (float)((ScoreGainNorm * 0) + (CellGainNorm * 0.5) + (PlacementScore * 0.5)); // 675
        //public float Rank => (float)((ScoreGainNorm * 0.69) + (CellGainNorm * 0.3) + (PlacementScore * 0.01)); // 675;62;19 16|6|1|0
        //public float Rank => (float)((ScoraGainNorm * 0.9) + (CellGainNorm * 0.1)); // 209
        //public float Rank => (float)((ScoraGainNorm * 0.8) + (CellGainNorm * 0.2)); // 209

        public IList<Candidate> Moves { get; set; }

        public GamePath()
        {
            Moves = new List<Candidate>();
            PlacementScore = 1;
        }

        public GamePath(GamePath source) : this()
        {
            if (source != null)
            {
                foreach (var candidate in source.Moves)
                    Moves.Add(candidate);

                CellsGain = source.CellsGain;
                ScoreGain = source.ScoreGain;
                CellCount = source.CellCount;
                PlacementScore = source.PlacementScore;
            }
        }
    }
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
                            MaxArea = MaxArea.MaximalRectangle(newBoard.Cells)
                        };
                        candidates.Add(candidate);
                        if (candidate.ScoreGain < 0)
                            throw new Exception("Inconceivable");
                    }
                }
            }

            //var c = from x in candidates orderby x.ScoreGain descending, x.CellsGain, x.LinesScore descending, x.PlacementScore select x;
            var c = from x in candidates orderby x.MaxArea descending select x;

            var w = c.First<Candidate>();

            placement = w.Placement;
            shapeId = w.ShapeId;
        }
    }
}