using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UvsChess;

namespace StudentAI
{
    class DummyHeuristic : IHeuristic
    {
        public int GetMoveValue(ChessBoard boardAfterMove, ChessColor myColor)
        {
            return 1;
        }
    }
}
