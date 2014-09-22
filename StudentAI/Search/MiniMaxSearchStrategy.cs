using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UvsChess;
using StudentAI.Heuristics;

namespace StudentAI.Search
{
    internal class MiniMaxSearchStrategy : SearchStrategy
    {
        public MiniMaxSearchStrategy(IChessAI ai, IHeuristic heuristic) : base(ai, heuristic)
        { }

        protected override ChessMove SelectFromAvailableMoves(ChessBoard board, ChessColor myColor, IList<ChessMove> moves)
        {
            throw new NotImplementedException();
        }
    }
}
