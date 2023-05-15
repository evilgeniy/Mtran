using ConsoleTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static ConsoleApp1.LA;
using static ConsoleApp1.Token;
using static ConsoleApp1.Semantic;

namespace ConsoleApp1
{

    class Program
    {

        static void TokenDict(Dictionary<string, Token> dict)
        {
            ConsoleTable ConsTab = new ConsoleTable("Token", "Descr");
            foreach (Token Token in dict.Values)
            {
                ConsTab.AddRow(Token.Val, Token.DescrSt);
            }
            ConsTab.Write();
        }

        static string NodeWChildren(SA.ExpressionNode node, string Idention)
        {
            if (node == null)
            {
                return "";
            }
            SA.ValidateNode(node);
            StringBuilder StrBuid = new StringBuilder();
            StrBuid.AppendLine($"{Idention} {node.Oper.Val}");
            if (node.L != null)
                StrBuid.Append(NodeWChildren(node.L, Idention + "\\"));
            if (node.R != null)
                StrBuid.Append(NodeWChildren(node.R, Idention + "\\"));
            return StrBuid.ToString();
        }

        static void SynTree(IEnumerable<SA.ExpressionNode> nodes, int nestingLevel = 1)
        {
            string Idention = new String('|', nestingLevel);
            foreach (var node in nodes)
            {

                Console.Write(NodeWChildren(node, Idention));
                Console.WriteLine(Idention);
                SynTree(node.Block, nestingLevel + 1);
            }
        }

        static string DescrErr(int IndexInCL, string codeLine)
        {
            StringBuilder StrBuid = new StringBuilder(codeLine);
            StrBuid.AppendLine();
            StrBuid.Append(new string(' ', IndexInCL));
            StrBuid.Append('^');
            return StrBuid.ToString();
        }

        static void Work(IEnumerable<string> codeLines)
        {
            Dictionary<string, Token> Constants = new Dictionary<string, Token>();
            Dictionary<string, Token> Opers = new Dictionary<string, Token>();
            Dictionary<string, Token> Variables = new Dictionary<string, Token>();
            Dictionary<string, Token> Keywords = new Dictionary<string, Token>();
            List<LexErr> errors = new List<LexErr>();
            Tree<SA.ExpressionNode> tree = new Tree<SA.ExpressionNode>(null);
            Tree<SA.ExpressionNode> CurBlock = tree;
            int LNum = 0;
            SA sa = new SA();
            LA la = new LA();
            int prevLI = 0;

            foreach (string line in codeLines)
            {
                Construction constr = la.AL(line, LNum);
                if (constr.Tokens.Count == 0)
                {
                    LNum++;
                    continue;
                }
                for (int i = 0; i < constr.Tokens.Count; i++)
                {
                    Token Token = constr.Tokens[i];
                    if (Token.IsReservedIdToken)
                        Keywords.TryAdd(Token.Val, Token);
                    else if (Token.IsOper)
                        Opers.TryAdd(Token.Val, Token);
                    else if (Token.Const)
                        Constants.TryAdd(Token.Val, Token);
                    else if (Token.TT != TTs.UNKNOWN)
                    {
                        Variables.TryAdd(Token.Val, Token);
                    }
                }

                if (constr.Errors)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\t\t ERRORS");
                    Console.ResetColor();
                    foreach (LexErr error in constr.Error)
                    {
                        Console.WriteLine($"line {error.CLN + 1} char {error.IndexInCL + 1} :: {error.TypErr}");
                        Console.WriteLine(error.Descrip);
                    }
                    Console.Read();
                    Environment.Exit(1);
                }

                SA.ExpressionNode node = null;
                bool ElifElsNode = false;
                bool BlockOpen = false;

                node = sa.Analyse(constr.Tokens, out BlockOpen, out ElifElsNode);

                int DiffIdent = prevLI - constr.Idention;
                if (DiffIdent > 0)
                {
                    for (int i = prevLI - 1; i >= constr.Idention; i--)
                    {
                        CurBlock = CurBlock.Parents;
                        if (CurBlock.Idention == i)
                            break;
                    }

                    if (node.Oper.IsElif && !CurBlock.Last().Oper.IsIf)
                    {
                        throw new SA.SinError(
                                "elif block not allowed here",
                                node.Oper.Val,
                                node.Oper.CLI,
                                node.Oper.CLN
                            );
                    }
                    else if (node.Oper.IsElse && !(CurBlock.Last().Oper.IsIf || CurBlock.Last().Oper.IsElif))
                    {
                        throw new SA.SinError(
                                "else block not allowed here",
                                line,
                                node.Oper.CLI,
                                node.Oper.CLN
                            );
                    }
                }
                prevLI = constr.Idention;
                LNum++;
                if (BlockOpen)
                {
                    if ((node.Oper.IsElif || node.Oper.IsElse) && !CurBlock.Last().Oper.IsIf && !CurBlock.Last().Oper.IsElif)
                    {
                        throw new SA.SinError(
                            "lacks IF clause for elif|else block to appear",
                            node.Oper.Val,
                            node.Oper.CLI,
                            node.Oper.CLN
                            );
                    }
                    CurBlock.Add(node);
                    CurBlock.Last().Block = new Tree<SA.ExpressionNode>(CurBlock);
                    CurBlock = CurBlock.Last().Block;
                    CurBlock.Idention = constr.Idention;
                    continue;
                }

                CurBlock.Add(node);
            }
            if (errors.Any())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\t\t ERRORS");
                Console.ResetColor();
                foreach (LexErr error in errors)
                {
                    Console.WriteLine($"line {error.CLN + 1} char {error.IndexInCL + 1} :: {error.TypErr}");
                    Console.WriteLine(error.Descrip);
                }
            }
            Console.WriteLine("SYNTAX TREE:\n");
            SynTree(tree);

            Console.WriteLine("\n \t\t ConstantsS");
            TokenDict(Constants);

            Console.WriteLine("\n \t\t Variables");
            TokenDict(Variables);

            Console.WriteLine("\n \t\t KEYWORS");
            TokenDict(Keywords);

            Console.WriteLine("\n \t\t OperS");
            TokenDict(Opers);

        }

        static void Main(string[] args)
        {

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            string FILENAME = "sort2.py";
            IEnumerable<string> codeLines = System.IO.File.ReadLines(FILENAME);
            try
            {
                Work(codeLines);
                Console.WriteLine("\nПирамидальная сортировка\n");
                Console.WriteLine("0.38, 1.66, 4.764, 18.46, 26.863\n");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("***Interpretation complete***");
                Console.ResetColor();
            }
            catch (SA.SinError e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"SEMANTICK ERROR {e.Message}");
                Console.ResetColor();
                Console.WriteLine($"line {e.LN} char {e.PosL}:");
                Console.WriteLine(DescrErr(e.PosL, codeLines.ElementAt(e.LN).Trim()));
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine($"SYNTAX ERROR {e.Message}");
                Console.WriteLine("block opening element has nothing in its block!");
            }

            Console.Read();
        }
    }
}
