using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Workflow.Providers.GlassGraphClient.Models;

namespace Workflow.Providers.GlassGraphClient.FunctionalAreas
{
    public class FilesAnalyzer
    {
        public static Dictionary<string, List<string>> TechnologyExtensions = new Dictionary<string, List<string>> {
            { "cs", new List<string> { ".cs", ".csproj", ".xproj", ".ksproj" } },
            { "vb", new List<string> { ".vpb",".cls"} },
            { "web", new List<string> { ".html", ".css", ".less", ".php", ".php3", ".php4", ".php5", ".php7", ".sass", ".scss"} },
            { "js", new List<string> { ".js"} },
            { "ts", new List<string> { ".ts"} },
            { "py", new List<string> { ".py", ".py3", ".pyw"} },
            { "apex", new List<string> { ".apex"} },
            { "c", new List<string> { ".c", ".h"} },
            { "cobol", new List<string> { ".cbl"} },
            { "cpp", new List<string> { ".cpp", ".h", ".hpp"} },
            { "delphi", new List<string> { ".dfm", ".dpr"} },
            { "erlang", new List<string> { ".er"} },
            { "fortan", new List<string> { ".f90"} },
            { "fs", new List<string> { ".fs"} },
            { "go", new List<string> { ".go"} },
            { "java", new List<string> { ".java"} },
            { "kotlin", new List<string> { ".kt"} },
            { "lisp", new List<string> { ".lisp"} },
            { "matlab", new List<string> { ".m"} },
            { "objectivec", new List<string> { ".mm"} },
            { "pascal", new List<string> { ".ps"} },
            { "perl", new List<string> { ".pl", ".pm"} },
            { "powershell", new List<string> { ".ps1"} },
            { "r", new List<string> { ".r"} },
            { "ruby", new List<string> { ".rb"} },
            { "rust", new List<string> { ".rs"} },
            { "scala", new List<string> { ".scala"} },
            { "smalltalk", new List<string> { ".st"} },
            { "sql", new List<string> { ".sql", ".pls" } },
            { "swift", new List<string> { ".swift"} },
            { "visualbasic", new List<string> { ".cls", ".frm", ".bas", ".vbp"} },
            { "visualbasicnet", new List<string> { ".vb" } }
        };

        public List<FunctionalAreaItem> Run(List<FileDataItem> data, string technology)
        {
            var inputData = new Dictionary<string, int>();

            var files = data.Where(w => TechnologyExtensions[technology].Contains(w.Extension)).ToList();

            files.ForEach(f =>
            {
                var item = f.Filename.Replace("/", ".").Trim('.');
                var listWords = item.Split(".", item.Split(".").Length - 1).ToList();

                var filename = listWords.Last();
                listWords = listWords.Take(listWords.Count - 1).ToList();
                foreach (Match match in Regex.Matches(filename, "[A-Z](?:[a-z]+|[A-Z]*(?=[A-Z]|$))"))
                {
                    listWords.Add(match.Value);
                }

                var key = string.Join(".", listWords);
                if (inputData.ContainsKey(key))
                {
                    inputData[key]++;
                }
                else
                {
                    inputData.Add(key, 1);
                }
            });

            var tree = CalcHelper.CreateTree(inputData);

            var result = CalcHelper.RelativeChildrenCount(tree)
                .OrderBy(o => o.Level)
                .ThenByDescending(o => o.Pct)
                .ToList();

            return result.Select(s => new FunctionalAreaItem { Name = s.Name, Count = s.Count }).ToList();
        }
    }
}
