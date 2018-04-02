using Avalonia.Animation;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace TVSPlayerServer
{
    public class View{
        private static Grid SwitchableContent { get; set; }
        private static Grid ContentOnTop { get; set; }

        public View(Grid switchable, Grid onTop) {
            SwitchableContent = switchable;
            ContentOnTop = onTop;
        }


        /// <summary>
        /// Sets view to base of window
        /// </summary>
        /// <param name="view"></param>
        public static void SetView(UserControl view) {
            // Finish this later now it just fades out, I want it also to fade in. Also make some nice "Animations" class with a nice wrapper for this crap
            var anim = Animate.Property(SwitchableContent, Grid.OpacityProperty, 1, 0, new LinearDoubleEasing(), new TimeSpan(0, 0, 0, 0, 300));
            var how = anim.Subscribe((double value) => {
                if (value == 0) {
                    if (SwitchableContent.Children.Count > 0) {
                        var children = SwitchableContent.Children;
                        SwitchableContent.Children.RemoveAt(0);
                    }
                    SwitchableContent.Children.Add(view);
                }       
           });
          
           
        }

        /// <summary>
        /// Adds a top level view
        /// </summary>
        /// <param name="view"></param>
        public static void AddView(UserControl view) {
            ContentOnTop.Children.Add(view);
        }

        /// <summary>
        /// Removes top-most view
        /// </summary>
        public static void RemoveView() {
            ContentOnTop.Children.RemoveAt(ContentOnTop.Children.Count - 1);
        }

        /// <summary>
        /// Removes all views that are on top
        /// </summary>
        public static void RemoveAllViews() {
            ContentOnTop.Children.Clear();
        }
    }
}
