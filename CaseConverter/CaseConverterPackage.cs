using System;
using System.Runtime.InteropServices;
using CaseConverter.Options;
using Microsoft.VisualStudio.Shell;

namespace CaseConverter
{
    /// <summary>
    /// It is a package placed as an extension.
    /// </summary>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "2.3", IconResourceID = 400)] // Visual Studio のヘルプ/バージョン情報に表示される情報です。
    [Guid(PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideOptionPage(typeof(GeneralOptionPage), PackageName, "General", 100, 101, true)]
    [ProvideProfile(typeof(GeneralOptionPage), PackageName, "General", 110, 110, true)]
    public sealed class CaseConverterPackage : Package
    {
        /// <summary>
        /// Package ID.
        /// </summary>
        public const string PackageGuidString = "3293cb25-75b9-4d5a-a248-ea3cf25fc4c8";

        /// <summary>
        /// It is the name of the package.
        /// </summary>
        public const string PackageName = "Case Converter";

        /// <summary>
        /// Gets an option for general settings.
        /// </summary>
        /// <returns>General Setting Options</returns>
        public GeneralOption GetGeneralOption()
        {
            return (GeneralOption)GetDialogPage(typeof(GeneralOptionPage)).AutomationObject;
        }

        /// <summary>
        /// Initialize the package.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            ConvertCaseCommand.Initialize(this);
        }
    }
}
