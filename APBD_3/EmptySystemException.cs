namespace APBD_3;
class EmptySystemException : Exception
{
    public EmptySystemException() : base("Operation system is not installed.") { }
}