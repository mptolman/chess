using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UvsChess;

namespace StudentAI
{
    internal class MiniMaxSearchStrategy : SearchStrategy
    {
        public MiniMaxSearchStrategy(IChessAI ai, MoveGenerator moveGenerator) : base(ai, moveGenerator)
        { }

        protected override ChessMove SelectFromAvailableMoves(ChessBoard board, ChessColor myColor, IList<ChessMove> moves)
        {
            throw new NotImplementedException();
        }
    }
}
