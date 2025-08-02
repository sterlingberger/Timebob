using System.Diagnostics;
using System.Drawing;

namespace Timebob
{
    public class Game
    {
        public int val_0 => DateTime.Now.Hour / 10;
        public int val_1 => DateTime.Now.Hour % 10;
        public int val_2 => DateTime.Now.Minute / 10;
        public int val_3 => DateTime.Now.Minute % 10;
        public int val_4 => DateTime.Now.Second / 10;
        public int val_5 => DateTime.Now.Second % 10;

        public int? ActiveButtonId = null;

        public DateTime? SwitchActive = null;

        public Stopwatch ScoreTimer = new Stopwatch();

        public void SwitchOff()
        {
            SwitchActive = null;
            foreach (KeyValuePair<int, Timebutton> kvp in Timebuttons)
            {
                kvp.Value.IsSwitched = false;
                kvp.Value.IsClicked = false;
            }
        }

        public Game(int level)
        {
            if (level == 0)
                return;

            StartNewLevel(level);
            ScoreTimer.Start();
        }

        public bool IsGameFinished()
        {
            foreach (KeyValuePair<int, Timebutton> kvp in Timebuttons)
            {
                if (kvp.Value.IsSolved == false)
                    return false;
            }

            ScoreTimer.Stop();

            return true;
        }

        private void StartNewLevel(int nswitches)
        {
            InitTimebuttons();

            List<int> alreadyswitched = new List<int>();
            Random rnd = new Random();

            for (int i = 0; i < nswitches; i++)
            {
                int first = rnd.Next(0, 14);
                while (alreadyswitched.Contains(first))
                {
                    first = rnd.Next(0, 14);
                }
                alreadyswitched.Add(first);

                int second = rnd.Next(0, 14);

                while (first == second || alreadyswitched.Contains(second))
                {
                    second = rnd.Next(0, 14);
                }
                alreadyswitched.Add(second);

                TimeValueId tmp1 = Timebuttons[first].TimeValueId;
                TimeValueId tmp2 = Timebuttons[second].TimeValueId;

                Timebuttons[first].TimeValueId = tmp2;
                Timebuttons[second].TimeValueId = tmp1;
            }
        }

        public void SwitchButtonStates(int btnid1, int btnid2)
        {
            TimeValueId tmp1 = Timebuttons[btnid1].TimeValueId;
            TimeValueId tmp2 = Timebuttons[btnid2].TimeValueId;

            Timebuttons[btnid1].TimeValueId = tmp2;
            Timebuttons[btnid2].TimeValueId = tmp1;

            SwitchActive = DateTime.Now;
            Timebuttons[btnid1].IsSwitched = true;
            Timebuttons[btnid2].IsSwitched = true;
            ActiveButtonId = null;
        }

        public Dictionary<int, Timebutton> Timebuttons = new Dictionary<int, Timebutton>
        {
           {0, new Timebutton(TimeValueId.T0, TimeUnit.Second0)},
           {1, new Timebutton(TimeValueId.M0, TimeUnit.Second0)},
           {2, new Timebutton(TimeValueId.B0, TimeUnit.Second0)},
           {3, new Timebutton(TimeValueId.TL, TimeUnit.Second0)},
           {4, new Timebutton(TimeValueId.TR, TimeUnit.Second0)},
           {5, new Timebutton(TimeValueId.BL, TimeUnit.Second0)},
           {6, new Timebutton(TimeValueId.BR, TimeUnit.Second0)},
           {7, new Timebutton(TimeValueId.T0, TimeUnit.Second1)},
           {8, new Timebutton(TimeValueId.M0, TimeUnit.Second1)},
           {9, new Timebutton(TimeValueId.B0, TimeUnit.Second1)},
           {10, new Timebutton(TimeValueId.TL,TimeUnit.Second1)},
           {11, new Timebutton(TimeValueId.TR,TimeUnit.Second1)},
           {12, new Timebutton(TimeValueId.BL,TimeUnit.Second1)},
           {13, new Timebutton(TimeValueId.BR,TimeUnit.Second1)}
        };

        private void InitTimebuttons()
        {
            Timebuttons = new Dictionary<int, Timebutton>
        {
           {0, new Timebutton(TimeValueId.T0, TimeUnit.Second0)},
           {1, new Timebutton(TimeValueId.M0, TimeUnit.Second0)},
           {2, new Timebutton(TimeValueId.B0, TimeUnit.Second0)},
           {3, new Timebutton(TimeValueId.TL, TimeUnit.Second0)},
           {4, new Timebutton(TimeValueId.TR, TimeUnit.Second0)},
           {5, new Timebutton(TimeValueId.BL, TimeUnit.Second0)},
           {6, new Timebutton(TimeValueId.BR, TimeUnit.Second0)},
           {7, new Timebutton(TimeValueId.T0, TimeUnit.Second1)},
           {8, new Timebutton(TimeValueId.M0, TimeUnit.Second1)},
           {9, new Timebutton(TimeValueId.B0, TimeUnit.Second1)},
           {10, new Timebutton(TimeValueId.TL,TimeUnit.Second1)},
           {11, new Timebutton(TimeValueId.TR,TimeUnit.Second1)},
           {12, new Timebutton(TimeValueId.BL,TimeUnit.Second1)},
           {13, new Timebutton(TimeValueId.BR,TimeUnit.Second1)}
        };
        }

