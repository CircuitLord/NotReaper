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

        public void DeleteTarget(TargetData data) {
            var action = new NRActionRemoveNote {targetData = data};
            timeline.Tools.undoRedoManager.AddAction(action);
        }

        public void MoveTarget(TargetData data, QNT_Timestamp newTime) {
            TargetTimelineMoveIntent intent = new TargetTimelineMoveIntent();
            intent.targetData = data;
            intent.startTick = data.time;
            intent.intendedTick = newTime;

            List<TargetTimelineMoveIntent> list = new List<TargetTimelineMoveIntent>();
            list.Add(intent);
            timeline.MoveTimelineTargets(list);
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


#if UNITY_EDITOR
            timeline.SetupBlankTest();
#endif
            //Setup repeater sections (0 - 500)
            RepeaterSection section = new RepeaterSection(0, new QNT_Timestamp(0), new QNT_Timestamp(500));
            timeline.AddRepeaterSection(section);

            // (600 - 1000) Only 400 long, so any notes from 400-500 should not be replicated
            RepeaterSection section2 = new RepeaterSection(0, new QNT_Timestamp(600), new QNT_Timestamp(1000));
            timeline.AddRepeaterSection(section2);

            // (1200 - 1700) Another section, 500 long, so we can check replication that happens here but not in section2
            RepeaterSection section3 = new RepeaterSection(0, new QNT_Timestamp(1200), new QNT_Timestamp(1700));
            timeline.AddRepeaterSection(section3);

            // (2000 - 2500) Another repeater zone, 500 wide, with a different id
            RepeaterSection section4 = new RepeaterSection(1, new QNT_Timestamp(2000), new QNT_Timestamp(2500));
            timeline.AddRepeaterSection(section4);

            // (2600 - 3100) with id 1, to test replication of another zone
            RepeaterSection section5 = new RepeaterSection(1, new QNT_Timestamp(2600), new QNT_Timestamp(3100));
            timeline.AddRepeaterSection(section5);

            //Create a target at time 50, this should be repeated in the second section, at time 650
            CreateTarget(new QNT_Timestamp(50), TargetBehavior.Standard, TargetHandType.Left);
            Assert.That(timeline.FindTargetData(new QNT_Timestamp(50), TargetBehavior.Standard, TargetHandType.Left) != null, "Target couldn't be created!");
            Assert.That(timeline.FindTargetData(new QNT_Timestamp(600 + 50), TargetBehavior.Standard, TargetHandType.Left) != null, "Target wasn't created in repeater!");
            Assert.That(timeline.FindTargetData(new QNT_Timestamp(1200 + 50), TargetBehavior.Standard, TargetHandType.Left) != null, "Target wasn't created in repeater!");

            //Create another target at 450, this shouldn't be repeated since 600 + 450 = 1050, which is outside the repeated section
            CreateTarget(new QNT_Timestamp(450), TargetBehavior.Standard, TargetHandType.Left);
            Assert.That(timeline.FindTargetData(new QNT_Timestamp(450), TargetBehavior.Standard, TargetHandType.Left) != null, "Target couldn't be created!");
            Assert.That(timeline.FindTargetData(new QNT_Timestamp(600 + 450), TargetBehavior.Standard, TargetHandType.Left) == null, "There shouldn't be any targets here, since the repeater zone isn't large enough!");
            Assert.That(timeline.FindTargetData(new QNT_Timestamp(1200 + 450), TargetBehavior.Standard, TargetHandType.Left) != null, "This repeater section was long enough, so the target should have been created here.");

            //Delete target at time 50, this should be replicated, removing the target at time 650
            DeleteTarget(timeline.FindTargetData(new QNT_Timestamp(50), TargetBehavior.Standard, TargetHandType.Left));
            Assert.That(timeline.FindTargetData(new QNT_Timestamp(50), TargetBehavior.Standard, TargetHandType.Left) == null, "Target should have been found and deleted!");
            Assert.That(timeline.FindTargetData(new QNT_Timestamp(600 + 50), TargetBehavior.Standard, TargetHandType.Left) == null, "Target should have been deleted, since the the target at tick 50 was removed.");
            Assert.That(timeline.FindTargetData(new QNT_Timestamp(1200 + 50), TargetBehavior.Standard, TargetHandType.Left) == null, "Target should have been deleted, since the the target at tick 50 was removed.");

            //Delete target at time 450, this should always just work
            DeleteTarget(timeline.FindTargetData(new QNT_Timestamp(450), TargetBehavior.Standard, TargetHandType.Left));
            Assert.That(timeline.FindTargetData(new QNT_Timestamp(450), TargetBehavior.Standard, TargetHandType.Left) == null, "Target was not properly deleted!");
            Assert.That(timeline.FindTargetData(new QNT_Timestamp(1200 + 450), TargetBehavior.Standard, TargetHandType.Left) == null, "Target in repeater zone was deleted, so this one should have been as well.");

            //Create target outside repeater, move inside, check replication
            {
                //First we create a target at 550, which is not in any repeater zone
                CreateTarget(new QNT_Timestamp(550), TargetBehavior.Standard, TargetHandType.Left);

                //First, we move it to time 450, which is inside repeater zone 1, and should be replicated to zone 3, but not zone 2
                MoveTarget(timeline.FindTargetData(new QNT_Timestamp(550), TargetBehavior.Standard, TargetHandType.Left), new QNT_Timestamp(450));
                Assert.That(timeline.FindTargetData(new QNT_Timestamp(450), TargetBehavior.Standard, TargetHandType.Left) != null, "Target wan't moved properly!");
                Assert.That(timeline.FindTargetData(new QNT_Timestamp(600 + 450), TargetBehavior.Standard, TargetHandType.Left) == null, "Target should not be replicated at this location, since the repeater zone isn't large enough.");
                Assert.That(timeline.FindTargetData(new QNT_Timestamp(1200 + 450), TargetBehavior.Standard, TargetHandType.Left) != null, "Target was not properly replicated to this location, even though the repeater is long enough.");

                //Next, move the target into a zone which should be replicated across all 3 zones
                MoveTarget(timeline.FindTargetData(new QNT_Timestamp(450), TargetBehavior.Standard, TargetHandType.Left), new QNT_Timestamp(250));
                Assert.That(timeline.FindTargetData(new QNT_Timestamp(250), TargetBehavior.Standard, TargetHandType.Left) != null, "Target wan't moved properly!");
                Assert.That(timeline.FindTargetData(new QNT_Timestamp(600 + 250), TargetBehavior.Standard, TargetHandType.Left) != null, "Target was not properly replicated to this location, even though the repeater is long enough.");
                Assert.That(timeline.FindTargetData(new QNT_Timestamp(1200 + 250), TargetBehavior.Standard, TargetHandType.Left) != null, "Target was not properly replicated to this location, even though the repeater is long enough.");

                //Move the target back to 450, check proper replication
                MoveTarget(timeline.FindTargetData(new QNT_Timestamp(250), TargetBehavior.Standard, TargetHandType.Left), new QNT_Timestamp(450));
                Assert.That(timeline.FindTargetData(new QNT_Timestamp(450), TargetBehavior.Standard, TargetHandType.Left) != null, "Target wan't moved properly!");
                Assert.That(timeline.FindTargetData(new QNT_Timestamp(600 + 450), TargetBehavior.Standard, TargetHandType.Left) == null, "Target should not be replicated at this location, since the repeater zone isn't large enough.");
                Assert.That(timeline.FindTargetData(new QNT_Timestamp(1200 + 450), TargetBehavior.Standard, TargetHandType.Left) != null, "Target was not properly replicated to this location, even though the repeater is long enough.");

                //Move the target outside, check replication
                MoveTarget(timeline.FindTargetData(new QNT_Timestamp(450), TargetBehavior.Standard, TargetHandType.Left), new QNT_Timestamp(550));
                Assert.That(timeline.FindTargetData(new QNT_Timestamp(550), TargetBehavior.Standard, TargetHandType.Left) != null, "Target wan't moved properly!");
                Assert.That(timeline.FindTargetData(new QNT_Timestamp(600 + 550), TargetBehavior.Standard, TargetHandType.Left) == null, "Target shouldn't be replicated at this position, since it's outside the repeater zone!");
                Assert.That(timeline.FindTargetData(new QNT_Timestamp(1200 + 550), TargetBehavior.Standard, TargetHandType.Left) == null, "Target shouldn't be replicated at this position, since it's outside the repeater zone!");

                //Destroy Target
                DeleteTarget(timeline.FindTargetData(new QNT_Timestamp(550), TargetBehavior.Standard, TargetHandType.Left));
            }

            //Move from a repeater zone to another different id zone
            {
                //First we create a target at 50, which we already know is properly replicated (from above tests)
                CreateTarget(new QNT_Timestamp(50), TargetBehavior.Standard, TargetHandType.Left);

                MoveTarget(timeline.FindTargetData(new QNT_Timestamp(50), TargetBehavior.Standard, TargetHandType.Left), new QNT_Timestamp(2050));
                Assert.That(timeline.FindTargetData(new QNT_Timestamp(50), TargetBehavior.Standard, TargetHandType.Left) == null, "Target wasn't moved properly!");
                Assert.That(timeline.FindTargetData(new QNT_Timestamp(600 + 50), TargetBehavior.Standard, TargetHandType.Left) == null, "Target shouldn't be replicated at this position, since we moved the target ouside of this repeater ID!");
                Assert.That(timeline.FindTargetData(new QNT_Timestamp(1200 + 50), TargetBehavior.Standard, TargetHandType.Left) == null, "Target shouldn't be replicated at this position, since we moved the target ouside of this repeater ID!");
                Assert.That(timeline.FindTargetData(new QNT_Timestamp(2000 + 50), TargetBehavior.Standard, TargetHandType.Left) != null, "Target wasn't moved into it's new position!");
                Assert.That(timeline.FindTargetData(new QNT_Timestamp(2600 + 50), TargetBehavior.Standard, TargetHandType.Left) != null, "Target wasn't replicated properly into the new repeater section!");
            }

            //Move from a repeater zone out into another repeater zone of the same id
            {
                CreateTarget(new QNT_Timestamp(50), TargetBehavior.Standard, TargetHandType.Left);
                MoveTarget(timeline.FindTargetData(new QNT_Timestamp(50), TargetBehavior.Standard, TargetHandType.Left), new QNT_Timestamp(700));

                Assert.That(timeline.FindTargetData(new QNT_Timestamp(50), TargetBehavior.Standard, TargetHandType.Left) == null, "Target wasn't moved properly!");
                Assert.That(timeline.FindTargetData(new QNT_Timestamp(600 + 50), TargetBehavior.Standard, TargetHandType.Left) == null, "Target wasn't removed properly!");
                Assert.That(timeline.FindTargetData(new QNT_Timestamp(1200 + 50), TargetBehavior.Standard, TargetHandType.Left) == null, "Target wasn't removed properly!");

                Assert.That(timeline.FindTargetData(new QNT_Timestamp(100), TargetBehavior.Standard, TargetHandType.Left) != null, "Target wasn't replicated properly!");
                Assert.That(timeline.FindTargetData(new QNT_Timestamp(600 + 100), TargetBehavior.Standard, TargetHandType.Left) != null, "Target wasn't moved properly!");
                Assert.That(timeline.FindTargetData(new QNT_Timestamp(1200 + 100), TargetBehavior.Standard, TargetHandType.Left) != null, "Target wasn't replicated properly!");
            }

            //Creating repeater zone in a section with notes should destroy notes

            //Copy paste?
        }
    }
}
