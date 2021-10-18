using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TasksShared
{
    public class ScreenDetect
    {
        private static Screen Detect_oneNonPrimaryScreen()
        {/* Detect the first not Primary Screen */

            Screen[] screens = Screen.AllScreens;
            Screen nonPrimaryS = Screen.PrimaryScreen;
            foreach (Screen s in screens)
            {
                if (s.Primary == false)
                {
                    nonPrimaryS = s;
                    break;
                }
            }
            return nonPrimaryS;
        }

        public static Screen TaskUIScreen()
        {/* Return the  Screen for Showing the Task UI*/
            Screen taskUIScreen = Detect_oneNonPrimaryScreen();

            return taskUIScreen;
        }

        public static Screen TaskPresentationScreen()
        {/* Return the  Screen for Showing the Task Presentation*/
            Screen taskPresentScreen = Screen.PrimaryScreen;

            return taskPresentScreen;
        }
    }
}
