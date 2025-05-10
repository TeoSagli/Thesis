using System;
using System.Collections.Generic;


[Serializable]
public class DataRoot
{
    public List<PuzzleData> puzzleData;
    public List<PuzzleData2D> puzzleData2D;
    public List<PuzzleData3D> puzzleData3D;
    public List<QuizData> quizData;
}