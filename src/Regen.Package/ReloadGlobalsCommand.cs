using System;
using System.ComponentModel.Design;
using System.IO;
using System.Media;
using System.Text.RegularExpressions;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Regen.Compiler;
using Regen.Engine;
using Regen.Helpers;
using Regen.Parser;
using Task = System.Threading.Tasks.Task;

namespace Regen {
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class ReloadGlobalsCommand {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0106;

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
        private ReloadGlobalsCommand(AsyncPackage package, OleMenuCommandService commandService) {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
            Execute(null, null);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static ReloadGlobalsCommand Instance { get; private set; }

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
            Instance = new ReloadGlobalsCommand(package, commandService);
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
            string solutionDir = System.IO.Path.GetDirectoryName(dte.Solution.FullName);

            Logger.Log("Searching for *.regen files at: "+solutionDir);
            var files = Directory.GetFiles(solutionDir, "*.regen", SearchOption.AllDirectories);
            if (files.Length == 0)
                return;

            RegenEngine.Globals.Clear(); //clear existing.

            foreach (var file in files) {
                Logger.Log($"Loading globals from {file}");

                try {
                    var content = File.ReadAllText(file);
                    var compiler = new RegenCompiler();
                    compiler.CompileGlobal(content); //just compile to see if it is compilable.
                    RegenEngine.Globals.Add(content);
                } catch (Exception e) {
                    Logger.Log($"Failed parsing \"{file}\", stopping load...");
                    Logger.Log(e);
                    break;
                }
            }

            // now set the cursor to the beginning of the function
            //textSelection.MoveToPoint(function.StartPoint);
            void Message(string msg) {
                // Show a message box to prove we were here
                VsShellUtilities.ShowMessageBox(
                    this.package,
                    msg,
                    "Regen",
                    OLEMSGICON.OLEMSGICON_INFO,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            }
        }
    }
}