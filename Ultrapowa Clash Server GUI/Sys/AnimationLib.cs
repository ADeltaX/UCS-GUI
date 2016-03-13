using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace UCS
{
    class AnimationLib
    {

        //Made by ADeltaX :P

        /// <summary>
        /// Use this method to make an animation for a control in Y axis
        /// </summary>
        /// <param name="cntrl">The targhetting Control</param>
        /// <param name="YPos">Here the position to add</param>
        /// <param name="TimeSecond">The duration of the animation</param>
        /// <param name="TimeMillisecond">The delay of the animation</param>
        public static void MoveToTargetY(FrameworkElement cntrl, double YPos, double TimeSecond, double TimeMillisecond = 0)
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
        /// Use this method to make an animation for a control in X axis
        /// </summary>
        /// <param name="cntrl">The targhetting Control</param>
        /// <param name="XPos">Here the position to add</param>
        /// <param name="TimeSecond">The duration of the animation</param>
        /// <param name="TimeMillisecond">The delay of the animation</param>
        public static void MoveToTargetX(FrameworkElement cntrl, double XPos, double TimeSecond, double TimeMillisecond = 0)
        {
            cntrl.Margin = new Thickness(cntrl.Margin.Left - XPos, cntrl.Margin.Top, cntrl.Margin.Right + XPos, cntrl.Margin.Bottom);
            QuadraticEase EP = new QuadraticEase();
            EP.EasingMode = EasingMode.EaseOut;

            var DirX = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(TimeSecond)),
                From = 0,
                To = XPos,
                BeginTime = TimeSpan.FromMilliseconds(TimeMillisecond),
                EasingFunction = EP,
                AutoReverse = false
            };
            cntrl.RenderTransform = new TranslateTransform();
            cntrl.RenderTransform.BeginAnimation(TranslateTransform.XProperty, DirX);
        }

        public static void MoveToTargetXwoMargin(Control cntrl, double XPos, double FromXPos=0, double TimeSecond=0, double TimeMillisecond = 0)
        {
            QuadraticEase EP = new QuadraticEase();
            EP.EasingMode = EasingMode.EaseOut;

            var DirX = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(TimeSecond)),
                From = FromXPos,
                To = XPos,
                BeginTime = TimeSpan.FromMilliseconds(TimeMillisecond),
                EasingFunction = EP,
                AutoReverse = false
            };
            cntrl.RenderTransform = new TranslateTransform();
            cntrl.RenderTransform.BeginAnimation(TranslateTransform.XProperty, DirX);
        }

        /// <summary>
        /// Use this method to make an animation for a window in Y axis
        /// </summary>
        /// <param name="cntrl">The targhetting window</param>
        /// <param name="FromYPos">The position before the final position</param>
        /// <param name="YPos">The final position</param>
        /// <param name="TimeSecond">The duration on the animation</param>
        /// <param name="TimeMillisecond">The delay of the animation</param>
        public static void MoveWindowToTargetY(Control cntrl, double FromYPos, double YPos, double TimeSecond, double TimeMillisecond = 0)
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

        /// <summary>
        /// Use this method to make an animation for a window in X axis
        /// </summary>
        /// <param name="cntrl">The targhetting window</param>
        /// <param name="FromXPos">The position before the final position</param>
        /// <param name="XPos">The final position</param>
        /// <param name="TimeSecond">The duration on the animation</param>
        /// <param name="TimeMillisecond">The delay of the animation</param>
        public static void MoveWindowToTargetX(Control cntrl, double FromXPos, double XPos, double TimeSecond, double TimeMillisecond = 0)
        {
            QuadraticEase EP = new QuadraticEase();
            EP.EasingMode = EasingMode.EaseInOut;

            var DirX = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(TimeSecond)),
                From = XPos - FromXPos,
                To = XPos,
                BeginTime = TimeSpan.FromMilliseconds(TimeMillisecond),
                EasingFunction = EP,
                AutoReverse = false
            };

            cntrl.BeginAnimation(Window.TopProperty, DirX);
        }

        public static void ChangeForegroundColor(Control cntrl, Color FromColor, Color ToColor, double TimeSecond, double TimeMillisecond = 0)
        {
            QuadraticEase EP = new QuadraticEase();
            EP.EasingMode = EasingMode.EaseInOut;

            var DirColor = new ColorAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(TimeSecond)),
                From = FromColor,
                To = ToColor,
                BeginTime = TimeSpan.FromMilliseconds(TimeMillisecond),
                EasingFunction = EP,
                AutoReverse = false
            };

            cntrl.BeginAnimation(SolidColorBrush.ColorProperty,DirColor);
        }

    }
}
