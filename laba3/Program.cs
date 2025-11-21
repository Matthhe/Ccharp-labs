using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

class Program
{
    static void Main(string[] args)
    {
        string concordanceText = "Peter Piper picked a peck of pickled peppers. " +
                               "A peck of pickled peppers Peter Piper picked. " +
                               "If Peter Piper picked a peck of pickled peppers, " +
                               "Where's the peck of pickled peppers Peter Piper picked?";

        var parser = new TextParser();
        Text text = parser.Parse(concordanceText);

        Console.WriteLine("Исходный текст для конкорданса:");
        Console.WriteLine(concordanceText);
        Console.WriteLine();

        // 1. Вывести все предложения в порядке возрастания количества слов
        Console.WriteLine("1. Предложения в порядке возрастания количества слов:");
        var sentencesByWordCount = text.GetSentencesOrderedByWordCount();
        foreach (var sentence in sentencesByWordCount)
        {
            Console.WriteLine($"   Слов: {sentence.WordCount} -> {sentence}");
        }
        Console.WriteLine();

        // 2. Вывести все предложения в порядке возрастания длины предложения
        Console.WriteLine("2. Предложения в порядке возрастания длины:");
        var sentencesByLength = text.GetSentencesOrderedByLength();
        foreach (var sentence in sentencesByLength)
        {
            Console.WriteLine($"   Длина: {sentence.Length} -> {sentence}");
        }
        Console.WriteLine();

        // 3. Во всех вопросительных предложениях найти слова заданной длины
        Console.WriteLine("3. Слова длиной 3 в вопросительных предложениях:");
        var wordsInQuestions = text.FindWordsInQuestions(3);
        if (wordsInQuestions.Count > 0)
        {
            foreach (var word in wordsInQuestions)
            {
                Console.WriteLine($"{word}");
            }
        }
        else
        {
            Console.WriteLine("  Слов заданной длины в вопросительных предложениях не найдено");
        }
        Console.WriteLine();

        // 4. Удалить из текста все слова заданной длины, начинающиеся с согласной буквы
        Console.WriteLine("4. Удаляем слова длиной 4, начинающиеся с согласной буквы:");
        Console.WriteLine("Текст до удаления:");
        Console.WriteLine(text);
        text.RemoveWords(4);
        Console.WriteLine("Текст после удаления:");
        Console.WriteLine(text);
        Console.WriteLine();

        // 5. В некотором предложении текста заменить слова заданной длины на указанную подстроку
        Console.WriteLine("5. Замена слов заданной длины на подстроку:");
        Console.WriteLine("Текст до замены:");
        Console.WriteLine(text);
        
        // Заменяем слова длиной 3 символа в предложении с индексом 1
        Console.WriteLine("Заменяем слова длиной 3 символа в предложении с индексом 1 на '3':");
        text.ReplaceWordsInSentence(1, 3, "3");
        Console.WriteLine(text);
        Console.WriteLine();
        
        // Дополнительный пример
        Console.WriteLine("Заменяем слова длиной 5 символов во всех предложениях на '5':");
        text.ReplaceWordsInAllSentences(5, "5");
        Console.WriteLine(text);
        Console.WriteLine();

        // 6. Удалить стоп-слова из текста
        Console.WriteLine("6. Удаляем стоп-слова:");
        Console.WriteLine("Текст до удаления стоп-слов:");
        Console.WriteLine(text);
        
        var stopWords = LoadStopWords("stopwords_ru.txt");
        text.RemoveStopWords(stopWords);
        
        Console.WriteLine("Текст после удаления стоп-слов:");
        Console.WriteLine(text);
        Console.WriteLine();

        // 7. Экспортировать текстовый объект в XML-документ
        Console.WriteLine("7. Экспорт в XML-документ:");
        SerializeToXml(text, "text.xml");
        Console.WriteLine("Текст сериализован в XML файл: text.xml");

        // 8. Построение конкорданса
        Console.WriteLine("8. КОНКОРДАНС ТЕКСТА:");
        var concordance = text.BuildConcordance();
        string formattedConcordance = concordance.GetFormattedConcordance();
        Console.WriteLine(formattedConcordance);

        // Демонстрация поиска конкретного слова
        Console.WriteLine("Информация о слове 'piper':");
        var wordInfo = concordance.GetWordInfo("piper");
        if (wordInfo.HasValue)
        {
            Console.WriteLine($"Количество вхождений: {wordInfo.Value.totalCount}");
            Console.WriteLine($"Номера предложений: {string.Join(", ", wordInfo.Value.sentenceNumbers)}");
        }
        Console.WriteLine();
    }

    // Загрузка стоп-слов из файла
    static HashSet<string> LoadStopWords(string filename)
    {
        var stopWords = new HashSet<string>();
        try
        {
            if (File.Exists(filename))
            {
                var lines = File.ReadAllLines(filename);
                foreach (var line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                        stopWords.Add(line.Trim().ToLower());
                }
                Console.WriteLine($"   Загружено стоп-слов: {stopWords.Count}");
            }
            else
            {
                Console.WriteLine($"   Файл {filename} не найден. Используем пустой список стоп-слов.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   Ошибка загрузки стоп-слов: {ex.Message}");
        }
        return stopWords;
    }

    // Сериализация в XML
    static void SerializeToXml(Text text, string filename)
    {
        try
        {
            var serializer = new XmlSerializer(typeof(Text), new Type[] 
            { 
                typeof(Sentence)
            });
            
            using (var writer = new StreamWriter(filename))
            {
                serializer.Serialize(writer, text);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка сериализации: {ex.Message}");
            Console.WriteLine($"Подробности: {ex.InnerException?.Message}");
        }
    }
}