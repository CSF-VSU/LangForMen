namespace LangForRealMen.ParserLogic
{
    public class ParserBase
    {

        // пробельные символы по умолчанию
        public const string DefaultWhitespaces = " \n\r\t";


        // разбираемая строка
        private readonly string _source;
        // позиция указателя
        private int pos = 0;


        public ParserBase(string source)
        {
            _source = source;
        }


        public string Source
        {
            get
            {
                return _source;
            }
        }

        public int Pos
        {
            get
            {
                return pos;
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
                pos++;
        }

        // пропускает незначащие (пробельные) символы
        public virtual void Skip()
        {
            while (DefaultWhitespaces.IndexOf(this[pos]) >= 0)
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
                        this.pos = pos;
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
            this.pos = pos;
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
