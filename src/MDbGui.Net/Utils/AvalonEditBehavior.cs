﻿using ICSharpCode.AvalonEdit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;

namespace MDbGui.Net.Utils
{
    /// <summary>
    /// http://stackoverflow.com/questions/18964176/two-way-binding-to-avalonedit-document-text-using-mvvm
    /// </summary>
    public sealed class AvalonEditBehavior : Behavior<TextEditor>
    {
        public static readonly DependencyProperty GiveMeTheTextProperty =
            DependencyProperty.Register("GiveMeTheText", typeof(string), typeof(AvalonEditBehavior),
            new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, PropertyChangedCallback));

        public string GiveMeTheText
        {
            get { return (string)GetValue(GiveMeTheTextProperty); }
            set { SetValue(GiveMeTheTextProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject != null)
            {
                if (GiveMeTheText != null)
                    AssociatedObject.Document.Text = GiveMeTheText; 
                AssociatedObject.TextChanged += AssociatedObjectOnTextChanged;
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (AssociatedObject != null)
                AssociatedObject.TextChanged -= AssociatedObjectOnTextChanged;
        }

        private void AssociatedObjectOnTextChanged(object sender, EventArgs eventArgs)
        {
            var textEditor = sender as TextEditor;
            if (textEditor != null)
            {
                if (textEditor.Document != null)
                    GiveMeTheText = textEditor.Document.Text;
            }
        }

        private static void PropertyChangedCallback(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var behavior = dependencyObject as AvalonEditBehavior;
            if (behavior.AssociatedObject != null)
            {
                var editor = behavior.AssociatedObject as TextEditor;
                if (editor.Document != null)
                {
                    if (dependencyPropertyChangedEventArgs.NewValue != null)
                    {
                        var caretOffset = editor.CaretOffset;
                        editor.Document.Text = dependencyPropertyChangedEventArgs.NewValue.ToString();
                        if (caretOffset > editor.Document.Text.Length)
                            editor.CaretOffset = editor.Document.Text.Length;
                        else
                            editor.CaretOffset = caretOffset;
                    }
                    else
                    {
                        editor.Document.Text = "";
                        editor.CaretOffset = 0;
                    }
                }
            }
        }
    }
}
