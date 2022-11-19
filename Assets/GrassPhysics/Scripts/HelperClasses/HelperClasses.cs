using UnityEngine;

namespace ShadedTechnology.GrassPhysics
{
    /// <summary>
    /// Stores global constants
    /// </summary>
    public static class GlobalConstants
    {
        /// <summary>
        /// Constant grass depth camera movement step size in pixels
        /// </summary>
        public const int STEP_PX = 32;

        /// <summary>
        /// Max count of grass actors in Simple Physics Mode (You can change this value but after that I recommend you to restart Unity)
        /// </summary>
        public const int MAX_GRASS_ACTORS = 10;
    }

    /// <summary>
    /// Contains useful math functions
    /// </summary>
    public static class MathHelper
    {
        /// <summary>
        /// Divides two floats and returns floor of result as integer
        /// </summary>
        /// <param name="a">Numerator</param>
        /// <param name="b">Denominator</param>
        /// <returns>Floor of division result in int</returns>
        public static int FloorDivide(float a, float b)
        {
            return (Mathf.FloorToInt(a / b));
        }

        public static Vector3Int FloorDivide(Vector3 a, float b)
        {
            return new Vector3Int(FloorDivide(a.x, b), FloorDivide(a.y, b), FloorDivide(a.z, b));
        }

        /// <summary>
        /// Positive modulo
        /// </summary>
        public static float PosMod(float a, float b)
        {
            return ((a % b + b) % b);
        }

        public static Vector3 PosMod(Vector3 a, float b)
        {
            return new Vector3(PosMod(a.x, b), PosMod(a.y, b), PosMod(a.z, b));
        }
    }
}

