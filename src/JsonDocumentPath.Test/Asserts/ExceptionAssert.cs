using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace JDocument.Test
{
    public static class ExceptionAssert
    {
        public static TException Throws<TException>(Action action, params string[] possibleMessages) where TException : Exception
        {
            try
            {
                action();
                //Assert.Throws<TException>(action);
                //Assert.Fail("Exception of type {0} expected. No exception thrown.", typeof(TException).Name);
                return null;
            }
            catch (TException ex)
            {
                if (possibleMessages == null || possibleMessages.Length == 0)
                {
                    return ex;
                }
                foreach (string possibleMessage in possibleMessages)
                {
                    if (StringAssert.Equals(possibleMessage, ex.Message))
                    {
                        return ex;
                    }
                }

                throw new Exception("Unexpected exception message." + Environment.NewLine + "Expected one of: " + string.Join(Environment.NewLine, possibleMessages) + Environment.NewLine + "Got: " + ex.Message + Environment.NewLine + Environment.NewLine + ex);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Exception of type {0} expected; got exception of type {1}.", typeof(TException).Name, ex.GetType().Name), ex);
            }
        }
    }
}
