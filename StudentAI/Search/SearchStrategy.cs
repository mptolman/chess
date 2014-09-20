using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UvsChess;

namespace StudentAI
{
    abstract class SearchStrategy
    {
        protected MoveGenerator _moveGenerator;

        public SearchStrategy(MoveGenerator moveGenerator)
        {
            _moveGenerator = moveGenerator;
        }

        public ChessMove GetNextMove(ChessBoard board, ChessColor myColor)
        {
            ChessMove nextMove = null;

            var allMoves = _moveGenerator.GetAllMoves(board, myColor);

            if (allMoves.Count == 0)
            {
                // This must be stalemate.
                // Technically, we would need to make sure we're not in check before declaring stalemate,
                // but if we were in check, then the opponent would have flagged checkmate on their last move.
                // If they didn't set the checkmate flag, then our move validator would have caught their error.
                nextMove = new ChessMove(null, null);
                nextMove.Flag = ChessFlag.Stalemate;
            }
            else
            {
                nextMove = SelectFromAvailableMoves(board, myColor, allMoves);   
            }

            return nextMove;
        }

        protected abstract ChessMove SelectFromAvailableMoves(ChessBoard board, ChessColor myColor, IList<ChessMove> moves);
    }
}
