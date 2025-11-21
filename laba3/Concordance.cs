using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Concordance
{
    // Словаь для хранение конкорданса: слово (общее количество, номера предложений)
    private SortedDictionary<string, (int totalCount, SortedSet<int> sentenceNumbers)> concordanceData;

    public Concordance()
    {
        concordanceData = new SortedDictionary<string, (int, SortedSet<int>)>();
    }

    // Построение конкорданса из текста
    public void BuildFromText(Text text)
    {
        concordanceData.Clear();

        for (int i = 0; i < text.Sentences.Count; i++)
        {
            var sentence = text.Sentences[i];
            var sentenceNumber = i + 1; // Нумерация с 1

            // Все слова из предложения
            var wordsInSentence = sentence.Elements
                .OfType<Word>()
                .Select(w => w.Value.ToLower())
                .ToList();

            // Группировка слов в предление для подсчета вхождений
            var wordGroups = wordsInSentence
                .GroupBy(word => word)
                .Select(g => new { Word = g.Key, Count = g.Count() });

            foreach (var wordGroup in wordGroups)
            {
                string word = wordGroup.Word;
                int countInSentence = wordGroup.Count;

                if (!concordanceData.ContainsKey(word))
                {
                    // Создается запись
                    concordanceData[word] = (countInSentence, new SortedSet<int> { sentenceNumber });
                }
                else
                {
                    // Если слово уже есть, то бновляем данные
                    var (totalCount, sentenceNumbers) = concordanceData[word];
                    totalCount += countInSentence;
                    sentenceNumbers.Add(sentenceNumber);
                    concordanceData[word] = (totalCount, sentenceNumbers);
                }
            }
        }
    }

    public string GetFormattedConcordance()
    {
        var result = new StringBuilder();
        
        // Макс длиннас слова
        int maxWordLength = concordanceData.Keys.Max(word => word.Length);

        foreach (var entry in concordanceData)
        {
            string word = entry.Key;
            int totalCount = entry.Value.totalCount;
            var sentenceNumbers = entry.Value.sentenceNumbers;

            // Форматируем строку: слово + точки + количество + номера предложений
            string dots = new string('.', maxWordLength - word.Length + 5);
            string sentenceNumbersStr = string.Join(" ", sentenceNumbers);
            
            result.AppendLine($"{word}{dots}{totalCount}: {sentenceNumbersStr}");
        }

        return result.ToString();
    }

    // Получение сырых данных конкорданса (для дополнительного использования)
    public SortedDictionary<string, (int totalCount, SortedSet<int> sentenceNumbers)> GetRawData()
    {
        return new SortedDictionary<string, (int, SortedSet<int>)>(concordanceData);
    }

    // Поиск информации о конкретном слове
    public (int totalCount, SortedSet<int> sentenceNumbers)? GetWordInfo(string word)
    {
        string lowerWord = word.ToLower();
        if (concordanceData.ContainsKey(lowerWord))
        {
            return concordanceData[lowerWord];
        }
        return null;
    }

    // Получение всех слов конкорданса
    public IEnumerable<string> GetAllWords()
    {
        return concordanceData.Keys;
    }

    // Очистка конкорданса
    public void Clear()
    {
        concordanceData.Clear();
    }
}