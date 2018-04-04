using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EscapeDBUsage.Helpers
{
    public static class FoldersHelper
    {
        public static string DataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"\EscapeDBUsage\Data");
        public static string DataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"\EscapeDBUsage\Data\sprints.json");
    }
}
