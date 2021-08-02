using System;
using System.IO;
using System.Linq;
using System.Windows;
using CaseConverter.Converters;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace CaseConverter
{
    /// <summary>
    /// 文字列をスネークケース⇒キャメルケース⇒パスカルケースの順に変換するコマンドです。
    /// This command converts a character string in the order of snake case ⇒ camel case ⇒ pascal case.
    /// </summary>
    internal sealed class ConvertCaseCommand : CommandBase
    {
        /// <summary>
        /// コマンドのIDです。
        /// The ID of the command.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// コマンドメニューグループのIDです。
        /// The ID of the command menu group.
        /// </summary>
        public static readonly Guid CommandSet = new Guid("f038e966-3a02-4eef-bfad-cd8fab3c4d6d");

        /// <summary>
        /// シングルトンのインスタンスを取得します。
        /// Get an instance of a singleton.
        /// </summary>
        public static ConvertCaseCommand Instance { get; private set; }

        /// <summary>
        /// インスタンスを初期化します。
        /// Initialize the instance.
        /// </summary>
        /// <param name="package">コマンドを提供するパッケージ</param>
        private ConvertCaseCommand(Package package) : base(package, CommandId, CommandSet)
        {
        }

        /// <summary>
        /// このコマンドのシングルトンのインスタンスを初期化します。
        /// Initializes a singleton instance of this command.
        /// </summary>
        /// <param name="package">コマンドを提供するパッケージPackage that provides commands</param>
        public static void Initialize(Package package)
        {
            Instance = new ConvertCaseCommand(package);
        }

        /// <inheritdoc />
        protected override void Execute(object sender, EventArgs e)
        {
            var dte = ServiceProvider.GetService(typeof(DTE)) as DTE;
            var doc = dte.Documents.Item("tps.json");
            var textDocument = dte.ActiveDocument.Object("TextDocument") as TextDocument;
            if (textDocument != null)
            {
                var convertPatterns = ((CaseConverterPackage)Package).GetGeneralOption().Patterns.ToList();

                var selection = textDocument.Selection;
                if (selection.IsEmpty == false)
                {
                    string pasteJson;
                    string replace;
                    var selectedText = selection.Text;
                    if (selectedText[0] == '"')
                    {
                        pasteJson = $"\"{StringCaseConverter.Convert(selectedText, convertPatterns)}\": {selectedText}";
                        if (Path.GetExtension(dte.ActiveDocument.FullName) == ".xaml")
                        {
                            //replace = $"\"{StringCaseConverter.Convert(selectedText, convertPatterns)}\"";
                            replace = $"\"{{l:Localize {{x:Static trackMan:tps+{StringCaseConverter.Convert(selectedText, convertPatterns)}}}}}\"";
                            //"{l:Localize {x:Static trackMan:tps+puttingTopView.trajectory}}"
                        }
                        else
                        {
                            //replace = $"{StringCaseConverter.Convert(selectedText, convertPatterns)}.Text()";
                            replace = $"tps.{StringCaseConverter.Convert(selectedText, convertPatterns)}.Text()";
                        }
                    }
                    else
                    {
                        pasteJson = $"\"{StringCaseConverter.Convert(selectedText, convertPatterns)}\": \"{selectedText}\"";
                        if (Path.GetExtension(dte.ActiveDocument.FullName) == ".xaml")
                        {
                            replace = $"{StringCaseConverter.Convert(selectedText, convertPatterns)}";
                        }
                        else
                        {
                            replace = $"{StringCaseConverter.Convert(selectedText, convertPatterns)}.Text()";
                        }
                    }

                    Clipboard.SetData(DataFormats.Text, pasteJson);
                    selection.ReplaceText(selectedText, replace);
                    doc.Activate();
                }
                else
                {
                    var point = selection.ActivePoint;
                    var startPoint = CreateStartPoint(point);
                    var endPoint = CreateEndPoint(point, startPoint);

                    var targetText = startPoint.GetText(endPoint);
                    var word = targetText.TrimEnd(' ');
                    var convertedWord = StringCaseConverter.Convert(word, convertPatterns);

                    if (word != convertedWord)
                    {
                        var left = point.AbsoluteCharOffset - startPoint.AbsoluteCharOffset;
                        selection.CharLeft(false, left);

                        var trimCount = targetText.Length - word.Length;
                        var right = endPoint.AbsoluteCharOffset - point.AbsoluteCharOffset - trimCount;
                        selection.CharRight(true, right);

                        //selection.ReplaceText(word, convertedWord);
                        //"Her er en fisk"
                        var pasteJson = $"\"{StringCaseConverter.Convert(word, convertPatterns)}\": {word}";
                        Clipboard.SetData(DataFormats.Text, pasteJson);
                        var replace = $"\"{StringCaseConverter.Convert(word, convertPatterns)}\"";
                        selection.ReplaceText(word, replace);
                        //selection.Copy();
                        doc.Activate();
                    }
                }
            }
        }

        /// <summary>
        /// 文字列の終了位置を作成します。
        /// Creates the end position of the string.
        /// </summary>
        private static EditPoint CreateEndPoint(VirtualPoint point, EditPoint startPoint)
        {
            var result = point.CreateEditPoint();
            if (point.AtEndOfLine || result.GetText(1) == " ")
            {
                return result;
            }

            result = startPoint.CreateEditPoint();
            result.WordRight();
            return result;
        }

        /// <summary>
        /// 文字列の開始位置を作成します。
        /// Creates the starting position of the string.
        /// </summary>
        private static EditPoint CreateStartPoint(VirtualPoint point)
        {
            var result = point.CreateEditPoint();
            if (point.AtStartOfLine || GetLeftText(point, 1) == " ")
            {
                return result;
            }

            result.WordLeft();

            var tempPoint = result.CreateEditPoint();
            tempPoint.WordRight();

            return point.AbsoluteCharOffset == tempPoint.AbsoluteCharOffset ?
                point.CreateEditPoint() : result;
        }

        /// <summary>
        /// 指定の位置の左の文字を取得します。
        /// Gets the character to the left of the specified position.
        /// </summary>
        private static string GetLeftText(VirtualPoint point, int count)
        {
            var editPoint = point.CreateEditPoint();
            editPoint.CharLeft(count);

            return editPoint.GetText(1);
        }
    }
}
