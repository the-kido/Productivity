// Thanks Google AI Studio -- great name by the way!


using Godot;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

public partial class DeleteThis : Node
{
	
    // Export a color variable to set the tint in the Godot editor
    [Export]
    public Color AcrylicTintColor = new Color(0.5f, 0.5f, 0.5f, 0.8f); // Default: Greyish with 80% alpha

    // --- P/Invoke Definitions ---

    // Enums and Structs for SetWindowCompositionAttribute (Acrylic)
    internal enum AccentState
    {
        ACCENT_DISABLED = 0,
        ACCENT_ENABLE_GRADIENT = 1,
        ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
        ACCENT_ENABLE_BLURBEHIND = 3,
        ACCENT_ENABLE_ACRYLICBLURBEHIND = 4, // Used for Acrylic
        ACCENT_INVALID_STATE = 5
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct AccentPolicy
    {
        public AccentState AccentState;
        public uint AccentFlags; // Typically 2 for legacy acrylic
        public uint GradientColor; // ABGR color format
        public uint AnimationId;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct WindowCompositionAttribData
    {
        public WindowCompositionAttribute Attribute;
        public IntPtr Data; // Pointer to AccentPolicy
        public int SizeOfData;
    }

    internal enum WindowCompositionAttribute
    {
        // ... other attributes omitted for brevity ...
        WCA_ACCENT_POLICY = 19
        // ... other attributes omitted ...
    }

    // Struct for DwmExtendFrameIntoClientArea (Hide Title Bar)
    [StructLayout(LayoutKind.Sequential)]
    public struct Margins
    {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    }

    // Win32 Function Imports
    private static partial class NativeMethods
    {
        [DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttribData data);

        [DllImport("dwmapi.dll")]
        internal static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref Margins pMarInset);
    }

    // --- Godot Lifecycle Method ---

    public override void _Ready()
    {
        (GetParent() as Window).AboutToPopup += Apply;
    }

	private bool applied = false;
    public async void Apply()
    {
        await Task.Delay(100);
        // Ensure this only runs on Windows
        if (OS.GetName() != "Windows")
        {
            GD.PushWarning("Acrylic effect only works on Windows.");
            return;
        }

        // Delay slightly to ensure the window handle is valid
        // Sometimes needed if called too early in the lifecycle.
		
		if (applied) return;
		else
		{
			ApplyAcrylicEffect();
			applied = true;
		}

        // Optional: Set window flags if needed (e.g., always on top)
        // DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.AlwaysOnTop, true, 0);

        // Make the background transparent so acrylic shows through
        // GetViewport().TransparentBg = true;
    }

    private void ApplyAcrylicEffect()
    {
        // Get the main window handle (HWND)
        // IntPtr hwnd = DisplayServer.WindowGetNativeHandle(DisplayServer.HandleType.WindowHandle, 0); // 0 is the main window ID
		IntPtr hwnd = Thing.GetActiveWindow();

        if (hwnd == IntPtr.Zero)
        {
            GD.PushError("Failed to get window handle.");
            return;
        }

        EnableCustomAcrylic(hwnd, AcrylicTintColor);
        HideTitleBarContent(hwnd);
		CustomizeWindowFrame(hwnd);
		RemoveTitleBarButtons(hwnd);
    }

    // --- Helper Functions ---

