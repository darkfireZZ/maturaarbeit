namespace Cubing.ThreeByThree
{
    /// <summary>
    /// Contains constants related to the 3x3x3 cube.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// The number of corner pieces.
        /// </summary>
        public const int NumCorners = 8;
        /// <summary>
        /// The number of edge pieces.
        /// </summary>
        public const int NumEdges = 12;
        /// <summary>
        /// The number of center pieces
        /// </summary>
        public const int NumCenters = 6;

        /// <summary>
        /// The number of edge pieces in the U layer.
        /// </summary>
        public const int NumUEdges = 4;
        /// <summary>
        /// The number of edge pieces in the D layer.
        /// </summary>
        public const int NumDEdges = 4;
        /// <summary>
        /// The number of edge pieces in the equator layer.
        /// </summary>
        public const int NumEquatorEdges = 4;
        /// <summary>
        /// The number of edges in the U and D layers.
        /// </summary>
        public const int NumUdEdges = NumUEdges + NumDEdges;

        /// <summary>
        /// The number of faces.
        /// </summary>
        public const int NumFaces = 6;
        /// <summary>
        /// The number of axes.
        /// </summary>
        public const int NumAxes = 3;
        /// <summary>
        /// The number of different moves in half turn metric (HTM).
        /// </summary>
        public const int NumMoves = 18;
        /// <summary>
        /// The number of different rotations in half turn metric (HTM).
        /// </summary>
        public const int NumRotations = 9;
    }
}