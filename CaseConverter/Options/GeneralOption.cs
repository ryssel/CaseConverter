using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CaseConverter.Converters;

namespace CaseConverter.Options
{
    /// <summary>
    /// It is an option for general settings.
    /// </summary>
    public class GeneralOption
    {
        /// <summary>
        /// Gets or sets the string conversion pattern option.
        /// </summary>
        [Category("Basic")]
        [DisplayName("Conversion Pattern")]
        [Description("This is an order to convert a string.")]
        [TypeConverter(typeof(PatternOptionsConverter))]
        public PatternOption[] PatternOptions { get; set; } =
            new[]
            {
                //new PatternOption { Pattern = StringCasePattern.SnakeCase },
                new PatternOption { Pattern = StringCasePattern.CamelCase },
                //new PatternOption { Pattern = StringCasePattern.PascalCase }
            };

        /// <summary>
        /// Gets or sets a string conversion pattern.
        /// </summary>
        [Browsable(false)]
        public IEnumerable<StringCasePattern> Patterns => PatternOptions.Select(x => x.Pattern);
    }
}
