//using Microsoft.JSInterop;

//namespace CourseworkPastPaperApplication2.Client.Shared
//{
//    public class UnloadEventHelper
//    {
//        private readonly Func<EventArgs, Task> _callback;

//        public UnloadEventHelper(Func<EventArgs, Task> callback)
//        {
//            _callback = callback;
//        }

//        [JSInvokable]
//        public Task OnUnloadEvent(EventArgs args) => _callback(args);
//    }

//    public class UnloadEventInterop : IDisposable
//    {
//        private readonly IJSRuntime _jsRuntime;
//        private DotNetObjectReference<UnloadEventHelper> Reference;

//        public UnloadEventInterop(IJSRuntime jsRuntime)
//        {
//            _jsRuntime = jsRuntime;
//        }

//        public ValueTask<string> SetupUnloadEventCallback(Func<EventArgs, Task> callback)
//        {
//            Reference = DotNetObjectReference.Create(new ScrollEventHelper(callback));
//            // addUnloadEventListener will be a js function we create later
//            return _jsRuntime.InvokeAsync<string>("addUnloadEventListener", Reference);
//        }

//        public void Dispose()
//        {
//            Reference?.Dispose();
//        }
//    }
//}
