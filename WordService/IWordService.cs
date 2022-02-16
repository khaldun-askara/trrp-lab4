using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WordService
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени интерфейса "IService1" в коде и файле конфигурации.
    [ServiceContract]
    public interface IWordService
    {
        [OperationContract]
        int GetWordsCount(bool isMain);
        [OperationContract]
        Word GetWord(int i);
        [OperationContract]
        int InsertWord(string word, bool isMain);
        [OperationContract]
        void UpdateWord(int id, string word, bool isMain);
        [OperationContract]
        void DeleteWord(int id, bool isMain);
        [OperationContract]
        void MoveFromSuggestions(int id);
        [OperationContract]
        string GetRandomWord();
        [OperationContract]
        void DoBackup();
    }

    [DataContract]
    public class Word
    {
        int id;
        string value;

        [DataMember]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        [DataMember]
        public string Value
        {
            get { return value; }
            set { this.value = value; }
        }
    }
}
