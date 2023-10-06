using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using FlexWords.Entities.Classes;
using FlexWords.Entities.Enums;
using FlexWords.Extensions;
using FlexWords.Translator;
using FlexWords.Dialog.Handlers;

namespace FlexWords.Dialog.Controls
{
    public partial class FlexWordsDialog
    {
        #region algorithms

        private IReadOnlyList<GroupPanel> GenerateWordGroups(Book book, Workspace workspace)
        {
            int wordIndex = 0;
            IReadOnlyList<WordItem[]> groups = book.CurrentWordGroups();
            GroupPanel[] result = new GroupPanel[groups.Count];

            for (int i = 0; i < groups.Count; i++)
            {
                GroupPanel groupPanel = Instantiate<GroupPanel>();
                result[i] = groupPanel;

                for (int j = 0; j < groups[i].Length; j++)
                {
                    WordItem item = groups[i][j];

                    WordPanel wordPanel = Instantiate<WordPanel>();
                    wordPanel.Dialog = this;
                    wordPanel.Flag = item.Flag;
                    wordPanel.Text = item.Value.Value ?? string.Empty;
                    wordPanel.IsTextType = item.Value.Type == WordType.Text;
                    groupPanel.Add(wordPanel, wordIndex++, j, workspace);

                    for (int k = 0; k < item.Letters.Length; k++)
                    {
                        LetterControl control = Instantiate<LetterControl>();
                        wordPanel.Add(control, item.Letters[k], k, workspace);
                    }
                }
            }

            return result;
        }
        private void RedrawLines(IReadOnlyList<GroupPanel> items, Workspace workspace)
        {
            int index = 0;
            int lineIndex = 0, groupIndex = 0;
            LinePanel linePanel = Instantiate<LinePanel>();
            linePanel.LineIndex = lineIndex++;
            linePanel.Readspace = workspace;
            textItemsContainer.Children.Add(linePanel);

            double maxWidth = textItemsContainer.ActualWidth;

            foreach (GroupPanel groupPanel in items)
            {
                if (linePanel.Children.Count is 0)
                {
                    groupPanel.Index = index++;
                    groupPanel.GroupIndex = groupIndex++;
                    groupPanel.Readspace = workspace;
                    linePanel.Children.Add(groupPanel);

                    continue;
                }

                if (maxWidth < linePanel.CalculatedWidth + groupPanel.CalculatedWidth + Options.WordSpacing)
                {
                    linePanel = Instantiate<LinePanel>();
                    linePanel.LineIndex = lineIndex++;
                    linePanel.Readspace = workspace;
                    textItemsContainer.Children.Add(linePanel);

                    groupIndex = 0;
                    groupPanel.Index = index++;
                    groupPanel.GroupIndex = groupIndex++;
                    groupPanel.Readspace = workspace;
                    linePanel.Children.Add(groupPanel);

                    continue;
                }

                groupPanel.Index = index++;
                groupPanel.GroupIndex = groupIndex++;
                groupPanel.Readspace = workspace;
                linePanel.Children.Add(groupPanel);
            }

            // AdaptiveKerning
            if (Options.AdaptiveKerning && !Options.AdaptiveWordSpacing)
            {
                for (int i = 0; i < textItemsContainer.Children.Count - 1; i++)
                {
                    linePanel = (LinePanel)textItemsContainer.Children[i];

                    if (linePanel.CalculatedWidth > maxWidth) continue;

                    double remainder = maxWidth - linePanel.CalculatedWidth;
                    double additionalLeftOffset = remainder / linePanel.KerningCount;

                    linePanel.SetAdditionalKerning(additionalLeftOffset);
                }

                return;
            }

            // AdaptiveKerning & AdaptiveWordSpacing
            if (Options.AdaptiveWordSpacing && Options.AdaptiveKerning)
            {
                for (int i = 0; i < textItemsContainer.Children.Count - 1; i++)
                {
                    linePanel = (LinePanel)textItemsContainer.Children[i];

                    if (linePanel.Children.Count < 2) continue;

                    int wordSpacingCount = linePanel.Children.Count - 1;
                    double remainder = maxWidth - linePanel.CalculatedWidth;

                    if (remainder < Options.WordSpacing * wordSpacingCount) continue;

                    remainder -= Options.WordSpacing * wordSpacingCount;
                    double additionalLeftOffset = remainder / linePanel.KerningCount;

                    linePanel.SetAdditionalKerning(additionalLeftOffset);
                }
            }

            if (!Options.AdaptiveWordSpacing) return;

            // AdaptiveWordSpacing
            for (int i = 0; i < textItemsContainer.Children.Count - 1; i++)
            {
                linePanel = (LinePanel)textItemsContainer.Children[i];
                double remainder = maxWidth - linePanel.CalculatedWidth;

                if (linePanel.Children.Count < 2) continue;

                double additionalLeftOffset = remainder / (linePanel.Children.Count - 1);

                for (int j = 1; j < linePanel.Children.Count; j++)
                {
                    GroupPanel groupPanel = (GroupPanel)linePanel.Children[j];
                    double leftMargin = groupPanel.Margin.Left + additionalLeftOffset;

                    groupPanel.Margin = new Thickness(leftMargin, 0, 0, 0);
                }
            }
        }
        private IReadOnlyList<GroupPanel> Recompose(Workspace? workspace = null)
        {
            int index = 0;
            GroupPanel groupPanel;
            LinePanel linePanel;
            int capacity = textItemsContainer.Children.Cast<LinePanel>().Sum(i => i.Children.Count);
            var result = new GroupPanel[capacity];

            while (textItemsContainer.Children.Count > 0)
            {
                linePanel = (LinePanel)textItemsContainer.Children[0];

                while (linePanel.Children.Count > 0)
                {
                    groupPanel = (GroupPanel)linePanel.Children[0];

                    if (workspace is not null)
                    {
                        foreach (WordPanel wordPanel in groupPanel.Children)
                        {
                            foreach (LetterControl letterControl in wordPanel.Children)
                            {
                                letterControl.Readspace = workspace;
                                letterControl.RecalculateWidth();
                            }

                            wordPanel.Readspace = workspace;
                        }
                    }

                    linePanel.Children.Remove(groupPanel);
                    groupPanel.Reset();
                    result[index++] = groupPanel;
                }

                textItemsContainer.Children.Remove(linePanel);
                linePanel.Reset();
                Push(linePanel);
            }

            return result;
        }
        private void FullRecompose()
        {
            LetterControl letterControl;
            WordPanel wordPanel;
            GroupPanel groupPanel;
            LinePanel linePanel;

            while (textItemsContainer.Children.Count > 0)
            {
                linePanel = (LinePanel)textItemsContainer.Children[0];

                while (linePanel.Children.Count > 0)
                {
                    groupPanel = (GroupPanel)linePanel.Children[0];

                    while (groupPanel.Children.Count > 0)
                    {
                        wordPanel = (WordPanel)groupPanel.Children[0];

                        while (wordPanel.Children.Count > 0)
                        {
                            letterControl = (LetterControl)wordPanel.Children[0];

                            wordPanel.Children.Remove(letterControl);
                            letterControl.Reset();
                            Push(letterControl);
                        }

                        groupPanel.Children.Remove(wordPanel);
                        wordPanel.Reset();
                        Push(wordPanel);
                    }

                    linePanel.Children.Remove(groupPanel);
                    groupPanel.Reset();
                    Push(groupPanel);
                }

                textItemsContainer.Children.Remove(linePanel);
                linePanel.Reset();
                Push(linePanel);
            }
        }
        private void SoftRedrawLines(Book book, Workspace workspace)
        {
            int index = 0;

            IReadOnlyList<WordItem> items = book.CurrentWords();

            foreach (LinePanel linePanel in textItemsContainer.Children)
            {
                foreach (GroupPanel groupPanel in linePanel.Children)
                {
                    foreach (WordPanel wordPanel in groupPanel.Children)
                    {
                        wordPanel.Flag = items[index++].Flag;
                        wordPanel.SoftUpdate(workspace);
                    }
                }

                linePanel.Readspace = CurrentWorkspace;
            }
        }
        private void SuperSoftRedrawLines(Workspace workspace)
        {
            foreach (LinePanel linePanel in textItemsContainer.Children)
            {
                foreach (GroupPanel groupPanel in linePanel.Children)
                {
                    foreach (WordPanel wordPanel in groupPanel.Children)
                    {
                        wordPanel.SuperSoftUpdate(workspace);
                    }
                }

                linePanel.Readspace = CurrentWorkspace;
            }
        }

