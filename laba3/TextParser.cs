using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class TextParser
{
    public Text Parse(string input)
    {
        var text = new Text();
        
        if (string.IsNullOrEmpty(input))
            return text;
            
        // Разбиваем текст на предложения
        string[] sentenceSplits = Regex.Split(input, @"(?<=[.!?])");
        
        foreach (var sentenceSplit in sentenceSplits)
        {
            if (string.IsNullOrWhiteSpace(sentenceSplit)) 
                continue;
            
            var sentence = ParseSentence(sentenceSplit.Trim());
            if (sentence.Elements.Count > 0)
            {
                text.Sentences.Add(sentence);
            }
        }
        
        return text;
    }
    
    private Sentence ParseSentence(string sentenceText)
    {
        var sentence = new Sentence();
        
        // Улучшенное регулярное выражение для обработки апострофов (Where's)
        var matches = Regex.Matches(sentenceText, @"(\w+|\w+'\w+)|(\s+)|([^\w\s])");
        
        foreach (Match match in matches)
        {
            if (string.IsNullOrWhiteSpace(match.Value))
                continue;
                
            if (Regex.IsMatch(match.Value, @"^[\w']+$")) // Разрешаем апострофы в словах
            {
                // Это слово
                sentence.Elements.Add(new Word(match.Value));
            }
            else if (Regex.IsMatch(match.Value, @"[^\w\s]"))
            {
                // Это знак препинания
                sentence.Elements.Add(new Punctuation(match.Value));
            }
        }
        
        return sentence;
    }
}