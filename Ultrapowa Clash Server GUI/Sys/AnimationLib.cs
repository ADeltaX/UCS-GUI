using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Ultrapowa_Clash_Server_GUI
{
    class AnimationLib
    {

        //Made by ADeltaX :P

        /// <summary>
        /// Use this method to make an animation for a control
        /// </summary>
        /// <param name="cntrl">The targhetting Control</param>
        /// <param name="YPos">Here the position to add</param>
        /// <param name="TimeSecond">The duration of the animation</param>
        /// <param name="TimeMillisecond">The delay of the animation</param>
        public static void MoveToTarget(Control cntrl, double YPos, double TimeSecond, double TimeMillisecond = 0)
        {
            cntrl.Margin = new Thickness(cntrl.Margin.Left, cntrl.Margin.Top - YPos, cntrl.Margin.Right, cntrl.Margin.Bottom + YPos);
            QuadraticEase EP = new QuadraticEase();
            EP.EasingMode = EasingMode.EaseOut;

            var DirY = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(TimeSecond)),
                From = 0,
                To = YPos,
                BeginTime = TimeSpan.FromMilliseconds(TimeMillisecond),
                EasingFunction = EP,
                AutoReverse = false
            };
            cntrl.RenderTransform = new TranslateTransform();
            cntrl.RenderTransform.BeginAnimation(TranslateTransform.YProperty, DirY);
        }


        /// <summary>
        /// Use this method to make an animation for a window
        /// </summary>
        /// <param name="cntrl">The targhetting window</param>
        /// <param name="FromYPos">The position before the final position</param>
        /// <param name="YPos">The final position</param>
        /// <param name="TimeSecond">The duration on the animation</param>
        /// <param name="TimeMillisecond">The delay of the animation</param>
        public static void MoveWindowToTarget(Control cntrl, double FromYPos, double YPos, double TimeSecond, double TimeMillisecond = 0)
        {
            QuadraticEase EP = new QuadraticEase();
            EP.EasingMode = EasingMode.EaseInOut;

            var DirY = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(TimeSecond)),
                From = YPos - FromYPos,
                To = YPos,
                BeginTime = TimeSpan.FromMilliseconds(TimeMillisecond),
                EasingFunction = EP,
                AutoReverse = false
            };

            cntrl.BeginAnimation(Window.TopProperty, DirY);
        }
    }
}
