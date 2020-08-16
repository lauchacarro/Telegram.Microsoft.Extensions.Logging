using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Telegram.Microsoft.Extensions.Logging.Helpers
{

    //https://stackoverflow.com/a/34679657/10760567
    public class StringTemplate
    {
        /// <summary>Map of replacements for characters prefixed with a backward slash</summary>
        private static readonly Dictionary<char, string> EscapeChars
            = new Dictionary<char, string>
            {
                ['r'] = "\r",
                ['n'] = "\n",
                ['\\'] = "\\",
                ['{'] = "{",
            };

        /// <summary>Pre-compiled regular expression used during the rendering process</summary>
        private static readonly Regex RenderExpr = new Regex(@"\\.|{([a-z0-9_.\-]+)}",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>Template string associated with the instance</summary>
        public string TemplateString { get; }

        /// <summary>Create a new instance with the specified template string</summary>
        /// <param name="templateString">Template string associated with the instance</param>
        public StringTemplate(string templateString)
        {
            if (templateString == null)
            {
                throw new ArgumentNullException(nameof(templateString));
            }

            this.TemplateString = templateString;
        }

        /// <summary>Render the template using the supplied variable values</summary>
        /// <param name="variables">Variables that can be substituted in the template string</param>
        /// <returns>The rendered template string</returns>
        public string Render(Dictionary<string, object> variables)
        {
            return Render(this.TemplateString, variables);
        }

        /// <summary>Render the supplied template string using the supplied variable values</summary>
        /// <param name="templateString">The template string to render</param>
        /// <param name="variables">Variables that can be substituted in the template string</param>
        /// <returns>The rendered template string</returns>
        public static string Render(string templateString, Dictionary<string, object> variables)
        {
            if (templateString == null)
            {
                throw new ArgumentNullException(nameof(templateString));
            }

            return RenderExpr.Replace(templateString, Match =>
            {
                switch (Match.Value[0])
                {
                    case '\\':
                        if (EscapeChars.ContainsKey(Match.Value[1]))
                        {
                            return EscapeChars[Match.Value[1]];
                        }
                        break;

                    case '{':
                        if (variables.ContainsKey(Match.Groups[1].Value))
                        {
                            return variables[Match.Groups[1].Value].ToString();
                        }
                        break;
                }

                return string.Empty;
            });

        }
    }
}
