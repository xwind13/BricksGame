using BricksGame.Logic;
using System;

namespace Assets.Scripts.Events
{
    public class SideFieldClickEventArgs : EventArgs
    {
        public Side Side { get; set; }

        public uint PosIdx { get; set; }
    }
}
