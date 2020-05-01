using System;
using System.IO;

namespace Cubing.ThreeByThree.TwoPhase
{
    /// <summary>
    /// Stores all the frequently used tables for the two-phase solver.
    /// </summary>
    public static class TableController
    {
        /// <summary>
        /// The directory where the tables are saved.
        /// </summary>
        public static string Directory { get; set; } = @"C:\ProgramData\nbCubeLib";

        /// <summary>
        /// The edge orientation move table.
        /// </summary>
        public static short[,] EoMoveTable { get; set; } = null;
        /// <summary>
        /// The corner orientation move table.
        /// </summary>
        public static short[,] CoMoveTable { get; set; } = null;
        /// <summary>
        /// The equator distribution move table.
        /// </summary>
        public static short[,] EquatorDistributionMoveTable { get; set; } = null;
        /// <summary>
        /// The equator permutation move table.
        /// </summary>
        public static sbyte[,] EquatorPermutationMoveTable { get; set; } = null;
        /// <summary>
        /// The corner permutation move table.
        /// </summary>
        public static ushort[,] CpMoveTable { get; set; } = null;
        /// <summary>
        /// The U and D edge permutation move table.
        /// </summary>
        public static ushort[,] UdEdgePermutationMoveTable { get; set; } = null;

        /// <summary>
        /// The pruning table for phase 1.
        /// </summary>
        public static byte[] Phase1PruningTable { get; set; } = null;

        /// <summary>
        /// The pruning table for phase 2.
        /// </summary>
        public static byte[] Phase2PruningTable { get; set; } = null;

        /// <summary>
        /// Initialize the corner orientation move table for phase 1.
        /// </summary>
        public static void InitializeCoMoveTable()
        {
            if (CoMoveTable is null)
            {
                Console.WriteLine("Initializing corner orientation move table");
                CoMoveTable = MoveTables.CreateCoMoveTable();
            }
        }

        /// <summary>
        /// Initialize the edge orientation move table for phase 1.
        /// </summary>
        public static void InitializeEoMoveTable()
        {
            if (EoMoveTable is null)
            {
                Console.WriteLine("Initializing edge orientation move table");
                EoMoveTable = MoveTables.CreateEoMoveTable();
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
        /// Initialize the corner permutation move table.
        /// </summary>
        public static void InitializeCpMoveTable()
        {
            if (CpMoveTable is null)
            {
                Console.WriteLine("Initializing corner permutation move table");
                CpMoveTable = MoveTables.CreateCpMoveTable();
            }
        }

        /// <summary>
        /// Initialize the U and D edge permutation table.
        /// </summary>
        public static void InitializeUdEdgePermutationMoveTable()
        {
            if (UdEdgePermutationMoveTable is null)
            {
                Console.WriteLine("Initializing U and D edge permutation move table");
                UdEdgePermutationMoveTable = MoveTables.CreateUdEdgePermutationMoveTable();
            }
        }

        /// <summary>
        /// Initialize the move tables for phase 1.
        /// </summary>
        public static void InitializePhase1MoveTables()
        {
            InitializeCoMoveTable();
            InitializeEoMoveTable();
            InitializeEquatorDistributionMoveTable();
        }

        /// <summary>
        /// Initialize the move tables for phase 2.
        /// </summary>
        public static void InitializePhase2MoveTables()
        {
            InitializeCpMoveTable();
            InitializeUdEdgePermutationMoveTable();
        }

        //TODO handle exceptions
        /// <summary>
        /// Initialize the pruning table for phase 1.
        /// </summary>
        public static void InitializePhase1PruningTable()
        {
            if (Phase1PruningTable is null)
            {
                string file = Directory + @"\phase1pruning.table";
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

        //TODO handle exceptions
        /// <summary>
        /// Initialize the pruning table for phase 2.
        /// </summary>
        public static void InitializePhase2PruningTable()
        {
            if (Phase2PruningTable is null)
            {
                string file = Directory + @"\phase2pruning.table";
                if (File.Exists(file))
                {
                    Console.WriteLine("Loading phase 2 pruning table");
                    Phase2PruningTable = File.ReadAllBytes(file);
                }
                else
                {
                    Console.WriteLine("Initializing phase 2 pruning table");
                    Phase2PruningTable = PruningTables.CreatePhase2Table();
                    File.WriteAllBytes(file, Phase2PruningTable);
                }
            }
        }

        //IMPR documentation
        /// <summary>
        /// Initialize all frequently used tables for phase 1.
        /// </summary>
        public static void InitializePhase1Tables()
        {
            InitializePhase1MoveTables();
            InitializePhase1PruningTable();
        }

        //IMPR documentation
        /// <summary>
        /// Initialize all frequently used tables for phase 2.
        /// </summary>
        public static void InitializePhase2Tables()
        {
            InitializePhase2MoveTables();
            InitializePhase2PruningTable();
        }
    }
}