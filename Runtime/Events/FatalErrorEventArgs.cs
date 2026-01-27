using EasyGameFramework.Core;
using EasyGameFramework.Core.Event;

namespace EasyGameFramework.Bootstrap
{
    public class FatalErrorEventArgs : GameEventArgs
    {
        public string Message { get; private set; }

        public static FatalErrorEventArgs Create(string message = null)
        {
            var eventArgs = ReferencePool.Acquire<FatalErrorEventArgs>();
            eventArgs.Message = message;
            return eventArgs;
        }

        public override void Clear()
        {
            Message = null;
        }
    }
}
