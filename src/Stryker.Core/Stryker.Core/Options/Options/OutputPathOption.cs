using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.IO.Abstractions;

namespace Stryker.Core.Options.Options
{
	public class OutputPathOption : BaseStrykerOption<string>
	{
        public OutputPathOption(string basepath, IFileSystem fileSystem, ILogger logger) : base(basepath)
		{
            var outputPath = Path.Combine(basepath, "StrykerOutput", DateTime.Now.ToString("yyyy-MM-dd.HH-mm-ss"));
            fileSystem.Directory.CreateDirectory(FilePathUtils.NormalizePathSeparators(outputPath));

            // Create output dir with gitignore
            var gitignorePath = FilePathUtils.NormalizePathSeparators(Path.Combine(basepath, "StrykerOutput", ".gitignore"));
            if (!fileSystem.File.Exists(gitignorePath))
            {
                try
                {
                    fileSystem.File.WriteAllText(gitignorePath, "*");
                }
                catch (IOException e)
                {
                    logger.LogWarning("Could't create gitignore file because of error {error}. \n" +
                        "If you use any diff compare features this may mean that stryker logs show up as changes.", e.Message);
                }
            }
        }

		public override StrykerOption Type => StrykerOption.OutputPath;
		public override string HelpText => "";
		public override string DefaultValue => "";
		protected override void Validate(params string[] parameters)
		{
            foreach (var param in parameters)
            {
                if (string.IsNullOrWhiteSpace(param))
                {
                    return;
                }
            }
        }
	}
}
