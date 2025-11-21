using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

[XmlRoot("text")]
public class Text
{
    [XmlElement("sentence")]
    public List<Sentence> Sentences { get; set; } = new List<Sentence>();
    
    public List<Sentence> GetSentencesOrderedByWordCount()
    {
        return Sentences.OrderBy(s => s.WordCount).ToList();
    }

    public List<Sentence> GetSentencesOrderedByLength()
    {
        return Sentences.OrderBy(s => s.Length).ToList();
    }

    public List<string> FindWordsInQuestions(int length)
    {
        var words = new List<string>();
        foreach (var sentence in Sentences.Where(s => s.IsQuestion))
        {
            var uniqueWords = sentence.GetUniqueWords()
                                      .Where(w => w.Value.Length == length)
                                      .Select(w => w.Value);
            words.AddRange(uniqueWords);
        }
        return words.Distinct().ToList();
    }

    public void RemoveWords(int length)
    {
        foreach (var sentence in Sentences)
        {
            var wordsToRemove = sentence.Elements.OfType<Word>()
                                        .Where(w => w.Value.Length == length && w.StartsWithConsonant())
                                        .ToList();

            foreach (var word in wordsToRemove)
            {
                sentence.Elements.Remove(word);
            }
        }
    }

    public void ReplaceWordsInSentence(int sentenceIndex, int wordLength, string replacement)
    {
        if (sentenceIndex < 0 || sentenceIndex >= Sentences.Count) return;

        var sentence = Sentences[sentenceIndex];
        foreach (var element in sentence.Elements)
        {
            if (element is Word word && word.Value.Length == wordLength)
            {
                word.Value = replacement;
            }
        }
    }

    public void ReplaceWordsInAllSentences(int wordLength, string replacement)
    {
        int totalReplaced = 0;
        
        foreach (var sentence in Sentences)
        {
            foreach (var element in sentence.Elements)
            {
                if (element is Word word && word.Value.Length == wordLength)
                {
                    word.Value = replacement;
                    totalReplaced++;
                }
            }
        }
        
        Console.WriteLine($"Всего заменено слов: {totalReplaced}");
    }

    public void RemoveStopWords(HashSet<string> stopWords)
    {
        foreach (var sentence in Sentences)
        {
            var wordsToRemove = sentence.Elements.OfType<Word>()
                                        .Where(w => stopWords.Contains(w.Value.ToLower()))
                                        .ToList();
            
            foreach (var word in wordsToRemove)
            {
                sentence.Elements.Remove(word);
            }
        }
    }

    public Concordance BuildConcordance()
    {
        var concordance = new Concordance();
        concordance.BuildFromText(this);
        return concordance;
    }
    
    public override string ToString()
    {
        return string.Join(" ", Sentences);
    }
}