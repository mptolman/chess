using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UvsChess;

namespace StudentAI
{
    interface IHeuristic
    {
        int GetMoveValue(ChessBoard boardAfterMove, ChessMove move, ChessColor myColor);
    }
}
