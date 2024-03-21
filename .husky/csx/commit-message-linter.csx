/// <summary>
/// a simple regex commit linter example
/// https://www.conventionalcommits.org/en/v1.0.0/
/// https://github.com/angular/angular/blob/22b96b9/CONTRIBUTING.md#type
/// </summary>

using System.Text.RegularExpressions;

private var pattern = @"^(?=.{1,90}$)(?:build|feature|ci|chore|dependabot|docs|fix|perf|refactor|revert|style|test)(?:\(.+\))*(?::).{4,}(?:#\d+)*(?<![\.\s])$";

private var msg = File.ReadAllLines(Args[0])[0];

if (Regex.IsMatch(msg, pattern))
   return 0;


Console.ForegroundColor = ConsoleColor.Red;
Console.WriteLine("");
Console.WriteLine($"Invalid commit message: {msg}");
Console.WriteLine("");
Console.ResetColor();
Console.WriteLine("Commit messages must be in the Conventional Commits format:");
Console.WriteLine("  <type>: <subject>");
Console.WriteLine("  <type>(<scope>): <subject>");
Console.WriteLine("");
Console.WriteLine("Where:");
Console.WriteLine("  - <type>: build, feature, ci, chore, dependabot, docs, fix, perf, refactor, revert, style, test");
Console.WriteLine("  - <scope>: (optional) usually used for story or issue number");
Console.WriteLine("  - <subject>: at least 4 characters long");
Console.WriteLine("");
Console.WriteLine("e.g: 'feature(ABC-123): subject' or 'fix: subject'");
Console.WriteLine("");
Console.WriteLine("More info: https://www.conventionalcommits.org/en/v1.0.0/");

return 1;