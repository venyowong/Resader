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

        public async Task<string> GetItem(string key)
        {
            return await this.js.InvokeAsync<string>("localStorage.getItem", key);
        }

        public async Task SetItem(string key, string value)
        {
            await this.js.InvokeVoidAsync("localStorage.setItem", key, value);
        }

        public async Task RemoveItem(string key)
        {
            await this.js.InvokeVoidAsync("localStorage.removeItem", key);
        }

        public async Task Alert(string message)
        {
            await js.InvokeVoidAsync("alert", message);
        }

        public async Task GoBack()
        {
            await js.InvokeVoidAsync("history.back");
        }
    }
}
