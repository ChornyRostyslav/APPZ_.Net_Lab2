using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace APPZ_Lab_2
{
    internal class BotService
    {
        private readonly TelegramBotClient botClient;
        private readonly WeatherService weatherService;

        public BotService(string token, string weatherApiKey)
        {
            botClient = new TelegramBotClient(token);
            weatherService = new WeatherService(weatherApiKey);
        }

        public async Task StartBotAsync()
        {
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };

            botClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions
            );

            var me = await botClient.GetMeAsync();
            Console.WriteLine($"Start listening for @{me.Username}");
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == UpdateType.Message && update.Message?.Text != null)
            {
                await HandleMessageAsync(botClient, update.Message);
            }
        }

        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        private async Task HandleMessageAsync(ITelegramBotClient botClient, Message message)
        {
            var chatId = message.Chat.Id;
            var messageText = message.Text;

            Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

            // Обробка команд
            switch (messageText.Split(' ')[0])
            {
                case "/start":
                    await botClient.SendTextMessageAsync(chatId, "Welcome! Use /help to see available commands.");
                    break;
                case "/help":
                    await botClient.SendTextMessageAsync(chatId, "Available commands: /start, /help, /excursions, /book, /cancel, /weather");
                    break;
                case "/excursions":
                    await HandleExcursionsCommand(chatId);
                    break;
                case "/book":
                    await HandleBookCommand(chatId, messageText);
                    break;
                case "/cancel":
                    await HandleCancelCommand(chatId, messageText);
                    break;
                case "/weather":
                    await HandleWeatherCommand(chatId, messageText);
                    break;
                default:
                    await botClient.SendTextMessageAsync(chatId, "Unknown command. Use /help to see available commands.");
                    break;
            }
        }

        private async Task HandleExcursionsCommand(long chatId)
        {
            // Додаємо список екскурсій
            var excursions = Program.excursions; // передбачається, що екскурсії знаходяться в Program
            var response = "Available excursions:\n";
            foreach (var excursion in excursions)
            {
                response += $"{excursion.Id}: {excursion.Name} - {excursion.Description}\n";
            }
            await botClient.SendTextMessageAsync(chatId, response);
        }

        private async Task HandleBookCommand(long chatId, string messageText)
        {
            // Логіка бронювання екскурсій
            var parts = messageText.Split(' ');
            if (parts.Length < 3)
            {
                await botClient.SendTextMessageAsync(chatId, "Usage: /book <excursion_id> <participants>");
                return;
            }

            if (int.TryParse(parts[1], out int excursionId) && int.TryParse(parts[2], out int participants))
            {
                var excursion = Program.excursions.FirstOrDefault(e => e.Id == excursionId);
                if (excursion != null)
                {
                    var userId = (int)chatId; // Використовуємо chatId як userId
                    excursion.Book(userId, participants);
                    await botClient.SendTextMessageAsync(chatId, $"Booked {participants} participants for excursion {excursion.Name}");
                }
                else
                {
                    await botClient.SendTextMessageAsync(chatId, "Excursion not found.");
                }
            }
            else
            {
                await botClient.SendTextMessageAsync(chatId, "Invalid input. Usage: /book <excursion_id> <participants>");
            }
        }

        private async Task HandleCancelCommand(long chatId, string messageText)
        {
            // Логіка скасування бронювання
            var parts = messageText.Split(' ');
            if (parts.Length < 2)
            {
                await botClient.SendTextMessageAsync(chatId, "Usage: /cancel <excursion_id>");
                return;
            }

            if (int.TryParse(parts[1], out int excursionId))
            {
                var excursion = Program.excursions.FirstOrDefault(e => e.Id == excursionId);
                if (excursion != null)
                {
                    var userId = (int)chatId; // Використовуємо chatId як userId
                    excursion.CancelBooking(userId);
                    await botClient.SendTextMessageAsync(chatId, $"Cancelled booking for excursion {excursion.Name}");
                }
                else
                {
                    await botClient.SendTextMessageAsync(chatId, "Excursion not found.");
                }
            }
            else
            {
                await botClient.SendTextMessageAsync(chatId, "Invalid input. Usage: /cancel <excursion_id>");
            }
        }

        private async Task HandleWeatherCommand(long chatId, string messageText)
        {
            var parts = messageText.Split(' ');
            if (parts.Length < 2)
            {
                await botClient.SendTextMessageAsync(chatId, "Usage: /weather <city>");
                return;
            }

            var city = string.Join(" ", parts.Skip(1));
            var weatherInfo = await weatherService.GetWeatherAsync(city);
            await botClient.SendTextMessageAsync(chatId, weatherInfo);
        }
    }
}
