using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using swf = System.Windows.Forms;

namespace TasksShared
{
    public class ShapeManipulate
    {
        public static Ellipse Create_Circle(double diameter)
        {/*
            Create the circle

            Args:
                Diameter: the Diameter of the Circle in Pixal

            */

            // Create an Ellipse  
            Ellipse circle = new Ellipse
            {

                // set the size, position of circleGo
                Height = diameter,
                Width = diameter
            };

            return circle;
        }


        public static Ellipse Show_Circle_OTopLeft(Ellipse circle, int[] cPoint_Pos_OTopLeft, SolidColorBrush brush_Fill)
        {/*
            Show the circle at cPoint_Pos_OTopLeft (Origin in the topLeft of the Screen)

            Args:
                circle: to Be Showed Circle

                cPoint_Pos_OTopLeft: the x, y Positions of the Circle center in Pixal (Origin in the topLeft of the Screen)

            */

            circle = Move_Circle_OTopLeft(circle, cPoint_Pos_OTopLeft);
            circle.Fill = brush_Fill;
            circle.Visibility = Visibility.Visible;

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



        public static Rectangle Create_NogoRect(double height, double width)
        {/*
            Create the circle

            Args:
                height, width: the height and width of the rectangle in Pixal

            */

            Rectangle rect = new Rectangle
            {
                Height = height,
                Width = width
            };
            return rect;
        }


        public static Crossing Create_OneCrossing(double lineLength)
        {/*
            Create the Crossing

            Args:
                lineLength: the length of  in Pixal

            */
            Crossing crossing = new Crossing(lineLength);

            return crossing;
        }


        public static int[] ConvertXY_OCenter2TopLeft(int[] xy_OCenter, int width, int height)
        {/*
            Convert X, Y in Coordinate System with Origin at Center into TopLeft of screen

            Positive is Right and Down

            Args:
                xy_OCenter: x, y in Coordinate System with Origin at Center of screen

                width, height: the width and height of the screen
            */

            int[] xy_TopLeft = new int[] { xy_OCenter[0] + width / 2, xy_OCenter[1] + height /2};

            return xy_TopLeft;
        }
    }


    public class Crossing : Shape
    {
        Line horiLine, vertLine;
        double length;
        public Crossing(double len)
        {
            length = len;

            horiLine = new Line();
            horiLine.X1 = 0;
            horiLine.Y1 = 0;
            horiLine.X2 = len;
            horiLine.Y2 = horiLine.Y1;

            vertLine = new Line();
            vertLine.X1 = 0;
            vertLine.Y1 = 0;
            vertLine.X2 = vertLine.X1;
            vertLine.Y2 = len;
        }


        public void Show_Crossing_OTopLeft(int[] cPoint_Pos_OTopLeft)
        {/*     Show  Crossing Containing One Horizontal Line and One Vertical Line at cPoint_Pos_OTopLeft
            *   centerPoint_Pos_OCenter: The Center Point X, Y Position of the Two Lines Intersect, Origin at Top Left
            * 
             */

            int x0 = cPoint_Pos_OTopLeft[0], y0 = cPoint_Pos_OTopLeft[1];
            horiLine.X1 = x0 - length / 2;
            horiLine.X2 = x0 + length / 2;
            horiLine.Y1 = y0;
            horiLine.Y2 = y0;

            vertLine.X1 = x0;
            vertLine.X2 = x0;
            vertLine.Y1 = y0 - length / 2;
            vertLine.Y2 = y0 + length / 2;


            horiLine.Visibility = Visibility.Visible;
            vertLine.Visibility = Visibility.Visible;
        }


        public void Hidden_Crossing()
        {
            horiLine.Visibility = Visibility.Hidden;
            vertLine.Visibility = Visibility.Hidden;
        }

        protected override Geometry DefiningGeometry => throw new System.NotImplementedException();
    }
}
