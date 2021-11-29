using Microsoft.JSInterop;

namespace Resader.Wasm.Services
{
    public class JsService
    {
        private readonly IJSRuntime js;

        public JsService(IJSRuntime js)
        {
            this.js = js;
        }

        public ValueTask<string> GetItem(string key) => this.js.InvokeAsync<string>("localStorage.getItem", key);

        public ValueTask SetItem(string key, string value) => this.js.InvokeVoidAsync("localStorage.setItem", key, value);

        public ValueTask RemoveItem(string key) => this.js.InvokeVoidAsync("localStorage.removeItem", key);

        public ValueTask Alert(string message) => this.js.InvokeVoidAsync("alert", message);

        public ValueTask<bool> Confirm(string message) => this.js.InvokeAsync<bool>("confirm", message);

        public ValueTask GoBack() => this.js.InvokeVoidAsync("history.back");

        public ValueTask<long> GetHeightToBottom() => this.js.InvokeAsync<long>("getHeightToBottom");

        public ValueTask ShowToast(string id) => this.js.InvokeVoidAsync("showToast", id);

        public ValueTask HideToast(string id) => this.js.InvokeVoidAsync("hideToast", id);

        public ValueTask<string> Md5(string input) => this.js.InvokeAsync<string>("md5", input);
    }
}
