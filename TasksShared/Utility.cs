using swf = System.Windows.Forms;

namespace TasksShared
{
    public class Utility
    {
        private static swf.Screen Detect_oneNonPrimaryScreen()
        {/* Detect the first not Primary Screen */

            swf.Screen[] screens = swf.Screen.AllScreens;
            swf.Screen nonPrimaryS = swf.Screen.PrimaryScreen;
            foreach (swf.Screen s in screens)
            {
                if (s.Primary == false)
                {
                    nonPrimaryS = s;
                    break;
                }
            }
            return nonPrimaryS;
        }

        public static swf.Screen TaskUIScreen()
        {/* Return the  Screen for Showing the Task UI*/
            swf.Screen taskUIScreen = Detect_oneNonPrimaryScreen();

            return taskUIScreen;
        }

        public static swf.Screen TaskPresentationScreen()
        {/* Return the  Screen for Showing the Task Presentation*/
            swf.Screen taskUIScreen = Detect_oneNonPrimaryScreen();

            return taskUIScreen;
        }
    }
}
