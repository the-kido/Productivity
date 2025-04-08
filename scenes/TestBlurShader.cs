using Godot;
using System;
using System.Runtime.InteropServices;

// NOT made by me; this was all chat gpt. Say "thanks gpt!"
public class Thing { 

    public Thing(int? id = null)
    {
        ApplyAcrylic(id);
    }

    private void ApplyAcrylic(int? id = null)
    {
        IntPtr hwnd;
        if (id == null)
        {
            hwnd = GetActiveWindow();
        }
        else
        {
            hwnd = (IntPtr) id;
        }
        GD.Print("windows: ", hwnd);

        var accent = new ACCENT_POLICY
        {
            AccentState = ACCENT_STATE.ACCENT_ENABLE_ACRYLICBLURBEHIND,
            GradientColor = unchecked(0x7F293a32) // AARRGGBB (opacity+tint)
        };

        var accentStructSize = Marshal.SizeOf(accent);
        IntPtr accentPtr = Marshal.AllocHGlobal(accentStructSize);
        Marshal.StructureToPtr(accent, accentPtr, false);

        var data = new WINDOWCOMPOSITIONATTRIBDATA
        {
            Attribute = 19, // WCA_ACCENT_POLICY
            Data = accentPtr,
            SizeOfData = accentStructSize
        };

        SetWindowCompositionAttribute(hwnd, ref data);

        Marshal.FreeHGlobal(accentPtr);

        
    }

    // Win32 structures and enums
    enum ACCENT_STATE
    {
        ACCENT_DISABLED = 0,
        ACCENT_ENABLE_GRADIENT = 1,
        ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
        ACCENT_ENABLE_BLURBEHIND = 3,
        ACCENT_ENABLE_ACRYLICBLURBEHIND = 4,
        ACCENT_ENABLE_HOSTBACKDROP = 5
    }

    struct ACCENT_POLICY
    {
        public ACCENT_STATE AccentState;
        public int AccentFlags;
        public int GradientColor;
        public int AnimationId;
    }

    struct WINDOWCOMPOSITIONATTRIBDATA
    {
        public int Attribute;
        public IntPtr Data;
        public int SizeOfData;
    }

    [DllImport("user32.dll")]
    public static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WINDOWCOMPOSITIONATTRIBDATA data);
}
 /*
 private ImageTexture _desktopTexture;
    private System.Timers.Timer _captureTimer;

    public override void _Ready()
    {
        _desktopTexture = new ImageTexture();
		Texture = _desktopTexture;

        CallDeferred("CaptureDesktop"); // Initial capture
    }

	double timer = 0;
    public override void _Process(double delta)
    {
		timer += delta;
		if (timer >= 2)
		{
			timer = 0;
			CaptureDesktop();
		}
    }


    private void CaptureDesktop()
    {
        try
        {
            // Get screen size
            int screenWidth = GetSystemMetrics(0);
            int screenHeight = GetSystemMetrics(1);

            IntPtr hDesk = GetDC(IntPtr.Zero);
            IntPtr hMemDC = CreateCompatibleDC(hDesk);
            IntPtr hBitmap = CreateCompatibleBitmap(hDesk, screenWidth, screenHeight);
            IntPtr hOld = SelectObject(hMemDC, hBitmap);
            BitBlt(hMemDC, 0, 0, screenWidth, screenHeight, hDesk, 0, 0, CopyPixelOperation.SourceCopy);
            SelectObject(hMemDC, hOld);

            // Save the screenshot
            System.Drawing.Bitmap screenshot = System.Drawing.Image.FromHbitmap(hBitmap);
            DeleteObject(hBitmap);
            ReleaseDC(IntPtr.Zero, hDesk);
            DeleteDC(hMemDC);

            // Convert to Godot texture
            using (MemoryStream ms = new MemoryStream())
            {
                screenshot.Save(ms, ImageFormat.Png);
                ms.Position = 0;

                var image = new Godot.Image();
                image.LoadPngFromBuffer(ms.ToArray());

				CallDeferred("UpdateBg", _desktopTexture);
				_desktopTexture.SetImage(image);
                // _desktopTexture.Update(image);
            }
        }
        catch (Exception e)
        {
            GD.PrintErr("Failed to capture desktop: ", e.Message);
        }
    }

	private void UpdateBg(ImageTexture image)
	{
		RenderingServer.GlobalShaderParameterSet("bg", image);
		GD.Print();
		RenderingServer.GlobalShaderParameterSet("bg", image);
	}

    // Windows API imports
    [DllImport("user32.dll")]
    private static extern IntPtr GetDC(IntPtr hwnd);
    [DllImport("gdi32.dll")]
    private static extern IntPtr CreateCompatibleDC(IntPtr hdc);
    [DllImport("gdi32.dll")]
    private static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);
    [DllImport("gdi32.dll")]
    private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);
    [DllImport("gdi32.dll")]
    private static extern bool BitBlt(IntPtr hdc, int x, int y, int cx, int cy, IntPtr hdcSrc, int x1, int y1, CopyPixelOperation rop);
    [DllImport("gdi32.dll")]
    private static extern bool DeleteObject(IntPtr hObject);
    [DllImport("gdi32.dll")]
    private static extern bool DeleteDC(IntPtr hdc);
    [DllImport("user32.dll")]
    private static extern int GetSystemMetrics(int nIndex);
    [DllImport("user32.dll")]
    private static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);
 */   


