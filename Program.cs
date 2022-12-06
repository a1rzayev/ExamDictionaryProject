using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.ConstrainedExecution;
//using System.Security.Cryptography;
//using System.Text;
using System.Threading;
using System.Xml.Serialization;
using MeineKlassen;

namespace VocabularyManager
{
    class Program
    { 
        #region Variables
        private static Vocabulary currentVocabulary = new Vocabulary("", "");
        static List<String> vocabulariesList = new List<String>();
        #endregion
        #region Methods
        static void RefreshVocabulariesList()
        { 
            using (FileStream fs = new FileStream("C:\\Vocabulary Manager\\Dictionaries List.xml", FileMode.Open))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<String>));
                vocabulariesList = serializer.Deserialize(fs) as List<String>;
            }
        }
        static void SaveVocabulariesList()
        { 
            using (FileStream fs = new FileStream("C:\\Vocabulary Manager\\Dictionaries List.xml", FileMode.Open))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<String>));
                serializer.Serialize(fs, vocabulariesList);
            }
        }
        
        
        
        static UInt16 MainMenu()
        {
            currentVocabulary = new Vocabulary("", "");
            UInt16 choise = 0;
            Boolean parseBool = false; 
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Vocabulary Manager\n\n");
                Console.WriteLine("1) Add new vocabularies");
                Console.WriteLine("2) Edit vocabulary");
                Console.WriteLine("3) Current vocabularies");
                Console.WriteLine("4) Translate");
                Console.WriteLine("5) Close");
                Console.Write("\nYour choice: ");
                parseBool = UInt16.TryParse(Console.ReadLine(), out choise);
                if (parseBool && (choise >= 1 && choise <= 5)) break;
                else { Console.Write("\nWrong input!"); Thread.Sleep(400); }
            } 
            return choise;
        } 
        static UInt16 AddVocabularyMenu()
        {
            UInt16 choise = 0;
            Boolean parseBool = false;
            MainLangInput:
            Console.Clear();
            Console.WriteLine("Vocabulary Manager\n\n");
            Console.Write("Enter first language: ");
            String mainLanguage = Console.ReadLine().ToLower();
            foreach (var elem in mainLanguage)
            {
                if (elem == '/' || elem == '\\' || elem == '.') {
                    Console.Write("\nWrong input!");
                    Thread.Sleep(400);
                    goto MainLangInput;
                }
            }
            
            SecondaryLangInput:
            Console.Write("Enter second language: ");
            String secondaryLanguage = Console.ReadLine().ToLower();
            foreach (var elem in secondaryLanguage)
            {
                if (elem == '/' || elem == '\\' || elem == '.')
                {
                    Console.Write("\nWrong input!");
                    Thread.Sleep(400); 
                    Console.Clear();
                    Console.WriteLine("Vocabulary Manager\n\n");
                    Console.WriteLine($"Enter first language: {mainLanguage}");
                    goto SecondaryLangInput;
                }
            }

            if (File.Exists($@"C:\Vocabulary Manager\Vocabularies\{mainLanguage} - {secondaryLanguage}.json"))
            {
                Console.WriteLine($"{mainLanguage} - {secondaryLanguage} is already exist! please try again!");
                Thread.Sleep(400);
                goto MainLangInput;
            }

            Vocabulary newVoc = new Vocabulary(mainLanguage, secondaryLanguage);
            vocabulariesList.Add(newVoc.name);
            SaveVocabulariesList();
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Vocabulary Manager\n\n");
                Console.WriteLine($"Enter first language: {newVoc.mainLanguage}");
                Console.Write($"Enter second language: {newVoc.secondaryLanguage}");
                Console.WriteLine($"\n\n{newVoc.name} has been successfully added");
                Console.WriteLine("\n1) Back to main menu");
                Console.WriteLine("2) Edit language");
                Console.WriteLine("3) Close");
                Console.Write("\nYour choice: ");
                parseBool = UInt16.TryParse(Console.ReadLine(), out choise);
                if (parseBool && (choise >= 1 && choise <= 3)) break;
                else
                {
                    Console.Write("\nWrong input!");
                    Thread.Sleep(400);
                }
            }

            if (choise == 2) currentVocabulary = newVoc; 
            return choise;
        } 
        static UInt16 EditVocabularyMenu()
        {
            EditVocabularyMenu:
            RefreshVocabulariesList();
            UInt16 i = 1; 
            Boolean vocExist = false;
            Console.Clear();
            Console.WriteLine("Vocabulary Manager\n\n");
            Console.WriteLine("Vocabularies:");
            foreach (var elem in vocabulariesList) { Console.WriteLine($"{i}. {elem}"); ++i; }
            Console.Write("Enter name of vocabulary: ");
            String vocToEdit = Console.ReadLine();
            foreach (var elem in vocabulariesList)
            {
                if (vocToEdit == elem)
                {
                    vocExist = true;
                    break;
                }
            }
            if (!vocExist)
            {
                Console.WriteLine($"There are no {vocToEdit} exist!");   
                Thread.Sleep(400);
                goto EditVocabularyMenu; 
            } 
            currentVocabulary.name = vocToEdit;
            currentVocabulary.RefreshFrom(vocToEdit);
            return EditLanguageMenu();
        }
        static UInt16 ShowVocabulariesMenu()
        {
            UInt16 choise = 0;
            Boolean parseBool = false;
            UInt16 i = 1;  
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Vocabulary Manager\n\n"); 
                foreach (var elem in vocabulariesList) { Console.WriteLine($"{i}. {elem}"); ++i; }
                Console.WriteLine("\n1) Back to main menu"); 
                Console.WriteLine("2) Close");
                Console.Write("\nYour choice: ");
                parseBool = UInt16.TryParse(Console.ReadLine(), out choise);
                if (parseBool && (choise >= 1 && choise <= 2)) break;
                else { Console.Write("\nWrong input!"); Thread.Sleep(400); }
            }
            return choise;
        } 
        static UInt16 TranslateMenu()
        {
            RefreshVocabulariesList();
            TranslateMenu:
            Boolean parseBool = false, vocExist = false, wordExist = false;
            UInt16 choise = 0, i = 1;
            Console.Clear();
            Console.WriteLine("Vocabulary Manager\n\n");
            Console.WriteLine("Vocabularies:");
            foreach (var elem in vocabulariesList) { Console.WriteLine($"{i}. {elem}"); ++i; }
            Console.Write("Enter name of vocabulary([*] back to main menu): ");
            String vocToEdit = Console.ReadLine();
            if (vocToEdit == "*") return 2;
            foreach (var elem in vocabulariesList)
            {
                if (vocToEdit == elem) vocExist = true; break;
            }

            if (!vocExist)
            {
                Console.WriteLine($"There are no {vocToEdit} exist!");   
                Thread.Sleep(400);
                goto TranslateMenu; 
            } 
            currentVocabulary.name = vocToEdit;
            currentVocabulary.RefreshFrom(vocToEdit);
            WordMenu:
            while (true)
            {
                i = 1;
                Console.Clear();
                Console.WriteLine("Vocabulary Manager\n\n");
                Console.WriteLine("Vocabularies:");
                foreach (var elem in vocabulariesList) { Console.WriteLine($"{i}. {elem}"); ++i; }
                Console.WriteLine($"Enter name of vocabulary: {vocToEdit}"); 
                Console.Write("Enter word to translate([*] back to main menu): ");
                String wordToTranslate = Console.ReadLine();
                if (wordToTranslate == "*") return 2;
                foreach (var elem in currentVocabulary.voc.Keys)
                {
                    if(wordToTranslate == elem)
                    {
                        wordExist = true;
                        break;
                    }
                }
                
                if (!wordExist)
                {
                    Console.WriteLine($"There are no {wordToTranslate} exist!");   
                    Thread.Sleep(400);
                    continue;
                }

                i = 1;
                Console.WriteLine("Translates: ");
                foreach (var elem in currentVocabulary.voc[wordToTranslate])
                {
                    Console.WriteLine($"{i + 1}) {elem}");
                    ++i;
                }
                
                while (true)
                {
                    Console.WriteLine("\n\n1) Translate others");
                    Console.WriteLine("2) Back to main menu"); 
                    Console.WriteLine("3) Close");
                    Console.Write("\nYour choice: ");
                    parseBool = UInt16.TryParse(Console.ReadLine(), out choise);
                    if (parseBool && (choise >= 1 && choise <= 3)) break;
                    else { Console.Write("\nWrong input!"); Thread.Sleep(400); }
                }

                if (choise == 1) goto WordMenu;
                else return choise;
            }
        } 
        
        
        
        static UInt16 EditLanguageMenu()
        {
            EditMenu:
            UInt16 choise = 0;
            Boolean parseBool = false; 
            String newWord = "";
            String wordToAddTranslate = "";
            String newTranslate = "";
            
            String wordToDelete = "";
            String wordToDeleteTranslate = "";
            String translateToDelete = "";
             
            String wordToReplace = "";
            String replacedWord = "";
            String wordToReplaceTranslate = "";
            String translateToReplace = "";
            String replacedTranslate = "";
            List<String> translates = new List<String>();
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Vocabulary Manager\n\n"); 
                Console.WriteLine("1) Add"); 
                Console.WriteLine("2) Delete");
                Console.WriteLine("3) Replace");
                Console.WriteLine("4) Back to main menu");
                Console.WriteLine("5) Close");
                Console.Write("\nYour choice: ");
                parseBool = UInt16.TryParse(Console.ReadLine(), out choise);
                if (parseBool && (choise >= 1 && choise <= 5)) break;
                else { Console.Write("\nWrong input!"); Thread.Sleep(400); }
            } 
            switch (choise)
            {
                case 1:
                    while (true)
                    {
                        Console.Clear();
                        Console.WriteLine("Vocabulary Manager\n\n");
                        Console.WriteLine("1) Add Word");
                        Console.WriteLine("2) Add Translate");
                        Console.WriteLine("3) Back to edit menu");
                        Console.Write("\nYour choice: ");
                        parseBool = UInt16.TryParse(Console.ReadLine(), out choise);
                        if (parseBool && (choise >= 1 && choise <= 3)) break;
                        else { Console.Write("\nWrong input!"); Thread.Sleep(400); }
                    }
                    switch (choise)
                    {
                        case 1:
                            if(!AddWordMenu(ref newWord, ref newTranslate, ref translates)) goto EditMenu;
                            break;
                        case 2:
                            if(!AddTranslatesMenu(ref wordToAddTranslate, ref newTranslate, ref translates)) goto EditMenu;
                            break;
                        case 3:
                            goto EditMenu;
                            break;
                    }
                    break;
                case 2:
                    while (true)
                    {
                        Console.Clear();
                        Console.WriteLine("Vocabulary Manager\n\n");
                        Console.WriteLine("1) Delete Word");
                        Console.WriteLine("2) Delete Translate");
                        Console.WriteLine("3) Back to edit menu");
                        Console.Write("\nYour choice: ");
                        parseBool = UInt16.TryParse(Console.ReadLine(), out choise);
                        if (parseBool && (choise >= 1 && choise <= 3)) break;
                        else { Console.Write("\nWrong input!"); Thread.Sleep(400); }
                    }
                    switch (choise)
                    {
                        case 1:
                            if(!DeleteWordMenu(ref wordToReplace)) goto EditMenu;
                            break;
                        case 2:
                            if(!DeleteTranslatesMenu(ref wordToReplaceTranslate,ref translateToReplace, ref translates)) goto EditMenu;
                            break;
                        case 3:
                            goto EditMenu;
                            break;
                    }
                    break;
                case 3:
                    while (true)
                    {
                        Console.Clear();
                        Console.WriteLine("Vocabulary Manager\n\n");
                        Console.WriteLine("1) Replace Word");
                        Console.WriteLine("2) Replace Translate");
                        Console.WriteLine("3) Back to edit menu");
                        Console.Write("\nYour choice: ");
                        parseBool = UInt16.TryParse(Console.ReadLine(), out choise);
                        if (parseBool && (choise >= 1 && choise <= 3)) break;
                        else { Console.Write("\nWrong input!"); Thread.Sleep(400); }
                    }
                    switch (choise)
                    {
                        case 1:
                            if(!ReplaceWordMenu(ref wordToDelete, ref replacedWord)) goto EditMenu;
                            break;
                        case 2:
                            if(!ReplaceTranslatesMenu(ref wordToDeleteTranslate,ref translateToDelete, ref replacedTranslate)) goto EditMenu;
                            break;
                        case 3:
                            goto EditMenu;
                            break;
                    }

                    break;
                case 4:
                    return 4;
                    break;
                case 5:
                    return 5;
                    break;
            }
            return choise;
        } 
        static void Close()
        { 
            Console.Clear();
            Environment.Exit(0);
        }
        
        
        
        static Boolean AddWordMenu(ref String newWord, ref String newTranslate, ref List<String> translates)
        {
            WordInput:
            currentVocabulary.Refresh();
            Console.Clear();
            Console.WriteLine("Vocabulary Manager\n\n");
            Console.Write("Enter word([*] back to edit menu): ");
            newWord = Console.ReadLine();
            foreach (var elem in currentVocabulary.voc.Keys)
            {
                if (newWord == elem)
                {
                    Console.WriteLine($"\n{newWord} is already exist!");
                    Thread.Sleep(400);
                    goto WordInput;
                }
            }

            if (newWord == "*") return false;

            while (true)
            {
                Console.Write("Enter translate([/] to stop): ");
                newTranslate = Console.ReadLine();
                if (newTranslate != "/") translates.Add(newTranslate);
                else break;
            }

            currentVocabulary.AddWordS(newWord, translates);
            Console.WriteLine("Word has been successfully added!");
            Thread.Sleep(400);
            newWord = "";
            newTranslate = "";
            translates.Clear();
            return false;
        } 
        static Boolean AddTranslatesMenu(ref String wordToAddTranslate, ref String newTranslate, ref List<String> translates)
        {
            currentVocabulary.Refresh();
            Boolean wordExist = false;
            AddTranslate:
            Console.Clear();
            Console.WriteLine("Vocabulary Manager\n\n");
            Console.Write("Enter word you want to have translates([*] back to edit menu): ");
            wordToAddTranslate = Console.ReadLine();
            if (wordToAddTranslate == "*") return false;
            foreach (var elem in currentVocabulary.voc.Keys)
            {
                if(wordToAddTranslate == elem)
                {
                    wordExist = true;
                    break;
                }
            }
            if (!wordExist)
            {
                Console.WriteLine($"There are no \'{wordToAddTranslate}\' in \'{currentVocabulary.name}\'!");
                Thread.Sleep(400);
                goto AddTranslate;
            }
            while (true)
            {
                Console.Write("Enter translate([/] to stop): ");
                newTranslate = Console.ReadLine();
                if (newTranslate != "/") translates.Add(newTranslate);
                else break;
            }  
            currentVocabulary.AddTranslateS(wordToAddTranslate, translates);
            Console.WriteLine("Translates have been successfully added!");
            Thread.Sleep(400);
            wordToAddTranslate = "";
            newTranslate = "";
            translates.Clear();
            return false;
        } 
        
        
        
        static Boolean DeleteWordMenu(ref String wordToDelete)
        {
            DeleteWord:
            currentVocabulary.Refresh();
            Boolean wordExist = false; 
            Console.Clear();
            Console.WriteLine("Vocabulary Manager\n\n");
            Console.Write("Enter word([*] back to edit menu): ");
            wordToDelete = Console.ReadLine();
            if (wordToDelete == "*") return false;
            foreach (var elem in currentVocabulary.voc.Keys)
            {
                if(wordToDelete == elem)
                {
                    wordExist = true;
                    break;
                }
            }

            if (!wordExist)
            {
                Console.WriteLine($"There are no \'{wordToDelete}\' in \'{currentVocabulary.name}\'!");
                Thread.Sleep(400);
                goto DeleteWord;
            }
             
            currentVocabulary.DeleteWordS(wordToDelete);
            Console.WriteLine("Translates have been successfully deleted!");
            Thread.Sleep(400);  
            return false;
        } 
        static Boolean DeleteTranslatesMenu(ref String wordToDelete, ref String translateToDelete, ref List<String> translates)
        {
            DeleteWord:
            Boolean wordExist = false, translateExist = false;
            currentVocabulary.Refresh();
            Console.Clear();
            Console.WriteLine("Vocabulary Manager\n\n");
            Console.Write("Enter word([*] back to edit menu): ");
            wordToDelete = Console.ReadLine();
            if (wordToDelete == "*") return false;
            foreach (var elem in currentVocabulary.voc.Keys)
            {
                if(wordToDelete == elem)
                {
                    wordExist = true;
                    break;
                }
            }

            if (!wordExist)
            {
                Console.WriteLine($"There are no \'{wordToDelete}\' in \'{currentVocabulary.name}\'!");
                Thread.Sleep(400);
                goto DeleteWord;
            }

            TranslateInput:
            for (UInt16 i = 0; i < currentVocabulary.voc[wordToDelete].Count - 1; ++i)
            {
                while (true)
                {
                    Console.Write($"Enter translate to delete([/] to stop): ");
                    translateToDelete = Console.ReadLine();
                    if (translateToDelete == "/") goto Final;
                    foreach (var elem in currentVocabulary.voc[wordToDelete])
                    {
                        if (translateToDelete == elem)
                        {
                            translateExist = true;
                            translates.Add(translateToDelete);
                        }
                    }

                    if (!translateExist)
                    {  
                        Console.WriteLine("Wrong translate!");
                        Thread.Sleep(400);
                        translates.Clear();
                        Console.Clear();
                        Console.WriteLine("Vocabulary Manager\n\n");
                        Console.WriteLine($"Enter word([*] back to edit menu): {wordToDelete}");
                        goto TranslateInput;
                    }
                }
            }
            Final:
            currentVocabulary.DeleteTranslateS(wordToDelete, translates);
            Console.WriteLine("Translates have been successfully deleted!");
            Thread.Sleep(400);  
            return false;
        }



        static Boolean ReplaceWordMenu(ref String wordToReplace, ref String replacedWord)
        {
            ReplaceWord:
            currentVocabulary.Refresh();
            Console.Clear();
            Console.WriteLine("Vocabulary Manager\n\n");
            Console.Write("Enter word to replace([*] back to edit menu): ");
            wordToReplace = Console.ReadLine();
            if (wordToReplace == "*") return false;
            Console.Write("Enter new word([*] back to edit menu): ");
            replacedWord = Console.ReadLine();
            if (replacedWord == "*") return false;
            if (!currentVocabulary.ReplaceWordS(wordToReplace, replacedWord))
            {
                Console.WriteLine("Wrong input!");
                Thread.Sleep(400);
                goto ReplaceWord;
            }
            
            return false;
        }

        static Boolean ReplaceTranslatesMenu(ref String wordToReplace, ref String translateToReplace, ref String replacedTranslate)
        {
            ReplaceTranslate:
            currentVocabulary.Refresh();
            Boolean wordExist = false;
            Console.Clear();
            Console.WriteLine("Vocabulary Manager\n\n");
            Console.Write("Enter word to replace([*] back to edit menu): ");
            wordToReplace = Console.ReadLine();
            if (wordToReplace == "*") return false;
            foreach (var elem in currentVocabulary.voc.Keys)
            {
                if (wordToReplace == elem)
                {
                    wordExist = true;
                    break;
                }
            }

            if (!wordExist)
            {
                Console.WriteLine($"There are no \'{wordExist}\' in \'{currentVocabulary.name}\'!");
                Thread.Sleep(400);
                goto ReplaceTranslate;
            }

            while (true)
            {
                Console.Write("Enter translate to replace([/] back to edit menu): ");
                translateToReplace = Console.ReadLine();;
                if (translateToReplace == "/") break;
                Console.Write("Enter new translate([/] back to edit menu): ");
                replacedTranslate = Console.ReadLine();
                if (replacedTranslate == "/") break;
                if (!currentVocabulary.ReplaceTranslateS(wordToReplace, translateToReplace, replacedTranslate))
                {
                    Console.WriteLine("Wrong input!");
                    Thread.Sleep(400); 
                }
            }

            return false;
        }


        #endregion
        
        
        
        
        static void Main(string[] args)
        { 
            # region Program start post
            DirectoryInfo mainDirectoryControl = new DirectoryInfo("C:\\Vocabulary Manager");
            if (!mainDirectoryControl.Exists) { mainDirectoryControl.Create(); }
            DirectoryInfo secondaryDirectoryControl = new DirectoryInfo("C:\\Vocabulary Manager\\Vocabularies");
            if (!secondaryDirectoryControl.Exists) { secondaryDirectoryControl.Create(); } 
            if (!File.Exists("C:\\Vocabulary Manager\\Dictionaries List.xml"))
            {
                using (FileStream fs = new FileStream("C:\\Vocabulary Manager\\Dictionaries List.xml", FileMode.Create))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<String>));
                    serializer.Serialize(fs, vocabulariesList);
                }
            }
            RefreshVocabulariesList();
            #endregion 
            #region Program
            MainMenu:
            switch (MainMenu())
            {
                case 1:
                    switch (AddVocabularyMenu())
                    {
                        case 1:
                            goto MainMenu;
                            break;
                        case 2:
                            switch (EditLanguageMenu())
                            { 
                                case 4:
                                    goto MainMenu;
                                    break;
                                case 5:
                                    Close();
                                    break;
                            }
                            break;
                        case 3:
                            Close();
                            break;
                    }
                    break;
                case 2:
                    switch (EditVocabularyMenu()) {
                        case 4:
                            goto MainMenu;
                            break;
                        case 5:
                            Close();
                            break;
                    } 
                    break;
                case 3:
                    switch (ShowVocabulariesMenu())
                    {
                        case 1:
                            goto MainMenu;
                            break;
                        case 2:
                            Close();
                            break;
                    }
                    break;
                case 4:
                    switch (TranslateMenu())
                    {
                        case 2:
                            goto MainMenu;
                            break;
                        case 3:
                            Close();
                            break;
                    }
                    break;
                case 5:
                    Close();
                    break;
            }
            #endregion
        }
    }
}
