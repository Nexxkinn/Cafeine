using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace BackgroundCheckDownloadNotify
{
    public sealed class Main : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance) {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();
            ///TODO : check nyaa.si
            ///Grab list on nyaa
            ///search : FANSUB NAME    TITLE

            deferral.Complete();
        }
    }
}
