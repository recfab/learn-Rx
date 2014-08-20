using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using System.Reactive.Disposables;

namespace IntroductionToRx
{
    class Observables
    {
        ISubject<string> textChanged = new Subject<string>();

        private IObservable<int> _lengthChanged;

        public Observables()
        {
            _lengthChanged = new LengthObservable(textChanged);
        }

        public virtual void OnTextChanged(string text)
        {
            textChanged.OnNext(text);
        }

        public IObservable<string> TextChanged { get { return textChanged; } }

        public IObservable<int> LengthChanged
        {
            get { return _lengthChanged; }
        }
    }

    public class LengthObservable : IObservable<int>
    {
        private Subject<int> _subject;

        private int _length;

        public LengthObservable(ISubject<string> textChanged)
        {
            _length = -1;

            _subject = new Subject<int>();

            var d = textChanged.Subscribe(text =>
            {
                if (_length != text.Length)
                {
                    _length = text.Length;

                    _subject.OnNext(_length);
                }
            });
        }

        #region Implementation of IObservable<out int>

        public IDisposable Subscribe(IObserver<int> observer)
        {
            return _subject.Subscribe(observer);
        }

        #endregion
    }
}
