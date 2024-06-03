namespace APPZ_Lab_2
{
    public interface IExcursion
    {
        int Id { get; }
        string Name { get; set; }
        string Description { get; set; }
        void Book(int userId, int participants);
        void CancelBooking(int userId);

        event Action<int, int> BookingCreated;
        event Action<int> BookingCanceled;
    }
}
