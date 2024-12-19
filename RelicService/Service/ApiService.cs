using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using RelicService.Tools;
using Vanara.PInvoke;

namespace RelicService.Service;

internal class ApiService
{
	private readonly List<string> _targetProcessNames;

	private readonly CancellationTokenSource _cts;

	private EventManager _eventManager;

	private StatusService _statusService;

	private uint _processId;

	public ApiService(EventManager eventManager, StatusService statusService)
	{
		int num = 2;
		List<string> list = new List<string>(num);
		CollectionsMarshal.SetCount(list, num);
		Span<string> span = CollectionsMarshal.AsSpan(list);
		int num2 = 0;
		span[num2] = "yuanshen.exe";
		num2++;
		span[num2] = "genshinimpact.exe";
		num2++;
		_targetProcessNames = list;
		_cts = new CancellationTokenSource();
//		base._002Ector();
		_eventManager = eventManager;
		_statusService = statusService;
		_eventManager.OnShutdown += OnShutdown;
		Task.Run((Func<Task?>)Worker, _cts.Token);
	}

	private async Task Worker()
	{
		while (!_cts.Token.IsCancellationRequested)
		{
			if (_processId == 0)
			{
				await Task.Delay(200);
				HWND hwnd = GetGameWindow();
				if (!(hwnd == default(HWND)))
				{
					if (!(await _statusService.IsServiceAvailable()))
					{
						LoadApiServer(hwnd);
					}
					else
					{
						User32.GetWindowThreadProcessId(hwnd, out _processId);
					}
				}
			}
			else
			{
				await Task.Delay(2000);
				_processId = (IsProcessAlive(_processId) ? _processId : 0u);
				while (_processId == 0 && GetGameWindow() != default(HWND))
				{
					await Task.Delay(200);
				}
			}
		}
	}

	private HWND GetGameWindow()
	{
		HWND targetWindow = default(HWND);
		User32.EnumWindows(delegate(HWND hWnd, nint _)
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			User32.GetClassName(hWnd, stringBuilder, stringBuilder.Capacity);
			if (stringBuilder.ToString() == "UnityWndClass")
			{
				User32.GetWindowThreadProcessId(hWnd, out var lpdwProcessId);
				if (IsTargetProcess(lpdwProcessId))
				{
					targetWindow = hWnd;
					return false;
				}
			}
			return true;
		}, 0);
		return targetWindow;
	}

	private bool IsTargetProcess(uint processId)
	{
		using Kernel32.SafeHPROCESS safeHPROCESS = Kernel32.OpenProcess(4096u, bInheritHandle: false, processId);
		if ((SafeHANDLE)safeHPROCESS == (nint)0)
		{
			return ShowLastError($"could not open process ({processId})");
		}
		StringBuilder stringBuilder = new StringBuilder(1024);
		uint lpdwSize = (uint)stringBuilder.Capacity;
		if (!Kernel32.QueryFullProcessImageName(safeHPROCESS, Kernel32.PROCESS_NAME.PROCESS_NAME_WIN32, stringBuilder, ref lpdwSize))
		{
			return ShowLastError($"could not query process path ({processId})");
		}
		string fileName = Path.GetFileName(stringBuilder.ToString());
		fileName = fileName.ToLower();
		return _targetProcessNames.Contains(fileName);
	}

	private bool LoadApiServer(HWND processWindow)
	{
		string text = "ApiServer.dll";
		using Kernel32.SafeHINSTANCE safeHINSTANCE = Kernel32.LoadLibrary(text);
		if (safeHINSTANCE == IntPtr.Zero)
		{
			return ShowLastError("could not load '" + text + "'");
		}
		nint procAddress = Kernel32.GetProcAddress(safeHINSTANCE, "WndProc");
		if (procAddress == IntPtr.Zero)
		{
			return ShowLastError("could not get 'WndProc' address");
		}
		User32.HookProc delegateForFunctionPointer = Marshal.GetDelegateForFunctionPointer<User32.HookProc>(procAddress);
		uint lpdwProcessId;
		uint windowThreadProcessId = User32.GetWindowThreadProcessId(processWindow, out lpdwProcessId);
		if (windowThreadProcessId == 0 || lpdwProcessId == 0)
		{
			return ShowLastError("could not get window thread id or process id");
		}
		if (User32.SetWindowsHookEx(User32.HookType.WH_GETMESSAGE, delegateForFunctionPointer, safeHINSTANCE, (int)windowThreadProcessId) == IntPtr.Zero)
		{
			return ShowLastError("could not set windows hook");
		}
		if (!User32.PostThreadMessage(windowThreadProcessId, 0u, IntPtr.Zero, IntPtr.Zero))
		{
			return ShowLastError("could not post thread message");
		}
		_processId = lpdwProcessId;
		return true;
	}

	private bool IsProcessAlive(uint processId)
	{
		using Kernel32.SafeHPROCESS safeHPROCESS = Kernel32.OpenProcess(4096u, bInheritHandle: false, processId);
		if ((SafeHANDLE)safeHPROCESS == (nint)0)
		{
			return false;
		}
		uint lpExitCode;
		return Kernel32.GetExitCodeProcess(safeHPROCESS, out lpExitCode) && lpExitCode == 259;
	}

	private bool ShowLastError(string message)
	{
		int lastWin32Error = Marshal.GetLastWin32Error();
		string lastPInvokeErrorMessage = Marshal.GetLastPInvokeErrorMessage();
		MessageBox.Show($"{message}\r\n\r\nError: {lastWin32Error}\r\nMessage: {lastPInvokeErrorMessage}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		return false;
	}

	private void OnShutdown(object? sender, EventArgs e)
	{
		_cts.Cancel();
	}
}
