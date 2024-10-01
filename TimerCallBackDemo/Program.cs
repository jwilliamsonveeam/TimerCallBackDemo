using System.Diagnostics;
using System.Resources;
using System.Runtime.InteropServices;

namespace TimerCallBackDemo;

public class Program
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void HandleTimerEvent(IntPtr sigval);    

    public static void Main(string[] args)
    {
        int procId = System.Diagnostics.Process.GetCurrentProcess().Id;
        int sleepMS = 30000;
        Console.WriteLine($"Sleeping for {sleepMS} ms, type strace -p {procId} to follow native trace for this process.");
        Thread.Sleep(sleepMS); // start strace while sleeping
        HandleTimerEvent callback = HandleTimer;
        int SIGEV_THREAD = 2;
        IntPtr _timerId = IntPtr.Zero;

        var configuration = new NativeMethods.sigevent
        {
            sigev_notify = SIGEV_THREAD,
            sigev_value = new NativeMethods.sigval
            {
                sigval_ptr = _timerId
            },
            sigev_notify_function = Marshal.GetFunctionPointerForDelegate(callback),            
        };

        var _ = NativeMethods.timer_create(NativeMethods.clockid_t.CLOCK_REALTIME, configuration, out _timerId);        

        var newValue = new NativeMethods.itimerspec
        {
            it_value = new NativeMethods.timespec
            {
                tv_sec = 1,
                tv_nsec = 1
            }
        };
        
        var result = NativeMethods.timer_settime(_timerId, (int)NativeMethods.timer_flags.TIMER_ABSTIME, newValue, IntPtr.Zero);  
        Thread.Sleep(3000);
        Console.WriteLine("SUCCESS!");
    }

    public static void HandleTimer(IntPtr sigval)
    {
        Console.WriteLine("In minimal delegate");        
    }
}