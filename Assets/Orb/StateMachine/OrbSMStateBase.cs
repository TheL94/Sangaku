﻿using Deirin.StateMachine;

namespace Sangaku
{
    public abstract class OrbSMStateBase : StateBase
    {
        public OrbSMContext context;

        public override void SetUp(IContext _context)
        {
            context = _context as OrbSMContext;
        }

    } 
}