using static Cubing.ThreeByThree.Constants;
using static Cubing.ThreeByThree.Corner;
using static Cubing.ThreeByThree.Edge;
using static Cubing.ThreeByThree.Face;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cubing.ThreeByThree
{
    /// <summary>
    /// A 3x3x3 cube represented by the orientation and the permutation of
    /// its cubies, stored as arrays.
    /// </summary>
    public class CubieCube : IEquatable<CubieCube>
    {
        /// <summary>
        /// The solved corner permutation state. Do not change.
        /// </summary>
        public static readonly Corner[] SolvedCP
            = { URF, UFL, ULB, UBR, DFR, DLF, DBL, DRB };
        /// <summary>
        /// The solved corner orientation state. Do not change.
        /// </summary>
        public static readonly int[] SolvedCO
            = { 0, 0, 0, 0, 0, 0, 0, 0 };
        /// <summary>
        /// The solved edge permutation state. Do not change.
        /// </summary>
        public static readonly Edge[] SolvedEP
            = { UF, UL, UB, UR, DF, DL, DB, DR, FR, FL, BL, BR };
        /// <summary>
        /// The solved edge orientation state. Do not change.
        /// </summary>
        public static readonly int[] SolvedEO
            = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        /// <summary>
        /// The solved center permutation state. Do not change.
        /// </summary>
        public static readonly Face[] SolvedCenters
            = { R, U, F, L, D, B };

        /// <summary>
        /// An array containing all the basic moves.
        /// <c><see cref="MovesArray"/>[(int)m]</c> is a solved cube with
        /// <c>m</c> applied to it, where <c>m</c> is a <see cref="Move"/>.
        /// Do not change.
        /// </summary>
        /// <remarks>
        /// See also <seealso cref="RotationsArray"/> and
        /// <seealso cref="MirrorsArray"/>.
        /// </remarks>
        public static readonly CubieCube[] MovesArray = null;
        /// <summary>
        /// An array containing all the basic rotations.
        /// <c><see cref="RotationsArray"/>[(int)r]</c> is a solved cube with
        /// <c>r</c> applied to it, where <c>r</c> is a <see cref="Rotation"/>.
        /// Do not change.
        /// </summary>
        /// <remarks>
        /// See also <seealso cref="MovesArray"/> and
        /// <seealso cref="MirrorsArray"/>.
        /// </remarks>
        public static readonly CubieCube[] RotationsArray = null;
        /// <summary>
        /// An array containing all the mirrors along a plane parallel to
        /// a face. <c><see cref="MirrorsArray"/>[(int)m]</c> is a solved
        /// cube with a mirror applied along the plane <c>m</c>, where
        /// <c>m</c> is a <see cref="Axis"/>. Do not change.
        /// </summary>
        /// <remarks>
        /// See also <seealso cref="MovesArray"/> and
        /// <seealso cref="RotationsArray"/>.
        /// </remarks>
        public static readonly CubieCube[] MirrorsArray = null;
        
        #region class methods
        #region factory methods
        /// <summary>
        /// Create a solved <see cref="CubieCube"/>.
        /// </summary>
        /// <returns>A new solved <see cref="CubieCube"/></returns>
        public static CubieCube CreateSolved()
            => new CubieCube()
            {
                CP = (Corner[])CubieCube.SolvedCP.Clone(),
                CO = (int[])CubieCube.SolvedCO.Clone(),
                EP = (Edge[])CubieCube.SolvedEP.Clone(),
                EO = (int[])CubieCube.SolvedEO.Clone(),
                Centers = (Face[])CubieCube.SolvedCenters.Clone()
            };

        /// <summary>
        /// Create a <see cref="CubieCube"/> with a specific state.
        /// </summary>
        /// <param name="cp">
        /// The corner permutation state of the cube.
        /// </param>
        /// <param name="co">
        /// The corner orientation state of the cube.
        /// </param>
        /// <param name="ep">
        /// The edge permutation state of the cube.
        /// </param>
        /// <param name="eo">
        /// The edge orientation state of the cube.
        /// </param>
        /// <param name="centers">
        /// The center permutation state of the cube.
        /// </param>
        /// <param name="isMirrored">
        /// Whether the cube is in a mirrored state.
        /// </param>
        /// <returns>A cube with the specified state.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if any of the parameters is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Throw if any of the parameters does not match the expected
        /// length.
        /// </exception>
        public static CubieCube Create(Corner[] cp, int[] co, Edge[] ep,
            int[] eo, Face[] centers, bool isMirrored = false)
        {
            #region parameter checks
            if (cp is null)
                throw new ArgumentNullException(nameof(cp) + " is null.");
            if (co is null)
                throw new ArgumentNullException(nameof(co) + " is null.");
            if (ep is null)
                throw new ArgumentNullException(nameof(ep) + " is null.");
            if (eo is null)
                throw new ArgumentNullException(nameof(eo) + " is null.");
            if (centers is null)
                throw new ArgumentNullException(nameof(centers) + " is null.");
            
            if (cp.Length != NumCorners)
                throw new ArgumentException(
                    nameof(cp) + ".Length must be " + NumCorners
                    + ": " + cp.Length);
            if (co.Length != NumCorners)
                throw new ArgumentException(
                    nameof(co) + ".Length must be " + NumCorners
                    + ": " + co.Length);
            if (ep.Length != NumEdges)
                throw new ArgumentException(
                    nameof(ep) + ".Length must be " + NumEdges
                    + ": " + ep.Length);
            if (eo.Length != NumEdges)
                throw new ArgumentException(
                    nameof(eo) + ".Length must be " + NumEdges
                    + ": " + eo.Length);
            if (centers.Length != NumFaces)
                throw new ArgumentException(
                    nameof(centers) + ".Length must be " + NumFaces
                    + ": " + centers.Length);
            #endregion parameter checks

            return new CubieCube()
            {
                CP = (Corner[])cp.Clone(),
                CO = (int[])co.Clone(),
                EP = (Edge[])ep.Clone(),
                EO = (int[])eo.Clone(),
                Centers = (Face[])centers.Clone(),
                IsMirrored = isMirrored
            };
        }

        /// <summary>
        /// Create a solved <see cref="CubieCube"/> and apply an alg.
        /// </summary>
        /// <param name="alg">The alg to apply</param>
        /// <returns>
        /// A <see cref="CubieCube"/> with <paramref name="alg"/> applied.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="alg"/> is null.
        /// </exception>
        public static CubieCube FromAlg(IEnumerable<Move> alg)
        {
            if (alg is null)
                throw new ArgumentNullException(nameof(alg) + " is null.");

            CubieCube returnValue = CubieCube.CreateSolved();
            returnValue.ApplyAlg(alg);
            return returnValue;
        }
        #endregion factory methods

        /// <summary>
        /// Compare whether two <see cref="CubieCube"/>s are in the same
        /// state.
        /// </summary>
        /// <param name="cube1">The first cube to compare.</param>
        /// <param name="cube2">The second cube to compare.</param>
        /// <returns>
        /// Whether the two cubes are in the same state.
        /// </returns>
        public static bool operator ==(CubieCube cube1, CubieCube cube2)
            => cube1.Equals(cube2);

        /// <summary>
        /// Compare whether two <see cref="CubieCube"/>s are not in the
        /// same state.
        /// </summary>
        /// <param name="cube1">The first cube to compare.</param>
        /// <param name="cube2">The second cube to compare.</param>
        /// <returns>
        /// Whether the two cubes are not in the same state.
        /// </returns>
        public static bool operator !=(CubieCube cube1, CubieCube cube2)
            => cube1.Equals(cube2);
        #endregion class methods

        private static readonly int Cw = 1, Ccw = 2;

        #region the representation of the moves as arrays
        private static readonly Corner[] _r1cp
            = { DFR, UFL, ULB, URF, DRB, DLF, DBL, UBR };
        private static readonly int[] _r1co
            = { Ccw, 0, 0, Cw, Cw, 0, 0, Ccw };
        private static readonly Edge[] _r1ep
            = { UF, UL, UB, FR, DF, DL, DB, BR, DR, FL, BL, UR };
        private static readonly int[] _r1eo
            = CubieCube.SolvedEO;

        private static readonly Corner[] _u1cp
            = { UBR, URF, UFL, ULB, DFR, DLF, DBL, DRB };
        private static readonly int[] _u1co
            = CubieCube.SolvedCO;
        private static readonly Edge[] _u1ep
            = { UR, UF, UL, UB, DF, DL, DB, DR, FR, FL, BL, BR };
        private static readonly int[] _u1eo
            = CubieCube.SolvedEO;

        private static readonly Corner[] _f1cp
            = { UFL, DLF, ULB, UBR, URF, DFR, DBL, DRB };
        private static readonly int[] _f1co
            = { Cw, Ccw, 0, 0, Ccw, Cw, 0, 0 };
        private static readonly Edge[] _f1ep
            = { FL, UL, UB, UR, FR, DL, DB, DR, UF, DF, BL, BR };
        private static readonly int[] _f1eo
            = { 1, 0, 0, 0, 1, 0, 0, 0, 1, 1, 0, 0 };

        private static readonly Corner[] _l1cp
            = { URF, ULB, DBL, UBR, DFR, UFL, DLF, DRB };
        private static readonly int[] _l1co
            = { 0, Cw, Ccw, 0, 0, Ccw, Cw, 0 };
        private static readonly Edge[] _l1ep
            = { UF, BL, UB, UR, DF, FL, DB, DR, FR, UL, DL, BR };
        private static readonly int[] _l1eo
            = CubieCube.SolvedEO;

        private static readonly Corner[] _d1cp
            = { URF, UFL, ULB, UBR, DLF, DBL, DRB, DFR };
        private static readonly int[] _d1co
            = CubieCube.SolvedCO;
        private static readonly Edge[] _d1ep
            = { UF, UL, UB, UR, DL, DB, DR, DF, FR, FL, BL, BR };
        private static readonly int[] _d1eo
            = CubieCube.SolvedEO;

        private static readonly Corner[] _b1cp
            = { URF, UFL, UBR, DRB, DFR, DLF, ULB, DBL };
        private static readonly int[] _b1co
            = { 0, 0, Cw, Ccw, 0, 0, Ccw, Cw };
        private static readonly Edge[] _b1ep
            = { UF, UL, BR, UR, DF, DL, BL, DR, FR, FL, UB, DB };
        private static readonly int[] _b1eo
            = { 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 1 };
        #endregion

        #region the representation of the rotations as arrays
        private static readonly Corner[] _x1cp
            = { DFR, DLF, UFL, URF, DRB, DBL, ULB, UBR };
        private static readonly int[] _x1co
            = { Ccw, Cw, Ccw, Cw, Cw, Ccw, Cw, Ccw };
        private static readonly Edge[] _x1ep
            = { DF, FL, UF, FR, DB, BL, UB, BR, DR, DL, UL, UR };
        private static readonly int[] _x1eo
            = { 1, 0, 1, 0, 1, 0, 1, 0, 0, 0, 0, 0 };
        private static readonly Face[] _x1centers
            = { R, F, D, L, B, U };

        private static readonly Corner[] _y1cp
            = { UBR, URF, UFL, ULB, DRB, DFR, DLF, DBL };
        private static readonly int[] _y1co
            = CubieCube.SolvedCO;
        private static readonly Edge[] _y1ep
            = { UR, UF, UL, UB, DR, DF, DL, DB, BR, FR, FL, BL };
        private static readonly int[] _y1eo
            = { 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1 };
        private static readonly Face[] _y1centers
            = { B, U, R, F, D, L };

        private static readonly Corner[] _z1cp
            = { UFL, DLF, DBL, ULB, URF, DFR, DRB, UBR };
        private static readonly int[] _z1co
            = { Cw, Ccw, Cw, Ccw, Ccw, Cw, Ccw, Cw };
        private static readonly Edge[] _z1ep
            = { FL, DL, BL, UL, FR, DR, BR, UR, UF, DF, DB, UB };
        private static readonly int[] _z1eo
            = { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
        private static readonly Face[] _z1centers
            = { U, L, F, D, R, B };
        #endregion

        #region the representation of the mirrors as arrays
        private static readonly Corner[] _xcp
            = { UFL, URF, UBR, ULB, DLF, DFR, DRB, DBL };
        private static readonly Edge[] _xep
            = { UF, UR, UB, UL, DF, DR, DB, DL, FL, FR, BR, BL };
        private static readonly Face[] _xcenters
            = { L, U, F, R, D, B };

        private static readonly Corner[] _ycp
            = { DFR, DLF, DBL, DRB, URF, UFL, ULB, UBR };
        private static readonly Edge[] _yep
            = { DF, DL, DB, DR, UF, UL, UB, UR, FR, FL, BL, BR };
        private static readonly Face[] _ycenters
            = { R, D, F, L, U, B };

        private static readonly Corner[] _zcp
            = { UBR, ULB, UFL, URF, DRB, DBL, DLF, DFR };
        private static readonly Edge[] _zep
            = { UB, UL, UF, UR, DB, DL, DF, DR, BR, BL, FL, FR };
        private static readonly Face[] _zcenters
            = { R, U, B, L, D, F };
        #endregion

        static CubieCube()
        {
            #region initialize MovesArray
            MovesArray = new CubieCube[NumMoves];

            MovesArray[(int)Move.R1]
                = CubieCube.Create(_r1cp, _r1co, _r1ep, _r1eo, SolvedCenters);
            MovesArray[(int)Move.U1]
                = CubieCube.Create(_u1cp, _u1co, _u1ep, _u1eo, SolvedCenters);
            MovesArray[(int)Move.F1]
                = CubieCube.Create(_f1cp, _f1co, _f1ep, _f1eo, SolvedCenters);
            MovesArray[(int)Move.L1]
                = CubieCube.Create(_l1cp, _l1co, _l1ep, _l1eo, SolvedCenters);
            MovesArray[(int)Move.D1]
                = CubieCube.Create(_d1cp, _d1co, _d1ep, _d1eo, SolvedCenters);
            MovesArray[(int)Move.B1]
                = CubieCube.Create(_b1cp, _b1co, _b1ep, _b1eo, SolvedCenters);

            foreach (Face face in Enum.GetValues(typeof(Face)))
            {
                CubieCube singleMove = MovesArray[(int)face * 3];
                MovesArray[(int)face * 3 + 1] = singleMove.Clone();
                MovesArray[(int)face * 3 + 1].Multiply(singleMove);
                MovesArray[(int)face * 3 + 2]
                    = MovesArray[(int)face * 3 + 1].Clone();
                MovesArray[(int)face * 3 + 2].Multiply(singleMove);
            }
            #endregion initialize MovesArray

            #region initialize RotationsArray
            RotationsArray = new CubieCube[NumRotations];

            RotationsArray[(int)Rotation.x1]
                = CubieCube.Create(_x1cp, _x1co, _x1ep, _x1eo, _x1centers);
            RotationsArray[(int)Rotation.y1]
                = CubieCube.Create(_y1cp, _y1co, _y1ep, _y1eo, _y1centers);
            RotationsArray[(int)Rotation.z1]
                = CubieCube.Create(_z1cp, _z1co, _z1ep, _z1eo, _z1centers);

            foreach (Axis axis in Enum.GetValues(typeof(Axis)))
            {
                CubieCube singleRotation = RotationsArray[(int)axis * 3];
                RotationsArray[(int)axis * 3 + 1] = singleRotation.Clone();
                RotationsArray[(int)axis * 3 + 1].Multiply(singleRotation);
                RotationsArray[(int)axis * 3 + 2] =
                    RotationsArray[(int)axis * 3 + 1].Clone();
                RotationsArray[(int)axis * 3 + 2].Multiply(singleRotation);
            }
            #endregion initialize RotationsArray

            #region initialize MirrorsArray
            MirrorsArray = new CubieCube[NumAxes];
            MirrorsArray[(int)Axis.x]
                = CubieCube.Create(_xcp, SolvedCO, _xep, SolvedEO, _xcenters,
                    isMirrored: true);
            MirrorsArray[(int)Axis.y]
                = CubieCube.Create(_ycp, SolvedCO, _yep, SolvedEO, _ycenters,
                    isMirrored: true);
            MirrorsArray[(int)Axis.z]
                = CubieCube.Create(_zcp, SolvedCO, _zep, SolvedEO, _zcenters,
                    isMirrored: true);
            #endregion initialize MirrorsArray
        }

        #region state
        /// <summary>
        /// Stores the corner permutation state of the cube.
        /// </summary>
        public Corner[] CP { get; set; } = null;
        /// <summary>
        /// Stores the corner orientation state of the cube.
        /// </summary>
        public int[] CO { get; set; } = null;
        /// <summary>
        /// Stores the edge permutation state of the cube.
        /// </summary>
        public Edge[] EP { get; set; } = null;
        /// <summary>
        /// Stores the edge orientation state of the cube.
        /// </summary>
        public int[] EO { get; set; } = null;
        /// <summary>
        /// Stores the center permutation state of the cube.
        /// </summary>
        public Face[] Centers { get; set; } = null;
        /// <summary>
        /// Whether the cube is in a mirrored state.
        /// </summary>
        public bool IsMirrored { get; set; } = false;
        #endregion state

        private CubieCube() { }

        #region methods
        /// <summary>
        /// Multiply this cube with another cube.
        /// </summary>
        /// <param name="cube">The cube to multiply with.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="cube"/> is null.
        /// </exception>
        public void Multiply(CubieCube cube)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");
            this.MultiplyCorners(cube);
            this.MultiplyEdges(cube);
            this.MultiplyCenters(cube);
        }

        /// <summary>
        /// Multiply this cube with another cube's corners.
        /// </summary>
        /// <param name="cube">The cube to multiply with.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="cube"/> is null.
        /// </exception>
        public void MultiplyCorners(CubieCube cube)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");
            Corner[] newCP = new Corner[NumCorners];
            int[] newCO = new int[NumCorners];
            for (int corner = 0; corner < NumCorners; corner++)
            {
                newCP[corner] = this.CP[(int)cube.CP[corner]];
                newCO[corner] = this.CO[(int)cube.CP[corner]];
                //TODO understand
                if (this.IsMirrored)
                    newCO[corner] = (3 + newCO[corner] - cube.CO[corner]) % 3;
                else
                    newCO[corner] = (newCO[corner] + cube.CO[corner]) % 3;
            }
            this.IsMirrored ^= cube.IsMirrored;
            this.CP = newCP;
            this.CO = newCO;
        }

        /// <summary>
        /// Multiply this cube with another cube's edges.
        /// </summary>
        /// <param name="cube">The cube to multiply with.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="cube"/> is null.
        /// </exception>
        public void MultiplyEdges(CubieCube cube)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");
            Edge[] newEP = new Edge[NumEdges];
            int[] newEO = new int[NumEdges];
            for (int edge = 0; edge < NumEdges; edge++)
            {
                newEP[edge] = this.EP[(int)cube.EP[edge]];
                newEO[edge] = this.EO[(int)cube.EP[edge]];
                newEO[edge] = (newEO[edge] + cube.EO[edge]) % 2;
            }
            this.EP = newEP;
            this.EO = newEO;
        }

        /// <summary>
        /// Multiply this cube with another cube's centers.
        /// </summary>
        /// <param name="cube">The cube to multiply with.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="cube"/> is null.
        /// </exception>
        public void MultiplyCenters(CubieCube cube)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");

            Face[] newCenters = new Face[NumFaces];
            for (int face = 0; face < NumFaces; face++)
                newCenters[face] = this.Centers[(int)cube.Centers[face]];
            this.Centers = newCenters;
        }

        /// <summary>
        /// Apply a move to the cube.
        /// </summary>
        /// <remarks>
        /// See <seealso cref="ApplyAlg(IEnumerable{Move})"/> for
        /// applying an alg to the cube.
        /// </remarks>
        /// <param name="move">The move to apply.</param>
        public void ApplyMove(Move move)
            => this.Multiply(MovesArray[(int)move]);

        /// <summary>
        /// Apply a rotation to the cube.
        /// </summary>
        /// <param name="rotation">The rotation to apply.</param>
        public void Rotate(Rotation rotation)
            => this.Multiply(RotationsArray[(int)rotation]);
        
        /// <summary>
        /// Mirror the cube.
        /// </summary>
        /// <param name="axis">The plane to mirror the cube at.</param>
        public void Mirror(Axis axis)
            => this.Multiply(MirrorsArray[(int)axis]);

        /// <summary>
        /// Apply an alg to the cube.
        /// </summary>
        /// <remarks>
        /// See <seealso cref="ApplyMove(Move)"/> for
        /// applying a single move.
        /// </remarks>
        /// <param name="moves">The moves to apply.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="moves"/> is null.
        /// </exception>
        public void ApplyAlg(IEnumerable<Move> moves)
        {
            if (moves is null)
                throw new ArgumentNullException(nameof(moves) + " is null.");
            
            foreach (Move move in moves) this.ApplyMove(move);
        }

        /// <summary>
        /// Compare whether this cube is in the same state as another
        /// cube.
        /// </summary>
        /// <param name="other">The cube to compare with.</param>
        /// <returns>
        /// Whether this cube is in the same state as another cube.
        /// </returns>
        public bool Equals(CubieCube other)
        {
            if (other is null)
                return false;
            return Enumerable.SequenceEqual(this.CP, other.CP) &&
                   Enumerable.SequenceEqual(this.CO, other.CO) &&
                   Enumerable.SequenceEqual(this.EP, other.EP) &&
                   Enumerable.SequenceEqual(this.EO, other.EO) &&
                   Enumerable.SequenceEqual(this.Centers, other.Centers) &&
                   this.IsMirrored == other.IsMirrored;
        }

        /// <summary>
        /// Compare whether this cube is equal to another object.
        /// </summary>
        /// <param name="obj">The object to compare with.</param>
        /// <returns>
        /// Whether this cube is equals to another object.
        /// </returns>
        public override bool Equals(object obj)
            => this.Equals(obj as CubieCube);

        /// <summary>
        /// Not implemented. Throws a
        /// <see cref="NotImplementedException"/> when called.
        /// </summary>
        /// <returns>
        /// Throws a <see cref="NotImplementedException"/>
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// Thrown in any case.
        /// </exception>
        public override int GetHashCode()
            => throw new NotImplementedException(
                "This object is not supposed to be used as key.");

        /// <inheritdoc/>
        public override string ToString()
        {
            return
                "CP: " + string.Join(" ", this.CP) + ", " +
                "CO: " + string.Join(" ", this.CO) + ", " +
                "EP: " + string.Join(" ", this.EP) + ", " +
                "EO: " + string.Join(" ", this.EO) + ", " +
                "Centers: " + string.Join(" ", this.Centers);
        }

        /// <summary>
        /// Create a deep copy of this object.
        /// </summary>
        /// <returns>A deep copy of this object.</returns>
        public CubieCube Clone()
            => new CubieCube()
            {
                CP = (Corner[])this.CP.Clone(),
                CO = (int[])this.CO.Clone(),
                EP = (Edge[])this.EP.Clone(),
                EO = (int[])this.EO.Clone(),
                Centers = (Face[])this.Centers.Clone(),
                IsMirrored = this.IsMirrored
            };
        #endregion methods
    }
}