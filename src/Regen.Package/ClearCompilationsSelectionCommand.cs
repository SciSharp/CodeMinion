using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Media;
using System.Text.RegularExpressions;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Regen.Compiler;
using Regen.Helpers;
using Regen.Parser;
using Task = System.Threading.Tasks.Task;

namespace Regen {
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class ClearCompilationsSelectionCommand {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0104;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("f2fc6ff4-8bb2-417b-950d-b5010e8ce4cb");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnableDisableRegenCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private ClearCompilationsSelectionCommand(AsyncPackage package, OleMenuCommandService commandService) {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static ClearCompilationsSelectionCommand Instance { get; private set; }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider {
            get { return this.package; }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package) {
            // Switch to the main thread - the call to AddCommand in EnableDisableRegenCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync((typeof(IMenuCommandService))) as OleMenuCommandService;
            Instance = new ClearCompilationsSelectionCommand(package, commandService);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs _) {
            ThreadHelper.ThrowIfNotOnUIThread();

            // open the file in a VS code window and activate the pane
            DTE dte = Package.GetGlobalService(typeof(DTE)) as DTE;
            Document doc = dte?.ActiveDocument;

            TextDocument txt = doc?.Object() as TextDocument;
            if (dte == null || doc == null || txt == null) {
                SystemSounds.Beep.Play();
                return;
            }

            TextSelection textSelection = doc.ActiveWindow.Document.Selection as TextSelection;
            var cursor = textSelection.ActivePoint as VirtualPoint;
            var index = cursor.AbsoluteCharOffset;

            var code = txt.GetText().Replace("\r", "");
            var matches = Regex.Matches(code, $@"(\#if\s{Regexes.DefineMarker}[\n\r]{{1,2}}    [\s|\S]*?    \#else[\n\r]{{1,2}} )  ([\s|\S]*?)   (?=\#endif)", Regexes.DefaultRegexOptions);
            var match = matches.Cast<Match>().FirstOrDefault(m => m.DoesMatchNests(index));
            if (match == null) {
                SystemSounds.Beep.Play();
                return;
            }

            code = code.RemoveStringBetween("#else", "#endif", Math.Max(match.Index - 1, 0)).Replace("#else#endif", "#else" + Environment.NewLine + Environment.NewLine + "#endif");
            try {
                var ed = txt.CreateEditPoint(txt.StartPoint);
                ed.Delete(txt.EndPoint);
                ed.Insert(code);
                var newLength = txt.EndPoint.AbsoluteCharOffset;
                textSelection.MoveToAbsoluteOffset(Math.Min(newLength - 1, index));

                textSelection.MoveToAbsoluteOffset(Math.Min(newLength - 1, match.Groups[2].Index));
            } catch (Exception e) {
                Logger.Log(e);
#if DEBUG
                Message($"Failed parsing file...\n" + e);
#else
                Message($"Failed parsing file...\n" + e.Message + "\n" + e.InnerException?.Message);
#endif
            }

            // now set the cursor to the beginning of the function
            //textSelection.MoveToPoint(function.StartPoint);
            void Message(string msg) {
                // Show a message box to prove we were here
                VsShellUtilities.ShowMessageBox(
                    this.package,
                    msg, //$"index: {pt.AbsoluteCharOffset}, path: {doc.FullName}\n+{textSelection.Text.Length}",
                    "Regen",
                    OLEMSGICON.OLEMSGICON_INFO,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            }
        }
    }
}