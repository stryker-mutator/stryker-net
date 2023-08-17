using FSharp.Compiler.Text;

namespace Stryker.Core.Helpers
{
    internal class RangeHelper
    {
        internal static Position GetPosition(string text, int index)
        {
            var line = 0;
            var col = 0;

            for (var i = 0; i < System.Math.Min(index, text.Length); i++)
            {
                if (text[i] == '\n')
                {
                    line++;
                    col = 0;
                }
                else
                {
                    col++;
                }
            }

            return PositionModule.mkPos(line, col);
        }

        internal static int GetIndex(string text, Position pos)
        {
            var line = 0;
            var col = 0;

            for (var i = 0; i < text.Length; i++)
            {
                if (line == pos.Line && col == pos.Column)
                {
                    return i;
                }

                if (text[i] == '\n')
                {
                    line++;
                    col = 0;
                }
                else
                {
                    col++;
                }
            }

            return -1;
        }

        public static Range FromBounds(string filePath, string text, int startIndex, int endIndex)
        {
            var startPos = GetPosition(text, startIndex);
            var endPos = GetPosition(text, endIndex);
            return RangeModule.mkRange(filePath, startPos, endPos);
        }
    }
}
