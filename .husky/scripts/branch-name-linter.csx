/// <summary>
/// a simple regex linter for branch naming strategy
/// based off https://www.devwithimagination.com/2020/04/13/git-commit-hooks-for-branch-naming-using-husky/
/// </summary>

using System.Text.RegularExpressions;

private var validTypes = new string[] { "bugfix", "chore", "dependabot", "docs", "feature", "hotfix" };
private var pattern = $"^(({string.Join("|", validTypes)})\\/[a-zA-Z0-9\\-_]{{4,}})$";

private var branchName = Args[0];

if (Regex.IsMatch(branchName, pattern))
{
  return 0;
}

Console.ForegroundColor = ConsoleColor.Red;
Console.WriteLine("");
Console.WriteLine($"Invalid branch name: {branchName}");
Console.WriteLine("");
Console.ResetColor();
Console.WriteLine("Branch names must be in the format:");
Console.WriteLine("  <type>/<subject>");
Console.WriteLine("  <type>/<scope>-<subject>");
Console.WriteLine("");
Console.WriteLine("Where:");
Console.WriteLine($"  - <type>: {string.Join(", ", validTypes)}");
Console.WriteLine("  - <scope>: (optional) usually used for story or issue number");
Console.WriteLine("  - <subject>: at least 4 characters long");
Console.WriteLine("");
Console.WriteLine("e.g: 'feature/ABC-123: subject' or 'hotfix/subject'");
Console.WriteLine("");

return 1;
