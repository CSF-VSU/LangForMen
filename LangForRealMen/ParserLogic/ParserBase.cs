using System.ComponentModel;

namespace LangForRealMen.ParserLogic
{
    public class ParserBase
    {

        // пробельные символы по умолчанию
        public const string DefaultWhitespaces = " \n\r\t";


        // разбираемая строка
        private string _source;
        // позиция указателя
        private int _pos;

        public virtual void Init(string source)
        {
            _source = source;
            _pos = 0;
        }


        public string Source
        {
            get
            {
                return _source;
            }
            set
            {
                _source = value;
            }
        }

        public int Pos
        {
            get
            {
                return _pos;
            }
        }

        protected char this[int index]
        {
            get
            {
                return index < _source.Length ? _source[index] : (char)0;
            }
        }

        // символ в текущей позиции указателя
        public char Current
        {
            get
            {
                return this[Pos];
            }
        }

        // определяет, достигнут ли конец строки
        public bool End
        {
            get
            {
                return Current == 0;
            }
        }

        // передвигает указатель на один символ
        public void Next()
        {
            if (!End)
                _pos++;
        }

        // пропускает незначащие (пробельные) символы
        public virtual void Skip()
        {
            while (DefaultWhitespaces.IndexOf(this[_pos]) >= 0)
                Next();
        }


        // распознает одну из строк;
        // при этом указатель смещается и пропускаются незначащие символы;
        // если ни одну из строк распознать нельзя, то возвращается null
        protected string MatchNoExcept(params string[] a)
        {
            int pos = Pos;
            foreach (string s in a)
            {
                bool match = true;
                foreach (char c in s)
                    if (Current == c)
                        Next();
                    else
                    {
                        _pos = pos;
                        match = false;
                        break;
                    }
                if (match)
                {
                    Skip();
                    return s;
                }
            }
            return null;
        }


        // проверяет, можно ли в текущей позиции указателя, распознать одну из строк;
        // указатель не смещается;
        public bool IsMatch(params string[] a)
        {
            var pos = Pos;
            var result = MatchNoExcept(a);
            _pos = pos;
            return result != null;
        }


        // распознает одну из строк;
        // при этом указатель смещается и пропускаются незначащие символы;
        // если ни одну из строк распознать нельзя, то выбрасывается исключение
        public string Match(params string[] a)
        {
            int pos = Pos;
            var result = MatchNoExcept(a);
            if (result == null)
            {
                string message = "Ожидалась одна из строк: ";
                bool first = true;
                foreach (string s in a)
                {
                    if (!first)
                        message += ", ";
                    message += string.Format("\"{0}\"", s);
                    first = false;
                }
                throw new ParserBaseException(string.Format("{0} (pos={1})", message, pos));
            }
            return result;
        }

        // то же, что и Match(params string[] a), для удобства
        public string Match(string s)
        {
            int pos = Pos;
            try
            {
                return Match(new[] { s });
            }
            catch
            {
                throw new ParserBaseException(s.Length == 1 ? string.Format("Ожидался символ: '{0}' (pos={1})", s, pos)
                                                            : string.Format("Ожидалась строка: \"{0}\" (pos={1})", s, pos));
            }
        }
    }
}
