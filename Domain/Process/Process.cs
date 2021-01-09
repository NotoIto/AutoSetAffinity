namespace Domain
{
    public record ProcessName(string Value);
    public record ProcessId(int Value);
    public record Process(ProcessName Name, ProcessId Id);
}
