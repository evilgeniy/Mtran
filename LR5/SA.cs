using System;
using System.Collections.Generic;
using System.Linq;
using static ConsoleApp1.Token;
namespace ConsoleApp1
{

    public class SA
    {

        protected int OpenBrackLvl = 0;
        protected int CurrBlockLvl = 0;
        public class ExpressionNode
        {
            public ExpressionNode L = null;
            public Token Oper = null;
            public ExpressionTypes Type;
            public int OperPriority = 0;
            public ExpressionNode R = null;
            public ExpressionNode Parents = null;
            public Tree<ExpressionNode> Block = new Tree<ExpressionNode>(null);

            public ExpressionNode LRotation()
            {
                ExpressionNode newRoot = new ExpressionNode()
                {
                    R = this.R.R,
                    Oper = this.R.Oper,
                    Type = this.R.Type,
                    Parents = this.Parents
                };
                newRoot.L = new ExpressionNode()
                {
                    L = this.L,
                    R = this.R.L,
                    Oper = this.Oper,
                    Type = this.Type,
                    Parents = newRoot
                };

                return newRoot;
            }

            public void InsDeepL(ExpressionNode node)
            {
                ExpressionNode temp = this;
                while (!(temp.L is null))
                {
                    temp = temp.L;
                }
                temp.L = node;
            }

            public override string ToString()
            {
                return $"({Oper.ToString()})";
            }

            public enum ExpressionTypes
            {
                UNKNOWN,
                UNARY_OPERATION,
                BINARY_OPERATION,
                BLOCK_OPENING_CONDITIONAL_OPERATION,
                BLOCK_OPENING_OPERATION,
                FUNCTION_CALL,
                FUNCTION_DEF,
                OPERAND
            };

            public static Dictionary<TTs, ExpressionTypes> TokensToExpressionTypes = new Dictionary<TTs, ExpressionTypes>()
            {
                [TTs.ASSIGN] = ExpressionTypes.BINARY_OPERATION,
                [TTs.COMMA] = ExpressionTypes.BINARY_OPERATION,
                [TTs.DOT] = ExpressionTypes.BINARY_OPERATION,
                [TTs.IF] = ExpressionTypes.BLOCK_OPENING_CONDITIONAL_OPERATION,
                [TTs.ELIF] = ExpressionTypes.BLOCK_OPENING_CONDITIONAL_OPERATION,
                [TTs.ELSE] = ExpressionTypes.BLOCK_OPENING_OPERATION,
                [TTs.FOR] = ExpressionTypes.BLOCK_OPENING_CONDITIONAL_OPERATION,
                [TTs.WHILE] = ExpressionTypes.BLOCK_OPENING_CONDITIONAL_OPERATION,
                [TTs.PLUS] = ExpressionTypes.BINARY_OPERATION,
                [TTs.MINUS] = ExpressionTypes.BINARY_OPERATION,
                [TTs.MODULE] = ExpressionTypes.BINARY_OPERATION,
                [TTs.DIVISION] = ExpressionTypes.BINARY_OPERATION,
                [TTs.MULTIPLICATION] = ExpressionTypes.BINARY_OPERATION,
                [TTs.NOT] = ExpressionTypes.UNARY_OPERATION,
                [TTs.AND] = ExpressionTypes.BINARY_OPERATION,
                [TTs.OR] = ExpressionTypes.BINARY_OPERATION,
                [TTs.IN] = ExpressionTypes.BINARY_OPERATION,
                [TTs.LOWER] = ExpressionTypes.BINARY_OPERATION,
                [TTs.LOWER_OR_EQUAL] = ExpressionTypes.BINARY_OPERATION,
                [TTs.GREATER] = ExpressionTypes.BINARY_OPERATION,
                [TTs.GREATER_OR_EQUAL] = ExpressionTypes.BINARY_OPERATION,
                [TTs.NOT_EQUAL] = ExpressionTypes.BINARY_OPERATION,
                [TTs.EQUAL] = ExpressionTypes.BINARY_OPERATION,
                [TTs.FUNCTION_DEFINITION] = ExpressionTypes.FUNCTION_DEF,
                [TTs.STRING_CONST] = ExpressionTypes.OPERAND,
                [TTs.INT_NUM] = ExpressionTypes.OPERAND,
                [TTs.FLOAT_NUM] = ExpressionTypes.OPERAND,
                [TTs.ID] = ExpressionTypes.OPERAND,
                [TTs.BUILT_IN_FUNCTION] = ExpressionTypes.OPERAND,
                [TTs.COLON] = ExpressionTypes.BLOCK_OPENING_OPERATION
            };
        }
        public ExpressionNode Analyse(IEnumerable<Token> Tokens, out bool NewBlock, out bool ElifElsNode)
        {

            OpenBrackLvl = 0;
            NewBlock = false;
            ElifElsNode = false;
            var FToken = Tokens.FirstOrDefault();

            if (FToken?.IsBlockOpeningOperation == true)
            {
                NewBlock = true;
                ElifElsNode = FToken.TT == TTs.ELSE || FToken.TT == TTs.ELIF;
                if (Tokens.LastOrDefault()?.TT != Token.TTs.COLON)

                {
                    var t = Tokens.LastOrDefault();
                    throw new SinError("colon expected", t.Val, t.CLI, t.CLN);
                }
            }
            ExpressionNode root = BTree(Tokens);
            if (OpenBrackLvl != 0)
            {

                throw new SinError("brackets do not match", Tokens.Last().Val, Tokens.Last().CLI, Tokens.Last().CLN);
            }

            return root;
        }

