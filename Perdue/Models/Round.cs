// ============================================================================
//    Author: Kenneth Perkins
//    Date:   May 14, 2021
//    Taken From: http://programmingnotes.org/
//    Code From: https://www.programmingnotes.org/7601/cs-how-to-round-a-number-to-the-nearest-x-using-cs/
//    File:  Utils.cs
//    Description: Handles general utility functions
// ============================================================================
using System;

namespace Utils
{
    public static class Round
    {
        public enum Type
        {
            Nearest,
            Up,
            Down
        }


        /// <summary>
        /// Rounds a number to the nearest X
        /// </summary>
        /// <param name="value">The value to round</param> 
        /// <param name="stepAmount">The amount to round the value by</param>
        /// <param name="type">The type of rounding to perform</param>
        /// <returns>The value rounded by the step amount and type</returns>
        public static decimal Amount(decimal value, decimal stepAmount
                    , Round.Type type = Round.Type.Nearest)
        {
            var inverse = 1 / stepAmount;
            var dividend = value * inverse;
            switch (type)
            {
                case Round.Type.Nearest:
                    dividend = Math.Round(dividend);
                    break;
                case Round.Type.Up:
                    dividend = Math.Ceiling(dividend);
                    break;
                case Round.Type.Down:
                    dividend = Math.Floor(dividend);
                    break;
                default:
                    throw new ArgumentException($"Unknown type: {type}", nameof(type));
            }
            var result = dividend / inverse;
            return result;
        }



        /// <summary>
        /// Rounds a number to the nearest X
        /// </summary>
        /// <param name="value">The value to round</param> 
        /// <param name="stepAmount">The amount to round the value by</param>
        /// <param name="type">The type of rounding to perform</param>
        /// <returns>The value rounded by the step amount and type</returns>
        public static int IntAmount(decimal value, decimal stepAmount
                    , Round.Type type = Round.Type.Nearest)
        {
            var inverse = 1 / stepAmount;
            var dividend = value * inverse;
            switch (type)
            {
                case Round.Type.Nearest:
                    dividend = Math.Round(dividend);
                    break;
                case Round.Type.Up:
                    dividend = Math.Ceiling(dividend);
                    break;
                case Round.Type.Down:
                    dividend = Math.Floor(dividend);
                    break;
                default:
                    throw new ArgumentException($"Unknown type: {type}", nameof(type));
            }
            var result = dividend / inverse;
            return Convert.ToInt32(result);
        }
    }
}