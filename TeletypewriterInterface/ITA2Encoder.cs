namespace TeletypewriterInterface
{
    public static class ITA2Encoder
    {
        public const byte letterMode = 0b11111;
        public const byte figuresMode = 0b11011;
        public const byte carriageReturn = 0b01000;
        public const byte lineFeed = 0b00010;
        public const byte whoAreYou = 0b01001;
        public const byte bell = 0b01011;
        public const byte space = 0b00100;

        public static readonly IReadOnlyDictionary<char, byte> letters = new Dictionary<char, byte>()
        {
            {'\r', carriageReturn}, {'\n', lineFeed},
            {'A', 0b00011}, {'B', 0b11001}, {'C', 0b01110}, {'D', 0b01001},
            {'E', 0b00001}, {'F', 0b01101}, {'G', 0b11010}, {'H', 0b10100},
            {'I', 0b00110}, {'J', 0b01011}, {'K', 0b01111}, {'L', 0b10010},
            {'M', 0b11100}, {'N', 0b01100}, {'O', 0b11000}, {'P', 0b10110},
            {'Q', 0b10111}, {'R', 0b01010}, {'S', 0b00101}, {'T', 0b10000},
            {'U', 0b00111}, {'V', 0b11110}, {'W', 0b10011}, {'X', 0b11101},
            {'Y', 0b10101}, {'Z', 0b10001}, {' ', space}
        };

        public static readonly IReadOnlyDictionary<char, byte> figures = new Dictionary<char, byte>()
        {
            {'\r', carriageReturn}, {'\n', lineFeed},
            {'\'',0b00101}, {'(', 0b01111}, {')', 0b10010}, {'+', 0b10001},
            {',', 0b01100}, {'-', 0b00011}, {'.', 0b11100}, {'/', 0b11101},
            {'0', 0b10110}, {'1', 0b10111}, {'2', 0b10011}, {'3', 0b00001},
            {'4', 0b01010}, {'5', 0b10000}, {'6', 0b10101}, {'7', 0b00111},
            {'8', 0b00110}, {'9', 0b11000}, {':', 0b01110}, {'=', 0b11110},
            {'?', 0b11001}, {' ', space}, {(char)0x07, bell}, {(char)0x05, whoAreYou},
            {'Ä', 0b01101}, {'Ö', 0b01110}, {'Ü', 0b01111}
        };

        public static IEnumerable<byte> GetBytes(string text)
        {
            bool lettersMode = true;

            yield return letterMode;
            foreach (char c in text.ToUpper())
            {
                if (letters.ContainsKey(c))
                {
                    if (!lettersMode)
                    {
                        yield return letterMode;
                        lettersMode = true;
                    }
                    yield return letters[c];
                }
                else if (figures.ContainsKey(c))
                {
                    if (lettersMode)
                    {
                        yield return figuresMode;
                        lettersMode = false;
                    }
                    yield return figures[c];
                }
                else
                {
                    yield return space;
                }
            }
        }
    }
}