        protected ExpressionNode BTree(IEnumerable<Token> Tokens, ExpressionNode Parents = null)
        {

            ExpressionNode root = null;
            ExpressionNode L = null;

            Token Token = Tokens.FirstOrDefault();

            if (Token is null)
                return null;





            if (Token.Const || Token.TT == Token.TTs.ID || Token.TT == Token.TTs.BUILT_IN_FUNCTION)
            {
                L = new ExpressionNode()
                {
                    Oper = Token,
                    Type = ExpressionNode.TokensToExpressionTypes.GetValueOrDefault(Token.TT, ExpressionNode.ExpressionTypes.UNKNOWN)
                };

                var tt = Tokens.ElementAtOrDefault(1)?.TT;
                if (tt == Token.TTs.OPENING_ROUND_BRACKET)
                {
                    root = L;
                    L = null;
                    root.Type = ExpressionNode.ExpressionTypes.FUNCTION_CALL;
                    root.R = BTree(Tokens.Skip(1));

                }
                else if (tt == Token.TTs.COLON)
                {
                    root = L;
                    L = null;
                    root.R = BTree(Tokens.Skip(2));

                    
                }

                else
                {
                    root = BTree(Tokens.Skip(1));
                    L.Parents = root;
                }
            }

            else if (Token.IsOpeningBracket)
            {
                this.OpenBrackLvl++;
                root = BTree(Tokens.Skip(1));

                root.OperPriority++;
            }

            else if (Token.IsClosingBracket)
            {
                this.OpenBrackLvl--;
                root = BTree(Tokens.Skip(1));
                if (root != null)
                    root.OperPriority--;
            }

            else if (Token.IsOper)
            {
                root = new ExpressionNode()
                {
                    Oper = Token,
                    Type = ExpressionNode.TokensToExpressionTypes.GetValueOrDefault(Token.TT, ExpressionNode.ExpressionTypes.UNKNOWN)
                };

                if (Token.TT == Token.TTs.MULTIPLICATION || Token.TT == Token.TTs.DIVISION)
                {
                    root.OperPriority++;
                }

                root.R = BTree(Tokens.Skip(1), root);
            }
            if (root is null)
            {
                if (L is null)
                    return null;
                L.Parents = Parents;
                return L;
            }
            root.Parents = Parents;

            if (L != null)
                root.InsDeepL(L);

            if (root.R != null && root.Oper.IsOper && root.R.Oper.IsOper && root.OperPriority > root.R.OperPriority)
                return root.LRotation();
            return root;
        }

        public static ExpressionNode ValidateNode(ExpressionNode node)
        {
            switch (node.Type)
            {
                case ExpressionNode.ExpressionTypes.BINARY_OPERATION:

                    if (node.L == null || node.R == null)
                    {
                        throw new SinError(
                            "binary operation lacks operand",
                            node.Oper.Val,
                            node.Oper.CLI,
                            node.Oper.CLN
                            );
                    }
                    break;

                case ExpressionNode.ExpressionTypes.BLOCK_OPENING_CONDITIONAL_OPERATION:

                    if (node.L != null || node.R == null)
                        throw new SinError(
                            "conditional Oper wrong usage",
                            node.Oper.Val,
                            node.Oper.CLI,
                            node.Oper.CLN
                            );
                    break;

                case ExpressionNode.ExpressionTypes.UNKNOWN:
                    throw new SinError(
                        "unknown expression",
                        node.Oper.Val,
                        node.Oper.CLI,
                        node.Oper.CLN
                        );
                case ExpressionNode.ExpressionTypes.OPERAND:
                    if (node.L != null)
                        throw new SinError(
                        "unknown Oper",
                        node.Oper.Val,
                        node.Oper.CLI,
                        node.Oper.CLN
                        );
                    break;
                default:
                    break;
            }
            return node;
        }

        public class SinError : FormatException
        {
            public string Val { get; set; }
            public int PosL { get; set; }
            public int LN { get; set; }
            public SinError(string message, string Value, int PL, int LNum) : base(message)
            {
                Value = Val;
                PL = PosL;
                LNum = LN;
            }
        }
    }
}
