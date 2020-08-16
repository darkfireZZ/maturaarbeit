using System;
using System.IO;

namespace Cubing.ThreeByThree.TwoPhase
{
    /// <summary>
    /// Stores all tables required by the two-phase solver.
    /// </summary>
    public static class TableController
    {
        /// <summary>
        /// The directory where the tables are saved.
        /// </summary>
        public static string Directory { get; set; } = @"C:\ProgramData\nbCubeData";

        /// <summary>
        /// The corner orientation move table.
        /// </summary>
        public static short[,] CornerOrientationMoveTable { get; set; } = null;
        /// <summary>
        /// The edge orientation move table.
        /// </summary>
        public static short[,] EdgeOrientationMoveTable { get; set; } = null;
        /// <summary>
        /// The equator distribution move table.
        /// </summary>
        public static short[,] EquatorDistributionMoveTable { get; set; } = null;
        /// <summary>
        /// The equator order move table.
        /// </summary>
        public static sbyte[,] EquatorOrderMoveTable { get; set; } = null;
        /// <summary>
        /// The equator permutation move table.
        /// </summary>
        public static short[,] EquatorPermutationMoveTable { get; set; } = null;
        /// <summary>
        /// The U-edge permutation move table.
        /// </summary>
        public static short[,] UEdgePermutationMoveTable { get; set; } = null;
        /// <summary>
        /// The D-edge permutation move table.
        /// </summary>
        public static short[,] DEdgePermutationMoveTable { get; set; } = null;
        /// <summary>
        /// The corner permutation move table.
        /// </summary>
        public static ushort[,] CornerPermutationMoveTable { get; set; } = null;
        /// <summary>
        /// The U- and D-edge order move table.
        /// </summary>
        public static ushort[,] UdEdgeOrderMoveTable { get; set; } = null;

        /// <summary>
        /// The pruning table for phase 1.
        /// </summary>
        public static byte[] Phase1PruningTable { get; set; } = null;

        /// <summary>
        /// The corner permutation and equator order pruning table for phase 2.
        /// </summary>
        public static byte[] Phase2CornerEquatorPruningTable { get; set; } = null;

        /// <summary>
        /// The corner permutation and U- and D-edge permutation pruning table for phase 2.
        /// </summary>
        public static byte[] Phase2CornerUdPruningTable { get; set; } = null;

        /// <summary>
        /// Initialize the corner orientation move table.
        /// </summary>
        public static void InitializeCornerOrientationMoveTable()
        {
            if (CornerOrientationMoveTable is null)
            {
                Console.WriteLine("Initializing corner orientation move table");
                CornerOrientationMoveTable = MoveTables.CreateCornerOrientationMoveTable();
            }
        }

        /// <summary>
        /// Initialize the edge orientation move table.
        /// </summary>
        public static void InitializeEdgeOrientationMoveTable()
        {
            if (EdgeOrientationMoveTable is null)
            {
                Console.WriteLine("Initializing edge orientation move table");
                EdgeOrientationMoveTable = MoveTables.CreateEdgeOrientationMoveTable();
            }
        }

        /// <summary>
        /// Initialize the equator distribution move table.
        /// </summary>
        public static void InitializeEquatorDistributionMoveTable()
        {
            if (EquatorDistributionMoveTable is null)
            {
                Console.WriteLine("Initializing equator distribution move table");
                EquatorDistributionMoveTable = MoveTables.CreateEquatorDistributionMoveTable();
            }
        }

        /// <summary>
        /// Initialize the equator order move table.
        /// </summary>
        public static void InitializeEquatorOrderMoveTable()
        {
            if (EquatorOrderMoveTable is null)
            {
                Console.WriteLine("Initializing equator order move table");
                EquatorOrderMoveTable = MoveTables.CreateEquatorOrderMoveTable();
            }
        }

        /// <summary>
        /// Initialize the equator permutation move table.
        /// </summary>
        public static void InitializeEquatorPermutationMoveTable()
        {
            if (EquatorPermutationMoveTable is null)
            {
                Console.WriteLine("Initializing equator permutation move table");
                EquatorPermutationMoveTable = MoveTables.CreateEquatorPermutationMoveTable();
            }
        }

