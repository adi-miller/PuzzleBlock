using System;
using System.Collections.Generic;

namespace PuzzleBlock
{
    /// <summary>
    /// 
    /// </summary>
    public static class MaxArea
    {
        public static int MaximalRectangle(bool[][] matrix)
        {
            int m = matrix.Length;
            if (m == 0)
                return 0;
            int n = matrix[0].Length;
            int maxArea = 0;
            int[] aux = new int[n];
            for (int i = 0; i < n; i++)
            {
                aux[i] = 0;
            }
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    aux[j] = matrix[i][j] ? aux[j] = 0 : aux[j] + (matrix[i][j] ? 0 : 1);
                }
                maxArea = Math.Max(maxArea, MaxAreaHist(aux));
            }
            return maxArea;
        }

        public static int MaxAreaHist(int[] heights)
        {
            int n = heights.Length;
            Stack<int> stack = new Stack<int>();
            stack.Push(0);
            int maxRect = heights[0];
            int top = 0;
            int leftSideArea = 0;
            int rightSideArea = heights[0];
            for (int i = 1; i < n; i++)
            {
                if (stack.Count == 0 || heights[i] >= heights[stack.Peek()])
                {
                    stack.Push(i);
                }
                else
                {
                    while (stack.Count != 0 && heights[stack.Peek()] > heights[i])
                    {
                        top = stack.Pop();
                        rightSideArea = heights[top] * (i - top);
                        leftSideArea = 0;
                        if (stack.Count != 0)
                        {
                            leftSideArea = heights[top] * (top - stack.Peek() - 1);
                        }
                        else
                        {
                            leftSideArea = heights[top] * top;
                        }
                        maxRect = Math.Max(maxRect, leftSideArea + rightSideArea);
                    }
                    stack.Push(i);
                }
            }
            while (stack.Count != 0)
            {
                top = stack.Pop();
                rightSideArea = heights[top] * (n - top);
                leftSideArea = 0;
                if (stack.Count != 0)
                {
                    leftSideArea = heights[top] * (top - stack.Peek() - 1);
                }
                else
                {
                    leftSideArea = heights[top] * top;
                }
                maxRect = Math.Max(maxRect, leftSideArea + rightSideArea);
            }
            return maxRect;
        }

    }
}
