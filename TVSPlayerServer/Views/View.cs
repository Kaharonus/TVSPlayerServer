using Avalonia.Animation;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
        public async static Task SetView(UserControl view) {
            await Anim.FadeOut(SwitchableContent, async () => {
                if (SwitchableContent.Children.Count > 0) {
                    SwitchableContent.Children.RemoveAt(0);
                }
                SwitchableContent.Children.Add(view);
            });
            await Anim.FadeIn(SwitchableContent);
        }

        /// <summary>
        /// Adds a top level view
        /// </summary>
        /// <param name="view"></param>
        public async static Task AddView(UserControl view) {
            view.Opacity = 0;
            ContentOnTop.Children.Add(view);
            await Task.Delay(50);
            await Anim.FadeIn(view);
        }

        /// <summary>
        /// Removes top-most view
        /// </summary>
        public async static Task RemoveView() {
            await Anim.FadeOut(ContentOnTop.Children[ContentOnTop.Children.Count - 1], ()=> {
                ContentOnTop.Children.RemoveAt(ContentOnTop.Children.Count - 1);
            });
        }

        /// <summary>
        /// Removes all views that are on top
        /// </summary>
        public async static Task RemoveAllViews() {
            foreach (var item in ContentOnTop.Children) {
                await Anim.FadeOut(item, () => {
                    ContentOnTop.Children.Clear();
                });
            }
        }
    }
}
