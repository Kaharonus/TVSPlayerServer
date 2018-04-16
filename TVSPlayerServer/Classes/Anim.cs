using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TVSPlayerServer
{
    class Anim {

        public async static Task FadeOut(IControl control, Action finished = null) {
            await WaitForFinish(control, Grid.OpacityProperty, 1, 0, finished);
        }

        public async static Task FadeIn(IControl control, Action finished = null) {
            await WaitForFinish(control, Grid.OpacityProperty, 0, 1, finished);
        }

        private async static Task WaitForFinish(IControl control, StyledProperty<double> property, double fromValue, double toValue, Action finished) {
            bool canFinish = false;
            Dispatcher.UIThread.Post(() => control.SetValue(property, fromValue));
            await Task.Delay(25);
            Animation<double> anim = Animate.Property(control, property, fromValue, toValue, new LinearDoubleEasing(), new TimeSpan(0, 0, 0, 0, 200));
            var sub = anim.Subscribe((double value) => {
                if (value == toValue) {
                    Dispatcher.UIThread.Post(() => control.SetValue(property, toValue));
                    if (finished != null) {
                        Dispatcher.UIThread.Post(finished);
                    }
                }
            }, () => { canFinish = true; });
            await Task.Run(async () => {
                while (!canFinish) {
                    await Task.Delay(50);
                }
            });
        }

    }
}
