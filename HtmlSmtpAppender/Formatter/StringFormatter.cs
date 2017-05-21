using System;
using System.Text.RegularExpressions;
using log4net.Appender.HtmlSmtp.Utils;

namespace log4net.Formatter
{
    /// <summary>
    ///   A simple implementation of <see cref = "IStringFormatter" />
    /// </summary>
    /// <seealso cref = "IStringFormatter" />
    public class StringFormatter : IStringFormatter
    {
        public const string VariableSubstitutionRegex =
            @"
                (?<percentage> %% )   # Escaped percentage sign

                |

                ( 
                    % 
                    (?<name> \w [\w\d]* )            # a name
                    (                                # and optional parameters in curly braces   
                        {                            # opening brace
                            (?<parameters> [^}]* )   # the parameters
                        }                            # closing brace      
                    )?                               
                )      
           ";


        public string Format(
            string format,
            Func<string, string, string> evaluator)
        {
            return Regex.Replace(
                format,
                VariableSubstitutionRegex,
                match => OnMatch(evaluator, match),
                RegexOptions.Compiled |
                    RegexOptions.CultureInvariant |
                        RegexOptions.IgnorePatternWhitespace);
        }

        private static string OnMatch(Func<string, string, string> evaluator, Match match)
        {
            if (match.Groups["percentage"].Captures.Count > 0)
            {
                return "%";
            }
            else if (match.Groups["name"].Captures.Count > 0)
            {
                var name = match.GetSingletonCapture("name");
                var parameters = match.GetSingletonOrDefaultCapture("parameters");
                return evaluator(name, parameters);
            }
            else
            {
                throw new ArgumentException("The input string failed to match the expected pattern");
            }
        }
    }
}