namespace ConsoleApp1.Utils
{
    public static class Fragmentation
    {
        public static float GetFragmentationScore(bool[][] matrix)
        {
            var iHold = false;
            var jHold = false;
            var frags = 0;
            for (int i = 0; i < matrix.Length; i++)
                for (int j = 0; j < matrix[i].Length; j++)
                {
                    if (iHold != matrix[i][j])
                        frags++;
                    iHold = matrix[i][j];
                    if (jHold != matrix[j][i])
                        frags++;
                    jHold = matrix[j][i];
                }

            return 1-(float)frags/114;
        }
    }
}
