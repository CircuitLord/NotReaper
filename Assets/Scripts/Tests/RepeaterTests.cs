using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using NotReaper;
using NotReaper.UI;
using NotReaper.UserInput;
using NotReaper.Timing;
using NotReaper.Models;
using UnityEngine.SceneManagement;
using NotReaper.Targets;
using NotReaper.Tools;

namespace Tests
{
    public class RepeaterTests
    {
        static Timeline timeline;

        public void CreateTarget(QNT_Timestamp time, TargetBehavior behavior, TargetHandType hand, Vector2? position = null) {
            if(!position.HasValue) {
                position = new Vector2(0,0);
            }

            TargetData data = new TargetData();
            data.x = position.Value.x;
            data.y = position.Value.y;
            data.handType = hand;
            data.behavior = behavior;
            data.SetTimeFromAction(time);

            var action = new NRActionAddNote {targetData = data};
            timeline.Tools.undoRedoManager.AddAction(action);
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator RunRepeaterTests()
        {
            SceneManager.LoadScene("Main", LoadSceneMode.Single);
            yield return null;

            //Disable editor input
            EditorInput.isTesting = true;

            //Close the pause menu
             var pauseMenuObj = GameObject.Find("PauseMenu");
            Assert.That(pauseMenuObj != null, "We need a PauseMenu to test!");
            Assert.That(pauseMenuObj.GetComponent<PauseMenu>() != null, "We need a PauseMenu to have a PauseMenu component!");
            pauseMenuObj.GetComponent<PauseMenu>().ClosePauseMenu();

            //Now, find the timeline and set it up for testing
            var timelineObj = GameObject.Find("Timeline");
            Assert.That(timelineObj != null, "We need a timeline to test!");
            timeline = timelineObj.GetComponent<Timeline>();
            Assert.That(timelineObj != null, "The object named 'Timeline' must have a 'Timeline' component!");

            timeline.SetupBlankTest();

            AddRemoveTargetsInRepeaterSection();
        }

        public void AddRemoveTargetsInRepeaterSection() {
            RepeaterSection section = new RepeaterSection(0, new QNT_Timestamp(0), new QNT_Timestamp(500));
            timeline.AddRepeaterSection(section);

            RepeaterSection section2 = new RepeaterSection(0, new QNT_Timestamp(600), new QNT_Timestamp(1000));
            timeline.AddRepeaterSection(section2);

            //Create a target at time 50, this should be repeated in the second section, at time 650
            CreateTarget(new QNT_Timestamp(50), TargetBehavior.Standard, TargetHandType.Left);
            Assert.That(timeline.FindTargetData(new QNT_Timestamp(50), TargetBehavior.Standard, TargetHandType.Left) != null, "Target couldn't be created!");
            Assert.That(timeline.FindTargetData(new QNT_Timestamp(600 + 50), TargetBehavior.Standard, TargetHandType.Left) != null, "Target wasn't created in repeater!");

            //Create another target at 450, this shouldn't be repeated since 600 + 450 = 1050, which is outside the repeated section
            CreateTarget(new QNT_Timestamp(450), TargetBehavior.Standard, TargetHandType.Left);
            Assert.That(timeline.FindTargetData(new QNT_Timestamp(450), TargetBehavior.Standard, TargetHandType.Left) != null, "Target couldn't be created!");
            Assert.That(timeline.FindTargetData(new QNT_Timestamp(600 + 450), TargetBehavior.Standard, TargetHandType.Left) == null, "There shouldn't be any targets here, since the repeater zone isn't large enough!");

            timeline.RemoveAllRepeaters();
            timeline.DeleteAllTargets();
        }
    }
}
