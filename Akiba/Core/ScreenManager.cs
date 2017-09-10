using System;
using System.Drawing;
using System.Windows.Forms;

namespace Akiba.Core
{
    static class ScreenManager
    {
        public static Screen GetGameScreen(IntPtr handle)
        {
            // Let's see where the game window is currently located...
            Rectangle windowLocation = GetWindowLocation(handle);

            var screenIntersection = new Rectangle();
            int bestCandidateArea = 0, candidateArea = 0;

            // User's primary screen is the best choice by default until we find something better.
            Screen bestCandidateScreen = Screen.PrimaryScreen;

            foreach (Screen screen in Screen.AllScreens)
            {
                screenIntersection = Rectangle.Intersect(screen.Bounds, windowLocation);

                // Calculate the surface of the intersection, the biggest one wins!
                candidateArea = screenIntersection.Height * screenIntersection.Width;

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
            NativeMethods.SetWindowLong(handle, NativeMethods.GWL_STYLE, NativeMethods.WS_VISIBLE);

            NativeMethods.MoveWindow(
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
            var rect = new NativeMethods.Rect();

            NativeMethods.GetWindowRect(handle, out rect);

            return new Rectangle(rect.Left, rect.Top, rect.Right, rect.Bottom);
        }
    }
}
