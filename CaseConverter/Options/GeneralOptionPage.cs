using Microsoft.VisualStudio.Shell;

namespace CaseConverter.Options
{
    /// <summary>
    /// It is an option page for general settings.
    /// </summary>
    public class GeneralOptionPage : DialogPage
    {
        /// <summary>
        /// It is an option for general settings.
        /// </summary>
        private readonly GeneralOption _option = new GeneralOption();

        /// <inheritdoc />
        public override object AutomationObject
        {
            get { return _option; }
        }
    }
}
