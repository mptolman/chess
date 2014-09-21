using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UvsChess;

namespace StudentAI.Heuristics
{
    internal class DummyHeuristic : IHeuristic
    {
        public int GetMoveValue(ChessBoard boardAfterMove, ChessMove move, ChessColor myColor)
        {
            return 1;
        }
    }
}
