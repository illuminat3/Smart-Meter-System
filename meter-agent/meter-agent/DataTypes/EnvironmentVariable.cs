namespace meter_agent.DataTypes;

public abstract class EnvironmentVariable<T>
{
    public T? Value { get; set; }

    public abstract bool Validate(EnvironmentVariable<T> variable);
}