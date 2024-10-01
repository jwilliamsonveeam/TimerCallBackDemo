using System;
using System.Runtime.InteropServices;

namespace TimerCallBackDemo;

public static class NativeMethods
{
    [DllImport("librt.so.1",  SetLastError = true, CallingConvention = CallingConvention.StdCall)]
    public static extern int timer_create(clockid_t clockId, sigevent callback, out IntPtr timerId);

    [DllImport("librt.so.1",  SetLastError = true, CallingConvention = CallingConvention.StdCall)]
    public static extern int timer_settime(IntPtr timerId, int flags, itimerspec newValue, IntPtr oldValue);

    [DllImport("librt.so.1",  SetLastError = true, CallingConvention = CallingConvention.StdCall)]
    public static extern int timer_delete(Int64 timerId);
    
    
// Native structures, disable struct warnings
#pragma warning disable CS0649 // Not used fields
#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
#pragma warning disable CA1815 // Override equals and operator equals on value types

    [StructLayout(LayoutKind.Sequential)]
    public struct timespec
    {
        public long tv_sec;
        public long tv_nsec;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class itimerspec
    {
        public timespec it_interval;
        public timespec it_value;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class sigevent
    {
        public sigval sigev_value;
        public int sigev_signo;
        public int sigev_notify;
        public IntPtr sigev_notify_function;// Pointer to: delegate void sigev_notify_function(sigval arg);
        public IntPtr sigev_notify_attributes;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct sigval // Union
    {
        [FieldOffset(0)]
        public int sival_int;
        [FieldOffset(0)]
        public IntPtr sigval_ptr;
    }

    [Flags]
    public enum timer_flags
    {
        TIMER_RELATIVETIME = 0,
        TIMER_ABSTIME  = 1
    }

    public enum clockid_t : long
    {
        CLOCK_REALTIME = 0, // A settable system-wide real-time clock
        CLOCK_BOOTTIME = 7, // Monotonic clock that measure time while system is suspended
        CLOCK_REALTIME_ALARM = 8, // like CLOCK_REALTIME, but will wake the system if it is suspended. Same as `resume` option for Windows timer
        CLOCK_BOOTTIME_ALARM = 9 // like CLOCK_BOOTTIME, but will wake the system if it is suspended. Same as `resume` option for Windows timer
    }

#pragma warning restore CA1815
#pragma warning restore CS8981
#pragma warning restore CS0649

    [DllImport("libc", SetLastError = true, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
    public static extern int getresuid(ref int readUserId, ref int effectiveUserId, ref int setUsedId);

    [DllImport("libc", SetLastError = true, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
    public static extern IntPtr getpwuid(int uid);
}