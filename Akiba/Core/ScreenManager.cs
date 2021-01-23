namespace Akiba.Core
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    internal static class ScreenManager
    {
        public static Screen GetGameScreen(IntPtr handle)
        {
            // Let's see where the game window is currently located...
            var windowLocation = GetWindowLocation(handle);

            var bestCandidateArea = 0;

            // User's primary screen is the best choice by default until we find something better.
            var bestCandidateScreen = Screen.PrimaryScreen;

            foreach (var screen in Screen.AllScreens)
            {
                var screenIntersection = Rectangle.Intersect(screen.Bounds, windowLocation);

                // Calculate the surface of the intersection, the biggest one wins!
                var candidateArea = screenIntersection.Height * screenIntersection.Width;

                if (candidateArea > bestCandidateArea)
                {
                    bestCandidateArea = candidateArea;
                    bestCandidateScreen = screen;
                }
            }

            return bestCandidateScreen;
        }

        public static void OccupyScreen(IntPtr handle, Screen screen)
        {
            _ = NativeMethods.SetWindowLong(handle, NativeMethods.GWL_STYLE, NativeMethods.WS_VISIBLE);

            _ = NativeMethods.MoveWindow(
                handle,
                screen.Bounds.Left,
                screen.Bounds.Top,
                screen.Bounds.Width,
                screen.Bounds.Height,
                true
            );
        }

        private static Rectangle GetWindowLocation(IntPtr handle)
        {
            _ = NativeMethods.GetWindowRect(handle, out var rect);

            return new Rectangle(rect.Left, rect.Top, rect.Right, rect.Bottom);
        }
    }
}
