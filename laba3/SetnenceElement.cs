public abstract class SentenceElement
{
    public string Value { get; set; } = string.Empty;
    
    protected SentenceElement() { }
    
    public override string ToString() => Value;
}
// base class for each element of our 
// sentence