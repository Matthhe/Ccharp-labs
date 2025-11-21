public class Word : SentenceElement
{
    public string Value { get; set; } = string.Empty;

    // Конструктор для сериализации
    public Word() { }
    
    public Word(string value)
    {
        Value = value;
    }
    
    public bool StartsWithConsonant()
    {
        if (string.IsNullOrEmpty(Value))
            return false;
            
        char firstChar = char.ToLower(Value[0]);
        string consonants = "бвгджзклмнпрстфхцчшщbcdfghjklmnpqrstvwxyz";
        return consonants.Contains(firstChar);
    }

    public override string ToString()
    {
        return Value;
    }
}