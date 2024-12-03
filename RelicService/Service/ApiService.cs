// Decompiled with JetBrains decompiler
// Type: RelicService.Service.ApiService
// Assembly: RelicService, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EA9BEB7B-7841-4D0A-A232-DCAF9A27085B
// Assembly location: RelicService.dll inside C:\Users\MBAINT\Downloads\win-x64\RelicService.exe)

using RelicService.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vanara.PInvoke;

#nullable enable
namespace RelicService.Service
{
  internal class ApiService
  {
    private readonly List<string> _targetProcessNames;
    private readonly CancellationTokenSource _cts;
    private EventManager _eventManager;
    private StatusService _statusService;
    private uint _processId;

    public ApiService(EventManager eventManager, StatusService statusService)
    {
      int num1 = 2;
      List<string> stringList = new List<string>(num1);
      CollectionsMarshal.SetCount<string>(stringList, num1);
      Span<string> span = CollectionsMarshal.AsSpan<string>(stringList);
      int num2 = 0;
      span[num2] = "yuanshen.exe";
      int num3 = num2 + 1;
      span[num3] = "genshinimpact.exe";
      int num4 = num3 + 1;
      this._targetProcessNames = stringList;
      this._cts = new CancellationTokenSource();
      // ISSUE: explicit constructor call
      base.\u002Ector();
      this._eventManager = eventManager;
      this._statusService = statusService;
      this._eventManager.OnShutdown += new EventHandler(this.OnShutdown);
      Task.Run(new Func<Task>(this.Worker), this._cts.Token);
    }

    private async Task Worker()
    {
label_10:
      while (!this._cts.Token.IsCancellationRequested)
      {
        if (this._processId == 0U)
        {
          await Task.Delay(200);
          HWND hwnd = this.GetGameWindow();
          if (!(hwnd == new HWND()))
          {
            if (!await this._statusService.IsServiceAvailable())
            {
              this.LoadApiServer(hwnd);
            }
            else
            {
              int windowThreadProcessId = (int) Vanara.PInvoke.User32.GetWindowThreadProcessId(hwnd, out this._processId);
            }
            hwnd = new HWND();
          }
        }
        else
        {
          await Task.Delay(2000);
          this._processId = this.IsProcessAlive(this._processId) ? this._processId : 0U;
          while (true)
          {
            if (this._processId == 0U && this.GetGameWindow() != new HWND())
              await Task.Delay(200);
            else
              goto label_10;
          }
        }
      }
    }

    private HWND GetGameWindow()
    {
      HWND targetWindow = new HWND();
      Vanara.PInvoke.User32.EnumWindows((Vanara.PInvoke.User32.EnumWindowsProc) ((hWnd, _) =>
      {
        StringBuilder lpClassName = new StringBuilder(256);
        Vanara.PInvoke.User32.GetClassName(hWnd, lpClassName, lpClassName.Capacity);
        if (string.op_Equality(lpClassName.ToString(), "UnityWndClass"))
        {
          uint lpdwProcessId;
          int windowThreadProcessId = (int) Vanara.PInvoke.User32.GetWindowThreadProcessId(hWnd, out lpdwProcessId);
          if (this.IsTargetProcess(lpdwProcessId))
          {
            targetWindow = hWnd;
            return false;
          }
        }
        return true;
      }), IntPtr.Zero);
      return targetWindow;
    }