        /// <summary>
        /// Initialize the U-edge permutation move table.
        /// </summary>
        public static void InitializeUEdgePermutationMoveTable()
        {
            if (UEdgePermutationMoveTable is null)
            {
                Console.WriteLine("Initializing U-edge permutation move table");
                UEdgePermutationMoveTable = MoveTables.CreateUEdgePermutationMoveTable();
            }
        }

        /// <summary>
        /// Initialize the D-edge permutation move table.
        /// </summary>
        public static void InitializeDEdgePermutationMoveTable()
        {
            if (DEdgePermutationMoveTable is null)
            {
                Console.WriteLine("Initializing D-edge permutation move table");
                DEdgePermutationMoveTable = MoveTables.CreateDEdgePermutationMoveTable();
            }
        }

        /// <summary>
        /// Initialize the corner permutation move table.
        /// </summary>
        public static void InitializeCornerPermutationMoveTable()
        {
            if (CornerPermutationMoveTable is null)
            {
                Console.WriteLine("Initializing corner permutation move table");
                CornerPermutationMoveTable = MoveTables.CreateCornerPermutationMoveTable();
            }
        }

        /// <summary>
        /// Initialize the U- and D-edge order move table.
        /// </summary>
        public static void InitializeUdEdgeOrderMoveTable()
        {
            if (UdEdgeOrderMoveTable is null)
            {
                Console.WriteLine("Initializing U- and D-edge order move table");
                UdEdgeOrderMoveTable = MoveTables.CreateUdEdgeOrderMoveTable();
            }
        }

        //TODO add exception handling
        /// <summary>
        /// Initialize the pruning table for phase 1.
        /// </summary>
        public static void InitializePhase1PruningTable()
        {
            if (Phase1PruningTable is null)
            {
                string file = Directory + @"\phase1.pruningtable";
                if (File.Exists(file))
                {
                    Console.WriteLine("Loading phase 1 pruning table");
                    Phase1PruningTable = File.ReadAllBytes(file);
                }
                else
                {
                    Console.WriteLine("Initializing phase 1 pruning table");
                    Phase1PruningTable = PruningTables.CreatePhase1Table();
                    File.WriteAllBytes(file, Phase1PruningTable);
                }
            }
        }

        //TODO add exception handling
        /// <summary>
        /// Initialize the corner permutation and equator order pruning table
        /// for phase 2.
        /// </summary>
        public static void InitializePhase2CornerEquatorPruningTable()
        {
            if (Phase2CornerEquatorPruningTable is null)
            {
                string file = Directory + @"\phase2CornerEquator.pruningtable";
                if (File.Exists(file))
                {
                    Console.WriteLine("Loading phase 2 corner permutation and equator order pruning table");
                    Phase2CornerEquatorPruningTable = File.ReadAllBytes(file);
                }
                else
                {
                    Console.WriteLine("Initializing phase 2 corner permutation and equator order pruning table");
                    Phase2CornerEquatorPruningTable = PruningTables.CreatePhase2CornerEquatorTable();
                    File.WriteAllBytes(file, Phase2CornerEquatorPruningTable);
                }
            }
        }

        //TODO handle exceptions
        /// <summary>
        /// Initialize the corner permutation and equator permutation pruning
        /// table for phase 2.
        /// </summary>
        public static void InitializePhase2CornerUdPruningTable()
        {
            if (Phase2CornerUdPruningTable is null)
            {
                string file = Directory + @"\phase2CornerUd.pruningtable";
                if (File.Exists(file))
                {
                    Console.WriteLine("Loading phase 2 corner permutation and U- and D-edge permutation pruning table");
                    Phase2CornerUdPruningTable = File.ReadAllBytes(file);
                }
                else
                {
                    Console.WriteLine("Initializing phase 2 corner permutation and U- and D-edge permutation pruning table");
                    Phase2CornerUdPruningTable = PruningTables.CreatePhase2CornerUdTable();
                    File.WriteAllBytes(file, Phase2CornerUdPruningTable);
                }
            }
        }
    }
}