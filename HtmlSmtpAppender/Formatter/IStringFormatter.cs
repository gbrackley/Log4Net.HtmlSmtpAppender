using System;
using log4net.Core;
using log4net.Layout;
using log4net.Util;

namespace log4net.Formatter
{
    /// <summary>
    ///   An interface for providing strings that are formatted dynamically by an appender.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     The log4net framework provides the ability for format:
    ///     <list type = "bullet">
    ///       <item>
    ///         An <see cref = "ILayout" /> to format <see cref = "LoggingEvent" />
    ///       </item>
    ///       <item>
    ///         A <see cref = "PatternString" /> to statically format a string
    ///         at configuration time.
    ///       </item>
    ///     </list>
    ///   </para>
    ///   <para>
    ///     There is no support for formatting a string dynamically by an appender, where
    ///     the appender can resolve the values substituted.
    ///   </para>
    /// </remarks>
    /// <seealso cref = "ILayout" />
    /// <seealso cref = "PatternString" />
    public interface IStringFormatter
    {
        /// <summary>
        ///   Format the given <paramref name = "format" /> using the 
        ///   <paramref name = "evaluator" /> function to convert the
        ///   substitution variables.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     This function uses the names and styles of conversion parameters as
        ///     the <see cref = "PatternString" /> class where possible.
        ///   </para>
        /// </remarks>
        /// <param name = "format">The format of the string</param>
        /// <param name = "evaluator">A function that will format a specific substituion value</param>
        string Format(
            string format,
            Func<string, string, string> evaluator);
    }
}
