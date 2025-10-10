using RelicService.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Vanara.PInvoke;



namespace RelicService.Service
{
    internal class ApiService
    {
        private readonly List<string> _targetProcessNames = ["yuanshen.exe", "genshinimpact.exe"];
        private readonly CancellationTokenSource _cts = new();

        private EventManager _eventManager;
        private StatusService _statusService;

        private uint _processId = 0;

        public ApiService(
            EventManager eventManager,
            StatusService statusService
            )
        {
            _eventManager = eventManager;
            _statusService = statusService;
            _eventManager.OnShutdown += OnShutdown;

            _ = Task.Run(Worker, _cts.Token);
        }

        private async Task Worker()
        {

            while (!_cts.Token.IsCancellationRequested)
            {

                if (_processId == 0)
                {
                    await Task.Delay(200);

                    var hwnd = GetGameWindow();
                    if (hwnd == default)
                        continue;

                    if (!await _statusService.IsServiceAvailable())
                        LoadApiServer(hwnd);
                    else
                        User32.GetWindowThreadProcessId(hwnd, out _processId);
                }
                else
                {
                    await Task.Delay(2000);
                    _processId = IsProcessAlive(_processId) ? _processId : 0;
                    while (_processId == 0 && GetGameWindow() != default) // wait for game window to be closed
                        await Task.Delay(200);
                }

            }

        }


        private HWND GetGameWindow()
        {
            HWND targetWindow = default;

            User32.EnumWindows((hWnd, _) =>
            {
                var windowClass = new StringBuilder(256);
                User32.GetClassName(hWnd, windowClass, windowClass.Capacity);

                if (windowClass.ToString() == "UnityWndClass")
                {
                    User32.GetWindowThreadProcessId(hWnd, out var pid);
                    if (IsTargetProcess(pid))
                    {
                        targetWindow = hWnd;
                        return false;
                    }
                }

                return true;
            }, default);

            return targetWindow;
        }

        private bool IsTargetProcess(uint processId)
        {
            using var processHandle = Kernel32.OpenProcess((uint)Kernel32.ProcessAccess.PROCESS_QUERY_LIMITED_INFORMATION, false, processId);
            if (processHandle == 0)
                return ShowLastError($"could not open process ({processId})");

            var processPath = new StringBuilder(1024);
            var processPathSize = (uint)processPath.Capacity;
            if (!Kernel32.QueryFullProcessImageName(processHandle, 0, processPath, ref processPathSize))
                return ShowLastError($"could not query process path ({processId})");

            var processName = Path.GetFileName(processPath.ToString());
            processName = processName.ToLower();

            return _targetProcessNames.Contains(processName);
        }

        private bool LoadApiServer(HWND processWindow)
        {
            var moduleName = "ApiServer.dll";

            using var pLibrary = Kernel32.LoadLibrary(moduleName);
            if (pLibrary == IntPtr.Zero)
                return ShowLastError($"could not load '{moduleName}'");

            var pWndProc = Kernel32.GetProcAddress(pLibrary, "WndProc");
            if (pWndProc == IntPtr.Zero)
                return ShowLastError($"could not get 'WndProc' address");

            var WndProc = Marshal.GetDelegateForFunctionPointer<User32.HookProc>(pWndProc);
            var windowThreadId = User32.GetWindowThreadProcessId(processWindow, out var processId);
            if (windowThreadId == 0 || processId == 0)
                return ShowLastError("could not get window thread id or process id");

            var hWndHook = User32.SetWindowsHookEx(User32.HookType.WH_GETMESSAGE, WndProc, pLibrary, (int)windowThreadId);
            if (hWndHook == IntPtr.Zero)
                return ShowLastError("could not set windows hook");

            if (!User32.PostThreadMessage(windowThreadId, 0, IntPtr.Zero, IntPtr.Zero))
                return ShowLastError("could not post thread message");

            _processId = processId;

            return true;
        }

        private bool IsProcessAlive(uint processId)
        {
            using var processHandle = Kernel32.OpenProcess((uint)Kernel32.ProcessAccess.PROCESS_QUERY_LIMITED_INFORMATION, false, processId);
            if (processHandle == 0)
                return false;

            return Kernel32.GetExitCodeProcess(processHandle, out var exitCode) && exitCode == 259;
        }

        private bool ShowLastError(string message)
        {
            var lastError = Marshal.GetLastWin32Error();
            var errorMessage = Marshal.GetLastPInvokeErrorMessage();

            MessageBox.Show($@"{message}

Error: {lastError}
Message: {errorMessage}", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        private void OnShutdown(object? sender, EventArgs e) => _cts.Cancel();

    }
}