public class Other 
{

    private const int DWMWA_CAPTION_COLOR = 35; // Attribute to set caption color
    private const int DWMWA_TEXT_COLOR = 36;    // Attribute to set text color (Limited: 0=auto, 1=light, 2=dark might not work well)
    private const int DWMWA_BORDER_COLOR = 34;  // Attribute to set border color

    // Import the specific function from dwmapi.dll
    [DllImport("dwmapi.dll", PreserveSig = true, SetLastError = true)]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int dwAttribute, ref uint pvAttribute, uint cbAttribute);

    // Import GetActiveWindow just in case DisplayServer handle is problematic (alternative)
    // [DllImport("user32.dll")]
    // private static extern IntPtr GetActiveWindow();

    // --- Helper Function to Set Color ---
    public static bool TrySetTitleBarColor(Color godotColor)
    {
        if (OS.GetName() != "Windows")
        {
            GD.PrintErr("Title bar coloring is only supported on Windows.");
            return false;
        }

        try
        {
            // Get the native window handle (HWND) for the main Godot window
            // DisplayServer.WindowGetNativeHandle is the modern way in Godot 4+
            IntPtr windowHandle = (IntPtr)DisplayServer.WindowGetNativeHandle(DisplayServer.HandleType.WindowHandle);

            if (windowHandle == IntPtr.Zero)
            {
                GD.PrintErr("Could not get window handle.");
                return false;
            }
            windowHandle = Thing.GetActiveWindow();


            // Convert Godot Color (0-1 floats) to Windows COLORREF (0x00BBGGRR uint)
            uint colorRef = (uint)((byte)(godotColor.B * 255) << 16 | (byte)(godotColor.G * 255) << 8 | (byte)(godotColor.R * 255));

            // Call DwmSetWindowAttribute to set the caption color
            // Pass the colorRef by reference
            int result = DwmSetWindowAttribute(windowHandle, DWMWA_CAPTION_COLOR, ref colorRef, sizeof(uint));

            if (result != 0) // 0 (S_OK) means success
            {
                GD.PrintErr($"DwmSetWindowAttribute failed with HRESULT: 0x{result:X8}. Make sure you are on Windows 11 22H2 or later.");
                // You might want to check Marshal.GetLastWin32Error() here as well, though HRESULT is primary.
                return false;
            }

            GD.Print("Successfully set title bar color (Windows).");

            // --- Optional: Set Border and Text Color ---
            // Uncomment below if you want to try setting these too.
            // Text color is often unreliable or just switches light/dark based on caption color.
            // uint borderColorRef = colorRef; // Or a different color
            // DwmSetWindowAttribute(windowHandle, DWMWA_BORDER_COLOR, ref borderColorRef, sizeof(uint));

            // uint textColorRef = 0; // e.g., 0=auto, 1=light (white), 2=dark (black) - often doesn't work as expected
            // DwmSetWindowAttribute(windowHandle, DWMWA_TEXT_COLOR, ref textColorRef, sizeof(uint));

            return true;
        }
        catch (Exception e)
        {
            GD.PrintErr($"Error setting title bar color: {e.Message}");
            return false;
        }
    } 
}