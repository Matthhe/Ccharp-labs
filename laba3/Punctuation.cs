public class Punctuation : SentenceElement
{
    public string Value { get; set; } = string.Empty;

    public Punctuation() { }
    
    public Punctuation(string value)
    {
        Value = value;
    }
    
    public bool IsEndOfSentence()
    {
        return Value == "." || Value == "!" || Value == "?" || Value == "..." || Value == "?!";
    }
    
    public string PunctuationType
    {
        get
        {
            return Value switch
            {
                "." => "Точка",
                "!" => "Восклицательный знак",
                "?" => "Вопросительный знак",
                "," => "Запятая",
                ":" => "Двоеточие",
                ";" => "Точка с запятой",
                "..." => "Многоточие",
                "?!" => "Вопрос-восклицание",
                _ => "Другой знак"
            };
        }
    }

    public override string ToString()
    {
        return Value;
    }
}