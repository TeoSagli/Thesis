using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    [System.Serializable]
    public class PuzzleData
    {
        private float pieceScale = 0.05f;
        private string titleStr;
        private int nCols = 1;
        private int nRows = 1;
        private int nDepth = 1;
    [JsonConstructor]
    public PuzzleData(float pieceScale, string titleStr, int nCols, int nRows, int nDepth)
        {
            PieceScale = pieceScale;
            TitleStr = titleStr;
            NCols = nCols;
            NRows = nRows;
            NDepth = nDepth;
        }
        public PuzzleData(float pieceScale, string titleStr, int nCols, int nRows)
        {
            PieceScale = pieceScale;
            TitleStr = titleStr;
            NCols = nCols;
            NRows = nRows;
        }
        public float PieceScale { get => pieceScale; set => pieceScale = value; }
        public string TitleStr { get => titleStr; set => titleStr = value; }
        public int NCols { get => nCols; set => nCols = value; }
        public int NRows { get => nRows; set => nRows = value; }
        public int NDepth { get => nDepth; set => nDepth = value; }
    }

