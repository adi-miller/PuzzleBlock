namespace ConsoleApp1.Utils
{
    public static class Fragmentation
    {
        public static float GetFragmentationScore(bool[][] matrix)
        {
            var frags = 0;
            for (int i = 0; i < matrix.Length; i++)
            {
                var iHold = false;
                var jHold = false;
                for (int j = 0; j < matrix[i].Length; j++)
                {
                    if (iHold != matrix[i][j])
                        frags++;
                    iHold = matrix[i][j];
                    if (jHold != matrix[j][i])
                        frags++;
                    jHold = matrix[j][i];
                }
            }

            return 1-(float)frags/120;
        }

        public static int SurroundedSignals(bool[][] matrix)
        {
            int res = 0;
            for (int i = 0; i < matrix.Length; i++)
                for (int j = 0; j < matrix[i].Length; j++)
                    if (Surrounded(matrix, i, j))
                        res++;

            return res;
        }

        private static bool Surrounded(bool[][] matrix, int x, int y)
        {
            return 
                (x == 0 || matrix[x - 1][y]) && 
                (x == matrix.Length-1 || matrix[x + 1][y]) && 
                (y == 0 || matrix[x][y - 1]) && 
                (y == matrix[x].Length-1 || matrix[x][y + 1]);
        }
    }
}