    private bool IsTargetProcess(uint processId)
    {
      using (Kernel32.SafeHPROCESS safeHprocess = Kernel32.OpenProcess((ACCESS_MASK) 4096U, false, processId))
      {
        if ((SafeHANDLE) safeHprocess == IntPtr.Zero)
        {
          DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(25, 1);
          interpolatedStringHandler.AppendLiteral("could not open process (");
          interpolatedStringHandler.AppendFormatted<uint>(processId);
          interpolatedStringHandler.AppendLiteral(")");
          return this.ShowLastError(interpolatedStringHandler.ToStringAndClear());
        }
        StringBuilder lpExeName = new StringBuilder(1024);
        uint capacity = (uint) lpExeName.Capacity;
        if (Kernel32.QueryFullProcessImageName((HPROCESS) safeHprocess, Kernel32.PROCESS_NAME.PROCESS_NAME_WIN32, lpExeName, ref capacity))
          return this._targetProcessNames.Contains(Path.GetFileName(lpExeName.ToString()).ToLower());
        DefaultInterpolatedStringHandler interpolatedStringHandler1 = new DefaultInterpolatedStringHandler(31, 1);
        interpolatedStringHandler1.AppendLiteral("could not query process path (");
        interpolatedStringHandler1.AppendFormatted<uint>(processId);
        interpolatedStringHandler1.AppendLiteral(")");
        return this.ShowLastError(interpolatedStringHandler1.ToStringAndClear());
      }
    }

    private bool LoadApiServer(HWND processWindow)
    {
      string lpFileName = "ApiServer.dll";
      using (Kernel32.SafeHINSTANCE safeHinstance = Kernel32.LoadLibrary(lpFileName))
      {
        if ((SafeHANDLE) safeHinstance == IntPtr.Zero)
          return this.ShowLastError("could not load '" + lpFileName + "'");
        IntPtr procAddress = Kernel32.GetProcAddress((HINSTANCE) safeHinstance, "WndProc");
        if (procAddress == IntPtr.Zero)
          return this.ShowLastError("could not get 'WndProc' address");
        Vanara.PInvoke.User32.HookProc forFunctionPointer = Marshal.GetDelegateForFunctionPointer<Vanara.PInvoke.User32.HookProc>(procAddress);
        uint lpdwProcessId;
        uint windowThreadProcessId = Vanara.PInvoke.User32.GetWindowThreadProcessId(processWindow, out lpdwProcessId);
        if (windowThreadProcessId == 0U || lpdwProcessId == 0U)
          return this.ShowLastError("could not get window thread id or process id");
        if ((SafeHANDLE) Vanara.PInvoke.User32.SetWindowsHookEx(Vanara.PInvoke.User32.HookType.WH_GETMESSAGE, forFunctionPointer, (HINSTANCE) safeHinstance, (int) windowThreadProcessId) == IntPtr.Zero)
          return this.ShowLastError("could not set windows hook");
        if (!Vanara.PInvoke.User32.PostThreadMessage(windowThreadProcessId, 0U, IntPtr.Zero, IntPtr.Zero))
          return this.ShowLastError("could not post thread message");
        this._processId = lpdwProcessId;
        return true;
      }
    }

    private bool IsProcessAlive(uint processId)
    {
      using (Kernel32.SafeHPROCESS safeHprocess = Kernel32.OpenProcess((ACCESS_MASK) 4096U, false, processId))
      {
        uint lpExitCode;
        return !((SafeHANDLE) safeHprocess == IntPtr.Zero) && Kernel32.GetExitCodeProcess((HPROCESS) safeHprocess, out lpExitCode) && lpExitCode == 259U;
      }
    }

    private bool ShowLastError(string message)
    {
      int lastWin32Error = Marshal.GetLastWin32Error();
      string pinvokeErrorMessage = Marshal.GetLastPInvokeErrorMessage();
      DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(22, 3);
      interpolatedStringHandler.AppendFormatted(message);
      interpolatedStringHandler.AppendLiteral("\r\n\r\nError: ");
      interpolatedStringHandler.AppendFormatted<int>(lastWin32Error);
      interpolatedStringHandler.AppendLiteral("\r\nMessage: ");
      interpolatedStringHandler.AppendFormatted(pinvokeErrorMessage);
      int num = (int) MessageBox.Show(interpolatedStringHandler.ToStringAndClear(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
      return false;
    }

    private void OnShutdown(object? sender, EventArgs e) => this._cts.Cancel();
  }
}
