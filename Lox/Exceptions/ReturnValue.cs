namespace Lox.Exceptions
{
    public class ReturnValue : Exception
    {
        public readonly object returnValue;

        public ReturnValue(object returnValue)
        {
            this.returnValue = returnValue;
        }
    }
}
