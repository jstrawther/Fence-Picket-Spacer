using System;
using System.Collections.Generic;
using System.Linq;

namespace FenceSpacing
{
    /// <summary>
    /// Calculate the optimal spacing for fence pickets, given:
    /// - Total span width
    /// - Picket board width
    /// - Target spacing 
    /// </summary>
    class Program
    {
        static readonly decimal BOARD_WIDTH = 3.5M;
        //static readonly decimal TARGET_SPACING = 1.25M;
        static readonly decimal TARGET_SPACING = 1.35M;

        static void Main(string[] args)
        {
            string[] lines = System.IO.File.ReadAllLines(@"fence-spacing-input.txt");
            foreach(var line in lines)
            {
                decimal totalWidth = decimal.Parse(line);
                var result = FindOptimalSpacing(totalWidth, TARGET_SPACING, BOARD_WIDTH);
                Console.WriteLine($"{totalWidth}|{result.optimalSpacing}|{result.numberOfPickets}|{result.optimalSpacingWithoutGoingUnder}|{result.numPicketsWithoutGoingUnder}");
            }

            Console.Read();
        }

        static (int numberOfPickets, decimal optimalSpacing, int numPicketsWithoutGoingUnder, decimal optimalSpacingWithoutGoingUnder)  FindOptimalSpacing(decimal totalWidth, decimal targetSpacing, decimal boardWidth)
        {
            Dictionary<int, (decimal spacing, decimal distanceFromTargetSpacing)> spacingByNumberOfPickets = new Dictionary<int, (decimal spacing, decimal distanceFromTargetSpacing)>();
            decimal currentSpacing = totalWidth;
            int numPickets = 1;
            while(currentSpacing > targetSpacing)
            {
                currentSpacing = Spacing(totalWidth, numPickets, boardWidth);
                var distanceFromTargetSpacing = Math.Abs(currentSpacing - targetSpacing);
                spacingByNumberOfPickets.Add(numPickets, (currentSpacing, distanceFromTargetSpacing));
                numPickets++;
            }

            var minDistance = spacingByNumberOfPickets.Min(x => x.Value.distanceFromTargetSpacing);
            var minItem = spacingByNumberOfPickets.First(x => x.Value.distanceFromTargetSpacing == minDistance);

            var minDistanceWithoutGoingUnder = spacingByNumberOfPickets.Where(x => x.Value.spacing >= targetSpacing).Min(x => x.Value.distanceFromTargetSpacing);
            var minItemWithoutGoingUnder = spacingByNumberOfPickets.First(x => x.Value.distanceFromTargetSpacing == minDistanceWithoutGoingUnder);

            return (minItem.Key, minItem.Value.spacing, minItemWithoutGoingUnder.Key, minItemWithoutGoingUnder.Value.spacing);
        }

        static decimal Spacing(decimal totalWidth, int numPickets, decimal boardWidth)
        {
            return (totalWidth - (numPickets * boardWidth)) / (numPickets + 1);
        }
    }
}
