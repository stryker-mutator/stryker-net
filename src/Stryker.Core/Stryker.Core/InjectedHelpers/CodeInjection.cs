﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Stryker.Core.InjectedHelpers
{
    public static class CodeInjection
    {
        // files to be injected into the mutated assembly
        private static readonly string[] Files = {"Stryker.Core.InjectedHelpers.MutantControl.cs",
            "Stryker.Core.InjectedHelpers.Coverage.CommunicationChannel.cs"};
        private const string PatternForCheck = "\\/\\/ *check with: *([^\\r\\n]+)";

        public static readonly string SelectorExpression;
        public static readonly string HelperNamespace;

        static CodeInjection()
        {
            var helper = GetSourceFromResource("Stryker.Core.InjectedHelpers.MutantControl.cs");
            var extractor = new Regex(PatternForCheck);
            var result = extractor.Match(helper);
            if (!result.Success)
            {
                throw new InvalidDataException("Internal error: failed to find expression for mutant selection.");
            }

            HelperNamespace = GetRandomNamespace();
            SelectorExpression = result.Groups[1].Value.Replace("Stryker", HelperNamespace);
            foreach (var file in Files)
            {
                var fileContents = GetSourceFromResource(file).Replace("Stryker", HelperNamespace);
                MutantHelpers.Add(file, fileContents);
            }
        }

        private static string GetRandomNamespace()
        {
            // Create a string of characters, numbers, special characters that allowed in the password  
            const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();

            var chars = new char[15];
            for (int i = 0; i < 15; i++)
            {
                chars[i] = validChars[random.Next(0, validChars.Length)];
            }
            return "Stryker" + new string(chars);
        }

        public static IDictionary<string, string> MutantHelpers { get; } = new Dictionary<string, string>();

        private static string GetSourceFromResource(string sourceResourceName)
        {
            string helper;
            using (var stream =
                typeof(CodeInjection).Assembly.GetManifestResourceStream(sourceResourceName))
            {
                using (var reader = new StreamReader(stream))
                {
                    helper = reader.ReadToEnd();
                }
            }

            return helper;
        }
    }
}
