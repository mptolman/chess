using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UvsChess;

namespace StudentAI
{
    class GreedySearchStrategy : SearchStrategy
    {
        public GreedySearchStrategy(MoveGenerator moveGenerator) : base(moveGenerator)
        { }

        protected override ChessMove SelectFromAvailableMoves(ChessBoard board, ChessColor myColor, IList<ChessMove> moves)
        {
            throw new NotImplementedException();
        }
    }
}
