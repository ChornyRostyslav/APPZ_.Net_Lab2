using System.Threading.Tasks;

namespace APPZ_Lab_2
{
    class Program
    {
        public static List<IExcursion> excursions = new List<IExcursion>();
        public static List<IUser> users = new List<IUser>();

        static async Task Main(string[] args)
        {
            excursions.Add(new Excursion(1, "New York Tour", "Explore the city with our guide."));
            excursions.Add(new Excursion(2, "Louvre (Paris, France)", "Visit the famous museum in town."));

            var botService = new BotService("Токен телеграм бота", "Токен OpenWeatherMap");
            await botService.StartBotAsync();

            Console.ReadLine();
        }
    }
}
