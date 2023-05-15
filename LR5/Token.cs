using System.Collections.Generic;

namespace ConsoleApp1
{
    public class Token
    {
        public TTs TT { get; set; }
        public string Group { get; set; }
        public string Val { get; set; }
        public int Length { get; set; }
        public int CLN { get; set; }
        public int CLI { get; set; }

        public enum TTs
        {
            CLOSING_SQUARE_BRACKET,
            COLON,
            UNKNOWN,
            OPENING_CURLY_BRACKET,
            MULTIPLICATION,
            Comm,
            STRING_CONST,
            OPENING_ROUND_BRACKET,
            FLOAT_NUM,
            MINUS,
            CLOSING_CURLY_BRACKET,
            INT_NUM,
            ID,
            DIVISION,
            DOT,
            COMMA,
            ASSIGN,
            OPENING_SQUARE_BRACKET,
            OR,
            MODULE,
            CLOSING_ROUND_BRACKET,
            PLUS,
            GREATER,
            AND,
            EQUAL,
            NOT,
            GREATER_OR_EQUAL,
            FUNCTION_DEFINITION,
            ELIF,
            BUILT_IN_FUNCTION,
            RAISE,
            IF,
            IMPORT,
            ELSE,
            LOWER,
            LOWER_OR_EQUAL,
            WHILE,
            NOT_EQUAL,
            FOR,
            IN
        }

        public bool IsOper
        {
            get => SO.ContainsValue(TT) | BOO.ContainsValue(TT);
        }

        public bool IsReservedIdToken
        {
            get => RID.ContainsValue(TT);
        }

        public bool IsIf
        {
            get => TT == TTs.IF;
        }

        public bool IsBlockOpeningOperation
        {
            get => BOO.ContainsValue(TT);
        }

        public bool IsElse
        {
            get => TT == TTs.ELSE;
        }

        public bool IsElif
        {
            get => TT == TTs.ELIF;
        }

        public bool IsClosingBracket
        {
            get => this.TT == TTs.CLOSING_ROUND_BRACKET;
        }

        public bool IsOpeningBracket
        {
            get => this.TT == TTs.OPENING_ROUND_BRACKET;
        }

        public string DescrSt
        {
            get
            {
                if (this.IsReservedIdToken && !this.IsOper)
                    return $"Reserved keyword {this.TT}";

                if (this.IsOper)
                    return $"Operation {this.TT}";

                if (this.Const)
                    return $"{this.TT} Constants";

                if (this.TT == TTs.Comm)
                    return $"is # Comm";

                return $"is {this.TT}";
            }
        }

        public bool Const
        {
            get => this.TT == TTs.INT_NUM
                    || this.TT == TTs.STRING_CONST
                    || this.TT == TTs.FLOAT_NUM;
        }

        public static Dictionary<string, TTs> BOO = new Dictionary<string, TTs>()
        {
            ["while"] = TTs.WHILE,
            ["elif"] = TTs.ELIF,
            ["import"] = TTs.IMPORT,
            ["def"] = TTs.FUNCTION_DEFINITION,
            ["else"] = TTs.ELSE,
            ["and"] = TTs.AND,
            ["if"] = TTs.IF,
            ["or"] = TTs.OR,
            ["raise"] = TTs.RAISE,
            ["not"] = TTs.NOT,
            ["for"] = TTs.FOR,
            ["in"] = TTs.IN
        };

        public static Dictionary<string, TTs> SO = new Dictionary<string, TTs>()
        {
            ["/"] = TTs.DIVISION,
            ["="] = TTs.ASSIGN,
            [":"] = TTs.COLON,
            ["=="] = TTs.EQUAL,
            ["{"] = TTs.OPENING_CURLY_BRACKET,
            ["%"] = TTs.MODULE,
            [">"] = TTs.GREATER,
            ["+"] = TTs.PLUS,
            ["["] = TTs.OPENING_SQUARE_BRACKET,
            [")"] = TTs.CLOSING_ROUND_BRACKET,
            ["-"] = TTs.MINUS,
            ["!="] = TTs.NOT_EQUAL,
            ["]"] = TTs.CLOSING_SQUARE_BRACKET,
            ["("] = TTs.OPENING_ROUND_BRACKET,
            ["<="] = TTs.LOWER_OR_EQUAL,
            ["*"] = TTs.MULTIPLICATION,
            ["."] = TTs.DOT,
            [","] = TTs.COMMA,
            [">="] = TTs.GREATER_OR_EQUAL,
            ["}"] = TTs.CLOSING_CURLY_BRACKET,
            ["<"] = TTs.LOWER
        };

        public static Dictionary<string, TTs> RID = new Dictionary<string, TTs>()
        {
            ["type"] = TTs.BUILT_IN_FUNCTION,
            ["print"] = TTs.BUILT_IN_FUNCTION,
            ["min"] = TTs.BUILT_IN_FUNCTION,
            ["in"] = TTs.IN,
            ["input"] = TTs.BUILT_IN_FUNCTION,
            ["or"] = TTs.OR,
            ["range"] = TTs.BUILT_IN_FUNCTION,
            ["if"] = TTs.IF,
            ["else"] = TTs.ELSE,
            ["import"] = TTs.IMPORT,
            ["abs"] = TTs.BUILT_IN_FUNCTION,
            ["elif"] = TTs.ELIF,
            ["max"] = TTs.BUILT_IN_FUNCTION,
            ["not"] = TTs.NOT,
            ["def"] = TTs.FUNCTION_DEFINITION,
            ["int"] = TTs.BUILT_IN_FUNCTION,
            ["raise"] = TTs.RAISE,
            ["while"] = TTs.WHILE,
            ["float"] = TTs.BUILT_IN_FUNCTION,
            ["and"] = TTs.AND,
            ["for"] = TTs.FOR
        };

        public override string ToString()
        {
            return $"{TT}: {Val}";
        }
    }
}
