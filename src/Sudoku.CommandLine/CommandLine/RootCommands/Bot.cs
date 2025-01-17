﻿#define AUTO_SEND_MESSAGE_AFTER_MEMBER_JOINED
#define ALLOW_MEMBER_REQUEST
#undef ALLOW_INVITATION
#define BASIC_LOG_INFO_OUTPUT
#define ALLOW_PERIODIC_OPERATION

namespace Sudoku.CommandLine.RootCommands;

/// <summary>
/// Represents a bot command.
/// </summary>
[RootCommand("bot", DescriptionResourceKey = "_Description_Bot")]
[SupportedArguments("bot")]
[Usage("bot -a <address> -q <number> -k <key>", IsPattern = true)]
[Usage("bot -a localhost:8080 -q 1357924680 -k HelloWorld", DescriptionResourceKey = "_Usage_Bot_1")]
[SupportedOSPlatform("windows")]
file sealed class Bot : IExecutable
{
	/// <summary>
	/// The address.
	/// </summary>
	[DoubleArgumentsCommand('a', "address", DescriptionResourceKey = "_Description_Address_Bot")]
	public string Address { get; set; } = "localhost:8080";

	/// <summary>
	/// The number of the bot.
	/// </summary>
	[DoubleArgumentsCommand('q', "qq", DescriptionResourceKey = "_Description_BotNumber_Bot", IsRequired = true)]
	public string BotNumber { get; set; } = string.Empty;

	/// <summary>
	/// The verify key.
	/// </summary>
	[DoubleArgumentsCommand('k', "key", DescriptionResourceKey = "_Description_VerifyKey_Bot", IsRequired = true)]
	public string VerifyKey { get; set; } = string.Empty;


	/// <inheritdoc/>
	public async Task ExecuteAsync(CancellationToken cancellationToken = default)
	{
		using var bot = new MiraiBot { Address = Address, QQ = BotNumber, VerifyKey = VerifyKey };

		try
		{
			await bot.LaunchAsync();

			bot.SubscribeJoined(OnBotJoined);
			bot.SubscribeLeft(OnBotLeft);
			bot.SubscribeKicked(OnBotKicked);
			bot.SubscribeGroupMessageReceived(OnGroupMessageReceivedAsync);
#if AUTO_SEND_MESSAGE_AFTER_MEMBER_JOINED
			bot.SubscribeMemberJoined(OnMemberJoinedAsync);
#endif
#if ALLOW_MEMBER_REQUEST
			bot.SubscribeNewMemberRequested(OnNewMemberRequestedAsync);
#endif
#if ALLOW_INVITATION
			bot.SubscribeNewInvitationRequested(OnNewInvitationRequestedAsync);
#endif
			bot.SubscribeMuted();
			bot.SubscribeUnmuted();

			(await AccountManager.GetGroupsAsync()).ForEach(static group => RunningContexts.TryAdd(group.Id, new()));

#if BASIC_LOG_INFO_OUTPUT
			await Terminal.WriteLineAsync(R["_Message_BootSuccess"]!, ConsoleColor.DarkGreen);
#endif
		}
		catch (Exception ex)
		{
#if BASIC_LOG_INFO_OUTPUT
			await Terminal.WriteLineAsync(
				ex switch
				{
					FlurlHttpException => R["_Message_BootFailed_Mirai"]!,
					InvalidResponseException => R["_Message_BootFailed_Connection"]!,
					_ => throw ex
				},
				ConsoleColor.DarkRed
			);
#endif
		}

#if ALLOW_PERIODIC_OPERATION
		PeriodicOperationPool.Shared.EnqueueRange(
			from type in typeof(PeriodicOperation).Assembly.GetTypes()
			where type.IsAssignableTo(typeof(PeriodicOperation))
			let constructor = type.GetConstructor(Array.Empty<Type>())
			where constructor is not null
			let operation = Activator.CreateInstance(type) as PeriodicOperation
			where operation is not null
			select operation
		);
#endif

		Terminal.Pause();
	}

	/// <summary>
	/// Triggers when bot has already left the group.
	/// </summary>
	/// <param name="e">The event handler.</param>
	private void OnBotLeft(LeftEvent e) => RunningContexts.TryRemove(e.Group.Id, out _);

	/// <summary>
	/// Triggers when bot has already been kicked by group owner.
	/// </summary>
	/// <param name="e">The event handler.</param>
	private void OnBotKicked(KickedEvent e) => RunningContexts.TryRemove(e.Group.Id, out _);

	/// <summary>
	/// Triggers when bot has already joined in the target group.
	/// </summary>
	/// <param name="e">The event handler.</param>
	private void OnBotJoined(JoinedEvent e) => RunningContexts.TryAdd(e.Group.Id, new());

	/// <summary>
	/// Triggers when the group has been received a new message.
	/// </summary>
	/// <param name="e">The event handler.</param>
	private async void OnGroupMessageReceivedAsync(GroupMessageReceiver e)
	{
		switch (e)
		{
			case
			{
				GroupId: var groupId,
				Sender: var sender,
				MessageChain: [SourceMessage, AtMessage { Target: var qq }, PlainMessage { Text: var message }]
			}
			when qq == BotNumber
				&& RunningContexts.TryGetValue(groupId, out var context)
				&& context is { AnsweringContext.CurrentRoundAnsweredValues: { } answeredValues }:
			{
				// At message: special use. Gaming will rely on this case.
				// I think this way to handle messages is too complex and ugly. I may change the design on commands later.
				var trimmed = message.Trim();
				if (int.TryParse(trimmed, out var validInteger))
				{
					answeredValues.Add(new(sender, validInteger));
				}

				break;
			}
			case { Sender.Permission: var permission, MessageChain: (_, { } messageTrimmed, _) }:
			{
				// Normal command message.
				foreach (var type in typeof(CommandAttribute).Assembly.GetDerivedTypes<Command>())
				{
					if (type.GetConstructor(Array.Empty<Type>()) is not null
						&& type.GetCustomAttribute<CommandAttribute>() is { AllowedPermissions: var allowPermissions, IsDeprecated: false }
						&& Array.IndexOf(allowPermissions, permission) != -1
						&& await ((Command)Activator.CreateInstance(type)!).ExecuteAsync(messageTrimmed, e))
					{
						return;
					}
				}

				break;
			}
		}
	}

#if ALLOW_INVITATION
	/// <summary>
	/// Triggers when someone has been invited by another one to join in this group.
	/// </summary>
	/// <param name="e">The event handler.</param>
	private async void OnNewInvitationRequestedAsync(NewInvitationRequestedEvent e)
	{
		if (e is { GroupId: var groupId } && groupId == R["SudokuGroupQQ"])
		{
			await e.ApproveAsync();
		}
	}
#endif

#if ALLOW_MEMBER_REQUEST
	/// <summary>
	/// Triggers when someone has requested that he wants to join in this group.
	/// </summary>
	/// <param name="e">The event handler.</param>
	private async void OnNewMemberRequestedAsync(NewMemberRequestedEvent e)
	{
		const string answerLocatorStr = "\u7b54\u6848\uff1a";

		if (e is { GroupId: var groupId, Message: var message }
			&& message.IndexOf(answerLocatorStr) is var answerLocatorStrIndex and not -1
			&& answerLocatorStrIndex + answerLocatorStr.Length is var finalIndex && finalIndex < message.Length
			&& message[finalIndex..] is var finalMessage
			&& groupId == R["SudokuGroupQQ"])
		{
			if (BilibiliPattern().IsMatch(finalMessage.Trim()))
			{
				await e.ApproveAsync();
			}
			else
			{
				await e.RejectAsync(R["_MessageFormat_MemberJoinedRejected"]!);
			}
		}
	}
#endif

#if AUTO_SEND_MESSAGE_AFTER_MEMBER_JOINED
	/// <summary>
	/// Triggers when someone has already joined in this group.
	/// </summary>
	/// <param name="e">The event handler.</param>
	private async void OnMemberJoinedAsync(MemberJoinedEvent e)
	{
		if (e.Member.Group is { Id: var groupId } group && groupId == R["SudokuGroupQQ"])
		{
			await group.SendGroupMessageAsync(R["_MessageFormat_SampleMemberJoined"]!);
		}
	}
#endif
}
