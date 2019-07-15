using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Regen.Compiler;
using Regen.Engine;
using Regen.Helpers;
using Task = System.Threading.Tasks.Task;

namespace Regen {
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class CompileTemplateCommand {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0105;

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
        private CompileTemplateCommand(AsyncPackage package, OleMenuCommandService commandService) {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static CompileTemplateCommand Instance { get; private set; }

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
            Instance = new CompileTemplateCommand(package, commandService);
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

            TextDocument templateDoc = doc?.Object() as TextDocument;
            if (dte == null || doc == null || templateDoc == null) {
                SystemSounds.Beep.Play();
                return;
            }

            var templatePath = doc.FullName;
            var templateContent = templateDoc.GetText().Replace("\r", "");

            //compile outputs
            var outputs = RegenFileTemplateEngine.Compile(templateContent, templatePath);

            //find out if these files exist
            var projitem = doc.ProjectItem.ContainingProject;
            Logger.Log(projitem.FullName);
            Logger.Log(projitem.FileName);
            var solution = Package.GetGlobalService(typeof(IVsSolution)) as IVsSolution;
            var projects = GetProjects(solution);

            Project targetProject = null;
            List<ProjectItem> targetItems = null;

            //get current project and its files
            foreach (Project project in projects) {
                if (!projitem.FullName.Equals(project.FullName))
                    continue;
                targetProject = project;
                Logger.Log(project.FullName);
                targetItems = new List<ProjectItem>(project.ProjectItems.Cast<ProjectItem>());
                foreach (ProjectItem projectItem in targetItems) {
                    Logger.Log("    " + projectItem.Name);
                }

                break;
            }

            if (targetProject == null || targetItems == null)
                throw new Exception("ERR #0010");

            //every output either write file or replace contents.
            foreach ((string path, string content) in outputs) {

                //we compile regen frames
                var output = RegenEngine.Compile(content);

                //we add the compiled file to solution
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllText(path, output, Encoding.UTF8);
                AddFile(targetProject, path);
            }

            //now after we successfully outputted these files
            //we need to compile the files



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

        List<string> GetAllProjectFiles(ProjectItems projectItems, string extension) {
            List<string> returnValue = new List<string>();

            foreach (ProjectItem projectItem in projectItems) {
                for (short i = 1; i <= projectItems.Count; i++) {
                    string fileName = projectItem.FileNames[i];
                    if (Path.GetExtension(fileName).ToLower() == extension)
                        returnValue.Add(fileName);
                }

                returnValue.AddRange(GetAllProjectFiles(projectItem.ProjectItems, extension));
            }

            return returnValue;
        }

        void AddFolderAndFiles(EnvDTE.Project project, string folderName) {
            EnvDTE.ProjectItem folder;
            string folderFullName;
            string[] fileFullNames;

            try {
                folder = project.ProjectItems.AddFolder(folderName);

                folderFullName = folder.FileNames[0];

                Logger.Log(folderFullName);
                //CreateFiles(folderFullName);

                fileFullNames = System.IO.Directory.GetFiles(folderFullName);

                foreach (string fileFullName in fileFullNames) {
                    folder.ProjectItems.AddFromFile(fileFullName);
                }
            } catch (Exception ex) {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        void AddFile(EnvDTE.Project project, string file) {
            project.ProjectItems.AddFromFile(file);
            return;
            try
            {

                var target = new FileInfo(Path.GetDirectoryName(file)).FullName.Remove(0, new FileInfo(project.FullName).Directory.FullName.Length);

                if (!string.IsNullOrEmpty(target)) {
                    //var folder = GetOrCreate(project, target);
                    //folder.ProjectItems.AddFromFile(file);
                } else {
                    project.ProjectItems.AddFromFile(file);
                }
            } catch (Exception ex) {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        ProjectItem GetOrCreate(Project project, string path, ProjectItem projectItem = null) {
            ThreadHelper.ThrowIfNotOnUIThread();

            ProjectItem Iterate(ProjectItem it, string[] allparts, int index) {
                if (index >= allparts.Length)
                    return it;

                var currpart = allparts[index];
                var projitem = project.ProjectItems.Cast<ProjectItem>().FirstOrDefault(pi => pi.Name == currpart);
                if (projitem == null)
                    return Iterate(it.ProjectItems.AddFolder(currpart), allparts, index + 1);
                return Iterate(projitem, allparts, index + 1);
            }

            var parts = path.Trim('\\', '/').TrimStart('.').Replace("\\", "/").Split('/');
            if (projectItem == null) {
                var projpart = parts.First();
                var dir = project.ProjectItems.Cast<ProjectItem>().FirstOrDefault(pi => pi.Name == projpart);
                return Iterate(dir ?? project.ProjectItems.AddFolder(projpart, EnvDTE.Constants.vsProjectItemKindPhysicalFolder), parts, 1);
            } else {
                return Iterate(projectItem, parts, 0);
            }
        }

        List<string> GetFilesNotInProject(Project project) {
            List<string> returnValue = new List<string>();
            string startPath = Path.GetDirectoryName(project.FullName);
            List<string> projectFiles = GetAllProjectFiles(project.ProjectItems, ".cs");

            foreach (var file in Directory.GetFiles(startPath, "*.cs", SearchOption.AllDirectories))
                if (!projectFiles.Contains(file))
                    returnValue.Add(file);

            return returnValue;
        }

        IEnumerable<EnvDTE.Project> GetProjects(IVsSolution solution) {
            foreach (IVsHierarchy hier in GetProjectsInSolution(solution)) {
                EnvDTE.Project project = GetDTEProject(hier);
                if (project != null)
                    yield return project;
            }
        }

        IEnumerable<IVsHierarchy> GetProjectsInSolution(IVsSolution solution, __VSENUMPROJFLAGS flags = __VSENUMPROJFLAGS.EPF_LOADEDINSOLUTION) {
            if (solution == null)
                yield break;

            IEnumHierarchies enumHierarchies;
            Guid guid = Guid.Empty;
            solution.GetProjectEnum((uint) flags, ref guid, out enumHierarchies);
            if (enumHierarchies == null)
                yield break;

            IVsHierarchy[] hierarchy = new IVsHierarchy[1];
            uint fetched;
            while (enumHierarchies.Next(1, hierarchy, out fetched) == VSConstants.S_OK && fetched == 1) {
                if (hierarchy.Length > 0 && hierarchy[0] != null)
                    yield return hierarchy[0];
            }
        }

        EnvDTE.Project GetDTEProject(IVsHierarchy hierarchy) {
            if (hierarchy == null)
                throw new ArgumentNullException("hierarchy");

            object obj;
            hierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int) __VSHPROPID.VSHPROPID_ExtObject, out obj);
            return obj as EnvDTE.Project;
        }
    }
}