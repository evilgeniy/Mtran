using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using static ConsoleApp1.Token;

namespace ConsoleApp1
{
    public class LA
    {
        private List<Construction> constr = new List<Construction>();
        private const string IntRG = "INT";
        private const string FRG = "FLOAT";
        private const string IRG = "ID";
        private const string SRG = "STR";
        private const string OpRG = "OPER";
        private const string CRG = "COMM";
        private const string ORG = "OTHER";
        private Regex reg = new Regex(
                @"\s*(?:(?<COMM>#.*)|(?<STR>[\""'].*[\""'])"
                    + @"|(?<FLOAT>[+-]*[0-9]+\.[0-9]*)|(?<INT>[+-]*\d+)"
                    + @"|(?<OPER>[+\-\/*<=>!%(){},.\[\]:]+)"
                    + @"|(?<ID>\w+)|(?<OTHER>.+\s?))",
                RegexOptions.Compiled | RegexOptions.IgnoreCase
                );
        public class LexErr
        {
            public enum TypesEr { UNEXPECTED_Token, UNDEFINED_FUNCTION }

            public TypesEr TypErr { get; set; }

            public string Val { get; set; }

            public int CLN { get; set; }

            public int IndexInCL { get; set; }

            public int Length { get; set; }

            protected string Descr = "Unexpected Token";

            public string Descrip => Descr;

            public void CDescr(string CL)
            {
                StringBuilder STRBUILD = new StringBuilder(CL);
                STRBUILD.AppendLine();
                STRBUILD.Append(new string(' ', this.IndexInCL));
                STRBUILD.Append('^');
                this.Descr = STRBUILD.ToString();
            }
        }
        public class Construction
        {
            public List<Token> Tokens { get; set; }
            public List<LexErr> Error { get; set; }
            public bool Errors { get => Error.Count > 0; }
            public int Idention { get; set; }
        }

        public Construction AL(string CL, int LN)
        {
            var trimL = CL.TrimStart('\t');
            int spaces = CL.Length - trimL.Length;
            var (Tokens, errors) = ParseLine(trimL, LN);

            return new Construction()
            {
                Tokens = Tokens,
                Error = errors,
                Idention = spaces
            };
        }

        public void ALs(IEnumerable<string> CLs)
        {
            int LN = 0;
            foreach (string line in CLs)
            {
                var ResultOfAnalise = AL(line, LN);
                constr.Add(ResultOfAnalise);
                LN++;
            }
        }

        public (List<Token> Tokens, List<LexErr> errors) ParseLine(string CL, int LN)
        {
            List<Token> Tokens = new List<Token>();
            List<LexErr> errors = new List<LexErr>();
            MatchCollection match = reg.Matches(CL);
            string[] gNames = reg.GetGroupNames();

            foreach (Match coincidence in match)
            {
                GroupCollection group = coincidence.Groups;
                for (int i = 1; i < gNames.Length; i++)
                {
                    if (group[gNames[i]].Success)
                    {
                        string trimVal = group[gNames[i]].Value.Trim(' ');
                        if (trimVal.Length == 0)
                            break;

                        TTs type = GetTT(gNames[i], trimVal);
                        if (type == TTs.UNKNOWN)
                        {
                            LexErr error = new LexErr()
                            {
                                CLN = LN,
                                Val = group[gNames[i]].Value,
                                IndexInCL = coincidence.Index,
                                Length = coincidence.Length
                            };
                            error.CDescr(CL);
                            errors.Add(error);
                        }

                        Tokens.Add(
                            new Token
                            {
                                Val = group[gNames[i]].Value,
                                Group = gNames[i],
                                CLN = LN,
                                CLI = coincidence.Index,
                                Length = coincidence.Length,
                                TT = type
                            }
                            );
                    }
                }
            }
            return (Tokens, errors);
        }

        public static TTs GetTT(string matchG, string Val)
        {
            switch (matchG)
            {
                case CRG:
                    return TTs.Comm;
                case FRG:
                    return TTs.FLOAT_NUM;
                case SRG:
                    return TTs.STRING_CONST;
                case IRG:
                    return IDTT(Val);
                case IntRG:
                    return TTs.INT_NUM;
                case OpRG:
                    return OperTT(Val);
                case ORG:
                default:
                    return TTs.UNKNOWN;
            }
        }
        public static TTs IDTT(string Val)
        {
            if (RID.TryGetValue(Val, out TTs TT)
                || SO.TryGetValue(Val, out TT)
                || BOO.TryGetValue(Val, out TT))
            {
                return TT;
            }
            return TTs.ID;
        }

        public static TTs OperTT(string Val)
        {
            if (SO.TryGetValue(Val, out TTs TT) || BOO.TryGetValue(Val, out TT))
            {
                return TT;
            }
            return TTs.UNKNOWN;
        }
    }
}