    private void EnableCustomAcrylic(IntPtr hwnd, Color tintColor)
    {
        try
        {
            var accent = new AccentPolicy();
            accent.AccentState = AccentState.ACCENT_ENABLE_ACRYLICBLURBEHIND;
            // Flags = 2 is often used for legacy Acrylic, tells DWM to draw window borders.
            // Might not be strictly necessary when DwmExtendFrameIntoClientArea covers everything.
            accent.AccentFlags = 2;
            accent.GradientColor = ColorToABGR(tintColor); // Set the custom tint color + alpha
            accent.AnimationId = 0;

            var accentPolicySize = Marshal.SizeOf(accent);
            IntPtr accentPtr = Marshal.AllocHGlobal(accentPolicySize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new WindowCompositionAttribData();
            data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY;
            data.SizeOfData = accentPolicySize;
            data.Data = accentPtr;

            int result = NativeMethods.SetWindowCompositionAttribute(hwnd, ref data);

            Marshal.FreeHGlobal(accentPtr); // Clean up allocated memory

            if (result != 0) // Non-zero usually indicates success for this specific API, oddly. Check docs/examples if issues.
            {
                GD.Print($"Successfully applied custom acrylic. Result code: {result}");
            }
            else
            {
                // If it fails, HRESULT might be available via Marshal.GetLastWin32Error() but requires SetLastError=true in DllImport
                GD.PushWarning($"SetWindowCompositionAttribute might have failed (result code: {result}). Error: {Marshal.GetLastWin32Error()}");
            }
        }
        catch (Exception e)
        {
            GD.PushError($"Error applying acrylic effect: {e.Message}");
        }
    }

    private void HideTitleBarContent(IntPtr hwnd)
    {
        try
        {
            // Setting margins to -1 extends the client area into the entire window frame,
            // effectively hiding the title bar drawing while keeping the frame interactions.
            Margins margins = new Margins { cxLeftWidth = -1, cxRightWidth = -1, cyTopHeight = -1, cyBottomHeight = -1 };
            int result = NativeMethods.DwmExtendFrameIntoClientArea(hwnd, ref margins);

            if (result == 0) // S_OK
            {
                GD.Print("Successfully extended client area into frame.");
            }
            else
            {
                GD.PushWarning($"DwmExtendFrameIntoClientArea failed (HRESULT: {result:X})");
                // You might need Marshal.GetLastWin32Error() here too if DllImport has SetLastError=true
            }
        }
        catch (Exception e)
        {
            GD.PushError($"Error extending client area: {e.Message}");
        }
    }

    /// <summary>
    /// Converts a Godot Color (RGBA float 0-1) to an ABGR uint used by Win32 APIs.
    /// </summary>
    private uint ColorToABGR(Color color)
    {
        // Ensure alpha is handled correctly. Acrylic often uses the alpha component
        // to determine the intensity/transparency of the effect itself.
        byte r = (byte)(Math.Clamp(color.R, 0f, 1f) * 255);
        byte g = (byte)(Math.Clamp(color.G, 0f, 1f) * 255);
        byte b = (byte)(Math.Clamp(color.B, 0f, 1f) * 255);
        byte a = (byte)(Math.Clamp(color.A, 0f, 1f) * 255); // Alpha from Godot Color

        // Format: Alpha << 24 | Blue << 16 | Green << 8 | Red
        return ((uint)a << 24) | ((uint)b << 16) | ((uint)g << 8) | r;
    }


private const int GWL_STYLE = -16; // Index for Get/SetWindowLongPtr to get/set window styles

    // Window Styles (subset) - Use long for compatibility with SetWindowLongPtr
    private const long WS_SYSMENU      = 0x00080000L; // Has a system menu (icon and close button)
    private const long WS_MINIMIZEBOX  = 0x00020000L; // Has a minimize button
    private const long WS_MAXIMIZEBOX  = 0x00010000L; // Has a maximize button
	 private const long WS_CAPTION      = 0x00C00000L; // Standard window caption (title bar + border)
    private const long WS_THICKFRAME   = 0x00040000L; // Standard resizable border/frame (same as WS_SIZEBOX)
    // Note: WS_CAPTION = 0x00C00000L includes WS_BORDER and WS_DLGFRAME.
    // Removing WS_CAPTION makes it borderless, which we want to avoid.
    // Removing WS_SYSMENU is usually sufficient to remove standard buttons.

    private partial class NativeMethods // Add these to the existing NativeMethods class
    {

        // New imports for window styles
        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtrW", SetLastError = true)] // Use W version for Unicode
        internal static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtrW", SetLastError = true)] // Use W version for Unicode
        internal static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        // Helper to check for 64-bit process
        internal static bool Is64BitProcess => IntPtr.Size == 8;

        // Fallback for 32-bit (though Godot 4+ C# is typically 64-bit)
        [DllImport("user32.dll", EntryPoint = "GetWindowLongW", SetLastError = true)]
        internal static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongW", SetLastError = true)]
        internal static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    }

    // Helper function to abstract Get/SetWindowLong/LongPtr
    private static IntPtr GetWindowLongPtrHelper(IntPtr hWnd, int nIndex)
    {
        if (NativeMethods.Is64BitProcess)
        {
            return NativeMethods.GetWindowLongPtr(hWnd, nIndex);
        }
        else
        {
            // Need to handle potential IntPtr conversion issues carefully on 32-bit if style value exceeds Int32.MaxValue
            // For standard styles, this is less likely, but good practice to be aware.
            return new IntPtr(NativeMethods.GetWindowLong(hWnd, nIndex));
        }
    }

    private static IntPtr SetWindowLongPtrHelper(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
    {
        if (NativeMethods.Is64BitProcess)
        {
            return NativeMethods.SetWindowLongPtr(hWnd, nIndex, dwNewLong);
        }
        else
        {
            return new IntPtr(NativeMethods.SetWindowLong(hWnd, nIndex, dwNewLong.ToInt32()));
        }
    }
	private void RemoveTitleBarButtons(IntPtr hwnd)
    {
        try
        {
            long currentStyle = GetWindowLongPtrHelper(hwnd, GWL_STYLE).ToInt64();
            if (currentStyle == 0 && Marshal.GetLastWin32Error() != 0)
            {
                 GD.PushError($"GetWindowLongPtr failed with error code: {Marshal.GetLastWin32Error()}");
                 return;
            }


            // Flags to remove: System Menu (which includes Close) and Minimize/Maximize boxes
            long flagsToRemove = WS_SYSMENU | WS_MINIMIZEBOX | WS_MAXIMIZEBOX;

            long newStyle = currentStyle & ~flagsToRemove; // Use bitwise AND NOT to remove the flags

            // Ensure WS_CAPTION is still present if it was before, otherwise it might become borderless
            // WS_CAPTION includes WS_BORDER implicitly for standard windows.
            // If you manually removed WS_CAPTION elsewhere, you might need WS_BORDER or WS_THICKFRAME explicitly.
            // if ((currentStyle & WS_CAPTION) == WS_CAPTION) {
            //     newStyle |= WS_CAPTION; // Ensure caption style stays if it existed
            // }


            IntPtr previousStyle = SetWindowLongPtrHelper(hwnd, GWL_STYLE, new IntPtr(newStyle));
             if (previousStyle == IntPtr.Zero && Marshal.GetLastWin32Error() != 0)
             {
                GD.PushError($"SetWindowLongPtr failed with error code: {Marshal.GetLastWin32Error()}");
                return;
             }

            // Optional: Force redraw the window frame
            // SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP_FRAMECHANGED | SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_NOOWNERZORDER);
            // Note: Requires importing SetWindowPos and SWP constants if needed. Often DWM handles repaint automatically.

            GD.Print("Attempted to remove standard title bar buttons.");

        }
        catch (Exception e)
        {
            GD.PushError($"Error removing title bar buttons: {e.Message}");
        }
    }

	private void CustomizeWindowFrame(IntPtr hwnd)
    {
        try
        {
            long currentStyle = GetWindowLongPtrHelper(hwnd, GWL_STYLE).ToInt64();
            if (currentStyle == 0 && Marshal.GetLastWin32Error() != 0)
            {
                 GD.PushError($"GetWindowLongPtr failed with error code: {Marshal.GetLastWin32Error()}");
                 return;
            }

            // --- Modification Start ---
            // Flags to remove: SysMenu (buttons), Minimize/Maximize boxes, AND the Caption area
            long flagsToRemove = WS_SYSMENU | WS_MINIMIZEBOX | WS_MAXIMIZEBOX | WS_CAPTION;

            // Flags to ensure are present: A resizable border
            long flagsToAdd = WS_THICKFRAME; // Use WS_BORDER for a non-resizable thin border

            // Calculate the new style: Remove unwanted flags, then ensure required flags are set
            long newStyle = (currentStyle & ~flagsToRemove) | flagsToAdd;
            // --- Modification End ---


            IntPtr previousStyle = SetWindowLongPtrHelper(hwnd, GWL_STYLE, new IntPtr(newStyle));
             if (previousStyle == IntPtr.Zero && Marshal.GetLastWin32Error() != 0)
             {
                GD.PushError($"SetWindowLongPtr failed with error code: {Marshal.GetLastWin32Error()}");
                return;
             }

            // Optional: Force Windows to redraw the frame and recalculate client area.
            // May be needed if visual glitches occur or size doesn't update immediately.
            // Requires importing SetWindowPos and SWP_ constants.
            // NativeMethods.SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0,
            //      NativeMethods.SWP_FRAMECHANGED | NativeMethods.SWP_NOMOVE |
            //      NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOZORDER |
            //      NativeMethods.SWP_NOOWNERZORDER);


            GD.Print("Attempted to remove caption area and standard buttons, ensuring thick frame.");
        }
        catch (Exception e)
        {
            GD.PushError($"Error customizing window frame: {e.Message}");
        }
    }
}