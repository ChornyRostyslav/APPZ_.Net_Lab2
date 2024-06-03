namespace APPZ_Lab_2
{
    using System;

    public class InvalidInputException : Exception
    {
        public InvalidInputException(string message) : base(message) { }
    }
}