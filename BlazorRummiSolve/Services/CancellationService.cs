using Microsoft.JSInterop;

namespace BlazorRummiSolve.Services;

public class CancellationService
{
    private readonly IJSRuntime _jsRuntime;
    private CancellationTokenSource? _currentCts;
    private static CancellationService? _instance;

    public CancellationService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
        _instance = this;
    }

    public async Task<CancellationToken> CreateTokenAsync()
    {
        // ReSharper disable once MethodHasAsyncOverload
        _currentCts?.Cancel();
        _currentCts = new CancellationTokenSource();

        // Setup simple unload listener
        try
        {
            await _jsRuntime.InvokeVoidAsync("eval", """
                if (!window.rummiCancelSetup) {
                    window.rummiCancelSetup = true;
                    window.addEventListener('beforeunload', () => DotNet.invokeMethodAsync('BlazorRummiSolve', 'CancelFromJS'));
                    window.addEventListener('unload', () => DotNet.invokeMethodAsync('BlazorRummiSolve', 'CancelFromJS'));
                }
            """);
        }
        catch { /* Ignore if JS not ready */ }

        return _currentCts.Token;
    }

    [JSInvokable("CancelFromJS")]
    public static void CancelFromJs()
    {
        _instance?._currentCts?.Cancel();
    }
}