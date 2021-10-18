using System;
using System.Collections.Generic;
using swf = System.Windows.Forms;

namespace TasksShared
{
    public class Utility
    {
        static public int ratioIn2Pixal = 96;

        public static int Inch2Pixal(float inlen)
        {/* convert length with unit inch to unit pixal, 96 pixals = 1 inch = 2.54 cm

            args:   
                cmlen: to be converted length (unit: inch)

            return:
                pixalen: converted length with unit pixal
         */

            int pixalen = (int)(inlen * ratioIn2Pixal);

            return pixalen;
        }



        public static List<int[]> GenDefaultPositions_GoNogoTask(int n, int radius)
        {/*
                Generate the default optional X, Y Positions (origin in center) for workArea
                1. The  Points equally in a circle for the first 8 (origin = [0, 0], radius)
                2. The nineth is at (0, 0) for n = 9

                Unit is pixal

                Args:
                    n: the number of generated positions (n <=9)
                    radius: the radius of the circle (Pixal)

            */

            List<int[]> postions9_OCenter_List = new List<int[]>();

            // Points 1,2 at 0 and pi Degrees
            for (int i = 0; i < 2; i++)
            {
                double deg = Math.PI * i;
                int x = (int)(radius * Math.Cos(deg)), y = -(int)(radius * Math.Sin(deg));
                postions9_OCenter_List.Add(new int[] { x, y });
            }
            // Points 3, 4  at pi/2 and 3pi/2 Degrees
            for (int i = 0; i < 2; i++)
            {
                double deg = Math.PI * i + Math.PI / 2;
                int x = (int)(radius * Math.Cos(deg)), y = -(int)(radius * Math.Sin(deg));
                postions9_OCenter_List.Add(new int[] { x, y });
            }
            // Points 5-8 at pi/4, 3pi/4, 5pi/4 and 7pi/4 Degrees
            for (int i = 0; i < 4; i++)
            {
                double deg = 2 * Math.PI / 4 * i + Math.PI / 4;
                int x = (int)(radius * Math.Cos(deg)), y = -(int)(radius * Math.Sin(deg));
                postions9_OCenter_List.Add(new int[] { x, y });
            }
            // Point 9 at (0, 0)
            postions9_OCenter_List.Add(new int[] { 0, 0 });


            List<int[]> defaultPostions_OCenter_List = new List<int[]>();
            for (int i = 0; i < n; i++)
            {
                defaultPostions_OCenter_List.Add(postions9_OCenter_List[i]);
            }


            return defaultPostions_OCenter_List;

        }
    }
}
