using System;
using System.Linq;
using System.Reactive.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Reactive;
using UnifiedProgrammingModel.DictionaryService;

namespace UnifiedProgrammingModel
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var txt = new TextBox();
            var lst = new ListBox { Top = txt.Height + 10 };
            var frm = new Form { Controls = { txt, lst } };

            // TODO: Convert txt.TextChanged to IObservable<EventPattern<EventArgs>> and assign it to textChanged.
            // HINT: Try using FromEventPattern.
            var textChanged =
                Observable.FromEventPattern(txt, "TextChanged")
                    .Select(e => ((TextBox) e.Sender).Text)
                    .Throttle(TimeSpan.FromMilliseconds(300));
            // Throttling based on suggestion in comments of the video.

            // TODO: Convert BeginMatch/EndMatch to Func<string, IObservable<DictionaryWord[]>> and assign it to getSuggestions.
            // HINT: Try using FromAsyncPattern
            // NOTE: Need to specify the types. Compiler isn't smart enough to figure them out itself apparently.
            var getSuggestions = Observable.FromAsyncPattern<string, DictionaryWord[]>(BeginMatch, EndMatch);

            var results = from text in textChanged
                          where text.Length >= 3
                          from suggestions in getSuggestions(text)
                          select suggestions;

            using (results
                .ObserveOn(lst)
                .Subscribe(words =>
                {
                    lst.Items.Clear();
                    lst.Items.AddRange(words.Select(word => word.Word).Take(10).ToArray());
                }))
            {
                Application.Run(frm);
            }
        }

        private static DictServiceSoapClient service = new DictServiceSoapClient("DictServiceSoap");

        private static IAsyncResult BeginMatch(string prefix, AsyncCallback callback, object state)
        {
            return service.BeginMatchInDict("wn", prefix, "prefix", callback, state);
        }

        private static DictionaryWord[] EndMatch(IAsyncResult result)
        {
            return service.EndMatchInDict(result);
        }
    }
}
