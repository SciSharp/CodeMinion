using System;
using EnvDTE;

namespace Regen.Helpers {
    public static class DocumentHelper {
        public static string GetText(this EditPoint startPoint, EditPoint endPoint) {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            return startPoint.GetText(endPoint);
        }

        public static string GetText(this TextDocument textDocument) {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            var startPoint = textDocument.StartPoint.CreateEditPoint();
            var endPoint = textDocument.EndPoint.CreateEditPoint();
            return startPoint.GetText(endPoint);

            //startPoint.ReplaceText(endPoint, xamlSource, 0);
        }

        public static Document GetActiveDocument(this DTE dte) {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            return dte.ActiveDocument?.ActiveWindow?.Document;
        }

        public static TextDocument GetTextDocument(this Document document) {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            return (TextDocument) document.Object("TextDocument");
        }

        public static TextDocument GetActiveTextDocument(this DTE dte) {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            return (TextDocument) dte.GetActiveDocument().Object("TextDocument");
        }

        public static void ActiveDocumentsAction(this DTE dte, Func<TextDocument, bool> func, bool issave = true) {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            foreach (var item in dte.ActiveDocument.Collection) {
                if (item is Document document) {
                    var selection = document.Selection;
                    if (selection != null) {
                        var textDocument = document.GetTextDocument();
                        //var abbb = textDocument.ReplaceText("！", "!");
                        if (func.Invoke(textDocument) && issave) {
                            document.Save();
                        }
                    }
                }
            }
        }
    }
}