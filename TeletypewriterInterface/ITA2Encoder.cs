
using System.Diagnostics;

namespace TeletypewriterInterface
{
    public static class ITA2Encoder
    {
        public static class SpecialBytes
        {
            public const byte letterMode = 0b11111;
            public const byte figuresMode = 0b11011;
            public const byte carriageReturn = 0b01000;
            public const byte lineFeed = 0b00010;
            public const byte whoAreYou = 0b01001;
            public const byte bell = 0b01011;
            public const byte space = 0b00100;
        }

        public static class SpecialChars
        {
            public const char letterMode = '\x0f';
            public const char figuresMode = '\x0e';
            public const char carriageReturn = '\r';
            public const char lineFeed = '\n';
            public const char whoAreYou = '\x05';
            public const char bell = '\x07';
            public const char space = ' ';
        }

        public static readonly IReadOnlyDictionary<char, byte> universalSignals = new Dictionary<char, byte>()
        {
            {SpecialChars.letterMode, SpecialBytes.letterMode}, {SpecialChars.figuresMode, SpecialBytes.figuresMode},
            {SpecialChars.carriageReturn, SpecialBytes.carriageReturn}, {SpecialChars.lineFeed, SpecialBytes.lineFeed},
            {SpecialChars.space, SpecialBytes.space}
        };

        public static readonly IReadOnlyDictionary<byte, char> universalSignalsInverse = Util.InvertDictionary(universalSignals);

        public static readonly IReadOnlyDictionary<char, byte> letters = new Dictionary<char, byte>()
        {
            {'A', 0b00011}, {'B', 0b11001}, {'C', 0b01110}, {'D', 0b01001},
            {'E', 0b00001}, {'F', 0b01101}, {'G', 0b11010}, {'H', 0b10100},
            {'I', 0b00110}, {'J', 0b01011}, {'K', 0b01111}, {'L', 0b10010},
            {'M', 0b11100}, {'N', 0b01100}, {'O', 0b11000}, {'P', 0b10110},
            {'Q', 0b10111}, {'R', 0b01010}, {'S', 0b00101}, {'T', 0b10000},
            {'U', 0b00111}, {'V', 0b11110}, {'W', 0b10011}, {'X', 0b11101},
            {'Y', 0b10101}, {'Z', 0b10001}
        };

        public static readonly IReadOnlyDictionary<byte, char> lettersInverse = Util.InvertDictionary(letters);

        public static readonly IReadOnlyDictionary<char, byte> figures = new Dictionary<char, byte>()
        {
            {'\'',0b00101}, {'(', 0b01111}, {')', 0b10010}, {'+', 0b10001},
            {',', 0b01100}, {'-', 0b00011}, {'.', 0b11100}, {'/', 0b11101},
            {'0', 0b10110}, {'1', 0b10111}, {'2', 0b10011}, {'3', 0b00001},
            {'4', 0b01010}, {'5', 0b10000}, {'6', 0b10101}, {'7', 0b00111},
            {'8', 0b00110}, {'9', 0b11000}, {':', 0b01110}, {'=', 0b11110},
            {'?', 0b11001},
            {SpecialChars.bell, SpecialBytes.bell}, {SpecialChars.whoAreYou, SpecialBytes.whoAreYou}
        };

        public static readonly IReadOnlyDictionary<byte, char> figuresInverse = Util.InvertDictionary(figures);

        public static IEnumerable<byte> GetBytes(string text)
        {
            bool lettersMode = true;

            yield return SpecialBytes.letterMode;
            foreach (char c in text.ToUpper())
            {
                if (universalSignals.ContainsKey(c))
                {
                    yield return universalSignals[c];
                }
                if (letters.ContainsKey(c))
                {
                    if (!lettersMode)
                    {
                        yield return SpecialBytes.letterMode;
                        lettersMode = true;
                    }
                    yield return letters[c];
                }
                else if (figures.ContainsKey(c))
                {
                    if (lettersMode)
                    {
                        yield return SpecialBytes.figuresMode;
                        lettersMode = false;
                    }
                    yield return figures[c];
                }
                else
                {
                    yield return SpecialBytes.space;
                }
            }
        }

        internal static char GetChar(byte data, bool isLetterMode)
        {
            if (universalSignalsInverse.ContainsKey(data))
            {
                return universalSignalsInverse[data];
            }
            if (isLetterMode)
            {
                return lettersInverse[data];
            }
            else
            {
                return figuresInverse[data];
            }
        }
    }
}
