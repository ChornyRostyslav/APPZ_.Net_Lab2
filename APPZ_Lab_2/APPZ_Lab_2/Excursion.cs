namespace APPZ_Lab_2
{
    public class Excursion : IExcursion
    {
        public int Id { get; private set; }
        private string name;
        private string description;
        private Dictionary<int, int> bookings = new Dictionary<int, int>();

        public string Name
        {
            get => name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new InvalidInputException("Name cannot be empty.");
                name = value;
            }
        }

        public string Description
        {
            get => description;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new InvalidInputException("Description cannot be empty.");
                description = value;
            }
        }

        public event Action<int, int> BookingCreated;
        public event Action<int> BookingCanceled;

        public Excursion(int id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }

        public void Book(int userId, int participants)
        {
            if (participants <= 0)
                throw new InvalidInputException("Number of participants must be greater than zero.");

            if (!bookings.ContainsKey(userId))
            {
                bookings[userId] = participants;
                BookingCreated?.Invoke(userId, participants);
            }
        }

        public void CancelBooking(int userId)
        {
            if (bookings.ContainsKey(userId))
            {
                bookings.Remove(userId);
                BookingCanceled?.Invoke(userId);
            }
        }

        public Dictionary<int, int> GetBookings()
        {
            return new Dictionary<int, int>(bookings);
        }

        public override string ToString()
        {
            return $"{Id}: {Name} - {Description}";
        }
    }
}