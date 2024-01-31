using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using HttpInstrumentationWithAsync;


namespace TestCases
{
    [TestFixture]
    public class MainFixture
    {
        [Test]
        public async Task  HttpSpansShouldHaveCorrectParentWhenRunSynchronously()
        {
            await Program.Main(args: new String[]{"sync"} );

            DoAsserts(Program.SyncActivities);
         

        }
        [Test]
        public async Task  HttpSpansShouldHaveCorrectParentWhenRunAsynchronously()
        {
            await Program.Main(args: new string[]{"async"} );

          
            DoAsserts(Program.AsyncActivities);

        }
        private void DoAsserts(IList<Activity>  activities)
        {
            const int expectedChildSpans = 2;
            var rootSpan1 = GetRootSpan(activities, "10");
            var rootSpan2 = GetRootSpan(activities, "20");
            var rootSpan3 = GetRootSpan(activities, "30");

            Assert.AreEqual(expectedChildSpans,FindChildSpans(activities, rootSpan1));
            Assert.AreEqual(expectedChildSpans,FindChildSpans(activities, rootSpan2) );
            Assert.AreEqual(expectedChildSpans,FindChildSpans(activities, rootSpan3) );
        }

        private static int FindChildSpans(IList<Activity> activities, Activity rootSpan1)
        {
            return activities.Count(a => a.RootId.Contains(rootSpan1.RootId));
        }

        private Activity GetRootSpan(IList<Activity>  activities, string displayName )
        {
            return activities.First(s => s.DisplayName == $"CallPostManEcho-{displayName}");
        }
    }
    
}