using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace PatternMapper
{
    /// <summary>
    /// Interaction logic for PatternMapperToolWindowControl.
    /// </summary>
    public partial class PatternMapperToolWindowControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PatternMapperToolWindowControl"/> class.
        /// </summary>
        public PatternMapperToolWindowControl()
        {
            this.InitializeComponent();
            Loaded += OnLoadedEventHandler;
        }

        private void OnLoadedEventHandler(object sender, RoutedEventArgs e)
        {
            FolderView.Items.Clear();

            foreach ( var proj in getProjectsInSolution( getCurrentSolution() ))
            {
                var item = new TreeViewItem();

                item.Header = proj.Name;
                item.Tag = proj.UniqueName;

                item.Items.Add(null);

                item.Expanded += Folder_Expanded;

                FolderView.Items.Add(item);
            }
        }

        private static Project getProjectByName(IVsSolution solution, string uniqueName)
        {
            IVsHierarchy ppHierarchy;
            solution.GetProjectOfUniqueName(uniqueName, out ppHierarchy);

            return asProject(ppHierarchy);
        }

        private static List<Project> getProjectsInSolution(IVsSolution solution)
        {
            var projects = new List<Project>();
            var nullGuid = Guid.Empty;
            solution.GetProjectEnum((uint)__VSGETPROJFILESFLAGS.GPFF_SKIPUNLOADEDPROJECTS, ref nullGuid, out IEnumHierarchies enumPtr);

            if (enumPtr != null)
            {
                uint actualResult = 0;
                IVsHierarchy[] nodes = new IVsHierarchy[1];

                while (0 == enumPtr.Next(1, nodes, out actualResult))
                {
                    Project project = asProject(nodes[0]);
                    if (project != null)
                    {
                        projects.Add(project);
                    }
                }
            }
            return projects;
        }

        private static Project asProject(IVsHierarchy hierarchy)
        {
            Object obj;
            hierarchy.GetProperty((uint)Microsoft.VisualStudio.VSConstants.VSITEMID_ROOT, (int)Microsoft.VisualStudio.Shell.Interop.__VSHPROPID.VSHPROPID_ExtObject, out obj);

            return (Project) obj;
        }

        private static IVsSolution getCurrentSolution()
        {
            IVsSolution solution = (IVsSolution)Package.GetGlobalService(typeof(IVsSolution));
            solution.GetSolutionInfo(out string solutionDirectory, out string solutionName, out string solutionDirectory2);

            return solution;
        }

        private void Folder_Expanded(object sender, RoutedEventArgs e)
        {
            var item = (TreeViewItem) sender;

            if (item.Items.Count != 1 || item.Items[0] != null)
                return;

            item.Items.Clear();

            //var proj = (Project)item.Tag;

            item.Items.Add(Directory.GetFiles(item.Tag.ToString()));

            //try
            //{
            //    var dirs = Directory.GetDirectories(fullPath);

            //    if (dirs.Length > 0)
            //        directories.AddRange(dirs);
            
            //}
            //catch { }

            //directories.ForEach(directoryPath =>
            //{
            //    var subItem = new TreeViewItem()
            //    {
            //        Header = Path.GetDirectoryName(directoryPath),
            //        Tag = directoryPath
            //    };

            //    subItem.Items.Add(null);

            //    subItem.Expanded += Folder_Expanded;

            //    item.Items.Add(subItem);
            //});
            
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                string.Format(System.Globalization.CultureInfo.CurrentUICulture, "Invoked '{0}'", this.ToString()),
                "PatternMapperToolWindow");
        }
    }
}