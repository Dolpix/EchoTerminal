namespace EchoTerminal
{
    public readonly struct CommandHintSegment
    {
        public readonly string Text;
        public readonly bool IsActive;

        public CommandHintSegment(string text, bool isActive)
        {
            Text = text;
            IsActive = isActive;
        }
    }
}