        #endregion

        public void SuperSoftUpdate(Workspace workspace)
        {
            SuperSoftRedrawLines(workspace);
        }

        public void Reorder(Workspace workspace)
        {
            RedrawLines(Recompose(Options.AdaptiveKerning ? workspace : null), workspace);
        }

        public void Update(Workspace workspace)
        {
            RedrawLines(Recompose(workspace), workspace);
        }

        public void ForceUpdate(Book book, Workspace workspace, bool isSameParagraph = false)
        {
            if (isSameParagraph)
            {
                SoftRedrawLines(book, workspace);
                _dictWords.Clear();

                return;
            }
            
            FullRecompose();
            RedrawLines(GenerateWordGroups(book, workspace), workspace);
            _dictWords.Clear();
        }
    }

    public partial class FlexWordsDialog
    {
        private readonly Dictionary<Type, object> _dictStorage = new()
        {
            { typeof(LinePanel), new Stack<LinePanel>() },
            { typeof(GroupPanel), new Stack<GroupPanel>() },
            { typeof(WordPanel), new Stack<WordPanel>() },
            { typeof(LetterControl), new Stack<LetterControl>() },
        };
        private readonly List<WordPanel> _dictWords = new();

        public ITranslatorHandler Translator { get; }

        public string SelectedText
        {
            get => string.Join(' ', _dictWords.OrderBy(i => i.Index).Select(i => i.Text));
        }

        public bool AreAnyTextSelected => _dictWords.Count > 0;

        public void UnselectAllWords()
        {
            string text = Clipboard.GetText(TextDataFormat.UnicodeText);

            while (AreAnyTextSelected)
            {
                _dictWords[^1].PerformMouseClick();
            }

            Clipboard.SetText(text, TextDataFormat.UnicodeText);
        }

        public void SelectedWordsFlickering(bool flag)
        {
            if (!AreAnyTextSelected) return;

            foreach (WordPanel panel in _dictWords)
            {
                panel.Flickering(flag);
            }
        }

        public void OnWordSelected(bool select, WordPanel panel)
        {
            if (select) _dictWords.Add(panel);
            else _dictWords.Remove(panel);

            if (!AreAnyTextSelected) return;

            if (Options.AutoCopySelectedText)
            {
                Clipboard.SetText(SelectedText, TextDataFormat.UnicodeText);
            }
        }

        private T Instantiate<T>() where T : new()
        {
            var stack = (Stack<T>)_dictStorage[typeof(T)];

            if (stack.Count > 0) return stack.Pop();

            return new T();
        }

        private void Push<T>(T item) where T : new()
        {
            var list = (Stack<T>)_dictStorage[typeof(T)];

            list.Push(item);
        }
    }
}
