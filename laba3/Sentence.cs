using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

public class Sentence
{
    [XmlElement("word")]
    public List<string> XmlWords 
    { 
        get 
        { 
            return Elements.OfType<Word>().Select(w => w.Value).ToList(); 
        }
        set 
        {
            // Для десериализации создаем объекты Word из строк
            if (value != null)
            {
                Elements.Clear();
                foreach (var wordValue in value)
                {
                    Elements.Add(new Word(wordValue));
                }
            }
        }
    }

    // Коллекция элементов для работы программы
    [XmlIgnore]
    public List<SentenceElement> Elements { get; set; } = new List<SentenceElement>();

    [XmlIgnore]
    public int WordCount => Elements.OfType<Word>().Count();

    [XmlIgnore]
    public int Length => ToString().Length;

    [XmlIgnore]
    public bool IsQuestion => Elements.OfType<Punctuation>().Any(p => p.Value == "?");

    public IEnumerable<Word> GetUniqueWords()
    {
        return Elements.OfType<Word>().GroupBy(w => w.Value.ToLower())
                                    .Select(g => g.First());
    }

    public override string ToString()
    {
        // Восстанавливаем пробелы между словами при выводе
        var result = new System.Text.StringBuilder();
        for (int i = 0; i < Elements.Count; i++)
        {
            if (i > 0 && Elements[i] is Word && Elements[i-1] is Word)
            {
                result.Append(" ");
            }
            result.Append(Elements[i].ToString());
        }
        return result.ToString();
    }
}