namespace APPZ_Lab_2
{
    using System;

    public class ExcursionNotFoundException : Exception
    {
        public ExcursionNotFoundException(string message) : base(message) { }
    }
}