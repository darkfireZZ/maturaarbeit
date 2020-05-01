namespace Cubing.ThreeByThree
{
    /// <summary>
    /// Contains constants related to the 3x3x3 cube.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// The number of corner pieces of a 3x3x3.
        /// </summary>
        public static int NumCorners = 8;
        /// <summary>
        /// The number of edge pieces of a 3x3x3.
        /// </summary>
        public static int NumEdges = 12;
        /// <summary>
        /// The number of center pieces of a 3x3x3.
        /// </summary>
        public static int NumCenters = 6;

        /// <summary>
        /// The number of faces of a 3x3x3.
        /// </summary>
        public static int NumFaces = 6;
        /// <summary>
        /// The number of axes of a 3x3x3.
        /// </summary>
        public static int NumAxes = 3;
        /// <summary>
        /// The number of different moves in half turn metric (HTM).
        /// </summary>
        public static int NumMoves = 18;
        /// <summary>
        /// The number of different rotations in half turn metric (HTM).
        /// </summary>
        public static int NumRotations = 9;
    }
}