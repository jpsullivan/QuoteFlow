using System;
using System.Collections.Generic;
using System.Linq;

namespace QuoteFlow.Core.Util
{
    public class CompositeDisposable : IDisposable
    {
        private readonly ICollection<IDisposable> _disposables;

        public CompositeDisposable(IDisposable disposable, IDisposable disposable2)
            : this(new List<IDisposable> { disposable, disposable2 })
        {
        }

        public CompositeDisposable(IEnumerable<IDisposable> disposables)
        {
            var nullCheckedList = disposables.Where(disposable => disposable != null).ToList();
            _disposables = nullCheckedList;
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }
    }
}
