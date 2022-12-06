using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json; 
using Newtonsoft.Json;

namespace MeineKlassen
{
    [Serializable]
    public class Vocabulary
    {
        #region Variables
        public String dataFile { get; set; }
        public String mainLanguage { get; set; }
        public String secondaryLanguage { get; set; }
        public String name { get; set; }
        public Dictionary<String, List<String>> voc { get; set; }
        #endregion
        #region Constructors
        public Vocabulary(String mainLang, String secondaryLang)
        {
            mainLanguage = mainLang;
            secondaryLanguage = secondaryLang;
            name = mainLang + " - " + secondaryLang;
            voc = new Dictionary<String, List<String>>();
            dataFile = $"C:\\Vocabulary Manager\\Vocabularies\\{name}.json";
            Save();
        }
        #endregion
        #region Methods
        public void AddWord(String word, List<String> translates)
        {
            voc.Add(word, translates);
        }
        public void AddWordS(String word, List<String> translates)
        {
            AddWord(word, translates);
            Save();
        } 
        
        
        public void AddTranslate(String word, List<String> translates)
        { 
            foreach (var elem in translates) voc[word].Add(elem); 
        }
        public void AddTranslateS(String word, List<String> translates)
        {
            foreach (var elem in translates) voc[word].Add(elem); 
            Save();
        }


        public void DeleteWord(String word)
        {
            voc.Remove(word);
        }
        public void DeleteWordS(String word)
        {
            voc.Remove(word);
            Recreate();
        }
        
        
        public void DeleteTranslate(String word, List<String> translates)
        {
            foreach (var elem in translates)
            {
                voc[word].Remove(elem);
            }
        }
        public void DeleteTranslateS(String word, List<String> translates)
        {
            DeleteTranslate(word, translates);
            Recreate();
        }


        public Boolean ReplaceWord(String word, String toReplace)
        {
            foreach (var elem in voc.Keys)
            {
                if (word == elem)
                {
                    voc.Add(toReplace, voc[word]);
                    voc.Remove(word);
                    return true;
                }
            }

            return false;
        } 
        public Boolean ReplaceWordS(String word, String toReplace)
        {
            if (ReplaceWord(word, toReplace))
            {
                Recreate();
                return true;
            }

            return false;
        }
        
        
        public Boolean ReplaceTranslate(String word, String translate, String toReplace)
        {
            UInt16 i = 0;
            foreach (var elem in voc.Keys)
            {
                if (word == elem)
                {
                    foreach (var body in voc[word])
                    {
                        if (translate == body)
                        {
                            voc[word][i] = toReplace;
                            return true;
                        } 
                        ++i;
                    }
                    return false;
                }
            } 
            return false;
        } 
        public Boolean ReplaceTranslateS(String word, String translate, String toReplace)
        {
            if (ReplaceTranslate(word, translate, toReplace))
            {
                Recreate();
                return true;
            } 
            return false;
        }
        
        
        public void Save()
        {
            using (FileStream fs = new FileStream($"C:\\Vocabulary Manager\\Vocabularies\\{name}.json", FileMode.OpenOrCreate))
            {
                using (Utf8JsonWriter writer = new Utf8JsonWriter(fs))
                {
                    System.Text.Json.JsonSerializer.Serialize(writer, voc);
                }
            }
        } 
        public void Recreate()
        {
            using (FileStream fs = new FileStream($"C:\\Vocabulary Manager\\Vocabularies\\{name}.json", FileMode.Create))
            {
                using (Utf8JsonWriter writer = new Utf8JsonWriter(fs))
                {
                    System.Text.Json.JsonSerializer.Serialize(writer, voc);
                }
            }
        }
        public void Refresh()
        {
            using (StreamReader sr = new StreamReader($"C:\\Vocabulary Manager\\Vocabularies\\{name}.json"))
            {
                String json = sr.ReadToEnd();
                voc = JsonConvert.DeserializeObject<Dictionary<String, List<String>>>(json);
            }
        }
        public void RefreshFrom(String _name)
        {
            using (StreamReader sr = new StreamReader($"C:\\Vocabulary Manager\\Vocabularies\\{_name}.json"))
            {
                String json = sr.ReadToEnd();
                voc = JsonConvert.DeserializeObject<Dictionary<String, List<String>>>(json);
            }
        }


        public override string ToString()
        { 
                StringBuilder wordsList = new StringBuilder();
                foreach (var elem in voc)
                {
                    wordsList.Append(elem.Key + ": ");
                    foreach (var val in elem.Value)
                    {
                        wordsList.Append(val + ".");
                    }
                    wordsList.Append("\n");
                }
                return wordsList.ToString(); 
        }

        #endregion 
    }
}