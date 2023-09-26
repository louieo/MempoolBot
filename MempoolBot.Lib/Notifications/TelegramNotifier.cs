using MempoolBot.Lib.Common;
using MempoolBot.Lib.MempoolSpace.Models;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MempoolBot.Lib.Notifications
{
	public class TelegramNotifier: INotifier
    {
        Settings _Settings;
        TelegramBotClient _BotClient;
        CancellationTokenSource _CancellationTokenSource = new CancellationTokenSource();

        long _ChatId;
        Guid _InstanceId = Guid.NewGuid();
        private bool disposedValue;

        public RecommendedFees? LatestFees { get; set; }

        public TelegramNotifier(Settings settings)
        {
            _Settings = settings;
            _BotClient = new TelegramBotClient(_Settings.TelegramBotToken);

            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
            };

            _BotClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: _CancellationTokenSource.Token
            );
        }

		public async Task SendFeesAsync(RecommendedFees currentFees, bool isRepeatNotification)
		{
            try
            {
                if (currentFees == null) return;

                if (_ChatId <= 0)
                {
                    Console.WriteLine("Not sending Telegram message because no ChatId");
                    return;
                }

                var feesMsg = $"{(isRepeatNotification ? "_Repeat Notification_: " : "")}*Current \"economy\" fee*: {currentFees.EconomyFee} sats/vbyte (configured threshold is {_Settings.EconomyRateThreshold} sats/vbyte)\n";
                if (LatestFees != null) feesMsg += $"*Previous \"economy\" fee*: {LatestFees?.EconomyFee} sats/vbyte";

                Console.WriteLine($"Sending message to ChatId {_ChatId} (from InstanceId {_InstanceId} running on {Environment.MachineName} using {_Settings.MempoolApiUrl})");
                Console.WriteLine($"Message: {feesMsg}");

                await _BotClient.SendTextMessageAsync(
                    chatId: _ChatId,
                    text: Utils.EscapeMarkdownSpecialCharacters(feesMsg),
                    parseMode: ParseMode.MarkdownV2,
                    cancellationToken: _CancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending new fee notification to Telegram: {ex}");
            }
        }

        private async Task SendWelcomeMessageAsync(Message message)
        {
            try
            {
                var welcomeMessage = $"Welcome {message?.From?.FirstName} to MempoolBot!\n" +
                                    $"You will receive notifications when the \"economy\" fee goes below {_Settings.EconomyRateThreshold} sats/vbyte\n" +
                                    $"Notifications will be repeated every {_Settings.NotifyRepeatFrequencyMinutes} minute{(_Settings.NotifyRepeatFrequencyMinutes > 1 ? "s" : "")}\n" +
                                    $"I'm running on machine {Environment.MachineName} using Mempool API {_Settings.MempoolApiUrl}";

                await _BotClient.SendTextMessageAsync(
                    chatId: _ChatId,
                    text: Utils.EscapeMarkdownSpecialCharacters(welcomeMessage),
                    parseMode: ParseMode.MarkdownV2,
                    cancellationToken: _CancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending new fee notification to Telegram: {ex}");
            }
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                // Only process Message updates: https://core.telegram.org/bots/api#message
                if (update.Message is not { } message)
                    return;

                // Only process text messages
                if (message.Text is not { } messageText)
                    return;

                // Only care if a new chat
                //if (_ChatId == message.Chat.Id)
                //    return;

                _ChatId = message.Chat.Id;

                Console.WriteLine($"Received a '{messageText}' message in chat {_ChatId}. TelegramUser is {message?.From?.FirstName} {message?.From?.LastName} ({message?.From?.Username})");

                // Supported commands
                if (message?.Text == "/start")
                    await SendWelcomeMessageAsync(message);
                else if (message?.Text == "/fees" && LatestFees != null)
                    await SendFeesAsync(LatestFees, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling Telegram update: {ex}");
            }
        }

        private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_ChatId > 0)
                    {
                        _BotClient.SendTextMessageAsync(
                           chatId: _ChatId,
                           text: $"MempoolBot server on on {Environment.MachineName} is closing. See ya!",
                           cancellationToken: _CancellationTokenSource.Token);
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~TelegramNotifier()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