        public enum TimeUnit
        {
            Hour0,
            Hour1,
            Minute0,
            Minute1,
            Second0,
            Second1
        }

        public enum TimeValueId //durchgezählte Segmentteile
        {
            T0 = 0,
            M0 = 1,
            B0 = 2,
            TL = 3,
            TR = 4,
            BL = 5,
            BR = 6
        }

    }

    public class Timebutton
    {
        public Game.TimeUnit TimeUnit { get; set; } //Position auf der Uhr, z.B. Hour0, Minute1, Second0, etc.
        public Game.TimeValueId TimeValueId { get; set; } //Segmentteil, z.B. T0, M0, B0, TL, TR, BL, BR //die wahre ValueId, die den Status des Buttons angibt, wechselt mit jedem Klick-Klick 2er Buttons

        public Game.TimeUnit OriginalTimeUnit { get; private set; }
        public Game.TimeValueId OriginalTimeValueId { get; private set; }

        public string Color => IsOn() ? "black" : "white";

        public bool IsClicked = false;

        public bool IsSwitched = false;

        public Timebutton(Game.TimeValueId timevalueid, Game.TimeUnit timeunit)
        {
            TimeUnit = timeunit;
            OriginalTimeUnit = timeunit;
            TimeValueId = timevalueid;
            OriginalTimeValueId = timevalueid;
        }

        public bool IsSolved => TimeUnit == OriginalTimeUnit && TimeValueId == OriginalTimeValueId;

        public string Border
        {
            get
            {
                if (IsSwitched)
                {
                    return "2px solid green"; //not yet implemented
                }

                if (IsClicked)
                    return "2px solid goldenrod";
                else
                    return "1px solid rgba(128, 128, 128, 0.2)";
            }
        }

        private bool IsOn()
        {
            int timevalue = 0;

            switch (TimeUnit)
            {
                case Game.TimeUnit.Hour0:
                    timevalue = DateTime.Now.Hour / 10;
                    break;
                case Game.TimeUnit.Hour1:
                    timevalue = DateTime.Now.Hour % 10;
                    break;
                case Game.TimeUnit.Minute0:
                    timevalue = DateTime.Now.Minute / 10;
                    break;
                case Game.TimeUnit.Minute1:
                    timevalue = DateTime.Now.Minute % 10;
                    break;
                case Game.TimeUnit.Second0:
                    timevalue = DateTime.Now.Second / 10;
                    break;
                case Game.TimeUnit.Second1:
                    timevalue = timevalue = DateTime.Now.Second % 10;
                    break;
            }
            switch (TimeValueId)
            {
                case Game.TimeValueId.T0:
                    if (timevalue == 0 || timevalue == 2 || timevalue == 3 || timevalue == 5 || timevalue == 6 || timevalue == 7 || timevalue == 8 || timevalue == 9)
                        return true;
                    else return false;
                case Game.TimeValueId.M0:
                    if (timevalue == 2 || timevalue == 3 || timevalue == 4 || timevalue == 5 || timevalue == 6 || timevalue == 8 || timevalue == 9)
                        return true;
                    else return false;
                case Game.TimeValueId.B0:
                    if (timevalue == 0 || timevalue == 2 || timevalue == 3 || timevalue == 5 || timevalue == 6 || timevalue == 8 || timevalue == 9)
                        return true;
                    else return false;
                case Game.TimeValueId.TL:
                    if (timevalue == 0 || timevalue == 4 || timevalue == 5 || timevalue == 6 || timevalue == 8 || timevalue == 9)
                        return true;
                    else return false;
                case Game.TimeValueId.TR:
                    if (timevalue == 0 || timevalue == 1 || timevalue == 2 || timevalue == 3 || timevalue == 4 || timevalue == 7 || timevalue == 8 || timevalue == 9)
                        return true;
                    else return false;
                case Game.TimeValueId.BL:
                    if (timevalue == 0 || timevalue == 2 || timevalue == 6 || timevalue == 8)
                        return true;
                    else return false;
                case Game.TimeValueId.BR:
                    if (timevalue == 0 || timevalue == 1 || timevalue == 3 || timevalue == 4 || timevalue == 5 || timevalue == 6 || timevalue == 7 || timevalue == 8 || timevalue == 9)
                        return true;
                    else return false;
                default: return false;
            }
        }
    }
}
