﻿namespace Sudoku.Platforms.QQ.Commands;

/// <summary>
/// Indicates the ranking command.
/// </summary>
[Command(Permissions.Owner, Permissions.Administrator)]
file sealed class RankingCommand : Command
{
	/// <inheritdoc/>
	public override string CommandName => R.Command("Ranking")!;

	/// <inheritdoc/>
	public override CommandComparison ComparisonMode => CommandComparison.Prefix;


	/// <inheritdoc/>
	protected override async Task<bool> ExecuteCoreAsync(string args, GroupMessageReceiver e)
	{
		if (e is not { Sender.Group: var group })
		{
			return false;
		}

		var folder = Environment.GetFolderPath(SpecialFolder.MyDocuments);
		if (!Directory.Exists(folder))
		{
			// Error. The computer does not contain "My Documents" folder.
			// This folder is special; if the computer does not contain the folder, we should return directly.
			return true;
		}

		// If the number of members are too large, we should only iterate the specified number of elements from top.
		var context = BotRunningContext.GetContext(group);
		var usersData = (await ICommandDataProvider.GetUserRankingListAsync(group, rankingEmptyCallback))!
			.Take(context?.Configuration.RankingDisplayUsersCount ?? 10);

		var rankingStr = string.Join(Environment.NewLine, usersData.Select(selector));
		await e.SendMessageAsync(
			new MessageChainBuilder()
				.Plain(R.MessageFormat("RankingResult")!)
				.Plain(Environment.NewLine)
				.Plain("---")
				.Plain(Environment.NewLine)
				.Plain(rankingStr)
				.Build()
		);
		return true;


		async Task rankingEmptyCallback() => await e.SendMessageAsync(R.MessageFormat("RankingListIsEmpty")!);

		static string selector((string Name, UserData Data) pair, int i)
		{
			if (pair is not (var name, { QQ: var qq, Score: var score }))
			{
				throw new();
			}

			var openBrace = R.Token("OpenBrace")!;
			var closedBrace = R.Token("ClosedBrace")!;
			var colon = R.Token("Colon")!;
			var comma = R.Token("Comma")!;
			var scoreSuffix = R["ExpString"]!;
			var grade = Scorer.GetGrade(score);
			var gradeSuffix = R["GradeString"]!;
			return $"#{i + 1}{colon}{name}{openBrace}{qq}{closedBrace}{comma}{score} {scoreSuffix}{comma}{grade} {gradeSuffix}";
		}
	}
}
