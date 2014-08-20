using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntroductionToRx
{
    class Events
    {
        private int _length = -1;

        public event Action<string> TextChanged;

        public virtual void OnTextChanged(string text)
        {
            var t = TextChanged;
            if (t != null)
                t(text);

            var newLength = text.Length;
            if (_length != newLength)
            {
                _length = newLength;
                var lc = _lengthChanged;
                if (lc != null)
                {
                    lc(_length);
                }
            }
        }

        private event Action<int> _lengthChanged;

        public event Action<int> LengthChanged
        {
            add { _lengthChanged += value; }
            remove { _lengthChanged -= value; }
        }
    }
}
