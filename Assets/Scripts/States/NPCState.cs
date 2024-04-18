namespace NPC.Core
{
    public abstract class NPCState
    {

        public abstract void OnEnter();
        public abstract void HandleUpdate();
        public abstract void OnExit();

    }
}

