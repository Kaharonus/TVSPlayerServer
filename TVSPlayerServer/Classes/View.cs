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
            if (SwitchableContent.Children.Count > 0) {
                var children = SwitchableContent.Children;
                SwitchableContent.Children.RemoveAt(0);
            }
            SwitchableContent.Children.Add(view);
        }

        /// <summary>
        /// Adds a top level view
        /// </summary>
        /// <param name="view"></param>
        public static void AddView(UserControl view) {

        }

        /// <summary>
        /// Removes top-most view
        /// </summary>
        public static void RemoveView() {

        }

        /// <summary>
        /// Removes all views that are on top
        /// </summary>
        public static void RemoveAllViews() {

        }
    }
}
