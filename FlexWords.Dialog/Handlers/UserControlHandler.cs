using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace FlexWords.Dialog.Handlers
{
    public class UserControlHandler : UserControl
    {
        public List<Action> Actions { get; } = new();
        public List<Action> TempActions { get; } = new();

        public bool IsCompleted => Actions.Count + TempActions.Count is 0;

        public void Complete()
        {
            if (IsCompleted) return;

            CompleteTemporaryActions();
            Actions.ForEach(i => i?.Invoke());
            Actions.Clear();
        }

        public void CompleteTemporaryActions()
        {
            TempActions.ForEach(i => i?.Invoke());
            TempActions.Clear();
        }

        public void SubscribeTo(Action action)
        {
            Actions.Add(action);
        }

        public void SubscribeTemporaryTo(Action action)
        {
            TempActions.Add(action);
        }

        public T Instantiate<T>() where T : class, new()
        {
            T obj = new();

            if (obj is UserControlHandler handler)
            {
                Actions.Add(handler.Complete);
            }

            return obj;
        }

        public T InstantiateTemporary<T>() where T : new()
        {
            T obj = new();

            if (obj is UserControlHandler handler)
            {
                TempActions.Add(handler.Complete);
            }

            return obj;
        }
    }
}
