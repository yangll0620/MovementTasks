using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TasksShared
{
    public class ShapeManipulate
    {
        public static Ellipse Create_Circle(double Diameter, SolidColorBrush brush_Fill)
        {/*
            Create the circle

            Args:
                Diameter: the Diameter of the Circle in Pixal

            */

            // Create an Ellipse  
            Ellipse circle = new Ellipse();

            // set the size, position of circleGo
            circle.Height = Diameter;
            circle.Width = Diameter;

            circle.Fill = brush_Fill;

            return circle;
        }


        public static Ellipse Move_Circle_OTopLeft(Ellipse circle, int[] cPoint_Pos_OTopLeft)
        {/*
            Move the circle into cPoint_Pos_OTopLeft (Origin in the topLeft of the Screen)

            Args:
                circle: to Be Moved Circle

                cPoint_Pos_OTopLeft: the x, y Positions of the Circle center in Pixal (Origin in the topLeft of the Screen)

            */


            circle.VerticalAlignment = VerticalAlignment.Top;
            circle.HorizontalAlignment = HorizontalAlignment.Left;

            circle.Margin = new Thickness(cPoint_Pos_OTopLeft[0] - circle.Width / 2, cPoint_Pos_OTopLeft[1] - circle.Height / 2, 0, 0);

            return circle;
        }
    }
}
